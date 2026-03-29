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

using System.Text.RegularExpressions;
using DemaConsulting.VersionMark.Cli;
using YamlDotNet.RepresentationModel;

namespace DemaConsulting.VersionMark.Linting;

/// <summary>
///     Provides lint functionality for checking .versionmark.yaml configuration files.
/// </summary>
internal static class Lint
{
    /// <summary>
    ///     Known valid keys for tool configuration entries.
    /// </summary>
    private static readonly HashSet<string> ValidToolKeys =
    [
        "command",
        "command-win",
        "command-linux",
        "command-macos",
        "regex",
        "regex-win",
        "regex-linux",
        "regex-macos"
    ];

    /// <summary>
    ///     Timeout for regex compilation and matching.
    /// </summary>
    private static readonly TimeSpan RegexTimeout = TimeSpan.FromSeconds(1);

    /// <summary>
    ///     Runs lint checks on the specified configuration file and reports all issues.
    /// </summary>
    /// <param name="context">The context for reporting output.</param>
    /// <param name="configFile">Path to the configuration file to lint.</param>
    /// <returns><see langword="true"/> if no issues were found; otherwise <see langword="false"/>.</returns>
    public static bool Run(Context context, string configFile)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(configFile);

        context.WriteLine($"Linting '{configFile}'...");

        // Check if file exists
        if (!File.Exists(configFile))
        {
            context.WriteError($"{configFile}: error: Configuration file not found");
            return false;
        }

        // Parse and validate YAML
        YamlStream yaml;
        try
        {
            using var reader = new StreamReader(configFile, System.Text.Encoding.UTF8);
            yaml = new YamlStream();
            yaml.Load(reader);
        }
        catch (YamlDotNet.Core.YamlException ex)
        {
            var location = FormatLocation(configFile, ex.Start.Line, ex.Start.Column);
            context.WriteError($"{location}: error: {ex.Message}");
            return false;
        }

        var issueCount = 0;

        // Check document exists
        if (yaml.Documents.Count == 0)
        {
            context.WriteError($"{configFile}: error: YAML file contains no documents");
            issueCount++;
            return issueCount == 0;
        }

        // Validate root is a mapping
        if (yaml.Documents[0].RootNode is not YamlMappingNode rootNode)
        {
            var location = FormatLocation(configFile, yaml.Documents[0].RootNode.Start.Line, yaml.Documents[0].RootNode.Start.Column);
            context.WriteError($"{location}: error: Root node must be a YAML mapping");
            issueCount++;
            return issueCount == 0;
        }

        // Check for unknown top-level keys (warnings are non-fatal)
        foreach (var keyNode in rootNode.Children.Keys.OfType<YamlScalarNode>().Where(k => k.Value != "tools"))
        {
            var location = FormatLocation(configFile, keyNode.Start.Line, keyNode.Start.Column);
            context.WriteLine($"{location}: warning: Unknown top-level key '{keyNode.Value}'");
        }

        // Check tools section exists
        if (!rootNode.Children.TryGetValue(new YamlScalarNode("tools"), out var toolsNode))
        {
            var location = FormatLocation(configFile, rootNode.Start.Line, rootNode.Start.Column);
            context.WriteError($"{location}: error: Configuration must contain a 'tools' section");
            issueCount++;
            return issueCount == 0;
        }

        // Validate tools is a mapping
        if (toolsNode is not YamlMappingNode toolsMapping)
        {
            var location = FormatLocation(configFile, toolsNode.Start.Line, toolsNode.Start.Column);
            context.WriteError($"{location}: error: The 'tools' section must be a mapping");
            issueCount++;
            return issueCount == 0;
        }

        // Check at least one tool is defined
        if (toolsMapping.Children.Count == 0)
        {
            var location = FormatLocation(configFile, toolsMapping.Start.Line, toolsMapping.Start.Column);
            context.WriteError($"{location}: error: Configuration must contain at least one tool");
            issueCount++;
        }

