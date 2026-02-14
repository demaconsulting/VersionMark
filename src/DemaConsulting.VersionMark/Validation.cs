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

using System.Runtime.InteropServices;
using DemaConsulting.TestResults.IO;

namespace DemaConsulting.VersionMark;

/// <summary>
///     Provides self-validation functionality for the VersionMark.
/// </summary>
internal static class Validation
{
    /// <summary>
    ///     Runs self-validation tests and optionally writes results to a file.
    /// </summary>
    /// <param name="context">The context containing command line arguments and program state.</param>
    public static void Run(Context context)
    {
        // Print validation header
        PrintValidationHeader(context);

        // Create test results collection
        var testResults = new DemaConsulting.TestResults.TestResults
        {
            Name = "VersionMark Self-Validation"
        };

        // Run core functionality tests
        RunVersionTest(context, testResults);
        RunHelpTest(context, testResults);

        // Run publish feature tests
        RunPublishRequiresReportParameterTest(context, testResults);
        RunPublishWithValidInputTest(context, testResults);
        RunPublishWithNoMatchingFilesTest(context, testResults);
        RunPublishWithUniformVersionsTest(context, testResults);
        RunPublishWithDifferingVersionsTest(context, testResults);

        // Calculate totals
        var totalTests = testResults.Results.Count;
        var passedTests = testResults.Results.Count(t => t.Outcome == DemaConsulting.TestResults.TestOutcome.Passed);
        var failedTests = testResults.Results.Count(t => t.Outcome == DemaConsulting.TestResults.TestOutcome.Failed);

        // Print summary
        context.WriteLine("");
        context.WriteLine($"Total Tests: {totalTests}");
        context.WriteLine($"Passed: {passedTests}");
        if (failedTests > 0)
        {
            context.WriteError($"Failed: {failedTests}");
        }
        else
        {
            context.WriteLine($"Failed: {failedTests}");
        }

        // Write results file if requested
        if (context.ResultsFile != null)
        {
            WriteResultsFile(context, testResults);
        }
    }

    /// <summary>
    ///     Prints the validation header with system information.
    /// </summary>
    /// <param name="context">The context for output.</param>
    private static void PrintValidationHeader(Context context)
    {
        context.WriteLine("# DEMA Consulting VersionMark");
        context.WriteLine("");
        context.WriteLine("| Information         | Value                                              |");
        context.WriteLine("| :------------------ | :------------------------------------------------- |");
        context.WriteLine($"| Tool Version        | {Program.Version,-50} |");
        context.WriteLine($"| Machine Name        | {Environment.MachineName,-50} |");
        context.WriteLine($"| OS Version          | {RuntimeInformation.OSDescription,-50} |");
        context.WriteLine($"| DotNet Runtime      | {RuntimeInformation.FrameworkDescription,-50} |");
        context.WriteLine($"| Time Stamp          | {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC{"",-29} |");
        context.WriteLine("");
    }

