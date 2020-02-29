using CommandLine;

namespace AssemblyInfo
{
    public class Options
    {
        /// <summary>
        /// Filename of assembly to inspect
        /// </summary>
        [Option('f', "filename", Required = false, HelpText = "Filename of assembly to inspect")]
        public string Filename { get; set; }

        /// <summary>
        /// Enable output to console
        /// </summary>
        [Option('p', "print", Required = false, HelpText = "Enable output to console")]
        public bool Print { get; set; }

        /// <summary>
        /// Enable pretty output to console
        /// </summary>
        [Option("pretty", Required = false, HelpText = "Enable pretty output to console")]
        public bool Pretty { get; set; }
    }
}
