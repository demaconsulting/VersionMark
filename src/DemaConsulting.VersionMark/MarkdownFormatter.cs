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
                // Use TryGetValue to avoid double lookup
                if (!toolVersions.TryGetValue(tool, out var versions))
                {
                    versions = [];
                    toolVersions[tool] = versions;
                }

                versions.Add((versionInfo.JobId, version));
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
            FormatVersionEntries(markdown, tool, versions);
        }

        return markdown.ToString();
    }

    /// <summary>
    ///     Formats version entries for a tool as multiple bullets when versions differ.
    /// </summary>
    /// <param name="markdown">The StringBuilder to append to.</param>
    /// <param name="tool">The tool name.</param>
    /// <param name="versions">List of job ID and version pairs for a tool.</param>
    private static void FormatVersionEntries(StringBuilder markdown, string tool, List<(string JobId, string Version)> versions)
    {
        // Check if all versions are the same
        var distinctVersions = versions.Select(v => v.Version).Distinct().ToList();

        // If all versions are the same, show single entry without job IDs
        if (distinctVersions.Count == 1)
        {
            markdown.AppendLine($"- **{tool}**: {distinctVersions[0]}");
            return;
        }

        // Otherwise, create multiple bullets - one for each version group
        var versionGroups = versions
            .GroupBy(v => v.Version)
            .OrderBy(g => g.Key, StringComparer.OrdinalIgnoreCase);

        foreach (var group in versionGroups)
        {
            // For each unique version, collect and sort the job IDs that use it
            var jobIds = group.Select(v => v.JobId).OrderBy(j => j, StringComparer.OrdinalIgnoreCase);
            var jobIdList = string.Join(", ", jobIds);

            // Format as separate bullet with tool name and version, showing which jobs use it
            markdown.AppendLine($"- **{tool}**: {group.Key} ({jobIdList})");
        }
    }
}
