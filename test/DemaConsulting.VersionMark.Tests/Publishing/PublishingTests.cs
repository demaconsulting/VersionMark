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
using DemaConsulting.VersionMark.Utilities;

namespace DemaConsulting.VersionMark.Tests.Publishing;

/// <summary>
///     Subsystem tests for the Publishing subsystem (capture data to markdown report pipeline).
/// </summary>
public class PublishingTests
{
    /// <summary>
    ///     Test that the publishing pipeline produces a valid markdown report from multiple captures.
    /// </summary>
    [Fact]
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
        Assert.False(string.IsNullOrWhiteSpace(report),
            "The publishing pipeline should produce a non-empty report");
        Assert.Contains("dotnet", report);
        Assert.Contains("git", report);
        Assert.Contains("8.0.100", report);
    }

    /// <summary>
    ///     Test that the publishing pipeline consolidates identical versions across jobs.
    /// </summary>
    [Fact]
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
        Assert.Contains("8.0.100", report);
        Assert.DoesNotContain("job-1", report);
    }

    /// <summary>
    ///     Test that the publishing pipeline shows individual job IDs when versions conflict across jobs.
    /// </summary>
    [Fact]
    public void Publishing_Format_ConflictingVersions_ShowsJobIds()
    {
        // Arrange - Create two version infos with different versions for the same tool
        var versionInfoA = new VersionInfo("job-a", new Dictionary<string, string>
        {
            { "dotnet", "8.0.100" }
        });
        var versionInfoB = new VersionInfo("job-b", new Dictionary<string, string>
        {
            { "dotnet", "9.0.200" }
        });
        var versionInfos = new[] { versionInfoA, versionInfoB };

        // Act - Run the publishing pipeline with conflicting versions
        var report = MarkdownFormatter.Format(versionInfos);

        // Assert - Each job ID should appear in the report to attribute the conflicting versions
        Assert.Contains("job-a", report);
        Assert.Contains("job-b", report);
    }

    /// <summary>
    ///     Test that the publishing pipeline uses the correct heading level when a custom report depth is specified.
    /// </summary>
    [Fact]
    public void Publishing_Format_WithCustomDepth_UsesCorrectHeadingLevel()
    {
        // Arrange - Create a simple version info to exercise the heading depth parameter
        var versionInfo = new VersionInfo("job-1", new Dictionary<string, string>
        {
            { "dotnet", "8.0.100" }
        });
        var versionInfos = new[] { versionInfo };

        // Act - Format with a custom depth of 3 to produce "###" headings
        var report = MarkdownFormatter.Format(versionInfos, reportDepth: 3);

        // Assert - The heading prefix should match the requested depth
        Assert.Contains("###", report);
    }

    /// <summary>
    ///     Test that the publishing pipeline requires the --report parameter and reports an error when it is missing.
    /// </summary>
    [Fact]
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
            Assert.Equal(1, context.ExitCode);
            Assert.Contains("--report", errWriter.ToString());
        }
        finally
        {
            Console.SetError(originalError);
        }
    }

    /// <summary>
    ///     Test that the publishing pipeline accepts glob patterns after -- and reads all matching files.
    /// </summary>
    [Fact]
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
            Assert.Equal(0, context.ExitCode);
            Assert.True(File.Exists(reportFile),
                "Report file should be created when glob pattern matches files");
            Assert.Contains("dotnet", File.ReadAllText(reportFile));
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
    ///     Test that the publishing pipeline accepts absolute glob patterns and reads all matching files.
    /// </summary>
    [Fact]
    public void Publishing_Run_WithAbsoluteGlobPattern_ReadsMatchingFiles()
    {
        // Arrange - Create a temp directory with JSON files and use an absolute glob pattern to match them
        var currentDir = Directory.GetCurrentDirectory();
        var tempDir = PathHelpers.SafePathCombine(Path.GetTempPath(), Path.GetRandomFileName());
        var reportFile = PathHelpers.SafePathCombine(tempDir, "report.md");
        try
        {
            Directory.CreateDirectory(tempDir);
            var versionInfo = new VersionInfo("job-abs", new Dictionary<string, string> { ["dotnet"] = "8.0.100" });
            versionInfo.SaveToFile(PathHelpers.SafePathCombine(tempDir, "versionmark-abs-job.json"));

            // Use a different working directory to confirm the absolute pattern is not relative to cwd
            Directory.SetCurrentDirectory(Path.GetTempPath());

            // Build an absolute glob pattern pointing directly into tempDir
            var absolutePattern = PathHelpers.SafePathCombine(tempDir, "versionmark-*.json");
            using var context = Context.Create([
                "--publish", "--report", reportFile, "--silent", "--", absolutePattern
            ]);

            // Act - Run the publish pipeline with an absolute glob pattern
            Program.Run(context);

            // Assert - The report should have been generated from the matched file
            Assert.Equal(0, context.ExitCode);
            Assert.True(File.Exists(reportFile),
                "Report file should be created when absolute glob pattern matches files");
            Assert.Contains("dotnet", File.ReadAllText(reportFile));
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
    [Fact]
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
                Assert.Equal(1, context.ExitCode);
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

    /// <summary>
    ///     Test that the publishing pipeline reports an error when no JSON files match the glob pattern.
    /// </summary>
    [Fact]
    public void Publishing_Run_WithGlobPatternMatchingNoFiles_ReportsError()
    {
        // Arrange - Create a temp directory with no JSON files matching the pattern
        var currentDir = Directory.GetCurrentDirectory();
        var tempDir = PathHelpers.SafePathCombine(Path.GetTempPath(), Path.GetRandomFileName());
        var reportFile = PathHelpers.SafePathCombine(tempDir, "report.md");
        try
        {
            Directory.CreateDirectory(tempDir);
            Directory.SetCurrentDirectory(tempDir);

            var originalError = Console.Error;
            try
            {
                using var errWriter = new StringWriter();
                Console.SetError(errWriter);
                using var context = Context.Create([
                    "--publish", "--report", reportFile, "--", "versionmark-*.json"
                ]);

                // Act - Run the publish pipeline with a pattern that matches no files
                Program.Run(context);

                // Assert - An error should be reported and exit code should be non-zero
                Assert.Equal(1, context.ExitCode);
                Assert.True(
                    errWriter.ToString().Length > 0,
                    "An error message should be written when no files match the glob pattern");
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

    /// <summary>
    ///     Test that the --report-depth parameter is applied end-to-end through Context.Create and Program.Run.
    /// </summary>
    [Fact]
    public void Publishing_Run_WithReportDepth_UsesCorrectDepth()
    {
        // Arrange - Create a temp directory with a JSON file and run with --report-depth 3
        var currentDir = Directory.GetCurrentDirectory();
        var tempDir = PathHelpers.SafePathCombine(Path.GetTempPath(), Path.GetRandomFileName());
        var reportFile = PathHelpers.SafePathCombine(tempDir, "report.md");
        try
        {
            Directory.CreateDirectory(tempDir);
            var versionInfo = new VersionInfo("job-depth", new Dictionary<string, string> { ["dotnet"] = "8.0.100" });
            versionInfo.SaveToFile(PathHelpers.SafePathCombine(tempDir, "versionmark-depth-job.json"));
            Directory.SetCurrentDirectory(tempDir);

            using var context = Context.Create([
                "--publish", "--report", reportFile, "--report-depth", "3", "--silent", "--", "versionmark-*.json"
            ]);

            // Act - Run the publish pipeline with --report-depth 3
            Program.Run(context);

            // Assert - The report heading should use depth-3 prefix "###"
            Assert.Equal(0, context.ExitCode);
            Assert.True(File.Exists(reportFile),
                "Report file should be created");
            Assert.Contains("###", File.ReadAllText(reportFile));
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
    ///     Test that <see cref="MarkdownFormatter.Format"/> throws <see cref="ArgumentOutOfRangeException"/>
    ///     when reportDepth is zero or negative.
    /// </summary>
    [Fact]
    public void Publishing_Format_ReportDepthZeroOrNegative_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var versionInfos = new[] { new VersionInfo("job-1", new Dictionary<string, string> { ["dotnet"] = "8.0.100" }) };

        // Act & Assert - depth 0 should throw
        Assert.Throws<ArgumentOutOfRangeException>(() => MarkdownFormatter.Format(versionInfos, reportDepth: 0));

        // Act & Assert - negative depth should also throw
        Assert.Throws<ArgumentOutOfRangeException>(() => MarkdownFormatter.Format(versionInfos, reportDepth: -1));
    }
}