    /// <summary>
    ///     Runs a test for version display functionality.
    /// </summary>
    /// <param name="context">The context for output.</param>
    /// <param name="testResults">The test results collection.</param>
    private static void RunVersionTest(Context context, DemaConsulting.TestResults.TestResults testResults)
    {
        var startTime = DateTime.UtcNow;
        var test = CreateTestResult("VersionMark_VersionDisplay");

        try
        {
            using var tempDir = new TemporaryDirectory();
            var logFile = PathHelpers.SafePathCombine(tempDir.DirectoryPath, "version-test.log");

            // Build command line arguments
            var args = new List<string>
            {
                "--silent",
                "--log", logFile,
                "--version"
            };

            // Run the program
            int exitCode;
            using (var testContext = Context.Create([.. args]))
            {
                Program.Run(testContext);
                exitCode = testContext.ExitCode;
            }

            // Check if execution succeeded
            if (exitCode == 0)
            {
                // Read log content
                var logContent = File.ReadAllText(logFile);

                // Verify version string is in log (version contains dots like 0.0.0)
                if (!string.IsNullOrWhiteSpace(logContent) &&
                    logContent.Split('.').Length >= 3)
                {
                    test.Outcome = DemaConsulting.TestResults.TestOutcome.Passed;
                    context.WriteLine($"✓ Version Display Test - PASSED");
                }
                else
                {
                    test.Outcome = DemaConsulting.TestResults.TestOutcome.Failed;
                    test.ErrorMessage = "Version string not found in log";
                    context.WriteError($"✗ Version Display Test - FAILED: Version string not found in log");
                }
            }
            else
            {
                test.Outcome = DemaConsulting.TestResults.TestOutcome.Failed;
                test.ErrorMessage = $"Program exited with code {exitCode}";
                context.WriteError($"✗ Version Display Test - FAILED: Exit code {exitCode}");
            }
        }
        // Generic catch is justified here as this is a test framework - any exception should be
        // recorded as a test failure to ensure robust test execution and reporting.
        catch (Exception ex)
        {
            HandleTestException(test, context, "Version Display Test", ex);
        }

        FinalizeTestResult(test, startTime, testResults);
    }

    /// <summary>
    ///     Runs a test for help display functionality.
    /// </summary>
    /// <param name="context">The context for output.</param>
    /// <param name="testResults">The test results collection.</param>
    private static void RunHelpTest(Context context, DemaConsulting.TestResults.TestResults testResults)
    {
        var startTime = DateTime.UtcNow;
        var test = CreateTestResult("VersionMark_HelpDisplay");

        try
        {
            using var tempDir = new TemporaryDirectory();
            var logFile = PathHelpers.SafePathCombine(tempDir.DirectoryPath, "help-test.log");

            // Build command line arguments
            var args = new List<string>
            {
                "--silent",
                "--log", logFile,
                "--help"
            };

            // Run the program
            int exitCode;
            using (var testContext = Context.Create([.. args]))
            {
                Program.Run(testContext);
                exitCode = testContext.ExitCode;
            }

            // Check if execution succeeded
            if (exitCode == 0)
            {
                // Read log content
                var logContent = File.ReadAllText(logFile);

                // Verify help text is in log
                if (logContent.Contains("Usage:") && logContent.Contains("Options:"))
                {
                    test.Outcome = DemaConsulting.TestResults.TestOutcome.Passed;
                    context.WriteLine($"✓ Help Display Test - PASSED");
                }
                else
                {
                    test.Outcome = DemaConsulting.TestResults.TestOutcome.Failed;
                    test.ErrorMessage = "Help text not found in log";
                    context.WriteError($"✗ Help Display Test - FAILED: Help text not found in log");
                }
            }
            else
            {
                test.Outcome = DemaConsulting.TestResults.TestOutcome.Failed;
                test.ErrorMessage = $"Program exited with code {exitCode}";
                context.WriteError($"✗ Help Display Test - FAILED: Exit code {exitCode}");
            }
        }
        // Generic catch is justified here as this is a test framework - any exception should be
        // recorded as a test failure to ensure robust test execution and reporting.
        catch (Exception ex)
        {
            HandleTestException(test, context, "Help Display Test", ex);
        }

        FinalizeTestResult(test, startTime, testResults);
    }

