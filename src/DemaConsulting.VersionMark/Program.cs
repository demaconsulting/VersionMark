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
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;

namespace DemaConsulting.VersionMark;

/// <summary>
///     Main program entry point for the VersionMark.
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
    ///     Main entry point for the VersionMark.
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

        // Priority 4.5: Publish command
        if (context.Publish)
        {
            RunPublish(context);
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
        context.WriteLine($"VersionMark version {Version}");
        context.WriteLine("Copyright (c) DEMA Consulting");
        context.WriteLine("");
    }

    /// <summary>
    ///     Prints usage information.
    /// </summary>
    /// <param name="context">The context for output.</param>
    private static void PrintHelp(Context context)
    {
        context.WriteLine("Usage: versionmark [options]");
        context.WriteLine("       versionmark --capture --job-id <id> [options] [-- tool1 tool2 ...]");
        context.WriteLine("       versionmark --publish --report <file> [options] [-- pattern1 pattern2 ...]");
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
        context.WriteLine("");
        context.WriteLine("Publish Mode:");
        context.WriteLine("  --publish                  Generate markdown report from JSON files");
        context.WriteLine("  --report <file>            Output markdown file (required)");
        context.WriteLine("  --report-depth <depth>     Heading depth for markdown (default: 2)");
        context.WriteLine("  -- <patterns...>           Glob patterns for JSON files (default: versionmark-*.json)");
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
    ///     Runs the publish command logic.
    /// </summary>
    /// <param name="context">The context containing command line arguments and program state.</param>
    private static void RunPublish(Context context)
    {
        // Validate required arguments
        if (string.IsNullOrEmpty(context.ReportFile))
        {
            context.WriteError("Error: --report is required for publish mode");
            return;
        }

        context.WriteLine($"Publishing version report to '{context.ReportFile}'...");

        try
        {
            // Get glob patterns (default to versionmark-*.json if none specified)
            var globPatterns = context.GlobPatterns.Length > 0
                ? context.GlobPatterns
                : new[] { "versionmark-*.json" };

            context.WriteLine($"Searching for JSON files with patterns: {string.Join(", ", globPatterns)}");

            // Find matching JSON files using glob patterns
            var jsonFiles = FindMatchingFiles(globPatterns);

            // Check if any files were found
            if (jsonFiles.Count == 0)
            {
                context.WriteError($"Error: No JSON files found matching patterns: {string.Join(", ", globPatterns)}");
                return;
            }

            context.WriteLine($"Found {jsonFiles.Count} JSON file(s)");

            // Load all version info files
            var versionInfos = LoadVersionInfoFiles(jsonFiles);

            context.WriteLine($"Loaded version information from {versionInfos.Count} file(s)");

            // Generate markdown report
            var markdown = MarkdownFormatter.Format(versionInfos, context.ReportDepth);

            // Write markdown to report file
            File.WriteAllText(context.ReportFile, markdown, System.Text.Encoding.UTF8);

            context.WriteLine("");
            context.WriteLine($"Report successfully written to {context.ReportFile}");
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            context.WriteError($"Error: {ex.Message}");
        }
    }

    /// <summary>
    ///     Finds files matching the specified glob patterns.
    /// </summary>
    /// <param name="globPatterns">Array of glob patterns to match.</param>
    /// <returns>List of matching file paths.</returns>
    private static List<string> FindMatchingFiles(string[] globPatterns)
    {
        var matcher = new Matcher();

        // Add all glob patterns to the matcher
        foreach (var pattern in globPatterns)
        {
            matcher.AddInclude(pattern);
        }

        // Execute the match against the current directory
        var result = matcher.Execute(new DirectoryInfoWrapper(new DirectoryInfo(Directory.GetCurrentDirectory())));

        // Return the full paths of matched files
        return result.Files
            .Select(f => Path.GetFullPath(f.Path))
            .OrderBy(f => f, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    /// <summary>
    ///     Loads VersionInfo instances from the specified JSON files.
    /// </summary>
    /// <param name="jsonFiles">List of JSON file paths to load.</param>
    /// <returns>List of loaded VersionInfo instances.</returns>
    /// <exception cref="ArgumentException">Thrown when a file cannot be read or parsed.</exception>
    private static List<VersionInfo> LoadVersionInfoFiles(List<string> jsonFiles)
    {
        var versionInfos = new List<VersionInfo>();

        // Load each JSON file
        foreach (var file in jsonFiles)
        {
            var versionInfo = VersionInfo.LoadFromFile(file);
            versionInfos.Add(versionInfo);
        }

        return versionInfos;
    }

    /// <summary>
    ///     Runs the main tool logic.
    /// </summary>
    /// <param name="context">The context containing command line arguments and program state.</param>
    private static void RunToolLogic(Context context)
    {
        context.WriteLine("VersionMark - Demo Functionality");
        context.WriteLine("This is a template project demonstrating best practices.");
        context.WriteLine("");
        context.WriteLine("Replace this with your actual tool implementation.");
    }
}
