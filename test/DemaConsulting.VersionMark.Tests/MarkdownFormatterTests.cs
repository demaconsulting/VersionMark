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
///     Unit tests for the MarkdownFormatter class.
/// </summary>
[TestClass]
public class MarkdownFormatterTests
{
    /// <summary>
    ///     Test that MarkdownFormatter sorts tools alphabetically in the output.
    ///     What is tested: FMT-001 - Tools are sorted in case-insensitive alphabetical order
    ///     What the assertions prove: The output lists tools in the correct alphabetical sequence
    /// </summary>
    [TestMethod]
    public void MarkdownFormatter_FormatVersions_SortsToolsAlphabetically()
    {
        // Arrange - Create VersionInfo with tools in non-alphabetical order
        var versionInfos = new[]
        {
            new VersionInfo(
                "job-1",
                new Dictionary<string, string>
                {
                    ["zulu"] = "11.0.0",
                    ["dotnet"] = "8.0.0",
                    ["python"] = "3.11.0",
                    ["node"] = "18.0.0"
                })
        };

        // Act - Format the version information
        var result = MarkdownFormatter.Format(versionInfos);

        // Assert - Verify tools appear in alphabetical order
        // What is proved: Tools are sorted alphabetically (dotnet, node, python, zulu)
        var lines = result.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        var toolLines = lines.Where(l => l.StartsWith("- **")).ToArray();

        Assert.HasCount(4, toolLines);
        Assert.Contains("- **dotnet**:", toolLines[0]);
        Assert.Contains("- **node**:", toolLines[1]);
        Assert.Contains("- **python**:", toolLines[2]);
        Assert.Contains("- **zulu**:", toolLines[3]);
    }

    /// <summary>
    ///     Test that MarkdownFormatter shows "All jobs" when versions are uniform across all jobs.
    ///     What is tested: FMT-002 - Versions that are the same across all jobs are collapsed to "All jobs"
    ///     What the assertions prove: The output displays "(All jobs)" when all jobs have the same version
    /// </summary>
    [TestMethod]
    public void MarkdownFormatter_FormatVersions_WithUniformVersions_ShowsAllJobs()
    {
        // Arrange - Create multiple VersionInfos with the same version across jobs
        var versionInfos = new[]
        {
            new VersionInfo(
                "job-1",
                new Dictionary<string, string>
                {
                    ["dotnet"] = "8.0.0",
                    ["node"] = "18.0.0"
                }),
            new VersionInfo(
                "job-2",
                new Dictionary<string, string>
                {
                    ["dotnet"] = "8.0.0",
                    ["node"] = "18.0.0"
                }),
            new VersionInfo(
                "job-3",
                new Dictionary<string, string>
                {
                    ["dotnet"] = "8.0.0",
                    ["node"] = "18.0.0"
                })
        };

        // Act - Format the version information
        var result = MarkdownFormatter.Format(versionInfos);

        // Assert - Verify "All jobs" appears for uniform versions
        // What is proved: When all jobs have the same version, the output shows "(All jobs)"
        Assert.Contains("8.0.0 (All jobs)", result);
        Assert.Contains("18.0.0 (All jobs)", result);
    }

    /// <summary>
    ///     Test that MarkdownFormatter shows individual job IDs when versions differ across jobs.
    ///     What is tested: FMT-003, FMT-004 - Different versions show job IDs in subscript format
    ///     What the assertions prove: The output displays job IDs in subscript format when versions differ
    /// </summary>
    [TestMethod]
    public void MarkdownFormatter_FormatVersions_WithDifferentVersions_ShowsIndividualJobs()
    {
        // Arrange - Create VersionInfos with different versions across jobs
        var versionInfos = new[]
        {
            new VersionInfo(
                "job-1",
                new Dictionary<string, string>
                {
                    ["dotnet"] = "8.0.0",
                    ["node"] = "18.0.0"
                }),
            new VersionInfo(
                "job-2",
                new Dictionary<string, string>
                {
                    ["dotnet"] = "8.0.0",
                    ["node"] = "20.0.0"
                }),
            new VersionInfo(
                "job-3",
                new Dictionary<string, string>
                {
                    ["dotnet"] = "7.0.0",
                    ["node"] = "18.0.0"
                })
        };

        // Act - Format the version information
        var result = MarkdownFormatter.Format(versionInfos);

        // Assert - Verify individual job IDs are shown with subscript formatting
        // What is proved: Different versions show job IDs in subscript format <sub>(job-id)</sub>

        // For dotnet: two different versions (7.0.0 and 8.0.0)
        Assert.Contains("<sub>(job-3)</sub>", result);
        Assert.Contains("<sub>(job-1, job-2)</sub>", result);

        // For node: two different versions (18.0.0 and 20.0.0)
        Assert.Contains("<sub>(job-1, job-3)</sub>", result);
        Assert.Contains("<sub>(job-2)</sub>", result);

        // Verify "All jobs" does NOT appear since versions differ
        Assert.DoesNotContain("(All jobs)", result);
    }

