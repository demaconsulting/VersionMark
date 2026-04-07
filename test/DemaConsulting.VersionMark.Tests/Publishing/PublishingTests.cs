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
using DemaConsulting.VersionMark.Publishing;
using DemaConsulting.VersionMark.SelfTest;

namespace DemaConsulting.VersionMark.Tests.Publishing;

/// <summary>
///     Subsystem tests for the Publishing subsystem (capture data to markdown report pipeline).
/// </summary>
[TestClass]
public class PublishingTests
{
    /// <summary>
    ///     Test that the publishing pipeline produces a valid markdown report from multiple captures.
    /// </summary>
    [TestMethod]
    public void Publishing_Format_MultipleCaptureFiles_ProducesConsolidatedReport()
    {
        // Arrange - Create version infos representing captures from multiple CI jobs
        var versionInfos = new[]
        {
            new VersionInfo("job-linux",
                new Dictionary<string, string>
                {
                    ["dotnet"] = "8.0.100",
                    ["git"] = "2.43.0"
                }),
            new VersionInfo("job-windows",
                new Dictionary<string, string>
                {
                    ["dotnet"] = "8.0.100",
                    ["git"] = "2.43.0"
                })
        };

        // Act - Run the full publishing pipeline to produce a markdown report
        var report = MarkdownFormatter.Format(versionInfos);

        // Assert - The report should contain version information for all tools
        Assert.IsFalse(string.IsNullOrWhiteSpace(report),
            "The publishing pipeline should produce a non-empty report");
        Assert.Contains("dotnet", report, "Report should include the dotnet tool");
        Assert.Contains("git", report, "Report should include the git tool");
        Assert.Contains("8.0.100", report, "Report should include the dotnet version");
    }

    /// <summary>
    ///     Test that the publishing pipeline consolidates identical versions across jobs.
    /// </summary>
    [TestMethod]
    public void Publishing_Format_IdenticalVersionsAcrossJobs_ConsolidatesVersions()
    {
        // Arrange - Create version infos with the same dotnet version across all jobs
        var versionInfos = new[]
        {
            new VersionInfo("job-1", new Dictionary<string, string> { ["dotnet"] = "8.0.100" }),
            new VersionInfo("job-2", new Dictionary<string, string> { ["dotnet"] = "8.0.100" }),
            new VersionInfo("job-3", new Dictionary<string, string> { ["dotnet"] = "8.0.100" })
        };

        // Act - Run the publishing pipeline
        var report = MarkdownFormatter.Format(versionInfos);

        // Assert - The report should show a single consolidated version, not per-job versions
        Assert.Contains("8.0.100", report, "Report should include the consolidated version");
        Assert.DoesNotContain("job-1", report,
            "Consolidated versions should not show individual job IDs");
    }

    /// <summary>
    ///     Test that the publishing pipeline shows individual job IDs when versions conflict across jobs.
    /// </summary>
    [TestMethod]
    public void Publishing_Format_ConflictingVersions_ShowsJobIds()
    {
        // Arrange
        var versionInfoA = new VersionInfo("job-a", new Dictionary<string, string>
        {
            { "dotnet", "8.0.100" }
        });
        var versionInfoB = new VersionInfo("job-b", new Dictionary<string, string>
        {
            { "dotnet", "9.0.200" }
        });
        var versionInfos = new[] { versionInfoA, versionInfoB };

        // Act
        var report = MarkdownFormatter.Format(versionInfos);

        // Assert
        StringAssert.Contains(report, "job-a");
        StringAssert.Contains(report, "job-b");
    }

    /// <summary>
    ///     Test that the publishing pipeline uses the correct heading level when a custom report depth is specified.
    /// </summary>
    [TestMethod]
    public void Publishing_Format_WithCustomDepth_UsesCorrectHeadingLevel()
    {
        // Arrange
        var versionInfo = new VersionInfo("job-1", new Dictionary<string, string>
        {
            { "dotnet", "8.0.100" }
        });
        var versionInfos = new[] { versionInfo };

        // Act
        var report = MarkdownFormatter.Format(versionInfos, reportDepth: 3);

        // Assert
        StringAssert.Contains(report, "###");
    }

