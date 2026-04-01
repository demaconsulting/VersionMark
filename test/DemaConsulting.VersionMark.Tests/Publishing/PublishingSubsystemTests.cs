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
using DemaConsulting.VersionMark.Publishing;

namespace DemaConsulting.VersionMark.Tests.Publishing;

/// <summary>
///     Subsystem tests for the Publishing subsystem (capture data to markdown report pipeline).
/// </summary>
[TestClass]
public class PublishingSubsystemTests
{
    /// <summary>
    ///     Test that the publishing pipeline produces a valid markdown report from multiple captures.
    /// </summary>
    [TestMethod]
    public void PublishingSubsystem_Format_MultipleCaptureFiles_ProducesConsolidatedReport()
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
    public void PublishingSubsystem_Format_IdenticalVersionsAcrossJobs_ConsolidatesVersions()
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
}