        // Validate each tool
        foreach (var toolEntry in toolsMapping.Children)
        {
            // Tool name must be a scalar
            if (toolEntry.Key is not YamlScalarNode toolKeyNode)
            {
                var location = FormatLocation(configFile, toolEntry.Key.Start.Line, toolEntry.Key.Start.Column);
                context.WriteError($"{location}: error: Tool names must be scalar values");
                issueCount++;
                continue;
            }

            var toolName = toolKeyNode.Value ?? string.Empty;

            // Tool config must be a mapping
            if (toolEntry.Value is not YamlMappingNode toolNode)
            {
                var location = FormatLocation(configFile, toolEntry.Value.Start.Line, toolEntry.Value.Start.Column);
                context.WriteError($"{location}: error: Tool '{toolName}' configuration must be a mapping");
                issueCount++;
                continue;
            }

            issueCount += ValidateTool(context, configFile, toolName, toolNode);
        }

        if (issueCount == 0)
        {
            context.WriteLine($"'{configFile}': No issues found");
        }

        return issueCount == 0;
    }

    /// <summary>
    ///     Validates a single tool configuration node and reports all issues.
    /// </summary>
    /// <param name="context">The context for reporting output.</param>
    /// <param name="configFile">Path to the configuration file (for error messages).</param>
    /// <param name="toolName">The name of the tool being validated.</param>
    /// <param name="toolNode">The YAML mapping node for the tool.</param>
    /// <returns>The number of issues found.</returns>
    private static int ValidateTool(Context context, string configFile, string toolName, YamlMappingNode toolNode)
    {
        var issueCount = 0;
        var hasCommand = false;
        var hasRegex = false;

        foreach (var entry in toolNode.Children)
        {
            // All tool config entries must be scalar key-value pairs
            if (entry.Key is not YamlScalarNode entryKeyNode)
            {
                var location = FormatLocation(configFile, entry.Key.Start.Line, entry.Key.Start.Column);
                context.WriteError($"{location}: error: Tool '{toolName}' configuration keys must be scalar values");
                issueCount++;
                continue;
            }

            if (entry.Value is not YamlScalarNode entryValueNode)
            {
                var location = FormatLocation(configFile, entry.Value.Start.Line, entry.Value.Start.Column);
                context.WriteError($"{location}: error: Tool '{toolName}' configuration values must be scalar values");
                issueCount++;
                continue;
            }

            var key = entryKeyNode.Value ?? string.Empty;
            var value = entryValueNode.Value ?? string.Empty;

            // Check for unknown keys (warnings are non-fatal)
            if (!ValidToolKeys.Contains(key))
            {
                var location = FormatLocation(configFile, entry.Key.Start.Line, entry.Key.Start.Column);
                context.WriteLine($"{location}: warning: Tool '{toolName}' has unknown key '{key}'");
            }

            // Track required fields
            if (key == "command")
            {
                hasCommand = true;

                // Validate command is not empty
                if (string.IsNullOrWhiteSpace(value))
                {
                    var location = FormatLocation(configFile, entry.Value.Start.Line, entry.Value.Start.Column);
                    context.WriteError($"{location}: error: Tool '{toolName}' 'command' must not be empty");
                    issueCount++;
                }
            }
            else if (key is "command-win" or "command-linux" or "command-macos")
            {
                // Validate OS-specific command overrides are not empty
                if (string.IsNullOrWhiteSpace(value))
                {
                    var location = FormatLocation(configFile, entry.Value.Start.Line, entry.Value.Start.Column);
                    context.WriteError($"{location}: error: Tool '{toolName}' '{key}' must not be empty");
                    issueCount++;
                }
            }
            else if (key == "regex")
            {
                hasRegex = true;

                // Validate regex is not empty
                if (string.IsNullOrWhiteSpace(value))
                {
                    var location = FormatLocation(configFile, entry.Value.Start.Line, entry.Value.Start.Column);
                    context.WriteError($"{location}: error: Tool '{toolName}' 'regex' must not be empty");
                    issueCount++;
                }
                else
                {
                    // Validate regex can be compiled
                    var compiledRegex = TryCompileRegex(context, configFile, toolName, key, value, entry.Value);
                    if (compiledRegex == null)
                    {
                        issueCount++;
                    }
                    else if (!compiledRegex.GetGroupNames().Contains("version"))
                    {
                        var location = FormatLocation(configFile, entry.Value.Start.Line, entry.Value.Start.Column);
                        context.WriteError($"{location}: error: Tool '{toolName}' 'regex' must contain a named 'version' capture group: (?<version>...)");
                        issueCount++;
                    }
                }
            }
            else if (key is "regex-win" or "regex-linux" or "regex-macos")
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    var location = FormatLocation(configFile, entry.Value.Start.Line, entry.Value.Start.Column);
                    context.WriteError($"{location}: error: Tool '{toolName}' '{key}' must not be empty");
                    issueCount++;
                }
                else
                {
                    // Validate OS-specific regex can be compiled
                    var compiledRegex = TryCompileRegex(context, configFile, toolName, key, value, entry.Value);
                    if (compiledRegex == null)
                    {
                        issueCount++;
                    }
                    else if (!compiledRegex.GetGroupNames().Contains("version"))
                    {
                        var location = FormatLocation(configFile, entry.Value.Start.Line, entry.Value.Start.Column);
                        context.WriteError($"{location}: error: Tool '{toolName}' '{key}' must contain a named 'version' capture group: (?<version>...)");
                        issueCount++;
                    }
                }
            }
        }

        // Report missing required fields
        if (!hasCommand)
        {
            var location = FormatLocation(configFile, toolNode.Start.Line, toolNode.Start.Column);
            context.WriteError($"{location}: error: Tool '{toolName}' must have a 'command' field");
            issueCount++;
        }

        if (!hasRegex)
        {
            var location = FormatLocation(configFile, toolNode.Start.Line, toolNode.Start.Column);
            context.WriteError($"{location}: error: Tool '{toolName}' must have a 'regex' field");
            issueCount++;
        }

        return issueCount;
    }

    /// <summary>
    ///     Tries to compile a regex pattern, reporting an error if compilation fails.
    /// </summary>
    /// <param name="context">The context for reporting output.</param>
    /// <param name="configFile">Path to the configuration file (for error messages).</param>
    /// <param name="toolName">The name of the tool being validated.</param>
    /// <param name="key">The configuration key (for error messages).</param>
    /// <param name="value">The regex pattern to compile.</param>
    /// <param name="node">The YAML node (for location).</param>
    /// <returns>The compiled <see cref="Regex"/>, or <see langword="null"/> if compilation failed.</returns>
    private static Regex? TryCompileRegex(Context context, string configFile, string toolName, string key, string value, YamlNode node)
    {
        try
        {
            return new Regex(value, RegexOptions.None, RegexTimeout);
        }
        catch (ArgumentException ex)
        {
            var location = FormatLocation(configFile, node.Start.Line, node.Start.Column);
            context.WriteError($"{location}: error: Tool '{toolName}' '{key}' contains an invalid regex: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    ///     Formats a file location string with 1-based line and column information.
    /// </summary>
    /// <param name="filePath">The file path.</param>
    /// <param name="line">The line number (0-based, as provided by YamlDotNet).</param>
    /// <param name="column">The column number (0-based, as provided by YamlDotNet).</param>
    /// <returns>A formatted location string with 1-based line and column numbers.</returns>
    private static string FormatLocation(string filePath, long line, long column)
    {
        // Convert from 0-based (YamlDotNet) to 1-based (display)
        return $"{filePath}({line + 1},{column + 1})";
    }
}
