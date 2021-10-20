using CommandLine;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace AssemblyInfo
{
    static class Program
    {
         [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool FreeConsole();

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static int Main(string[] args)
        {
            //Application.SetHighDpiMode(HighDpiMode.SystemAware);
            var returnValue = 0;
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed<Options>((o) =>
                {
                    var inspector = new AssemblyInspector();
                    if (!o.Print)
                    {
                        // launch gui
                        FreeConsole();
                        Application.EnableVisualStyles();
                        Application.SetCompatibleTextRenderingDefault(false);
                        Application.Run(new AssemlyInfoWindow(o, inspector));
                    }
                    else
                    {
                        // console output
                        if (File.Exists(o.Filename))
                        {
                            AssemblyData assemblyData;
                            try
                            {
                                assemblyData = inspector.Inspect(o.Filename);
                                if (assemblyData == null)
                                {
                                    Console.Error.WriteLine($"Error: File '{o.Filename}' is not a valid assembly.");
                                    returnValue = -2;
                                }
                            }
                            catch (FileNotFoundException)
                            {
                                Console.Error.WriteLine($"Error: Input file not found '{o.Filename}'");
                                returnValue = -1;
                                return;
                            }
                            
                            // output to stdout
                            var format = AssemblyDataFormatter.FormatterOutput.Parsable;
                            if (o.Pretty)
                                format = AssemblyDataFormatter.FormatterOutput.Pretty;

                            Console.WriteLine(AssemblyDataFormatter.GetAssemblyDataOutput(format, assemblyData));
                        }
                        else
                        {
                            Console.Error.WriteLine($"Error: Input file not found '{o.Filename}'");
                            returnValue = -1;
                        }
                    }
                })
                .WithNotParsed((errors) =>
                {
                    Console.Error.WriteLine($"Error: missing required arguments.");
                    foreach (var error in errors)
                        Console.Error.WriteLine($" - {error.ToString()}");
                    returnValue = 0;
                });
            return returnValue;
        }
    }
}
