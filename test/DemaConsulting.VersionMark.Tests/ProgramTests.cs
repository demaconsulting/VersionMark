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

using DemaConsulting.VersionMark.Capture;
using DemaConsulting.VersionMark.Cli;
using DemaConsulting.VersionMark.SelfTest;

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
        // Arrange - Redirect console output
        var originalOut = Console.Out;
        try
        {
            using var outWriter = new StringWriter();
            Console.SetOut(outWriter);

            // Act - Run with version flag
            using var context = Context.Create(["--version"]);
            Program.Run(context);

            // Assert - Verify version-only output
            var output = outWriter.ToString();
            Assert.IsFalse(string.IsNullOrWhiteSpace(output), "Version string should be printed");
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
        // Arrange - Redirect console output
        var originalOut = Console.Out;
        try
        {
            using var outWriter = new StringWriter();
            Console.SetOut(outWriter);

            // Act - Run with help flag
            using var context = Context.Create(["--help"]);
            Program.Run(context);

            // Assert - Verify usage information output
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
        // Arrange - Redirect console output
        var originalOut = Console.Out;
        try
        {
            using var outWriter = new StringWriter();
            Console.SetOut(outWriter);

            // Act - Run with validate flag
            using var context = Context.Create(["--validate"]);
            Program.Run(context);

            // Assert - Verify validation output
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
        // Arrange - Redirect console output
        var originalOut = Console.Out;
        try
        {
            using var outWriter = new StringWriter();
            Console.SetOut(outWriter);

            // Act - Run with no arguments
            using var context = Context.Create([]);
            Program.Run(context);

            // Assert - Verify default output
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
        // Arrange & Act - Get version property
        var version = Program.Version;

        // Assert - Verify version is non-empty
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
    ///     Test that Run with capture command and no tool filter captures all configured tools.
    /// </summary>
    [TestMethod]
    public void Program_Run_WithCaptureCommandNoToolFilter_CapturesAllConfiguredTools()
    {
        // Arrange - Set up unique temp directory with a two-tool config; do NOT specify tool names
        var currentDir = Directory.GetCurrentDirectory();
        var tempDir = PathHelpers.SafePathCombine(Path.GetTempPath(), Path.GetRandomFileName());
        var tempConfigFile = PathHelpers.SafePathCombine(tempDir, ".versionmark.yaml");
        var outputFile = PathHelpers.SafePathCombine(tempDir, "output.json");

        try
        {
            Directory.CreateDirectory(tempDir);

            File.WriteAllText(tempConfigFile, @"
tools:
  dotnet:
    command: dotnet --version
    regex: '(?<version>\d+\.\d+\.\d+)'
  git:
    command: git --version
    regex: 'git version (?<version>\d+\.\d+\.\d+)'
");

            Directory.SetCurrentDirectory(tempDir);

            var originalOut = Console.Out;
            try
            {
                using var outWriter = new StringWriter();
                Console.SetOut(outWriter);

                // No tool names after -- : should capture all tools from configuration
                using var context = Context.Create([
                    "--capture",
                    "--job-id", "test-job",
                    "--output", outputFile
                ]);

                // Act - Execute the capture command without specifying tool names
                Program.Run(context);

                // Assert - All tools defined in configuration must be captured
                Assert.AreEqual(0, context.ExitCode);
                Assert.IsTrue(File.Exists(outputFile), "Output file was not created");

                var versionInfo = VersionInfo.LoadFromFile(outputFile);
                Assert.AreEqual("test-job", versionInfo.JobId);
                Assert.IsTrue(versionInfo.Versions.ContainsKey("dotnet"), "Expected 'dotnet' to be captured");
                Assert.IsTrue(versionInfo.Versions.ContainsKey("git"), "Expected 'git' to be captured");
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
        var originalError = Console.Error;
        try
        {
            using var outWriter = new StringWriter();
            using var errWriter = new StringWriter();
            Console.SetOut(outWriter);
            Console.SetError(errWriter);
            using var context = Context.Create(["--capture"]);

            // Act - Run the program with incomplete capture arguments
            Program.Run(context);

            // Assert - Verify error message on stderr and non-zero exit code
            var errorOutput = errWriter.ToString();
            Assert.Contains("--job-id is required", errorOutput);
            Assert.AreEqual(1, context.ExitCode);
        }
        finally
        {
            Console.SetOut(originalOut);
            Console.SetError(originalError);
        }
    }

    /// <summary>
    ///     Test that Run with capture command with missing config file fails.
    /// </summary>
    [TestMethod]
    public void Program_Run_WithCaptureCommandWithMissingConfig_ReturnsError()
    {
        // Arrange - Create an isolated temp directory with no .versionmark.yaml present
        var currentDir = Directory.GetCurrentDirectory();
        var tempDir = PathHelpers.SafePathCombine(Path.GetTempPath(), Path.GetRandomFileName());
        try
        {
            Directory.CreateDirectory(tempDir);
            Directory.SetCurrentDirectory(tempDir);

            var originalOut = Console.Out;
            var originalError = Console.Error;
            try
            {
                using var outWriter = new StringWriter();
                using var errWriter = new StringWriter();
                Console.SetOut(outWriter);
                Console.SetError(errWriter);
                using var context = Context.Create([
                    "--capture",
                    "--job-id", "test-job"
                ]);

                // Act - Run capture command without available config file
                Program.Run(context);

                // Assert - Verify error is reported on stderr and exit code indicates failure
                var errorOutput = errWriter.ToString();
                Assert.Contains("error:", errorOutput);
                Assert.AreEqual(1, context.ExitCode);
            }
            finally
            {
                Console.SetOut(originalOut);
                Console.SetError(originalError);
            }
        }
        finally
        {
            // Restore original directory and clean up temp directory
            Directory.SetCurrentDirectory(currentDir);
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, recursive: true);
            }
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
        var originalError = Console.Error;
        try
        {
            using var outWriter = new StringWriter();
            using var errWriter = new StringWriter();
            Console.SetOut(outWriter);
            Console.SetError(errWriter);
            using var context = Context.Create(["--publish"]);

            // Act - Run publish command without --report parameter
            Program.Run(context);

            // Assert - Verify error message on stderr and non-zero exit code
            // What is proved: --publish without --report results in an error
            var errorOutput = errWriter.ToString();
            Assert.Contains("Error: --report is required for publish mode", errorOutput);
            Assert.AreEqual(1, context.ExitCode);
        }
        finally
        {
            Console.SetOut(originalOut);
            Console.SetError(originalError);
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
            var originalError = Console.Error;
            try
            {
                using var outWriter = new StringWriter();
                using var errWriter = new StringWriter();
                Console.SetOut(outWriter);
                Console.SetError(errWriter);
                using var context = Context.Create([
                    "--publish",
                    "--report", reportFile,
                    "--", "nonexistent-*.json"
                ]);

                // Act - Run publish command with pattern that matches no files
                Program.Run(context);

                // Assert - Verify error message on stderr and non-zero exit code
                // What is proved: No matching files results in an error
                var errorOutput = errWriter.ToString();
                Assert.Contains("Error: No JSON files found matching patterns:", errorOutput);
                Assert.AreEqual(1, context.ExitCode);
            }
            finally
            {
                Console.SetOut(originalOut);
                Console.SetError(originalError);
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
            var originalError = Console.Error;
            try
            {
                using var outWriter = new StringWriter();
                using var errWriter = new StringWriter();
                Console.SetOut(outWriter);
                Console.SetError(errWriter);
                using var context = Context.Create([
                    "--publish",
                    "--report", reportFile
                ]);

                // Act - Run publish command with invalid JSON file
                Program.Run(context);

                // Assert - Verify error message on stderr and non-zero exit code
                // What is proved: Invalid JSON results in an error
                var errorOutput = errWriter.ToString();
                Assert.Contains("Error: Failed to parse JSON file", errorOutput);
                Assert.AreEqual(1, context.ExitCode);
            }
            finally
            {
                Console.SetOut(originalOut);
                Console.SetError(originalError);
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
                Assert.Contains("# Tool Versions", reportContent);
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

    /// <summary>
    ///     Test that Run with lint flag passes for a valid config file.
    /// </summary>
    [TestMethod]
    public void Program_Run_WithLintFlag_ValidConfig_ReturnsSuccess()
    {
        // Arrange - Set up temp directory with a valid config file
        var tempDir = PathHelpers.SafePathCombine(Path.GetTempPath(), Path.GetRandomFileName());
        var configFile = PathHelpers.SafePathCombine(tempDir, ".versionmark.yaml");

        var originalOut = Console.Out;
        var originalError = Console.Error;
        try
        {
            Directory.CreateDirectory(tempDir);
            File.WriteAllText(configFile, @"---
tools:
  dotnet:
    command: dotnet --version
    regex: '(?<version>\d+\.\d+\.\d+)'
");

            using var outWriter = new StringWriter();
            using var errWriter = new StringWriter();
            Console.SetOut(outWriter);
            Console.SetError(errWriter);

            using var context = Context.Create(["--lint", configFile]);

            // Act - Run lint on valid config
            Program.Run(context);

            // Assert - Verify success with no output (lint mode is silent when no issues)
            Assert.AreEqual(0, context.ExitCode);
            var output = outWriter.ToString();
            Assert.IsTrue(string.IsNullOrEmpty(output), "Lint mode should produce no output when there are no issues");
        }
        finally
        {
            Console.SetOut(originalOut);
            Console.SetError(originalError);
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
        }
    }

    /// <summary>
    ///     Test that Run with lint flag fails for an invalid config file.
    /// </summary>
    [TestMethod]
    public void Program_Run_WithLintFlag_InvalidConfig_ReturnsError()
    {
        // Arrange - Set up temp directory with an invalid config file
        var tempDir = PathHelpers.SafePathCombine(Path.GetTempPath(), Path.GetRandomFileName());
        var configFile = PathHelpers.SafePathCombine(tempDir, ".versionmark.yaml");

        var originalOut = Console.Out;
        var originalError = Console.Error;
        try
        {
            Directory.CreateDirectory(tempDir);

            // Missing required 'regex' field
            File.WriteAllText(configFile, @"---
tools:
  dotnet:
    command: dotnet --version
");

            using var outWriter = new StringWriter();
            using var errWriter = new StringWriter();
            Console.SetOut(outWriter);
            Console.SetError(errWriter);

            using var context = Context.Create(["--lint", configFile]);

            // Act - Run lint on invalid config
            Program.Run(context);

            // Assert - Verify error is reported
            Assert.AreEqual(1, context.ExitCode);
            var errorOutput = errWriter.ToString();
            Assert.Contains("error", errorOutput);
        }
        finally
        {
            Console.SetOut(originalOut);
            Console.SetError(originalError);
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
        }
    }

    /// <summary>
    ///     Test that Run with lint flag without file uses default .versionmark.yaml.
    /// </summary>
    [TestMethod]
    public void Program_Run_WithLintFlag_NoFile_UsesDefaultConfigFile()
    {
        // Arrange - Set up temp directory with a default config file
        var currentDir = Directory.GetCurrentDirectory();
        var tempDir = PathHelpers.SafePathCombine(Path.GetTempPath(), Path.GetRandomFileName());
        var configFile = PathHelpers.SafePathCombine(tempDir, ".versionmark.yaml");

        var originalOut = Console.Out;
        var originalError = Console.Error;
        try
        {
            Directory.CreateDirectory(tempDir);
            File.WriteAllText(configFile, @"---
tools:
  dotnet:
    command: dotnet --version
    regex: '(?<version>\d+\.\d+\.\d+)'
");
            Directory.SetCurrentDirectory(tempDir);

            using var outWriter = new StringWriter();
            using var errWriter = new StringWriter();
            Console.SetOut(outWriter);
            Console.SetError(errWriter);

            using var context = Context.Create(["--lint"]);

            // Act - Run lint without specifying a config file
            Program.Run(context);

            // Assert - Verify it found and linted the default file (exit code 0 means success)
            Assert.AreEqual(0, context.ExitCode);
            Assert.IsTrue(string.IsNullOrEmpty(outWriter.ToString()),
                "Lint mode should produce no output when there are no issues");
        }
        finally
        {
            Console.SetOut(originalOut);
            Console.SetError(originalError);
            Directory.SetCurrentDirectory(currentDir);
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
        }
    }

    /// <summary>
    ///     Test that Run with lint flag suppresses the application banner.
    /// </summary>
    [TestMethod]
    public void Program_Run_WithLintFlag_ValidConfig_SuppressesBanner()
    {
        // Arrange - Set up temp directory with a valid config file
        var tempDir = PathHelpers.SafePathCombine(Path.GetTempPath(), Path.GetRandomFileName());
        var configFile = PathHelpers.SafePathCombine(tempDir, ".versionmark.yaml");

        var originalOut = Console.Out;
        var originalError = Console.Error;
        try
        {
            Directory.CreateDirectory(tempDir);
            File.WriteAllText(configFile, @"---
tools:
  dotnet:
    command: dotnet --version
    regex: '(?<version>\d+\.\d+\.\d+)'
");

            using var outWriter = new StringWriter();
            using var errWriter = new StringWriter();
            Console.SetOut(outWriter);
            Console.SetError(errWriter);

            using var context = Context.Create(["--lint", configFile]);

            // Act - Run lint on valid config
            Program.Run(context);

            // Assert - Verify the banner is not present in the output
            Assert.AreEqual(0, context.ExitCode);
            var output = outWriter.ToString();
            Assert.DoesNotContain("VersionMark version", output, "Banner should be suppressed in lint mode");
            Assert.DoesNotContain("Copyright", output, "Banner should be suppressed in lint mode");
        }
        finally
        {
            Console.SetOut(originalOut);
            Console.SetError(originalError);
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
        }
    }

    /// <summary>
    ///     Test that Run with lint flag outputs lint information in help.
    /// </summary>
    [TestMethod]
    public void Program_Run_WithHelpFlag_IncludesLintInformation()
    {
        // Arrange - Redirect console output
        var originalOut = Console.Out;
        try
        {
            using var outWriter = new StringWriter();
            Console.SetOut(outWriter);

            // Act - Run with help flag
            using var context = Context.Create(["--help"]);
            Program.Run(context);

            // Assert - Verify lint is documented in help output
            var output = outWriter.ToString();
            Assert.Contains("--lint", output);
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }
}