    /// <summary>
    ///     Test that the publishing pipeline requires the --report parameter and reports an error when it is missing.
    /// </summary>
    [TestMethod]
    public void Publishing_Run_WithoutReport_ReportsError()
    {
        // Arrange - Create a publish context without --report
        var originalError = Console.Error;
        try
        {
            using var errWriter = new StringWriter();
            Console.SetError(errWriter);
            using var context = Context.Create(["--publish"]);

            // Act - Run the publish pipeline without --report
            Program.Run(context);

            // Assert - An error should be reported and exit code should be non-zero
            Assert.AreEqual(1, context.ExitCode,
                "Publishing without --report should result in a non-zero exit code");
            StringAssert.Contains(errWriter.ToString(), "--report",
                "Error message should mention the missing --report parameter");
        }
        finally
        {
            Console.SetError(originalError);
        }
    }

    /// <summary>
    ///     Test that the publishing pipeline accepts glob patterns after -- and reads all matching files.
    /// </summary>
    [TestMethod]
    public void Publishing_Run_WithGlobPattern_ReadsMatchingFiles()
    {
        // Arrange - Create a temp directory with JSON files and use a glob pattern to match them
        var currentDir = Directory.GetCurrentDirectory();
        var tempDir = PathHelpers.SafePathCombine(Path.GetTempPath(), Path.GetRandomFileName());
        var reportFile = PathHelpers.SafePathCombine(tempDir, "report.md");
        try
        {
            Directory.CreateDirectory(tempDir);
            var versionInfo = new VersionInfo("job-glob", new Dictionary<string, string> { ["dotnet"] = "8.0.100" });
            versionInfo.SaveToFile(PathHelpers.SafePathCombine(tempDir, "versionmark-glob-job.json"));
            Directory.SetCurrentDirectory(tempDir);

            using var context = Context.Create([
                "--publish", "--report", reportFile, "--silent", "--", "versionmark-*.json"
            ]);

            // Act - Run the publish pipeline with a glob pattern
            Program.Run(context);

            // Assert - The report should have been generated from the matched file
            Assert.AreEqual(0, context.ExitCode,
                "Publishing with a valid glob pattern should succeed");
            Assert.IsTrue(File.Exists(reportFile),
                "Report file should be created when glob pattern matches files");
            StringAssert.Contains(File.ReadAllText(reportFile), "dotnet",
                "Report should contain content from the matched JSON file");
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
    ///     Test that the publishing pipeline reports an error when a JSON file is malformed.
    /// </summary>
    [TestMethod]
    public void Publishing_Run_WithMalformedJsonFile_ReportsError()
    {
        // Arrange - Create a temp directory with a malformed JSON file
        var currentDir = Directory.GetCurrentDirectory();
        var tempDir = PathHelpers.SafePathCombine(Path.GetTempPath(), Path.GetRandomFileName());
        var reportFile = PathHelpers.SafePathCombine(tempDir, "report.md");
        try
        {
            Directory.CreateDirectory(tempDir);
            File.WriteAllText(
                PathHelpers.SafePathCombine(tempDir, "versionmark-bad.json"),
                "{ this is not valid JSON }");
            Directory.SetCurrentDirectory(tempDir);

            var originalError = Console.Error;
            try
            {
                using var errWriter = new StringWriter();
                Console.SetError(errWriter);
                using var context = Context.Create([
                    "--publish", "--report", reportFile, "--", "versionmark-*.json"
                ]);

                // Act - Run the publish pipeline with a malformed JSON file
                Program.Run(context);

                // Assert - An error should be reported
                Assert.AreEqual(1, context.ExitCode,
                    "Publishing with malformed JSON should result in a non-zero exit code");
            }
            finally
            {
                Console.SetError(originalError);
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
}
