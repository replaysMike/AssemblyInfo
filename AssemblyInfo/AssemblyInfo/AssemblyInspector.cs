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
    [Serializable]
    public class AssemblyInspector
    {
        /// <summary>
        /// Max 1GB hash inspection length
        /// </summary>
        private const int MaxHashInspectionLength = 1 * 1024 * 1024 * 1024;

        private List<Assembly> ResolvedAssemblies = new List<Assembly>();
        private string _filename = string.Empty;

        public AssemblyData Inspect(string filename)
        {
            _filename = filename;
            if (File.Exists(filename))
                return InspectAssembly(filename);
            throw new FileNotFoundException(filename);
        }

        private AssemblyData InspectAssembly(string filename)
        {
            var fileExtension = Path.GetExtension(filename).ToLower();
            Assembly assembly = null;
            try
            {
                switch (fileExtension)
                {
                    case ".msi":
                        return InspectMsi(filename, new AssemblyData());
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
                        return new MediaInspector().InspectMedia(InspectFile(filename, new AssemblyData()), filename);
                    default:
                        AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;
                        AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
                        assembly = Assembly.LoadFrom(filename);
                        break;
                }
            }
            catch (FileLoadException ex)
            {
                return InspectFile(filename, new AssemblyData($"Could not load file! {ex.Message}"));
            }
            catch (BadImageFormatException ex)
            {
                return InspectFile(filename, new AssemblyData($"Unknown assembly type. {ex.Message}"));
            }

            return InspectAssembly(assembly, new AssemblyData());
        }

        private void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            if (!ResolvedAssemblies.Contains(args.LoadedAssembly))
                ResolvedAssemblies.Add(args.LoadedAssembly);
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (args.Name.Contains(".resources"))
                return null;
            if (ResolvedAssemblies.Any(x => x.FullName == args.Name))
                return ResolvedAssemblies.FirstOrDefault(x => x.FullName.Equals(args.Name));
            var tokens = args.Name.Split(",".ToCharArray());
            var assemblyFile = Path.Combine(new string[] { Path.GetDirectoryName(_filename), tokens[0] + ".dll" });
            System.Diagnostics.Debug.WriteLine("Resolving : " + args.Name);
            Assembly assembly;
            if (File.Exists(assemblyFile))
            {
                assembly = Assembly.LoadFile(assemblyFile);
                ResolvedAssemblies.Add(assembly);
                return assembly;
            }
            return null;
        }

        private AssemblyData InspectMsi(string filename, AssemblyData assemblyData)
        {
            var data = InspectFile(filename, assemblyData);
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

        private AssemblyData InspectFile(string filename, AssemblyData assemblyData)
        {
            var fileInfo = new FileInfo(filename);
            var fileVersion = FileVersionInfo.GetVersionInfo(filename);

            assemblyData.Name = Path.GetFileName(filename);
            assemblyData.Filename = Path.GetFileName(filename);
            assemblyData.ProductName = fileVersion.ProductName;
            assemblyData.FileDescription = fileVersion.FileDescription;
            assemblyData.Description = fileVersion.Comments;
            assemblyData.Company = fileVersion.CompanyName;
            assemblyData.FileVersion = fileVersion.FileVersion;
            assemblyData.ProductVersion = fileVersion.ProductVersion;
            assemblyData.Copyright = fileVersion.LegalCopyright;
            assemblyData.IsDebug = fileVersion.IsDebug;
            assemblyData.IsPatched = fileVersion.IsPatched;
            assemblyData.IsPreRelease = fileVersion.IsPreRelease;
            assemblyData.Language = fileVersion.Language;
            assemblyData.OriginalFilename = fileVersion.OriginalFilename;
            assemblyData.FileSize = Util.BytesToString(fileInfo.Length);
            assemblyData.FileLength = fileInfo.Length;
            assemblyData.FullPath = filename;

            if (fileInfo.Length < MaxHashInspectionLength)
                ComputeHash(assemblyData, filename);
            return assemblyData;
        }

        private AssemblyData InspectAssembly(Assembly assembly, AssemblyData assemblyData)
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
            var debuggableAttributeArguments = debuggable?.ConstructorArguments.FirstOrDefault();

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
            var declaredTypes = string.Empty;
            try
            {
                declaredTypes = string.Join(Environment.NewLine, assembly.DefinedTypes.Select(x => BuildTypeName(x)));
            }
            catch (ReflectionTypeLoadException ex)
            {
                declaredTypes = string.Join(Environment.NewLine, ex.Types.Select(x => BuildTypeName(x)));
                declaredTypes += $"{Environment.NewLine}LOAD FAILED TYPES{Environment.NewLine}";
                foreach(var loaderException in ex.LoaderExceptions)
                    declaredTypes += $"{loaderException.GetBaseException().Message}{Environment.NewLine}";
            }
            var assemblyNames = assembly.GetReferencedAssemblies();
            var loadedAssemblies = assemblyNames.Select(x =>
            {
                Assembly loadedAssembly = null;
                try
                {
                    loadedAssembly = Assembly.Load(x);
                }
                catch (Exception) { }
                return new { Name = x.FullName, Assembly = loadedAssembly };
            }).ToList();
            var dependentAssemblies = ResolvedAssemblies.Distinct().Select(x => new { Name = x.FullName, Assembly = x }).ToList();
            var dependencies = string.Join(Environment.NewLine, dependentAssemblies.Select(x => $"{x.Name}, {x.Assembly?.Location}"));
            var embeddedResources = assembly.GetManifestResourceNames();

            assemblyData.Name = assembly.FullName;
            assemblyData.Filename = Path.GetFileName(assembly.Location);
            assemblyData.ProductName = fileVersion.ProductName;
            assemblyData.FileDescription = fileVersion.FileDescription;
            assemblyData.Description = fileVersion.Comments;
            assemblyData.Company = fileVersion.CompanyName;
            assemblyData.Version = version.ToString();
            assemblyData.FileVersion = fileVersion.FileVersion;
            assemblyData.ProductVersion = fileVersion.ProductVersion;
            assemblyData.IsClsCompliant = (bool?)clsCompliant?.ConstructorArguments?.FirstOrDefault().Value ?? false;
            assemblyData.InformationalVersion = assemblyInformationalVersion?.ConstructorArguments?.FirstOrDefault().Value.ToString();
            assemblyData.Metadata = string.Join(",", metadataKeys.Select((x, i) => $"{x}={metadataValues[i].ToString()}"));
            assemblyData.IsStronglyNamed = assemblyName.GetPublicKeyToken().Length > 0;
            assemblyData.PublicKeyToken = string.Join("", assemblyName.GetPublicKeyToken().Select(b => b.ToString("x2")));
            assemblyData.Framework = targetFrameworkName;
            assemblyData.FrameworkVersion = targetFrameworkVersion;
            assemblyData.Copyright = fileVersion.LegalCopyright;
            assemblyData.Build = assemblyConfiguration?.ConstructorArguments?.FirstOrDefault().Value.ToString();
            assemblyData.IsDebug = fileVersion.IsDebug;
            assemblyData.DebuggableModes = debuggableAttributeArguments.HasValue ? ((DebuggableAttribute.DebuggingModes)debuggableAttributeArguments.Value.Value).ToString() : string.Empty;
            assemblyData.IsPatched = fileVersion.IsPatched;
            assemblyData.IsPreRelease = fileVersion.IsPreRelease;
            assemblyData.Language = fileVersion.Language;
            assemblyData.OriginalFilename = fileVersion.OriginalFilename;
            assemblyData.FileSize = Util.BytesToString(fileInfo.Length);
            assemblyData.FileLength = fileInfo.Length;
            assemblyData.FullPath = assembly.Location;
            assemblyData.DeclaredTypes = declaredTypes;
            assemblyData.Dependencies = dependencies;
            assemblyData.EmbeddedResources = string.Join(Environment.NewLine, embeddedResources);

            if (assemblyData.FileLength < MaxHashInspectionLength)
                ComputeHash(assemblyData, assembly.Location);
            return assemblyData;
        }

        private string BuildTypeName(Type type)
        {
            if (type == null)
                return string.Empty;
            if (type.IsGenericType)
            {
                var name = type.FullName;
                var startPos = -1;
                var len = 0;
                var parameters = ((TypeInfo)type).GenericTypeParameters;
                for (var i = 0; i < parameters.Length; i++)
                {
                    var key = $"`{(parameters[i].GenericParameterPosition + 1)}";
                    var index = name.IndexOf(key);
                    if (startPos < 0 && index >= 0)
                        startPos = index;
                    if (index >= 0)
                    {
                        len += parameters[i].Name.Length;
                        name = name.Replace(key, parameters[i].Name);
                    }
                }
                if (len > 0)
                {
                    name = name.Insert(startPos, "<");
                    name = name.Insert(startPos + len + 1, ">");
                }
                return $"{type.FullName} ({name})";
            }
            return type.FullName;
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