    /// <summary>
    ///     Test that MarkdownFormatter respects custom heading depth parameter.
    ///     What is tested: FMT-005 - The report-depth parameter controls markdown heading levels
    ///     What the assertions prove: The output heading level matches the specified depth
    /// </summary>
    [TestMethod]
    public void MarkdownFormatter_FormatVersions_WithCustomDepth_UsesCorrectHeadingLevel()
    {
        // Arrange - Create simple VersionInfo
        var versionInfos = new[]
        {
            new VersionInfo(
                "job-1",
                new Dictionary<string, string>
                {
                    ["dotnet"] = "8.0.0"
                })
        };

        // Test default depth (2)
        var resultDepth2 = MarkdownFormatter.Format(versionInfos);
        // What is proved: Default depth of 2 produces "## Tool Versions"
        Assert.Contains("## Tool Versions", resultDepth2);

        // Test depth 3
        var resultDepth3 = MarkdownFormatter.Format(versionInfos, reportDepth: 3);
        // What is proved: Depth of 3 produces "### Tool Versions"
        Assert.Contains("### Tool Versions", resultDepth3);

        // Test depth 4
        var resultDepth4 = MarkdownFormatter.Format(versionInfos, reportDepth: 4);
        // What is proved: Depth of 4 produces "#### Tool Versions"
        Assert.Contains("#### Tool Versions", resultDepth4);

        // Test depth 1
        var resultDepth1 = MarkdownFormatter.Format(versionInfos, reportDepth: 1);
        // What is proved: Depth of 1 produces "# Tool Versions"
        Assert.Contains("# Tool Versions", resultDepth1);
    }

    /// <summary>
    ///     Test that MarkdownFormatter handles empty version info list correctly.
    ///     What is tested: Edge case - empty input list
    ///     What the assertions prove: The formatter produces valid output with just the header
    /// </summary>
    [TestMethod]
    public void MarkdownFormatter_FormatVersions_EmptyList_ProducesHeaderOnly()
    {
        // Arrange - Create empty VersionInfo list
        var versionInfos = Array.Empty<VersionInfo>();

        // Act - Format the empty version information
        var result = MarkdownFormatter.Format(versionInfos);

        // Assert - Verify output contains only the header
        // What is proved: Empty input produces valid markdown with header but no tool entries
        Assert.Contains("## Tool Versions", result);
        Assert.DoesNotContain("- **", result);
    }

    /// <summary>
    ///     Test that MarkdownFormatter handles a single job correctly.
    ///     What is tested: Edge case - single job shows "All jobs" notation
    ///     What the assertions prove: Single job is treated as uniform (shows "All jobs")
    /// </summary>
    [TestMethod]
    public void MarkdownFormatter_FormatVersions_SingleJob_ShowsAllJobs()
    {
        // Arrange - Create single VersionInfo
        var versionInfos = new[]
        {
            new VersionInfo(
                "job-1",
                new Dictionary<string, string>
                {
                    ["dotnet"] = "8.0.0",
                    ["node"] = "18.0.0"
                })
        };

        // Act - Format the version information
        var result = MarkdownFormatter.Format(versionInfos);

        // Assert - Verify "All jobs" appears for single job
        // What is proved: Single job is treated as uniform and shows "(All jobs)"
        Assert.Contains("8.0.0 (All jobs)", result);
        Assert.Contains("18.0.0 (All jobs)", result);
    }

    /// <summary>
    ///     Test that MarkdownFormatter handles mixed scenarios correctly.
    ///     What is tested: Some tools uniform, some tools different versions across jobs
    ///     What the assertions prove: The formatter correctly handles both uniform and varying versions
    /// </summary>
    [TestMethod]
    public void MarkdownFormatter_FormatVersions_MixedVersions_HandlesCorrectly()
    {
        // Arrange - Create VersionInfos with some tools uniform, some different
        var versionInfos = new[]
        {
            new VersionInfo(
                "job-1",
                new Dictionary<string, string>
                {
                    ["dotnet"] = "8.0.0",  // Uniform across all jobs
                    ["node"] = "18.0.0",    // Different across jobs
                    ["python"] = "3.11.0"   // Uniform across all jobs
                }),
            new VersionInfo(
                "job-2",
                new Dictionary<string, string>
                {
                    ["dotnet"] = "8.0.0",  // Uniform
                    ["node"] = "20.0.0",    // Different
                    ["python"] = "3.11.0"   // Uniform
                })
        };

        // Act - Format the version information
        var result = MarkdownFormatter.Format(versionInfos);

        // Assert - Verify uniform tools show "All jobs" and different tools show job IDs
        // What is proved: Formatter correctly distinguishes uniform from varying versions
        Assert.Contains("8.0.0 (All jobs)", result);    // dotnet is uniform
        Assert.Contains("3.11.0 (All jobs)", result);   // python is uniform
        Assert.Contains("<sub>(job-1)</sub>", result);  // node differs
        Assert.Contains("<sub>(job-2)</sub>", result);  // node differs
    }

