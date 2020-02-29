using AssemblyInfo.Extensions;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Security.Cryptography;

namespace AssemblyInfo
{
    public class AssemblyInspector
    {
        /// <summary>
        /// Max 1GB hash inspection length
        /// </summary>
        private const int MaxHashInspectionLength = 1 * 1024 * 1024 * 1024;

        public AssemblyData Inspect(string filename)
        {
            if (File.Exists(filename))
                return InspectAssembly(filename);
            throw new FileNotFoundException(filename);
        }

        private AssemblyData InspectAssembly(string filename)
        {
            var assembly = Assembly.LoadFrom(filename);
            var assemblyName = assembly.GetName();
            var version = assemblyName.Version;
            var fileVersion = FileVersionInfo.GetVersionInfo(assembly.Location);
            var targetFramework = assembly.CustomAttributes.FirstOrDefault(x => x.AttributeType == typeof(TargetFrameworkAttribute));
            var assemblyConfiguration = assembly.CustomAttributes.FirstOrDefault(x => x.AttributeType == typeof(AssemblyConfigurationAttribute));
            var clsCompliant = assembly.CustomAttributes.FirstOrDefault(x => x.AttributeType == typeof(CLSCompliantAttribute));
            var assemblyInformationalVersion = assembly.CustomAttributes.FirstOrDefault(x => x.AttributeType == typeof(AssemblyInformationalVersionAttribute));
            var assemblyMetadatas = assembly.CustomAttributes.Where(x => x.AttributeType == typeof(AssemblyMetadataAttribute)).ToList();
            var debuggable = assembly.CustomAttributes.FirstOrDefault(x => x.AttributeType == typeof(DebuggableAttribute));
            var debuggableAttributeArguments = debuggable.ConstructorArguments.FirstOrDefault();
            var fileInfo = new FileInfo(filename);

            var frameworkString = targetFramework?.ConstructorArguments?.FirstOrDefault().Value.ToString();
            var targetFrameworkName = frameworkString
                ?.Split(new string[] { "," }, System.StringSplitOptions.None)
                .FirstOrDefault();
            var targetFrameworkVersion = frameworkString
                ?.Split(new string[] { "," }, System.StringSplitOptions.None)
                // remove version label
                ?.Skip(1).FirstOrDefault()?.Split(new string[] { "="}, System.StringSplitOptions.None)
                // remove v prepend
                ?.Skip(1).FirstOrDefault()?.Replace("v", "");
            var metadata = assemblyMetadatas?.SelectMany(x => x.ConstructorArguments.Select(y => y.Value.ToString()));
            var metadataKeys = metadata?.Where((x, i) => i % 2 == 0).ToList();
            var metadataValues = metadata?.Skip(1).Where((x, i) => i % 2 == 0).ToList();
            var data = new AssemblyData
            {
                Name = assembly.FullName,
                ProductName = fileVersion.ProductName,
                FileDescription = fileVersion.FileDescription,
                Description = fileVersion.Comments,
                Company = fileVersion.CompanyName,
                Version = version.ToString(),
                FileVersion = fileVersion.FileVersion,
                ProductVersion = fileVersion.ProductVersion,
                IsClsCompliant = (bool?)clsCompliant?.ConstructorArguments?.FirstOrDefault().Value ?? false,
                InformationalVersion = assemblyInformationalVersion?.ConstructorArguments?.FirstOrDefault().Value.ToString(),
                Metadata = string.Join(",", metadataKeys.Select((x, i) => $"{x}={metadataValues[i].ToString()}")),
                IsStronglyNamed = assemblyName.GetPublicKeyToken().Length > 0,
                PublicKeyToken = string.Join("", assemblyName.GetPublicKeyToken().Select(b => b.ToString("x2"))),
                Framework = targetFrameworkName,
                FrameworkVersion = targetFrameworkVersion,
                Copyright = fileVersion.LegalCopyright,
                Build = assemblyConfiguration?.ConstructorArguments?.FirstOrDefault().Value.ToString(),
                IsDebug = fileVersion.IsDebug,
                DebuggableModes = debuggableAttributeArguments != null ? ((DebuggableAttribute.DebuggingModes)debuggableAttributeArguments.Value).ToString() : string.Empty,
                IsPatched = fileVersion.IsPatched,
                IsPreRelease = fileVersion.IsPreRelease,
                Language = fileVersion.Language,
                OriginalFilename = fileVersion.OriginalFilename,
                FileSize = Util.BytesToString(fileInfo.Length),
                FileLength = fileInfo.Length
            };

            if (data.FileLength < MaxHashInspectionLength)
                ComputeHash(data, filename);
            return data;
        }

        private void ComputeHash(AssemblyData data, string filename)
        {
            var bytes = File.ReadAllBytes(filename);
            using (var sha = SHA256.Create())
            {
                var shaHash = sha.ComputeHash(bytes);
                data.Sha = AssemblyInfo.Extensions.ByteConverter.ToHex(shaHash);
            }
            using (var md5 = MD5.Create())
            {
                var md5Hash = md5.ComputeHash(bytes);
                data.Md5 = AssemblyInfo.Extensions.ByteConverter.ToHex(md5Hash);
            }
        }
    }
}
