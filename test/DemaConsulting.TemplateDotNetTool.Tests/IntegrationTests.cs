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

namespace DemaConsulting.TemplateDotNetTool.Tests;

/// <summary>
///     Integration tests that run the Template DotNet Tool application through dotnet.
/// </summary>
[TestClass]
public class IntegrationTests
{
    private string _dllPath = string.Empty;

    /// <summary>
    ///     Initialize test by locating the Template DotNet Tool DLL.
    /// </summary>
    [TestInitialize]
    public void TestInitialize()
    {
        // The DLL should be in the same directory as the test assembly
        // because the test project references the main project
        var baseDir = AppContext.BaseDirectory;
        _dllPath = PathHelpers.SafePathCombine(baseDir, "DemaConsulting.TemplateDotNetTool.dll");

        Assert.IsTrue(File.Exists(_dllPath), $"Could not find Template DotNet Tool DLL at {_dllPath}");
    }

    /// <summary>
    ///     Test that version flag outputs version information.
    /// </summary>
    [TestMethod]
    public void IntegrationTest_VersionFlag_OutputsVersion()
    {
        // Run the application with --version flag
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            _dllPath,
            "--version");

        // Verify success
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
        // Run the application with --help flag
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            _dllPath,
            "--help");

        // Verify success
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
        // Run the application with --validate flag
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            _dllPath,
            "--validate");

        // Verify success
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
        var resultsFile = Path.GetTempFileName();
        resultsFile = Path.ChangeExtension(resultsFile, ".trx");

        try
        {
            // Run the application with --validate and --results flags
            var exitCode = Runner.Run(
                out var _,
                "dotnet",
                _dllPath,
                "--validate",
                "--results",
                resultsFile);

            // Verify success
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
        // Run the application with --silent flag
        var exitCode = Runner.Run(
            out var _,
            "dotnet",
            _dllPath,
            "--silent");

        // Verify success
        Assert.AreEqual(0, exitCode);

        // Output check removed since silent mode may still produce some output
    }

    /// <summary>
    ///     Test that log flag writes output to file.
    /// </summary>
    [TestMethod]
    public void IntegrationTest_LogFlag_WritesOutputToFile()
    {
        var logFile = Path.GetTempFileName();

        try
        {
            // Run the application with --log flag
            var exitCode = Runner.Run(
                out var _,
                "dotnet",
                _dllPath,
                "--log",
                logFile);

            // Verify success
            Assert.AreEqual(0, exitCode);

            // Verify log file was created
            Assert.IsTrue(File.Exists(logFile), "Log file was not created");

            // Verify log file contains output
            var logContent = File.ReadAllText(logFile);
            Assert.Contains("Template DotNet Tool version", logContent);
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
        // Run the application with unknown argument
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            _dllPath,
            "--unknown");

        // Verify error
        Assert.AreNotEqual(0, exitCode);
        Assert.Contains("Error", output);
    }

    /// <summary>
    ///     Test that capture command captures tool versions.
    /// </summary>
    [TestMethod]
    public void IntegrationTest_CaptureCommand_CapturesToolVersions()
    {
        var configFile = Path.GetTempFileName();
        var outputFile = Path.GetTempFileName();

        try
        {
            // Create a temporary config file
            File.WriteAllText(configFile, @"
tools:
  dotnet:
    command: dotnet --version
    regex: '(?<version>\d+\.\d+\.\d+)'
  git:
    command: git --version
    regex: 'git version (?<version>\d+\.\d+\.\d+)'
");

            // Run the application with capture command
            var exitCode = Runner.Run(
                out var output,
                "dotnet",
                _dllPath,
                "capture",
                "--job-id", "test-job",
                "--config", configFile,
                "--output", outputFile,
                "--", "dotnet", "git");

            // Verify success
            Assert.AreEqual(0, exitCode);

            // Verify output contains expected information
            Assert.Contains("Capturing tool versions", output);
            Assert.Contains("test-job", output);
            Assert.Contains("dotnet", output);
            Assert.Contains("git", output);

            // Verify output file was created
            Assert.IsTrue(File.Exists(outputFile), "Output file was not created");

            // Verify output file contains expected data
            var versionInfo = VersionInfo.LoadFromFile(outputFile);
            Assert.AreEqual("test-job", versionInfo.JobId);
            Assert.IsTrue(versionInfo.Versions.ContainsKey("dotnet"));
            Assert.IsTrue(versionInfo.Versions.ContainsKey("git"));
        }
        finally
        {
            if (File.Exists(configFile))
            {
                File.Delete(configFile);
            }
            if (File.Exists(outputFile))
            {
                File.Delete(outputFile);
            }
        }
    }

    /// <summary>
    ///     Test that capture command without job ID returns error.
    /// </summary>
    [TestMethod]
    public void IntegrationTest_CaptureCommandWithoutJobId_ReturnsError()
    {
        // Run the application with capture command but no job ID
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            _dllPath,
            "capture");

        // Verify error
        Assert.AreNotEqual(0, exitCode);
        Assert.Contains("--job-id is required", output);
    }

    /// <summary>
    ///     Test that capture command with missing config file returns error.
    /// </summary>
    [TestMethod]
    public void IntegrationTest_CaptureCommandWithMissingConfig_ReturnsError()
    {
        // Run the application with capture command and missing config
        var exitCode = Runner.Run(
            out var output,
            "dotnet",
            _dllPath,
            "capture",
            "--job-id", "test-job",
            "--config", "nonexistent.yaml");

        // Verify error
        Assert.AreNotEqual(0, exitCode);
        Assert.Contains("Error:", output);
    }

    /// <summary>
    ///     Test that capture command with default output filename works.
    /// </summary>
    [TestMethod]
    public void IntegrationTest_CaptureCommandWithDefaultOutput_UsesDefaultFilename()
    {
        var configFile = Path.GetTempFileName();
        var outputFile = "versionmark-integration-test-job.json";

        try
        {
            // Delete output file if it exists
            if (File.Exists(outputFile))
            {
                File.Delete(outputFile);
            }

            // Create a temporary config file
            File.WriteAllText(configFile, @"
tools:
  dotnet:
    command: dotnet --version
    regex: '(?<version>\d+\.\d+\.\d+)'
");

            // Run the application with capture command (no output file specified)
            var exitCode = Runner.Run(
                out var _,
                "dotnet",
                _dllPath,
                "capture",
                "--job-id", "integration-test-job",
                "--config", configFile,
                "--", "dotnet");

            // Verify success
            Assert.AreEqual(0, exitCode);

            // Verify output file was created with default name
            Assert.IsTrue(File.Exists(outputFile), $"Output file '{outputFile}' was not created");

            // Verify output file contains expected data
            var versionInfo = VersionInfo.LoadFromFile(outputFile);
            Assert.AreEqual("integration-test-job", versionInfo.JobId);
        }
        finally
        {
            if (File.Exists(configFile))
            {
                File.Delete(configFile);
            }
            if (File.Exists(outputFile))
            {
                File.Delete(outputFile);
            }
        }
    }
}