    /// <summary>
    ///     Runs a test to verify that --report parameter is required for publish mode.
    /// </summary>
    /// <param name="context">The context for output.</param>
    /// <param name="testResults">The test results collection.</param>
    private static void RunPublishRequiresReportParameterTest(Context context, DemaConsulting.TestResults.TestResults testResults)
    {
        var startTime = DateTime.UtcNow;
        var test = CreateTestResult("VersionMark_PublishCommandWithoutReport_ReturnsError");

        try
        {
            using var tempDir = new TemporaryDirectory();
            var logFile = PathHelpers.SafePathCombine(tempDir.DirectoryPath, "publish-no-report.log");

            // Build command line arguments for publish without --report
            var args = new List<string>
            {
                "--silent",
                "--log", logFile,
                "--publish"
            };

            // Run the program
            int exitCode;
            using (var testContext = Context.Create([.. args]))
            {
                Program.Run(testContext);
                exitCode = testContext.ExitCode;
            }

            // Check if execution returned error
            if (exitCode == 1)
            {
                // Read log content
                var logContent = File.ReadAllText(logFile);

                // Verify error message is in log
                if (logContent.Contains("Error: --report is required for publish mode"))
                {
                    test.Outcome = DemaConsulting.TestResults.TestOutcome.Passed;
                    context.WriteLine($"✓ Publish Requires Report Parameter Test - PASSED");
                }
                else
                {
                    test.Outcome = DemaConsulting.TestResults.TestOutcome.Failed;
                    test.ErrorMessage = "Expected error message not found in log";
                    context.WriteError($"✗ Publish Requires Report Parameter Test - FAILED: Expected error message not found");
                }
            }
            else
            {
                test.Outcome = DemaConsulting.TestResults.TestOutcome.Failed;
                test.ErrorMessage = $"Program exited with code {exitCode}, expected 1";
                context.WriteError($"✗ Publish Requires Report Parameter Test - FAILED: Exit code {exitCode}");
            }
        }
        catch (Exception ex)
        {
            HandleTestException(test, context, "Publish Requires Report Parameter Test", ex);
        }

        FinalizeTestResult(test, startTime, testResults);
    }

    /// <summary>
    ///     Runs a test to verify that publish mode generates markdown report from JSON files.
    /// </summary>
    /// <param name="context">The context for output.</param>
    /// <param name="testResults">The test results collection.</param>
    private static void RunPublishWithValidInputTest(Context context, DemaConsulting.TestResults.TestResults testResults)
    {
        var startTime = DateTime.UtcNow;
        var test = CreateTestResult("VersionMark_PublishCommand_GeneratesMarkdownReport");

        try
        {
            using var tempDir = new TemporaryDirectory();
            var logFile = PathHelpers.SafePathCombine(tempDir.DirectoryPath, "publish-valid.log");
            var reportFile = PathHelpers.SafePathCombine(tempDir.DirectoryPath, "report.md");
            var jsonFile1 = PathHelpers.SafePathCombine(tempDir.DirectoryPath, "versionmark-job1.json");
            var jsonFile2 = PathHelpers.SafePathCombine(tempDir.DirectoryPath, "versionmark-job2.json");

            // Create test JSON files
            var versionInfo1 = new VersionInfo("job1", new Dictionary<string, string>
            {
                { "dotnet", "8.0.0" },
                { "node", "20.0.0" }
            });
            versionInfo1.SaveToFile(jsonFile1);

            var versionInfo2 = new VersionInfo("job2", new Dictionary<string, string>
            {
                { "dotnet", "8.0.0" },
                { "node", "20.0.0" }
            });
            versionInfo2.SaveToFile(jsonFile2);

            // Build command line arguments for publish
            var args = new List<string>
            {
                "--silent",
                "--log", logFile,
                "--publish",
                "--report", reportFile,
                "--report-depth", "2",
                "--",
                "versionmark-*.json"
            };

            // Save the current directory and change to temp directory
            var originalDir = Directory.GetCurrentDirectory();
            try
            {
                Directory.SetCurrentDirectory(tempDir.DirectoryPath);

                // Run the program
                int exitCode;
                using (var testContext = Context.Create([.. args]))
                {
                    Program.Run(testContext);
                    exitCode = testContext.ExitCode;
                }

                // Check if execution succeeded
                if (exitCode == 0)
                {
                    // Verify report file was created
                    if (File.Exists(reportFile))
                    {
                        var reportContent = File.ReadAllText(reportFile);

                        // Verify report contains expected content
                        if (reportContent.Contains("## Tool Versions") &&
                            reportContent.Contains("**dotnet**") &&
                            reportContent.Contains("**node**") &&
                            reportContent.Contains("8.0.0") &&
                            reportContent.Contains("20.0.0"))
                        {
                            test.Outcome = DemaConsulting.TestResults.TestOutcome.Passed;
                            context.WriteLine($"✓ Publish Command Generates Markdown Report Test - PASSED");
                        }
                        else
                        {
                            test.Outcome = DemaConsulting.TestResults.TestOutcome.Failed;
                            test.ErrorMessage = "Report content missing expected elements";
                            context.WriteError($"✗ Publish Command Generates Markdown Report Test - FAILED: Missing content");
                        }
                    }
                    else
                    {
                        test.Outcome = DemaConsulting.TestResults.TestOutcome.Failed;
                        test.ErrorMessage = "Report file was not created";
                        context.WriteError($"✗ Publish Command Generates Markdown Report Test - FAILED: No report file");
                    }
                }
                else
                {
                    test.Outcome = DemaConsulting.TestResults.TestOutcome.Failed;
                    test.ErrorMessage = $"Program exited with code {exitCode}";
                    context.WriteError($"✗ Publish Command Generates Markdown Report Test - FAILED: Exit code {exitCode}");
                }
            }
            finally
            {
                Directory.SetCurrentDirectory(originalDir);
            }
        }
        catch (Exception ex)
        {
            HandleTestException(test, context, "Publish Command Generates Markdown Report Test", ex);
        }

        FinalizeTestResult(test, startTime, testResults);
    }

