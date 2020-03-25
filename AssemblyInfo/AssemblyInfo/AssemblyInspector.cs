using AssemblyInfo.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Security.Cryptography;
using System.Text;

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
            var fileExtension = Path.GetExtension(filename).ToLower();
            Assembly assembly;
            try
            {
                switch (fileExtension)
                {
                    case ".msi":
                        return InspectMsi(filename);
                    case ".jpg":
                    case ".jpeg":
                    case ".gif":
                    case ".bmp":
                    case ".png":
                    case ".ico":
                    case ".pcx":
                    case ".webp":
                    case ".psd":
                    case ".xmp":
                    case ".mp4":
                    case ".mov":
                    case ".avi":
                    case ".qt":
                    case ".mpeg":
                    case ".m4v":
                        return new MediaInspector().InspectMedia(InspectFile(filename), filename);
                    default:
                        assembly = Assembly.LoadFrom(filename);
                        break;
                }               
            }
            catch (FileLoadException ex)
            {
                return InspectFile(filename);
            }
            catch (BadImageFormatException ex)
            {
                return InspectFile(filename);
            }

            return InspectAssembly(assembly);
        }

        private AssemblyData InspectMsi(string filename)
        {
            var data = InspectFile(filename);
            var inspector = new MsiInspector();
            var allProperties = new Dictionary<string, List<Dictionary<int, string>>>();
            var tables = inspector.GetAllProperties(filename, "_Tables");
            foreach (var table in tables)
            {
                var tableName = table.Values.Skip(1).First().ToString();
                allProperties.Add(tableName, inspector.GetAllProperties(filename, tableName));
            }
            var streams = inspector.GetAllProperties(filename, "_Streams");
            var storages = inspector.GetAllProperties(filename, "_Storages");
            var transformView = inspector.GetAllProperties(filename, "_TransformView");
            var properties = allProperties.Where(x => x.Key == "Property").Select(x => x.Value).FirstOrDefault();

            data.Version = GetProperty(properties, "ProductVersion");
            data.Name = GetProperty(properties, "ProductName");
            data.ProductName = GetProperty(properties, "ProductName");
            data.Company = GetProperty(properties, "Manufacturer");
            data.Metadata = GetAllMetadata(allProperties);
            return data;
        }

        private string GetAllMetadata(Dictionary<string, List<Dictionary<int, string>>> data)
        {
            var sb = new StringBuilder();
            foreach (var row in data)
            {
                sb.AppendLine(row.Key);
                sb.AppendLine($"----------------------");
                foreach (var prop in row.Value)
                    sb.AppendLine(string.Join(",", prop.Values));
                sb.AppendLine();
            }

            return sb.ToString();
        }

        private string GetProperty(List<Dictionary<int, string>> properties, string name)
        {
            return properties.Select(x => x.Values).FirstOrDefault(x => x.Skip(1).FirstOrDefault() == name).Skip(2).FirstOrDefault();
        }

        private AssemblyData InspectFile(string filename)
        {
            var fileInfo = new FileInfo(filename);
            var fileVersion = FileVersionInfo.GetVersionInfo(filename);
            var data = new AssemblyData
            {
                Name = Path.GetFileName(filename),
                Filename = Path.GetFileName(filename),
                ProductName = fileVersion.ProductName,
                FileDescription = fileVersion.FileDescription,
                Description = fileVersion.Comments,
                Company = fileVersion.CompanyName,
                FileVersion = fileVersion.FileVersion,
                ProductVersion = fileVersion.ProductVersion,
                Copyright = fileVersion.LegalCopyright,
                IsDebug = fileVersion.IsDebug,
                IsPatched = fileVersion.IsPatched,
                IsPreRelease = fileVersion.IsPreRelease,
                Language = fileVersion.Language,
                OriginalFilename = fileVersion.OriginalFilename,
                FileSize = Util.BytesToString(fileInfo.Length),
                FileLength = fileInfo.Length,
                FullPath = filename
            };
            if (fileInfo.Length < MaxHashInspectionLength)
                ComputeHash(data, filename);
            return data;
        }

        private AssemblyData InspectAssembly(Assembly assembly)
        {
            var fileInfo = new FileInfo(assembly.Location);
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

            var frameworkString = targetFramework?.ConstructorArguments?.FirstOrDefault().Value.ToString();
            var targetFrameworkName = frameworkString
                ?.Split(new string[] { "," }, System.StringSplitOptions.None)
                .FirstOrDefault();
            var targetFrameworkVersion = frameworkString
                ?.Split(new string[] { "," }, System.StringSplitOptions.None)
                // remove version label
                ?.Skip(1).FirstOrDefault()?.Split(new string[] { "=" }, System.StringSplitOptions.None)
                // remove v prepend
                ?.Skip(1).FirstOrDefault()?.Replace("v", "");
            var metadata = assemblyMetadatas?.SelectMany(x => x.ConstructorArguments.Select(y => y.Value.ToString()));
            var metadataKeys = metadata?.Where((x, i) => i % 2 == 0).ToList();
            var metadataValues = metadata?.Skip(1).Where((x, i) => i % 2 == 0).ToList();
            var data = new AssemblyData
            {
                Name = assembly.FullName,
                Filename = Path.GetFileName(assembly.Location),
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
                FileLength = fileInfo.Length,
                FullPath = assembly.Location
            };

            if (data.FileLength < MaxHashInspectionLength)
                ComputeHash(data, assembly.Location);
            return data;
        }

        private void ComputeHash(AssemblyData data, string filename)
        {
            var bytes = File.ReadAllBytes(filename);
            using (var sha = SHA256.Create())
            {
                var sha256Hash = sha.ComputeHash(bytes);
                data.Sha256 = AssemblyInfo.Extensions.ByteConverter.ToHex(sha256Hash).ToUpper();
            }
            using (var sha = SHA1.Create())
            {
                var shaHash = sha.ComputeHash(bytes);
                data.Sha = AssemblyInfo.Extensions.ByteConverter.ToHex(shaHash).ToUpper();
            }
            using (var md5 = MD5.Create())
            {
                var md5Hash = md5.ComputeHash(bytes);
                data.Md5 = AssemblyInfo.Extensions.ByteConverter.ToHex(md5Hash).ToUpper();
            }
        }
    }
}
