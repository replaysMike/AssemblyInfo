using CommandLine;

namespace AssemblyInfo
{
    public class Options
    {
        /// <summary>
        /// Filename of assembly to inspect
        /// </summary>
        [Option('f', "filename", Required = true, HelpText = "Filename of assembly to inspect")]
        public string Filename { get; set; }
    }
}
