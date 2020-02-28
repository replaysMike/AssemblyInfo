using CommandLine;
using System;
using System.Windows.Forms;

namespace AssemblyInfo
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed<Options>((o) =>
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new AssemlyInfoWindow(o));
                })
                .WithNotParsed((errors) =>
                {
                    Console.Error.WriteLine($"Error: missing required arguments.");
                    foreach (var error in errors)
                        Console.Error.WriteLine($" - {error.ToString()}");
                });
        }
    }
}