    /// <summary>
    ///     Runs a test to verify that publish mode reports an error when no matching files are found.
    /// </summary>
    /// <param name="context">The context for output.</param>
    /// <param name="testResults">The test results collection.</param>
    private static void RunPublishWithNoMatchingFilesTest(Context context, DemaConsulting.TestResults.TestResults testResults)
    {
        var startTime = DateTime.UtcNow;
        var test = CreateTestResult("VersionMark_PublishCommandWithNoMatchingFiles_ReturnsError");

        try
        {
            using var tempDir = new TemporaryDirectory();
            var logFile = PathHelpers.SafePathCombine(tempDir.DirectoryPath, "publish-no-files.log");
            var reportFile = PathHelpers.SafePathCombine(tempDir.DirectoryPath, "report.md");

            // Build command line arguments for publish with pattern that won't match
            var args = new List<string>
            {
                "--silent",
                "--log", logFile,
                "--publish",
                "--report", reportFile,
                "--",
                "nonexistent-*.json"
            };

            // Save the current directory and change to temp directory
            var originalDir = Directory.GetCurrentDirectory();
            try
            {
                Directory.SetCurrentDirectory(tempDir.DirectoryPath);

                // Run the program
                int exitCode;
                using (var testContext = Context.Create([.. args]))
                {
                    Program.Run(testContext);
                    exitCode = testContext.ExitCode;
                }

                // Check if execution returned error
                if (exitCode == 1)
                {
                    // Read log content
                    var logContent = File.ReadAllText(logFile);

                    // Verify error message is in log
                    if (logContent.Contains("Error: No JSON files found matching patterns"))
                    {
                        test.Outcome = DemaConsulting.TestResults.TestOutcome.Passed;
                        context.WriteLine($"✓ Publish With No Matching Files Test - PASSED");
                    }
                    else
                    {
                        test.Outcome = DemaConsulting.TestResults.TestOutcome.Failed;
                        test.ErrorMessage = "Expected error message not found in log";
                        context.WriteError($"✗ Publish With No Matching Files Test - FAILED: Expected error message not found");
                    }
                }
                else
                {
                    test.Outcome = DemaConsulting.TestResults.TestOutcome.Failed;
                    test.ErrorMessage = $"Program exited with code {exitCode}, expected 1";
                    context.WriteError($"✗ Publish With No Matching Files Test - FAILED: Exit code {exitCode}");
                }
            }
            finally
            {
                Directory.SetCurrentDirectory(originalDir);
            }
        }
        catch (Exception ex)
        {
            HandleTestException(test, context, "Publish With No Matching Files Test", ex);
        }

        FinalizeTestResult(test, startTime, testResults);
    }

