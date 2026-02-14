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

using System.Text;

namespace DemaConsulting.VersionMark;

/// <summary>
///     Provides markdown formatting functionality for version information.
/// </summary>
internal static class MarkdownFormatter
{
    /// <summary>
    ///     Formats a collection of VersionInfo records into a markdown string.
    /// </summary>
    /// <param name="versionInfos">The collection of VersionInfo records to format.</param>
    /// <param name="reportDepth">The heading depth for the section title (default: 2).</param>
    /// <returns>A markdown-formatted string representing the version information.</returns>
    public static string Format(IEnumerable<VersionInfo> versionInfos, int reportDepth = 2)
    {
        // Convert to list to allow multiple enumerations
        var versionList = versionInfos.ToList();

        // Build a dictionary of tool name -> list of (jobId, version) pairs
        var toolVersions = BuildToolVersionsDictionary(versionList);

        // Generate markdown output
        return GenerateMarkdown(toolVersions, reportDepth);
    }

    /// <summary>
    ///     Builds a dictionary mapping tool names to their versions across all jobs.
    /// </summary>
    /// <param name="versionInfos">The collection of VersionInfo records.</param>
    /// <returns>Dictionary mapping tool names to lists of job ID and version pairs.</returns>
    private static Dictionary<string, List<(string JobId, string Version)>> BuildToolVersionsDictionary(
        List<VersionInfo> versionInfos)
    {
        var toolVersions = new Dictionary<string, List<(string JobId, string Version)>>();

        // Iterate through each VersionInfo and build the tool versions dictionary
        foreach (var versionInfo in versionInfos)
        {
            foreach (var (tool, version) in versionInfo.Versions)
            {
                if (!toolVersions.ContainsKey(tool))
                {
                    toolVersions[tool] = [];
                }

                toolVersions[tool].Add((versionInfo.JobId, version));
            }
        }

        return toolVersions;
    }

    /// <summary>
    ///     Generates markdown output from the tool versions dictionary.
    /// </summary>
    /// <param name="toolVersions">Dictionary mapping tool names to job ID and version pairs.</param>
    /// <param name="reportDepth">The heading depth for the section title.</param>
    /// <returns>A markdown-formatted string.</returns>
    private static string GenerateMarkdown(
        Dictionary<string, List<(string JobId, string Version)>> toolVersions,
        int reportDepth)
    {
        var markdown = new StringBuilder();

        // Generate heading using the specified depth
        var headingPrefix = new string('#', reportDepth);
        markdown.AppendLine($"{headingPrefix} Tool Versions");
        markdown.AppendLine();

        // Sort tools alphabetically
        var sortedTools = toolVersions.Keys.OrderBy(k => k, StringComparer.OrdinalIgnoreCase);

        // Generate markdown for each tool
        foreach (var tool in sortedTools)
        {
            var versions = toolVersions[tool];
            var versionEntry = FormatVersionEntry(versions);
            markdown.AppendLine($"- **{tool}**: {versionEntry}");
        }

        return markdown.ToString();
    }

    /// <summary>
    ///     Formats a version entry for a tool based on whether versions are uniform or different.
    /// </summary>
    /// <param name="versions">List of job ID and version pairs for a tool.</param>
    /// <returns>Formatted version string with job IDs when appropriate.</returns>
    private static string FormatVersionEntry(List<(string JobId, string Version)> versions)
    {
        // Check if all versions are the same
        var distinctVersions = versions.Select(v => v.Version).Distinct().ToList();

        // If all versions are the same, show "All jobs"
        if (distinctVersions.Count == 1)
        {
            return $"{distinctVersions[0]} (All jobs)";
        }

        // Otherwise, group by version and show job IDs
        // When versions differ across jobs, we need to show which jobs have which versions
        var versionGroups = versions
            .GroupBy(v => v.Version)
            .OrderBy(g => g.Key, StringComparer.OrdinalIgnoreCase);

        // Build formatted version strings with subscripted job IDs
        // Each version gets its own entry showing which jobs use it
        var formattedVersions = new List<string>();
        foreach (var group in versionGroups)
        {
            // For each unique version, collect and sort the job IDs that use it
            var jobIds = group.Select(v => v.JobId).OrderBy(j => j, StringComparer.OrdinalIgnoreCase);
            var jobIdList = string.Join(", ", jobIds);
            
            // Format as "version <sub>(job1, job2)</sub>" for HTML subscript rendering
            formattedVersions.Add($"{group.Key} <sub>({jobIdList})</sub>");
        }

        // Join all version entries with commas to create the final output
        return string.Join(", ", formattedVersions);
    }
}
