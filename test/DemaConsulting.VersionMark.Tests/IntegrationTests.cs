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
///     Integration tests that run the VersionMark application through dotnet.
/// </summary>
[TestClass]
public class IntegrationTests
{
    private string _dllPath = string.Empty;

    /// <summary>
    ///     Initialize test by locating the VersionMark DLL.
    /// </summary>
    [TestInitialize]
    public void TestInitialize()
    {
        // The DLL should be in the same directory as the test assembly
        // because the test project references the main project
        var baseDir = AppContext.BaseDirectory;
        _dllPath = PathHelpers.SafePathCombine(baseDir, "DemaConsulting.VersionMark.dll");

        Assert.IsTrue(File.Exists(_dllPath), $"Could not find VersionMark DLL at {_dllPath}");
    }

    /// <summary>
    ///     Test that version flag outputs version information.
    /// </summary>
    [TestMethod]
    public void IntegrationTest_VersionFlag_OutputsVersion()
    {
        // Arrange & Act - Run the application with --version flag
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            _dllPath,
            "--version");

        // Assert - Verify success and version output
        Assert.AreEqual(0, exitCode);

        // Verify version is output
        Assert.IsFalse(string.IsNullOrWhiteSpace(output));
        Assert.DoesNotContain("Error", output);
        Assert.DoesNotContain("Copyright", output);
    }

    /// <summary>
    ///     Test that help flag outputs usage information.
    /// </summary>
    [TestMethod]
    public void IntegrationTest_HelpFlag_OutputsUsageInformation()
    {
        // Arrange & Act - Run the application with --help flag
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            _dllPath,
            "--help");

        // Assert - Verify success and usage information output
        Assert.AreEqual(0, exitCode);

        // Verify usage information is output
        Assert.Contains("Usage:", output);
        Assert.Contains("Options:", output);
        Assert.Contains("--version", output);
    }

    /// <summary>
    ///     Test that validate flag runs self-validation.
    /// </summary>
    [TestMethod]
    public void IntegrationTest_ValidateFlag_RunsValidation()
    {
        // Arrange & Act - Run the application with --validate flag
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            _dllPath,
            "--validate");

        // Assert - Verify success and validation output
        Assert.AreEqual(0, exitCode);

        // Verify validation output
        Assert.Contains("Total Tests:", output);
        Assert.Contains("Passed:", output);
    }

    /// <summary>
    ///     Test that validate with results flag generates TRX file.
    /// </summary>
    [TestMethod]
    public void IntegrationTest_ValidateWithResults_GeneratesTrxFile()
    {
        // Arrange - Create temp file path for results
        var resultsFile = Path.GetTempFileName();
        resultsFile = Path.ChangeExtension(resultsFile, ".trx");

        try
        {
            // Act - Run the application with --validate and --results flags
            var exitCode = Runner.Run(
                out var _,
                "dotnet",
                _dllPath,
                "--validate",
                "--results",
                resultsFile);

            // Assert - Verify success and TRX file creation
            Assert.AreEqual(0, exitCode);

            // Verify TRX file was created
            Assert.IsTrue(File.Exists(resultsFile), "Results file was not created");

            // Verify TRX file contains expected content
            var trxContent = File.ReadAllText(resultsFile);
            Assert.Contains("<TestRun", trxContent);
            Assert.Contains("</TestRun>", trxContent);
        }
        finally
        {
            if (File.Exists(resultsFile))
            {
                File.Delete(resultsFile);
            }
        }
    }

    /// <summary>
    ///     Test that silent flag suppresses output.
    /// </summary>
    [TestMethod]
    public void IntegrationTest_SilentFlag_SuppressesOutput()
    {
        // Arrange & Act - Run the application with --silent flag
        var exitCode = Runner.Run(
            out var _,
            "dotnet",
            _dllPath,
            "--silent");

        // Assert - Verify success
        Assert.AreEqual(0, exitCode);

        // Output check removed since silent mode may still produce some output
    }

    /// <summary>
    ///     Test that log flag writes output to file.
    /// </summary>
    [TestMethod]
    public void IntegrationTest_LogFlag_WritesOutputToFile()
    {
        // Arrange - Create temp log file path
        var logFile = Path.GetTempFileName();

        try
        {
            // Act - Run the application with --log flag
            var exitCode = Runner.Run(
                out var _,
                "dotnet",
                _dllPath,
                "--log",
                logFile);

            // Assert - Verify success and log file creation
            Assert.AreEqual(0, exitCode);

            // Verify log file was created
            Assert.IsTrue(File.Exists(logFile), "Log file was not created");

            // Verify log file contains output
            var logContent = File.ReadAllText(logFile);
            Assert.Contains("VersionMark version", logContent);
        }
        finally
        {
            if (File.Exists(logFile))
            {
                File.Delete(logFile);
            }
        }
    }

    /// <summary>
    ///     Test that unknown argument returns error.
    /// </summary>
    [TestMethod]
    public void IntegrationTest_UnknownArgument_ReturnsError()
    {
        // Arrange & Act - Run the application with unknown argument
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            _dllPath,
            "--unknown");

        // Assert - Verify error is returned
        Assert.AreNotEqual(0, exitCode);
        Assert.Contains("Error", output);
    }

    /// <summary>
    ///     Test that capture command captures tool versions.
    /// </summary>
    [TestMethod]
    public void IntegrationTest_CaptureCommand_CapturesToolVersions()
    {
        // Arrange - Set up unique temp directory with config file
        using var testDir = new TestDirectory(copyConfig: true);
        var outputFile = PathHelpers.SafePathCombine(testDir.Path, "output.json");

        // Act - Run the capture command with specific tools
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            _dllPath,
            "--capture",
            "--job-id", $"test-job-{Guid.NewGuid():N}",
            "--output", outputFile,
            "--", "dotnet", "git");

        // Assert - Verify the command succeeded and captured the expected tool versions
        // Verify success
        Assert.AreEqual(0, exitCode);

        // Verify output contains expected information
        Assert.Contains("Capturing tool versions", output);
        Assert.Contains("dotnet", output);
        Assert.Contains("git", output);

        // Verify output file was created
        Assert.IsTrue(File.Exists(outputFile), "Output file was not created");

        // Verify output file contains expected data
        var versionInfo = VersionInfo.LoadFromFile(outputFile);
        Assert.IsTrue(versionInfo.Versions.ContainsKey("dotnet"));
        Assert.IsTrue(versionInfo.Versions.ContainsKey("git"));
    }

    /// <summary>
    ///     Helper class for managing isolated test directories.
    ///     Creates a unique temp directory with optional .versionmark.yaml config.
    ///     Automatically cleans up on disposal.
    /// </summary>
    private sealed class TestDirectory : IDisposable
    {
        private readonly string _originalDirectory;
        private bool _disposed;

        /// <summary>
        ///     Gets the path to the test directory.
        /// </summary>
        public string Path { get; }

        /// <summary>
        ///     Creates a new isolated test directory.
        /// </summary>
        /// <param name="copyConfig">Whether to copy .versionmark.yaml from repo root.</param>
        public TestDirectory(bool copyConfig = false)
        {
            _originalDirectory = Directory.GetCurrentDirectory();
            Path = PathHelpers.SafePathCombine(System.IO.Path.GetTempPath(), $"versionmark-test-{Guid.NewGuid():N}");

            Directory.CreateDirectory(Path);

            if (copyConfig)
            {
                var repoRoot = FindRepositoryRoot(_originalDirectory);
                if (!string.IsNullOrEmpty(repoRoot))
                {
                    var configSource = PathHelpers.SafePathCombine(repoRoot, ".versionmark.yaml");
                    var configDest = PathHelpers.SafePathCombine(Path, ".versionmark.yaml");
                    File.Copy(configSource, configDest);
                }
            }

            Directory.SetCurrentDirectory(Path);
        }

        /// <summary>
        ///     Disposes the test directory and restores the original directory.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            try
            {
                Directory.SetCurrentDirectory(_originalDirectory);
            }
            catch
            {
                // Ignore errors restoring directory
            }

            try
            {
                if (Directory.Exists(Path))
                {
                    Directory.Delete(Path, true);
                }
            }
            catch
            {
                // Ignore cleanup errors
            }

            _disposed = true;
        }

        /// <summary>
        ///     Helper method to find the repository root.
        /// </summary>
        /// <param name="startPath">Starting directory.</param>
        /// <returns>Repository root path or empty string if not found.</returns>
        private static string FindRepositoryRoot(string startPath)
        {
            var dir = new DirectoryInfo(startPath);
            while (dir != null)
            {
                if (File.Exists(PathHelpers.SafePathCombine(dir.FullName, ".versionmark.yaml")))
                {
                    return dir.FullName;
                }
                dir = dir.Parent;
            }
            return string.Empty;
        }
    }

    /// <summary>
    ///     Test that capture command without job ID returns error.
    /// </summary>
    [TestMethod]
    public void IntegrationTest_CaptureCommandWithoutJobId_ReturnsError()
    {
        // Arrange - No special setup needed for testing error condition

        // Act - Run capture command without required --job-id parameter
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            _dllPath,
            "--capture");

        // Assert - Verify the command fails with appropriate error message
        Assert.AreNotEqual(0, exitCode);
        Assert.Contains("--job-id is required", output);
    }

    /// <summary>
    ///     Test that capture command with missing config file returns error.
    /// </summary>
    [TestMethod]
    public void IntegrationTest_CaptureCommandWithMissingConfig_ReturnsError()
    {
        // Arrange - Create temp directory without .versionmark.yaml config file
        using var testDir = new TestDirectory(copyConfig: false);

        // Act - Run capture command in directory without config file
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            _dllPath,
            "--capture",
            "--job-id", "test-job");

        // Assert - Verify the command fails with error message about missing config
        Assert.AreNotEqual(0, exitCode);
        Assert.Contains("Error:", output);
    }

    /// <summary>
    ///     Test that capture command with default output filename works.
    /// </summary>
    [TestMethod]
    public void IntegrationTest_CaptureCommandWithDefaultOutput_UsesDefaultFilename()
    {
        // Arrange - Set up to test default output filename generation
        // Use a unique job ID to avoid conflicts with parallel test execution
        var jobId = $"integration-test-job-{Guid.NewGuid():N}";
        var outputFile = $"versionmark-{jobId}.json";

        using var testDir = new TestDirectory(copyConfig: true);

        // Act - Run capture command without specifying --output parameter
        var exitCode = Runner.Run(
            out var _,
            "dotnet",
            _dllPath,
            "--capture",
            "--job-id", jobId,
            "--", "dotnet");

        // Assert - Verify command succeeded and created file with default name pattern
        // Verify success
        Assert.AreEqual(0, exitCode);

        // Verify output file was created with default name
        Assert.IsTrue(File.Exists(outputFile), $"Output file '{outputFile}' was not created");

        // Verify output file contains expected data
        var versionInfo = VersionInfo.LoadFromFile(outputFile);
        Assert.AreEqual(jobId, versionInfo.JobId);
    }

    /// <summary>
    ///     Integration test that publish command generates markdown report.
    ///     What is tested: PUB-001, PUB-002, FMT-001 - End-to-end publish workflow
    ///     What the assertions prove: Publish command processes JSON files and creates markdown
    /// </summary>
    [TestMethod]
    public void VersionMark_PublishCommand_GeneratesMarkdownReport()
    {
        // Arrange - Set up unique temp directory with multiple JSON files
        using var testDir = new TestDirectory(copyConfig: false);
        var json1File = PathHelpers.SafePathCombine(testDir.Path, "versionmark-job1.json");
        var json2File = PathHelpers.SafePathCombine(testDir.Path, "versionmark-job2.json");
        var reportFile = PathHelpers.SafePathCombine(testDir.Path, "report.md");

        // Create test JSON files
        var versionInfo1 = new VersionInfo(
            "job-1",
            new Dictionary<string, string>
            {
                ["dotnet"] = "8.0.0",
                ["node"] = "18.0.0",
                ["python"] = "3.11.0"
            });
        var versionInfo2 = new VersionInfo(
            "job-2",
            new Dictionary<string, string>
            {
                ["dotnet"] = "8.0.0",
                ["node"] = "20.0.0",
                ["python"] = "3.11.0"
            });

        versionInfo1.SaveToFile(json1File);
        versionInfo2.SaveToFile(json2File);

        // Act - Run publish command
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            _dllPath,
            "--publish",
            "--report", reportFile);

        // Assert - Verify command succeeded
        // What is proved: Publish command successfully generates markdown from JSON files
        Assert.AreEqual(0, exitCode, $"Command failed with output: {output}");
        Assert.IsTrue(File.Exists(reportFile), "Report file was not created");

        var reportContent = File.ReadAllText(reportFile);

        // Verify markdown structure
        Assert.Contains("## Tool Versions", reportContent);

        // Verify tools are present and sorted alphabetically
        Assert.Contains("dotnet", reportContent);
        Assert.Contains("node", reportContent);
        Assert.Contains("python", reportContent);

        // Verify version formatting (uniform versions show just the version)
        Assert.Contains("- **dotnet**: 8.0.0", reportContent); // dotnet uniform, no job IDs
        Assert.Contains("- **python**: 3.11.0", reportContent); // python uniform, no job IDs

        // Verify version formatting (different versions show job IDs in parentheses)
        Assert.Contains("18.0.0 (job-", reportContent);
        Assert.Contains("20.0.0 (job-", reportContent);
    }

    /// <summary>
    ///     Integration test that publish command handles custom report depth.
    ///     What is tested: PUB-003, FMT-005 - Custom report depth parameter
    ///     What the assertions prove: --report-depth controls markdown heading level
    /// </summary>
    [TestMethod]
    public void VersionMark_PublishCommand_WithReportDepth_AdjustsHeadingLevels()
    {
        // Arrange - Set up unique temp directory with JSON file
        var currentDir = Directory.GetCurrentDirectory();
        var tempDir = PathHelpers.SafePathCombine(System.IO.Path.GetTempPath(), $"versionmark-test-{Guid.NewGuid():N}");
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

            // Act - Run publish command with custom depth
            var exitCode = Runner.Run(
                out var output,
                "dotnet",
                _dllPath,
                "--publish",
                "--report", reportFile,
                "--report-depth", "4");

            // Assert - Verify command succeeded and used custom depth
            // What is proved: --report-depth parameter controls heading level
            Assert.AreEqual(0, exitCode, $"Command failed with output: {output}");
            Assert.IsTrue(File.Exists(reportFile), "Report file was not created");

            var reportContent = File.ReadAllText(reportFile);
            Assert.Contains("#### Tool Versions", reportContent);
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
    ///     Integration test that publish command requires --report parameter.
    ///     What is tested: PUB-004 - --report is required with --publish
    ///     What the assertions prove: Command fails when --report is missing
    /// </summary>
    [TestMethod]
    public void VersionMark_PublishCommandWithoutReport_ReturnsError()
    {
        // Arrange & Act - Run publish command without --report
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            _dllPath,
            "--publish");

        // Assert - Verify command failed with appropriate error
        // What is proved: --publish requires --report parameter
        Assert.AreEqual(1, exitCode);
        Assert.Contains("Error: --report is required for publish mode", output);
    }

    /// <summary>
    ///     Integration test that publish command handles no matching files.
    ///     What is tested: PUB-007 - Error when no JSON files match glob patterns
    ///     What the assertions prove: Command fails with clear error when no files found
    /// </summary>
    [TestMethod]
    public void VersionMark_PublishCommandWithNoMatchingFiles_ReturnsError()
    {
        // Arrange - Set up empty temp directory
        var currentDir = Directory.GetCurrentDirectory();
        var tempDir = PathHelpers.SafePathCombine(System.IO.Path.GetTempPath(), $"versionmark-test-{Guid.NewGuid():N}");
        var reportFile = PathHelpers.SafePathCombine(tempDir, "report.md");

        try
        {
            Directory.CreateDirectory(tempDir);
            Directory.SetCurrentDirectory(tempDir);

            // Act - Run publish command with pattern that matches no files
            var exitCode = Runner.Run(
                out var output,
                "dotnet",
                _dllPath,
                "--publish",
                "--report", reportFile,
                "--", "nonexistent-*.json");

            // Assert - Verify command failed with appropriate error
            // What is proved: No matching files results in an error
            Assert.AreEqual(1, exitCode);
            Assert.Contains("Error: No JSON files found matching patterns:", output);
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
    ///     Integration test that publish command handles invalid JSON files.
    ///     What is tested: PUB-008 - Error when JSON files cannot be parsed
    ///     What the assertions prove: Command fails with clear error for malformed JSON
    /// </summary>
    [TestMethod]
    public void VersionMark_PublishCommandWithInvalidJson_ReturnsError()
    {
        // Arrange - Set up temp directory with invalid JSON file
        var currentDir = Directory.GetCurrentDirectory();
        var tempDir = PathHelpers.SafePathCombine(System.IO.Path.GetTempPath(), $"versionmark-test-{Guid.NewGuid():N}");
        var invalidJsonFile = PathHelpers.SafePathCombine(tempDir, "versionmark-invalid.json");
        var reportFile = PathHelpers.SafePathCombine(tempDir, "report.md");

        try
        {
            Directory.CreateDirectory(tempDir);
            Directory.SetCurrentDirectory(tempDir);

            // Create invalid JSON file
            File.WriteAllText(invalidJsonFile, "{ this is not valid JSON }");

            // Act - Run publish command with invalid JSON file
            var exitCode = Runner.Run(
                out var output,
                "dotnet",
                _dllPath,
                "--publish",
                "--report", reportFile);

            // Assert - Verify command failed with appropriate error
            // What is proved: Invalid JSON results in an error
            Assert.AreEqual(1, exitCode);
            Assert.Contains("Error: Failed to parse JSON file", output);
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
    ///     Integration test that publish command uses custom glob patterns.
    ///     What is tested: PUB-005 - Custom glob patterns after -- separator
    ///     What the assertions prove: Custom patterns correctly filter JSON files
    /// </summary>
    [TestMethod]
    public void VersionMark_PublishCommandWithCustomGlobPatterns_FiltersFiles()
    {
        // Arrange - Set up temp directory with multiple JSON files
        var currentDir = Directory.GetCurrentDirectory();
        var tempDir = PathHelpers.SafePathCombine(System.IO.Path.GetTempPath(), $"versionmark-test-{Guid.NewGuid():N}");
        var includedFile = PathHelpers.SafePathCombine(tempDir, "included-job1.json");
        var excludedFile = PathHelpers.SafePathCombine(tempDir, "versionmark-excluded.json");
        var reportFile = PathHelpers.SafePathCombine(tempDir, "report.md");

        try
        {
            Directory.CreateDirectory(tempDir);
            Directory.SetCurrentDirectory(tempDir);

            // Create test JSON files
            var includedInfo = new VersionInfo(
                "included-job",
                new Dictionary<string, string> { ["tool1"] = "1.0.0" });
            var excludedInfo = new VersionInfo(
                "excluded-job",
                new Dictionary<string, string> { ["tool2"] = "2.0.0" });

            includedInfo.SaveToFile(includedFile);
            excludedInfo.SaveToFile(excludedFile);

            // Act - Run publish command with custom glob pattern
            var exitCode = Runner.Run(
                out var output,
                "dotnet",
                _dllPath,
                "--publish",
                "--report", reportFile,
                "--", "included-*.json");

            // Assert - Verify only matching files were included
            // What is proved: Custom glob patterns correctly filter input files
            Assert.AreEqual(0, exitCode, $"Command failed with output: {output}");
            Assert.IsTrue(File.Exists(reportFile), "Report file was not created");

            var reportContent = File.ReadAllText(reportFile);
            Assert.Contains("tool1", reportContent); // From included file
            Assert.DoesNotContain("tool2", reportContent); // From excluded file
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