    /// <summary>
    ///     Runs a test to verify that publish mode shows "All jobs" when versions are uniform.
    /// </summary>
    /// <param name="context">The context for output.</param>
    /// <param name="testResults">The test results collection.</param>
    private static void RunPublishWithUniformVersionsTest(Context context, DemaConsulting.TestResults.TestResults testResults)
    {
        var startTime = DateTime.UtcNow;
        var test = CreateTestResult("VersionMark_PublishCommand_WithUniformVersions_ShowsAllJobs");

        try
        {
            using var tempDir = new TemporaryDirectory();
            var logFile = PathHelpers.SafePathCombine(tempDir.DirectoryPath, "publish-uniform.log");
            var reportFile = PathHelpers.SafePathCombine(tempDir.DirectoryPath, "report.md");
            var jsonFile1 = PathHelpers.SafePathCombine(tempDir.DirectoryPath, "test1.json");
            var jsonFile2 = PathHelpers.SafePathCombine(tempDir.DirectoryPath, "test2.json");

            // Create test JSON files with same versions
            var versionInfo1 = new VersionInfo("job1", new Dictionary<string, string>
            {
                { "tool1", "1.0.0" }
            });
            versionInfo1.SaveToFile(jsonFile1);

            var versionInfo2 = new VersionInfo("job2", new Dictionary<string, string>
            {
                { "tool1", "1.0.0" }
            });
            versionInfo2.SaveToFile(jsonFile2);

            // Build command line arguments for publish
            var args = new List<string>
            {
                "--silent",
                "--log", logFile,
                "--publish",
                "--report", reportFile,
                "--",
                "test*.json"
            };

            // Save the current directory and change to temp directory
            var originalDir = Directory.GetCurrentDirectory();
            try
            {
                Directory.SetCurrentDirectory(tempDir.DirectoryPath);

                // Run the program
                int exitCode;
                using (var testContext = Context.Create([.. args]))
                {
                    Program.Run(testContext);
                    exitCode = testContext.ExitCode;
                }

                // Check if execution succeeded
                if (exitCode == 0)
                {
                    // Verify report file was created
                    if (File.Exists(reportFile))
                    {
                        var reportContent = File.ReadAllText(reportFile);

                        // Verify report shows "All jobs" for uniform versions
                        if (reportContent.Contains("1.0.0 (All jobs)"))
                        {
                            test.Outcome = DemaConsulting.TestResults.TestOutcome.Passed;
                            context.WriteLine($"✓ Publish With Uniform Versions Shows All Jobs Test - PASSED");
                        }
                        else
                        {
                            test.Outcome = DemaConsulting.TestResults.TestOutcome.Failed;
                            test.ErrorMessage = "Report does not show 'All jobs' for uniform versions";
                            context.WriteError($"✗ Publish With Uniform Versions Shows All Jobs Test - FAILED: Missing 'All jobs'");
                        }
                    }
                    else
                    {
                        test.Outcome = DemaConsulting.TestResults.TestOutcome.Failed;
                        test.ErrorMessage = "Report file was not created";
                        context.WriteError($"✗ Publish With Uniform Versions Shows All Jobs Test - FAILED: No report file");
                    }
                }
                else
                {
                    test.Outcome = DemaConsulting.TestResults.TestOutcome.Failed;
                    test.ErrorMessage = $"Program exited with code {exitCode}";
                    context.WriteError($"✗ Publish With Uniform Versions Shows All Jobs Test - FAILED: Exit code {exitCode}");
                }
            }
            finally
            {
                Directory.SetCurrentDirectory(originalDir);
            }
        }
        catch (Exception ex)
        {
            HandleTestException(test, context, "Publish With Uniform Versions Shows All Jobs Test", ex);
        }

        FinalizeTestResult(test, startTime, testResults);
    }

