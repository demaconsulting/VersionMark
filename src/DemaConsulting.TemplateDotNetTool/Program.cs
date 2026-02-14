// Copyright (c) DEMA Consulting
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System.Reflection;

namespace DemaConsulting.TemplateDotNetTool;

/// <summary>
///     Main program entry point for the Template DotNet Tool.
/// </summary>
internal static class Program
{
    /// <summary>
    ///     Gets the application version string.
    /// </summary>
    public static string Version
    {
        get
        {
            // Get the assembly containing this program
            var assembly = typeof(Program).Assembly;

            // Try to get version from assembly attributes, fallback to AssemblyVersion, or default to 0.0.0
            return assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
                   ?? assembly.GetName().Version?.ToString()
                   ?? "0.0.0";
        }
    }

    /// <summary>
    ///     Main entry point for the Template DotNet Tool.
    /// </summary>
    /// <param name="args">Command-line arguments.</param>
    /// <returns>Exit code: 0 for success, non-zero for failure.</returns>
    private static int Main(string[] args)
    {
        try
        {
            // Create context from command-line arguments
            using var context = Context.Create(args);

            // Run the program logic
            Run(context);

            // Return the exit code from the context
            return context.ExitCode;
        }
        catch (ArgumentException ex)
        {
            // Print expected argument exceptions and return error code
            Console.WriteLine($"Error: {ex.Message}");
            return 1;
        }
        catch (InvalidOperationException ex)
        {
            // Print expected operation exceptions and return error code
            Console.WriteLine($"Error: {ex.Message}");
            return 1;
        }
        catch (Exception ex)
        {
            // Print unexpected exceptions and re-throw to generate event logs
            Console.WriteLine($"Unexpected error: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    ///     Runs the program logic based on the provided context.
    /// </summary>
    /// <param name="context">The context containing command line arguments and program state.</param>
    public static void Run(Context context)
    {
        // Priority 1: Version query
        if (context.Version)
        {
            context.WriteLine(Version);
            return;
        }

        // Print application banner
        PrintBanner(context);

        // Priority 2: Help
        if (context.Help)
        {
            PrintHelp(context);
            return;
        }

        // Priority 3: Self-Validation
        if (context.Validate)
        {
            Validation.Run(context);
            return;
        }

        // Priority 4: Capture command
        if (context.Capture)
        {
            RunCapture(context);
            return;
        }

        // Priority 5: Main tool functionality
        RunToolLogic(context);
    }

    /// <summary>
    ///     Prints the application banner.
    /// </summary>
    /// <param name="context">The context for output.</param>
    private static void PrintBanner(Context context)
    {
        context.WriteLine($"Template DotNet Tool version {Version}");
        context.WriteLine("Copyright (c) DEMA Consulting");
        context.WriteLine("");
    }

    /// <summary>
    ///     Prints usage information.
    /// </summary>
    /// <param name="context">The context for output.</param>
    private static void PrintHelp(Context context)
    {
        context.WriteLine("Usage: templatetool [options]");
        context.WriteLine("       templatetool --capture --job-id <id> [options] [-- tool1 tool2 ...]");
        context.WriteLine("");
        context.WriteLine("Options:");
        context.WriteLine("  -v, --version              Display version information");
        context.WriteLine("  -?, -h, --help             Display this help message");
        context.WriteLine("  --silent                   Suppress console output");
        context.WriteLine("  --validate                 Run self-validation");
        context.WriteLine("  --results <file>           Write validation results to file (.trx or .xml)");
        context.WriteLine("  --log <file>               Write output to log file");
        context.WriteLine("");
        context.WriteLine("Capture Mode:");
        context.WriteLine("  --capture                  Capture tool versions");
        context.WriteLine("  --job-id <id>              Job ID for this capture (required)");
        context.WriteLine("  --output <file>            Output JSON file (default: versionmark-<job-id>.json)");
        context.WriteLine("  -- <tools...>              List of tool names to capture (default: all tools)");
    }

    /// <summary>
    ///     Runs the capture command logic.
    /// </summary>
    /// <param name="context">The context containing command line arguments and program state.</param>
    private static void RunCapture(Context context)
    {
        // Validate required arguments
        if (string.IsNullOrEmpty(context.JobId))
        {
            context.WriteError("Error: --job-id is required for capture mode");
            return;
        }

        // Determine output file path
        var outputFile = context.OutputFile ?? $"versionmark-{context.JobId}.json";

        context.WriteLine($"Capturing tool versions for job '{context.JobId}'...");
        context.WriteLine($"Output file: {outputFile}");

        try
        {
            // Load configuration from default location
            var config = VersionMarkConfig.ReadFromFile(".versionmark.yaml");

            // Determine which tools to capture
            var toolNames = context.ToolNames.Length > 0
                ? context.ToolNames
                : config.Tools.Keys.ToArray();

            context.WriteLine($"Capturing {toolNames.Length} tool(s)...");

            // Capture versions
            var versionInfo = config.FindVersions(toolNames, context.JobId);

            // Save to file
            versionInfo.SaveToFile(outputFile);

            // Display the captured versions to the user for verification
            context.WriteLine("");
            context.WriteLine("Captured versions:");
            foreach (var (tool, version) in versionInfo.Versions)
            {
                context.WriteLine($"  {tool}: {version}");
            }

            // Confirm successful save with output file location
            context.WriteLine("");
            context.WriteLine($"Version information saved to {outputFile}");
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            context.WriteError($"Error: {ex.Message}");
        }
    }

    /// <summary>
    ///     Runs the main tool logic.
    /// </summary>
    /// <param name="context">The context containing command line arguments and program state.</param>
    private static void RunToolLogic(Context context)
    {
        context.WriteLine("Template DotNet Tool - Demo Functionality");
        context.WriteLine("This is a template project demonstrating best practices.");
        context.WriteLine("");
        context.WriteLine("Replace this with your actual tool implementation.");
    }
}
