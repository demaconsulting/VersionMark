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

namespace DemaConsulting.VersionMark.Tests;

/// <summary>
///     Unit tests for the Program class.
/// </summary>
[TestClass]
public class ProgramTests
{
    /// <summary>
    ///     Test that Run with version flag displays version only.
    /// </summary>
    [TestMethod]
    public void Program_Run_WithVersionFlag_DisplaysVersionOnly()
    {
        var originalOut = Console.Out;
        try
        {
            using var outWriter = new StringWriter();
            Console.SetOut(outWriter);
            using var context = Context.Create(["--version"]);

            Program.Run(context);

            var output = outWriter.ToString();
            Assert.DoesNotContain("Copyright", output);
            Assert.DoesNotContain("VersionMark version", output);
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    /// <summary>
    ///     Test that Run with help flag displays usage information.
    /// </summary>
    [TestMethod]
    public void Program_Run_WithHelpFlag_DisplaysUsageInformation()
    {
        var originalOut = Console.Out;
        try
        {
            using var outWriter = new StringWriter();
            Console.SetOut(outWriter);
            using var context = Context.Create(["--help"]);

            Program.Run(context);

            var output = outWriter.ToString();
            Assert.Contains("Usage:", output);
            Assert.Contains("Options:", output);
            Assert.Contains("--version", output);
            Assert.Contains("--help", output);
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    /// <summary>
    ///     Test that Run with validate flag runs validation.
    /// </summary>
    [TestMethod]
    public void Program_Run_WithValidateFlag_RunsValidation()
    {
        var originalOut = Console.Out;
        try
        {
            using var outWriter = new StringWriter();
            Console.SetOut(outWriter);
            using var context = Context.Create(["--validate"]);

            Program.Run(context);

            var output = outWriter.ToString();
            Assert.Contains("Total Tests:", output);
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    /// <summary>
    ///     Test that Run with no arguments displays default behavior.
    /// </summary>
    [TestMethod]
    public void Program_Run_NoArguments_DisplaysDefaultBehavior()
    {
        var originalOut = Console.Out;
        try
        {
            using var outWriter = new StringWriter();
            Console.SetOut(outWriter);
            using var context = Context.Create([]);

            Program.Run(context);

            var output = outWriter.ToString();
            Assert.Contains("VersionMark version", output);
            Assert.Contains("Copyright", output);
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    /// <summary>
    ///     Test that version property returns non-empty version string.
    /// </summary>
    [TestMethod]
    public void Program_Version_ReturnsNonEmptyString()
    {
        var version = Program.Version;
        Assert.IsFalse(string.IsNullOrWhiteSpace(version));
    }

    /// <summary>
    ///     Test that Run with capture command captures tool versions.
    /// </summary>
    [TestMethod]
    public void Program_Run_WithCaptureCommand_CapturesToolVersions()
    {
        // Arrange - Set up unique temp directory with config file and redirect console output
        var currentDir = Directory.GetCurrentDirectory();
        var tempDir = PathHelpers.SafePathCombine(Path.GetTempPath(), Path.GetRandomFileName());
        var tempConfigFile = PathHelpers.SafePathCombine(tempDir, ".versionmark.yaml");
        var outputFile = PathHelpers.SafePathCombine(tempDir, "output.json");

        try
        {
            // Create unique temp directory for this test
            Directory.CreateDirectory(tempDir);

            // Create a temporary config file in temp directory
            File.WriteAllText(tempConfigFile, @"
tools:
  dotnet:
    command: dotnet --version
    regex: '(?<version>\d+\.\d+\.\d+)'
");

            // Change to temp directory so it finds the config
            Directory.SetCurrentDirectory(tempDir);

            var originalOut = Console.Out;
            try
            {
                using var outWriter = new StringWriter();
                Console.SetOut(outWriter);
                using var context = Context.Create([
                    "--capture",
                    "--job-id", "test-job",
                    "--output", outputFile,
                    "--", "dotnet"
                ]);

                // Act - Execute the capture command via Program.Run
                Program.Run(context);

                // Assert - Verify the command output, exit code, and captured data
                var output = outWriter.ToString();
                Assert.Contains("Capturing tool versions", output);
                Assert.Contains("test-job", output);
                Assert.Contains("dotnet", output);
                Assert.AreEqual(0, context.ExitCode);

                // Verify output file was created
                Assert.IsTrue(File.Exists(outputFile), "Output file was not created");

                // Verify output file contains expected data
                var versionInfo = VersionInfo.LoadFromFile(outputFile);
                Assert.AreEqual("test-job", versionInfo.JobId);
                Assert.IsTrue(versionInfo.Versions.ContainsKey("dotnet"));
            }
            finally
            {
                Console.SetOut(originalOut);
            }
        }
        finally
        {
            // Restore original directory
            Directory.SetCurrentDirectory(currentDir);

            // Clean up temp directory and all its contents
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, recursive: true);
            }
        }
    }

    /// <summary>
    ///     Test that Run with capture command without job ID fails.
    /// </summary>
    [TestMethod]
    public void Program_Run_WithCaptureCommandWithoutJobId_ReturnsError()
    {
        // Arrange - Set up context with capture flag but no job-id
        var originalOut = Console.Out;
        try
        {
            using var outWriter = new StringWriter();
            Console.SetOut(outWriter);
            using var context = Context.Create(["--capture"]);

            // Act - Run the program with incomplete capture arguments
            Program.Run(context);

            // Assert - Verify error message and non-zero exit code
            var output = outWriter.ToString();
            Assert.Contains("--job-id is required", output);
            Assert.AreEqual(1, context.ExitCode);
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    /// <summary>
    ///     Test that Run with capture command with missing config file fails.
    /// </summary>
    [TestMethod]
    public void Program_Run_WithCaptureCommandWithMissingConfig_ReturnsError()
    {
        // Arrange - Set up context in current directory (no .versionmark.yaml expected here)
        var originalOut = Console.Out;
        try
        {
            using var outWriter = new StringWriter();
            Console.SetOut(outWriter);
            using var context = Context.Create([
                "--capture",
                "--job-id", "test-job"
            ]);

            // Act - Run capture command without available config file
            Program.Run(context);

            // Assert - Verify error is reported and exit code indicates failure
            var output = outWriter.ToString();
            Assert.Contains("Error:", output);
            Assert.AreEqual(1, context.ExitCode);
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    /// <summary>
    ///     Test that Run with publish command requires --report parameter.
    ///     What is tested: PUB-004 - --report parameter is required in publish mode
    ///     What the assertions prove: Program exits with error when --report is missing
    /// </summary>
    [TestMethod]
    public void Program_Run_WithPublishCommandWithoutReport_ReturnsError()
    {
        // Arrange - Set up context without --report parameter
        var originalOut = Console.Out;
        try
        {
            using var outWriter = new StringWriter();
            Console.SetOut(outWriter);
            using var context = Context.Create(["--publish"]);

            // Act - Run publish command without --report parameter
            Program.Run(context);

            // Assert - Verify error message and non-zero exit code
            // What is proved: --publish without --report results in an error
            var output = outWriter.ToString();
            Assert.Contains("Error: --report is required for publish mode", output);
            Assert.AreEqual(1, context.ExitCode);
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    /// <summary>
    ///     Test that Run with publish command handles no matching files error.
    ///     What is tested: PUB-007 - Error reported when no JSON files match glob patterns
    ///     What the assertions prove: Program exits with error when no files are found
    /// </summary>
    [TestMethod]
    public void Program_Run_WithPublishCommandNoMatchingFiles_ReturnsError()
    {
        // Arrange - Set up unique temp directory with no JSON files
        var currentDir = Directory.GetCurrentDirectory();
        var tempDir = PathHelpers.SafePathCombine(Path.GetTempPath(), Path.GetRandomFileName());
        var reportFile = PathHelpers.SafePathCombine(tempDir, "report.md");

        try
        {
            Directory.CreateDirectory(tempDir);
            Directory.SetCurrentDirectory(tempDir);

            var originalOut = Console.Out;
            try
            {
                using var outWriter = new StringWriter();
                Console.SetOut(outWriter);
                using var context = Context.Create([
                    "--publish",
                    "--report", reportFile,
                    "--", "nonexistent-*.json"
                ]);

                // Act - Run publish command with pattern that matches no files
                Program.Run(context);

                // Assert - Verify error message and non-zero exit code
                // What is proved: No matching files results in an error
                var output = outWriter.ToString();
                Assert.Contains("Error: No JSON files found matching patterns:", output);
                Assert.AreEqual(1, context.ExitCode);
            }
            finally
            {
                Console.SetOut(originalOut);
            }
        }
        finally
        {
            Directory.SetCurrentDirectory(currentDir);
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
        }
    }

    /// <summary>
    ///     Test that Run with publish command handles invalid JSON files.
    ///     What is tested: PUB-008 - Error reported when JSON files cannot be parsed
    ///     What the assertions prove: Program exits with error when JSON is malformed
    /// </summary>
    [TestMethod]
    public void Program_Run_WithPublishCommandInvalidJson_ReturnsError()
    {
        // Arrange - Set up unique temp directory with invalid JSON file
        var currentDir = Directory.GetCurrentDirectory();
        var tempDir = PathHelpers.SafePathCombine(Path.GetTempPath(), Path.GetRandomFileName());
        var invalidJsonFile = PathHelpers.SafePathCombine(tempDir, "versionmark-invalid.json");
        var reportFile = PathHelpers.SafePathCombine(tempDir, "report.md");

        try
        {
            Directory.CreateDirectory(tempDir);
            Directory.SetCurrentDirectory(tempDir);

            // Create invalid JSON file
            File.WriteAllText(invalidJsonFile, "{ this is not valid JSON }");

            var originalOut = Console.Out;
            try
            {
                using var outWriter = new StringWriter();
                Console.SetOut(outWriter);
                using var context = Context.Create([
                    "--publish",
                    "--report", reportFile
                ]);

                // Act - Run publish command with invalid JSON file
                Program.Run(context);

                // Assert - Verify error message and non-zero exit code
                // What is proved: Invalid JSON results in an error
                var output = outWriter.ToString();
                Assert.Contains("Error: Failed to parse JSON file", output);
                Assert.AreEqual(1, context.ExitCode);
            }
            finally
            {
                Console.SetOut(originalOut);
            }
        }
        finally
        {
            Directory.SetCurrentDirectory(currentDir);
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
        }
    }

    /// <summary>
    ///     Test that Run with publish command generates markdown report.
    ///     What is tested: PUB-001, PUB-002, PUB-005, PUB-006, FMT-001 - Publish mode generates report
    ///     What the assertions prove: Valid JSON files are processed and markdown report is created
    /// </summary>
    [TestMethod]
    public void Program_Run_WithPublishCommand_GeneratesMarkdownReport()
    {
        // Arrange - Set up unique temp directory with multiple JSON files
        var currentDir = Directory.GetCurrentDirectory();
        var tempDir = PathHelpers.SafePathCombine(Path.GetTempPath(), Path.GetRandomFileName());
        var json1File = PathHelpers.SafePathCombine(tempDir, "versionmark-job1.json");
        var json2File = PathHelpers.SafePathCombine(tempDir, "versionmark-job2.json");
        var reportFile = PathHelpers.SafePathCombine(tempDir, "report.md");

        try
        {
            Directory.CreateDirectory(tempDir);
            Directory.SetCurrentDirectory(tempDir);

            // Create test JSON files
            var versionInfo1 = new VersionInfo(
                "job-1",
                new Dictionary<string, string>
                {
                    ["dotnet"] = "8.0.0",
                    ["node"] = "18.0.0"
                });
            var versionInfo2 = new VersionInfo(
                "job-2",
                new Dictionary<string, string>
                {
                    ["dotnet"] = "8.0.0",
                    ["node"] = "20.0.0"
                });

            versionInfo1.SaveToFile(json1File);
            versionInfo2.SaveToFile(json2File);

            var originalOut = Console.Out;
            try
            {
                using var outWriter = new StringWriter();
                Console.SetOut(outWriter);
                using var context = Context.Create([
                    "--publish",
                    "--report", reportFile
                ]);

                // Act - Run publish command
                Program.Run(context);

                // Assert - Verify report was created successfully
                // What is proved: Publish mode generates a markdown report from JSON files
                Assert.AreEqual(0, context.ExitCode);
                Assert.IsTrue(File.Exists(reportFile), "Report file was not created");

                var reportContent = File.ReadAllText(reportFile);
                Assert.Contains("## Tool Versions", reportContent);
                Assert.Contains("dotnet", reportContent);
                Assert.Contains("node", reportContent);
                Assert.Contains("8.0.0", reportContent);
            }
            finally
            {
                Console.SetOut(originalOut);
            }
        }
        finally
        {
            Directory.SetCurrentDirectory(currentDir);
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
        }
    }

    /// <summary>
    ///     Test that Run with publish command respects report-depth parameter.
    ///     What is tested: PUB-003, FMT-005 - Report depth controls markdown heading levels
    ///     What the assertions prove: Custom report depth is applied to generated markdown
    /// </summary>
    [TestMethod]
    public void Program_Run_WithPublishCommandCustomDepth_AdjustsHeadingLevels()
    {
        // Arrange - Set up unique temp directory with JSON file
        var currentDir = Directory.GetCurrentDirectory();
        var tempDir = PathHelpers.SafePathCombine(Path.GetTempPath(), Path.GetRandomFileName());
        var jsonFile = PathHelpers.SafePathCombine(tempDir, "versionmark-job1.json");
        var reportFile = PathHelpers.SafePathCombine(tempDir, "report.md");

        try
        {
            Directory.CreateDirectory(tempDir);
            Directory.SetCurrentDirectory(tempDir);

            // Create test JSON file
            var versionInfo = new VersionInfo(
                "job-1",
                new Dictionary<string, string> { ["dotnet"] = "8.0.0" });
            versionInfo.SaveToFile(jsonFile);

            var originalOut = Console.Out;
            try
            {
                using var outWriter = new StringWriter();
                Console.SetOut(outWriter);
                using var context = Context.Create([
                    "--publish",
                    "--report", reportFile,
                    "--report-depth", "3"
                ]);

                // Act - Run publish command with custom depth
                Program.Run(context);

                // Assert - Verify report uses custom heading depth
                // What is proved: --report-depth parameter controls markdown heading level
                Assert.AreEqual(0, context.ExitCode);
                Assert.IsTrue(File.Exists(reportFile), "Report file was not created");

                var reportContent = File.ReadAllText(reportFile);
                Assert.Contains("### Tool Versions", reportContent);
            }
            finally
            {
                Console.SetOut(originalOut);
            }
        }
        finally
        {
            Directory.SetCurrentDirectory(currentDir);
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
        }
    }
}