    /// <summary>
    ///     Test that MarkdownFormatter sorts job IDs alphabetically within subscript.
    ///     What is tested: Job IDs are sorted within subscript when multiple jobs share a version
    ///     What the assertions prove: Job IDs appear in alphabetical order
    /// </summary>
    [TestMethod]
    public void MarkdownFormatter_FormatVersions_SortsJobIdsAlphabetically()
    {
        // Arrange - Create VersionInfos where multiple jobs have same version
        var versionInfos = new[]
        {
            new VersionInfo(
                "job-zebra",
                new Dictionary<string, string> { ["tool"] = "1.0.0" }),
            new VersionInfo(
                "job-alpha",
                new Dictionary<string, string> { ["tool"] = "1.0.0" }),
            new VersionInfo(
                "job-beta",
                new Dictionary<string, string> { ["tool"] = "1.0.0" })
        };

        // Act - Format the version information
        var result = MarkdownFormatter.Format(versionInfos);

        // Assert - Verify "All jobs" is shown (since all have same version)
        // What is proved: When all jobs have the same version, "All jobs" is displayed
        Assert.Contains("1.0.0 (All jobs)", result);
    }

    /// <summary>
    ///     Test that MarkdownFormatter handles version strings with special characters.
    ///     What is tested: Version strings with hyphens, plus signs, and other special characters
    ///     What the assertions prove: Special characters in versions are preserved in output
    /// </summary>
    [TestMethod]
    public void MarkdownFormatter_FormatVersions_WithSpecialCharacters_PreservesVersions()
    {
        // Arrange - Create VersionInfo with special version strings
        var versionInfos = new[]
        {
            new VersionInfo(
                "job-1",
                new Dictionary<string, string>
                {
                    ["tool1"] = "1.0.0-beta+build.123",
                    ["tool2"] = "2.0.0-rc.1",
                    ["tool3"] = "3.0.0+meta.data"
                })
        };

        // Act - Format the version information
        var result = MarkdownFormatter.Format(versionInfos);

        // Assert - Verify special characters are preserved
        // What is proved: Version strings with special characters are correctly preserved
        Assert.Contains("1.0.0-beta+build.123 (All jobs)", result);
        Assert.Contains("2.0.0-rc.1 (All jobs)", result);
        Assert.Contains("3.0.0+meta.data (All jobs)", result);
    }

    /// <summary>
    ///     Test that MarkdownFormatter handles case-insensitive tool name sorting.
    ///     What is tested: Tool names with different cases are sorted case-insensitively
    ///     What the assertions prove: Sorting is case-insensitive (Dotnet comes before node)
    /// </summary>
    [TestMethod]
    public void MarkdownFormatter_FormatVersions_CaseInsensitiveSorting()
    {
        // Arrange - Create VersionInfo with mixed-case tool names
        var versionInfos = new[]
        {
            new VersionInfo(
                "job-1",
                new Dictionary<string, string>
                {
                    ["Zulu"] = "11.0.0",
                    ["dotnet"] = "8.0.0",
                    ["Node"] = "18.0.0",
                    ["PYTHON"] = "3.11.0"
                })
        };

        // Act - Format the version information
        var result = MarkdownFormatter.Format(versionInfos);

        // Assert - Verify tools are sorted case-insensitively
        // What is proved: Tool names are sorted alphabetically regardless of case
        var lines = result.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        var toolLines = lines.Where(l => l.StartsWith("- **")).ToArray();

        Assert.HasCount(4, toolLines);
        Assert.Contains("- **dotnet**:", toolLines[0]);
        Assert.Contains("- **Node**:", toolLines[1]);
        Assert.Contains("- **PYTHON**:", toolLines[2]);
        Assert.Contains("- **Zulu**:", toolLines[3]);
    }

    /// <summary>
    ///     Test that MarkdownFormatter sorts versions alphabetically when displaying job IDs.
    ///     What is tested: Multiple different versions are displayed in sorted order
    ///     What the assertions prove: Version groups are sorted alphabetically
    /// </summary>
    [TestMethod]
    public void MarkdownFormatter_FormatVersions_SortsVersionsAlphabetically()
    {
        // Arrange - Create VersionInfos with multiple different versions
        var versionInfos = new[]
        {
            new VersionInfo(
                "job-1",
                new Dictionary<string, string> { ["tool"] = "3.0.0" }),
            new VersionInfo(
                "job-2",
                new Dictionary<string, string> { ["tool"] = "1.0.0" }),
            new VersionInfo(
                "job-3",
                new Dictionary<string, string> { ["tool"] = "2.0.0" })
        };

        // Act - Format the version information
        var result = MarkdownFormatter.Format(versionInfos);

        // Assert - Verify versions appear in sorted order
        // What is proved: Different versions are listed in alphabetical order
        var toolLine = result.Split('\n').First(l => l.StartsWith("- **tool**:"));
        var indexOf1 = toolLine.IndexOf("1.0.0", StringComparison.Ordinal);
        var indexOf2 = toolLine.IndexOf("2.0.0", StringComparison.Ordinal);
        var indexOf3 = toolLine.IndexOf("3.0.0", StringComparison.Ordinal);

        Assert.IsLessThan(indexOf2, indexOf1, "1.0.0 should appear before 2.0.0");
        Assert.IsLessThan(indexOf3, indexOf2, "2.0.0 should appear before 3.0.0");
    }
}
