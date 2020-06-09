namespace AssemblyInfo
{
    public class AssemblyData
    {
        public AssemblyData() { }
        public AssemblyData(string message)
        {
            Messages = message;
        }

        public string Name { get; set; }
        public string ProductName { get; set; }
        public string FileDescription { get; set; }
        public string Description { get; set; }
        public string Company { get; set; }
        public string Version { get; set; }
        public string FileVersion { get; set; }
        public string ProductVersion { get; set; }
        public bool IsStronglyNamed { get; set; }
        public string PublicKeyToken { get; set; }
        public string Framework { get; set; }
        public string FrameworkVersion { get; set; }
        public string Copyright { get; set; }
        public string Build { get; set; }
        public string DebuggableModes { get; set; }
        public bool IsDebug { get; set; }
        public bool IsPatched { get; set; }
        public bool IsPreRelease { get; set; }
        public string Language { get; set; }
        public string OriginalFilename { get; set; }
        public string FileSize { get; set; }
        public long FileLength { get; set; }
        public string Sha256 { get; set; }
        public string Sha { get; set; }
        public string Md5 { get; set; }
        public bool IsClsCompliant { get; set; }
        public string InformationalVersion { get; set; }
        public string Metadata { get; set; }
        public string FullPath { get; set; }
        public string Filename { get; set; }
        public string Dependencies { get; set; }
        public string DeclaredTypes { get; set; }
        public string EmbeddedResources { get; set; }

        /// <summary>
        /// Warnings and Errors
        /// </summary>
        public string Messages { get; set; }

        public override string ToString() => Name;
    }
}