    /// <summary>
    ///     Runs a test to verify that publish mode shows job IDs when versions differ.
    /// </summary>
    /// <param name="context">The context for output.</param>
    /// <param name="testResults">The test results collection.</param>
    private static void RunPublishWithDifferingVersionsTest(Context context, DemaConsulting.TestResults.TestResults testResults)
    {
        var startTime = DateTime.UtcNow;
        var test = CreateTestResult("VersionMark_PublishCommand_WithDifferingVersions_ShowsJobIds");

        try
        {
            using var tempDir = new TemporaryDirectory();
            var logFile = PathHelpers.SafePathCombine(tempDir.DirectoryPath, "publish-differ.log");
            var reportFile = PathHelpers.SafePathCombine(tempDir.DirectoryPath, "report.md");
            var jsonFile1 = PathHelpers.SafePathCombine(tempDir.DirectoryPath, "test1.json");
            var jsonFile2 = PathHelpers.SafePathCombine(tempDir.DirectoryPath, "test2.json");

            // Create test JSON files with different versions
            var versionInfo1 = new VersionInfo("job1", new Dictionary<string, string>
            {
                { "tool1", "1.0.0" }
            });
            versionInfo1.SaveToFile(jsonFile1);

            var versionInfo2 = new VersionInfo("job2", new Dictionary<string, string>
            {
                { "tool1", "2.0.0" }
            });
            versionInfo2.SaveToFile(jsonFile2);

            // Build command line arguments for publish
            var args = new List<string>
            {
                "--silent",
                "--log", logFile,
                "--publish",
                "--report", reportFile,
                "--",
                "test*.json"
            };

            // Save the current directory and change to temp directory
            var originalDir = Directory.GetCurrentDirectory();
            try
            {
                Directory.SetCurrentDirectory(tempDir.DirectoryPath);

                // Run the program
                int exitCode;
                using (var testContext = Context.Create([.. args]))
                {
                    Program.Run(testContext);
                    exitCode = testContext.ExitCode;
                }

                // Check if execution succeeded
                if (exitCode == 0)
                {
                    // Verify report file was created
                    if (File.Exists(reportFile))
                    {
                        var reportContent = File.ReadAllText(reportFile);

                        // Verify report shows job IDs with subscripts for different versions
                        if (reportContent.Contains("<sub>(job1)</sub>") &&
                            reportContent.Contains("<sub>(job2)</sub>"))
                        {
                            test.Outcome = DemaConsulting.TestResults.TestOutcome.Passed;
                            context.WriteLine($"✓ Publish With Differing Versions Shows Job IDs Test - PASSED");
                        }
                        else
                        {
                            test.Outcome = DemaConsulting.TestResults.TestOutcome.Failed;
                            test.ErrorMessage = "Report does not show job IDs for different versions";
                            context.WriteError($"✗ Publish With Differing Versions Shows Job IDs Test - FAILED: Missing job IDs");
                        }
                    }
                    else
                    {
                        test.Outcome = DemaConsulting.TestResults.TestOutcome.Failed;
                        test.ErrorMessage = "Report file was not created";
                        context.WriteError($"✗ Publish With Differing Versions Shows Job IDs Test - FAILED: No report file");
                    }
                }
                else
                {
                    test.Outcome = DemaConsulting.TestResults.TestOutcome.Failed;
                    test.ErrorMessage = $"Program exited with code {exitCode}";
                    context.WriteError($"✗ Publish With Differing Versions Shows Job IDs Test - FAILED: Exit code {exitCode}");
                }
            }
            finally
            {
                Directory.SetCurrentDirectory(originalDir);
            }
        }
        catch (Exception ex)
        {
            HandleTestException(test, context, "Publish With Differing Versions Shows Job IDs Test", ex);
        }

        FinalizeTestResult(test, startTime, testResults);
    }

