using System;
using System.IO;
using Services.Admin;

namespace BlueprintGen
{
    // [FixItFredComment:Sprint1007 - CLI usability enhancement] Enhanced with --help options and command documentation
    class Program
    {
        static void Main(string[] args)
        {
            // [FixItFredComment:Sprint1007 - CLI usability enhancement] Added command-line argument parsing
            if (args.Length > 0 && (args[0] == "--help" || args[0] == "-h" || args[0] == "/?" || args[0] == "help"))
            {
                ShowHelp();
                return;
            }

            try
            {
                // Default behavior - generate blueprint PDF
                string mdPath = Path.Combine("..", "..", "Docs", "Blueprints", "Service-Atlanta-Revitalize-MasterPlan.md");
                string pdfPath = Path.Combine("..", "..", "Docs", "Blueprints", "Service-Atlanta-Revitalize-MasterPlan.pdf");

                // [FixItFredComment:Sprint1007 - CLI usability enhancement] Allow custom input/output paths
                if (args.Length >= 1 && !args[0].StartsWith("-"))
                {
                    mdPath = args[0];
                }
                if (args.Length >= 2 && !args[1].StartsWith("-"))
                {
                    pdfPath = args[1];
                }

                if (!File.Exists(mdPath))
                {
                    Console.WriteLine($"Error: Input file not found: {mdPath}");
                    Console.WriteLine("Use --help for usage information.");
                    Environment.Exit(1);
                }

                string markdown = File.ReadAllText(mdPath);
                BlueprintPdfComposer.GenerateBlueprintPdf(markdown, pdfPath);
                Console.WriteLine($"✅ PDF generated successfully at: {pdfPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}");
                Console.WriteLine("Use --help for usage information.");
                Environment.Exit(1);
            }
        }

        // [FixItFredComment:Sprint1007 - CLI usability enhancement] Comprehensive help documentation
        static void ShowHelp()
        {
            Console.WriteLine("BlueprintGen - Service Atlanta Blueprint PDF Generator");
            Console.WriteLine("==================================================");
            Console.WriteLine();
            Console.WriteLine("DESCRIPTION:");
            Console.WriteLine("  Converts markdown blueprint documents to PDF format for Service Atlanta documentation.");
            Console.WriteLine();
            Console.WriteLine("USAGE:");
            Console.WriteLine("  BlueprintGen [input.md] [output.pdf] [options]");
            Console.WriteLine();
            Console.WriteLine("ARGUMENTS:");
            Console.WriteLine("  input.md      Path to input markdown file (default: ../../Docs/Blueprints/Service-Atlanta-Revitalize-MasterPlan.md)");
            Console.WriteLine("  output.pdf    Path to output PDF file (default: ../../Docs/Blueprints/Service-Atlanta-Revitalize-MasterPlan.pdf)");
            Console.WriteLine();
            Console.WriteLine("OPTIONS:");
            Console.WriteLine("  --help, -h, /?    Show this help message and exit");
            Console.WriteLine();
            Console.WriteLine("EXAMPLES:");
            Console.WriteLine("  BlueprintGen");
            Console.WriteLine("    Generate PDF using default paths");
            Console.WriteLine();
            Console.WriteLine("  BlueprintGen custom.md");
            Console.WriteLine("    Generate PDF from custom.md to default output location");
            Console.WriteLine();
            Console.WriteLine("  BlueprintGen input.md output.pdf");
            Console.WriteLine("    Generate PDF from input.md to output.pdf");
            Console.WriteLine();
            Console.WriteLine("  BlueprintGen --help");
            Console.WriteLine("    Show this help message");
            Console.WriteLine();
            Console.WriteLine("EXIT CODES:");
            Console.WriteLine("  0    Success");
            Console.WriteLine("  1    Error (file not found, conversion failed, etc.)");
            Console.WriteLine();
            Console.WriteLine("For more information, visit: https://service-atlanta.com/docs");
        }
    }
}