    /// <summary>
    ///     Writes test results to a file in TRX or JUnit format.
    /// </summary>
    /// <param name="context">The context for output.</param>
    /// <param name="testResults">The test results to write.</param>
    private static void WriteResultsFile(Context context, DemaConsulting.TestResults.TestResults testResults)
    {
        if (context.ResultsFile == null)
        {
            return;
        }

        try
        {
            var extension = Path.GetExtension(context.ResultsFile).ToLowerInvariant();
            string content;

            if (extension == ".trx")
            {
                content = TrxSerializer.Serialize(testResults);
            }
            else if (extension == ".xml")
            {
                // Assume JUnit format for .xml extension
                content = JUnitSerializer.Serialize(testResults);
            }
            else
            {
                context.WriteError($"Error: Unsupported results file format '{extension}'. Use .trx or .xml extension.");
                return;
            }

            File.WriteAllText(context.ResultsFile, content);
            context.WriteLine($"Results written to {context.ResultsFile}");
        }
        // Generic catch is justified here as a top-level handler to log file write errors
        catch (Exception ex)
        {
            context.WriteError($"Error: Failed to write results file: {ex.Message}");
        }
    }

    /// <summary>
    ///     Creates a new test result object with common properties.
    /// </summary>
    /// <param name="testName">The name of the test.</param>
    /// <returns>A new test result object.</returns>
    private static DemaConsulting.TestResults.TestResult CreateTestResult(string testName)
    {
        return new DemaConsulting.TestResults.TestResult
        {
            Name = testName,
            ClassName = "Validation",
            CodeBase = "VersionMark"
        };
    }

    /// <summary>
    ///     Finalizes a test result by setting its duration and adding it to the collection.
    /// </summary>
    /// <param name="test">The test result to finalize.</param>
    /// <param name="startTime">The start time of the test.</param>
    /// <param name="testResults">The test results collection to add to.</param>
    private static void FinalizeTestResult(
        DemaConsulting.TestResults.TestResult test,
        DateTime startTime,
        DemaConsulting.TestResults.TestResults testResults)
    {
        test.Duration = DateTime.UtcNow - startTime;
        testResults.Results.Add(test);
    }

    /// <summary>
    ///     Handles test exceptions by setting failure information and logging the error.
    /// </summary>
    /// <param name="test">The test result to update.</param>
    /// <param name="context">The context for output.</param>
    /// <param name="testName">The name of the test for error messages.</param>
    /// <param name="ex">The exception that occurred.</param>
    private static void HandleTestException(
        DemaConsulting.TestResults.TestResult test,
        Context context,
        string testName,
        Exception ex)
    {
        test.Outcome = DemaConsulting.TestResults.TestOutcome.Failed;
        test.ErrorMessage = $"Exception: {ex.Message}";
        context.WriteError($"✗ {testName} - FAILED: {ex.Message}");
    }

    /// <summary>
    ///     Represents a temporary directory that is automatically deleted when disposed.
    /// </summary>
    private sealed class TemporaryDirectory : IDisposable
    {
        /// <summary>
        ///     Gets the path to the temporary directory.
        /// </summary>
        public string DirectoryPath { get; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TemporaryDirectory"/> class.
        /// </summary>
        public TemporaryDirectory()
        {
            DirectoryPath = PathHelpers.SafePathCombine(Path.GetTempPath(), $"versionmark_validation_{Guid.NewGuid()}");

            try
            {
                Directory.CreateDirectory(DirectoryPath);
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException)
            {
                throw new InvalidOperationException($"Failed to create temporary directory: {ex.Message}", ex);
            }
        }

        /// <summary>
        ///     Deletes the temporary directory and all its contents.
        /// </summary>
        public void Dispose()
        {
            try
            {
                if (Directory.Exists(DirectoryPath))
                {
                    Directory.Delete(DirectoryPath, true);
                }
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                // Ignore cleanup errors during disposal
            }
        }
    }
}
