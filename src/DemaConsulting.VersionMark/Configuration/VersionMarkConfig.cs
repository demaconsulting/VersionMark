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

using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using DemaConsulting.VersionMark.Capture;
using YamlDotNet.Core;
using YamlDotNet.RepresentationModel;

namespace DemaConsulting.VersionMark.Configuration;

/// <summary>
///     Configuration for a single tool in .versionmark.yaml file.
/// </summary>
public sealed record ToolConfig
{
    /// <summary>
    ///     Gets the dictionary of commands keyed by OS name (empty string for default, "win", "linux", "macos" for OS-specific).
    /// </summary>
    public IReadOnlyDictionary<string, string> Command { get; init; }

    /// <summary>
    ///     Gets the dictionary of regular expressions keyed by OS name (empty string for default, "win", "linux", "macos" for OS-specific).
    /// </summary>
    public IReadOnlyDictionary<string, string> Regex { get; init; }

    /// <summary>
    ///     Internal constructor for creating configurations.
    /// </summary>
    /// <param name="command">Dictionary of commands.</param>
    /// <param name="regex">Dictionary of regular expressions.</param>
    internal ToolConfig(Dictionary<string, string> command, Dictionary<string, string> regex)
    {
        Command = command;
        Regex = regex;
    }

    /// <summary>
    ///     Gets the current operating system name.
    /// </summary>
    /// <returns>The OS name: "win", "linux", "macos", or empty string if unknown.</returns>
    private static string GetCurrentOs()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return "win";
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return "linux";
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return "macos";
        }

        return string.Empty;
    }

    /// <summary>
    ///     Gets the effective command for the specified operating system.
    /// </summary>
    /// <param name="os">The operating system name, or null to use current OS.</param>
    /// <returns>The command to execute based on the specified OS.</returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when no OS-specific override exists for <paramref name="os"/> and no default
    ///     (empty-string) key is present in the <see cref="Command"/> dictionary.
    /// </exception>
    public string GetEffectiveCommand(string? os = null)
    {
        os ??= GetCurrentOs();

        if (Command.TryGetValue(os, out var osCommand))
        {
            return osCommand;
        }

        return Command.TryGetValue(string.Empty, out var defaultCommand)
            ? defaultCommand
            : throw new InvalidOperationException("No default command specified");
    }

    /// <summary>
    ///     Gets the effective regex for the specified operating system.
    /// </summary>
    /// <param name="os">The operating system name, or null to use current OS.</param>
    /// <returns>The regex to use based on the specified OS.</returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when no OS-specific override exists for <paramref name="os"/> and no default
    ///     (empty-string) key is present in the <see cref="Regex"/> dictionary.
    /// </exception>
    public string GetEffectiveRegex(string? os = null)
    {
        os ??= GetCurrentOs();

        if (Regex.TryGetValue(os, out var osRegex))
        {
            return osRegex;
        }

        return Regex.TryGetValue(string.Empty, out var defaultRegex)
            ? defaultRegex
            : throw new InvalidOperationException("No default regex specified");
    }
}

/// <summary>
///     Configuration loaded from .versionmark.yaml file.
/// </summary>
public sealed record VersionMarkConfig
{
    /// <summary>
    ///     Gets the dictionary of tool configurations keyed by tool name.
    /// </summary>
    public required Dictionary<string, ToolConfig> Tools { get; init; }

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
    ///     Parameterless constructor required for object initializer.
    /// </summary>
    public VersionMarkConfig()
    {
        Tools = [];
    }

    /// <summary>
    ///     Internal constructor for creating test configurations.
    /// </summary>
    /// <param name="tools">Dictionary of tool configurations.</param>
    [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
    internal VersionMarkConfig(Dictionary<string, ToolConfig> tools)
    {
        Tools = tools;
    }

    /// <summary>
    ///     Loads configuration from a .versionmark.yaml file, performing all lint validation
    ///     in a single pass and returning both the configuration and any issues found.
    /// </summary>
    /// <param name="filePath">Path to the YAML configuration file.</param>
    /// <returns>
    ///     A <see cref="VersionMarkLoadResult"/> containing the loaded <see cref="VersionMarkConfig"/>
    ///     (or <see langword="null"/> when fatal errors prevent loading) and a read-only list of
    ///     <see cref="LintIssue"/> objects describing all warnings and errors encountered.
    /// </returns>
    public static VersionMarkLoadResult Load(string filePath)
    {
        var issues = new List<LintIssue>();

        // Parse YAML, reporting any syntax errors with their source location
        YamlStream yaml;
        try
        {
            using var reader = new StreamReader(filePath, System.Text.Encoding.UTF8);
            yaml = new YamlStream();
            yaml.Load(reader);
        }
        catch (YamlException ex)
        {
            issues.Add(new LintIssue(filePath, ex.Start.Line + 1, ex.Start.Column + 1, LintSeverity.Error, $"Failed to parse YAML file: {ex.Message}"));
            return new VersionMarkLoadResult(null, issues);
        }
        catch (Exception ex) when (ex is FileNotFoundException or DirectoryNotFoundException)
        {
            issues.Add(new LintIssue(filePath, 1, 1, LintSeverity.Error, "Configuration file not found"));
            return new VersionMarkLoadResult(null, issues);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            issues.Add(new LintIssue(filePath, 1, 1, LintSeverity.Error, $"Failed to read file: {ex.Message}"));
            return new VersionMarkLoadResult(null, issues);
        }

        // Validate that the file contains at least one YAML document
        if (yaml.Documents.Count == 0)
        {
            issues.Add(new LintIssue(filePath, 1, 1, LintSeverity.Error, "YAML file contains no documents"));
            return new VersionMarkLoadResult(null, issues);
        }

        // Validate that the root node is a YAML mapping
        if (yaml.Documents[0].RootNode is not YamlMappingNode rootNode)
        {
            var node = yaml.Documents[0].RootNode;
            issues.Add(CreateIssue(filePath, node, LintSeverity.Error, "Root node must be a YAML mapping"));
            return new VersionMarkLoadResult(null, issues);
        }

        // Warn about unknown top-level keys; they are non-fatal
        foreach (var keyNode in rootNode.Children.Keys.OfType<YamlScalarNode>().Where(k => k.Value != "tools"))
        {
            issues.Add(CreateIssue(filePath, keyNode, LintSeverity.Warning, $"Unknown top-level key '{keyNode.Value}'"));
        }

        // Ensure a 'tools' section is present
        if (!rootNode.Children.TryGetValue(new YamlScalarNode("tools"), out var toolsNode))
        {
            issues.Add(CreateIssue(filePath, rootNode, LintSeverity.Error, "Configuration must contain a 'tools' section"));
            return new VersionMarkLoadResult(null, issues);
        }

        // Validate that 'tools' is a YAML mapping
        if (toolsNode is not YamlMappingNode toolsMapping)
        {
            issues.Add(CreateIssue(filePath, toolsNode, LintSeverity.Error, "The 'tools' section must be a mapping"));
            return new VersionMarkLoadResult(null, issues);
        }

        // Require at least one tool to be defined
        if (toolsMapping.Children.Count == 0)
        {
            issues.Add(CreateIssue(filePath, toolsMapping, LintSeverity.Error, "Configuration must contain at least one tool"));
            return new VersionMarkLoadResult(null, issues);
        }

        // Validate each tool entry and collect all issues before deciding whether to build the config
        var tools = new Dictionary<string, ToolConfig>();
        foreach (var toolEntry in toolsMapping.Children)
        {
            // Tool names must be scalar values
            if (toolEntry.Key is not YamlScalarNode toolKeyNode)
            {
                issues.Add(CreateIssue(filePath, toolEntry.Key, LintSeverity.Error, "Tool names must be scalar values"));
                continue;
            }

            var toolName = toolKeyNode.Value ?? string.Empty;

            // Tool names must be non-empty
            if (string.IsNullOrWhiteSpace(toolName))
            {
                issues.Add(CreateIssue(filePath, toolKeyNode, LintSeverity.Error, "Tool names must not be empty or whitespace"));
                continue;
            }

            // Tool configuration must be a mapping
            if (toolEntry.Value is not YamlMappingNode toolNode)
            {
                issues.Add(CreateIssue(filePath, toolEntry.Value, LintSeverity.Error, $"Tool '{toolName}' configuration must be a mapping"));
                continue;
            }

            // Validate all fields within the tool and accumulate issues
            ValidateTool(filePath, toolName, toolNode, issues, out var toolConfig);
            if (toolConfig != null)
            {
                tools[toolName] = toolConfig;
            }
        }

        // Return null config if any errors were found, so callers can distinguish warnings-only from failures
        if (issues.Any(i => i.Severity == LintSeverity.Error))
        {
            return new VersionMarkLoadResult(null, issues);
        }

        // Build and return the successfully validated configuration
        return new VersionMarkLoadResult(new VersionMarkConfig(tools), issues);
    }

    /// <summary>
    ///     Reads configuration from a .versionmark.yaml file.
    /// </summary>
    /// <param name="filePath">Path to the YAML configuration file.</param>
    /// <returns>Parsed configuration.</returns>
    /// <exception cref="ArgumentException">Thrown when the file cannot be read or parsed.</exception>
    public static VersionMarkConfig ReadFromFile(string filePath)
    {
        // Delegate to Load and convert the first error to an ArgumentException for backward compatibility
        var result = Load(filePath);
        var firstError = result.Issues.FirstOrDefault(i => i.Severity == LintSeverity.Error);
        if (firstError != null)
        {
            throw new ArgumentException(firstError.ToString());
        }

        return result.Config!;
    }

    /// <summary>
    ///     Validates a single tool's configuration node, adding issues to the shared list
    ///     and producing a <see cref="ToolConfig"/> when all required fields are valid.
    /// </summary>
    /// <param name="filePath">Path to the configuration file (for issue location).</param>
    /// <param name="toolName">Name of the tool being validated.</param>
    /// <param name="toolNode">YAML mapping node containing the tool's fields.</param>
    /// <param name="issues">Shared list to which all discovered issues are appended.</param>
    /// <param name="toolConfig">
    ///     Set to a <see cref="ToolConfig"/> when validation succeeds; otherwise <see langword="null"/>.
    /// </param>
    private static void ValidateTool(
        string filePath,
        string toolName,
        YamlMappingNode toolNode,
        List<LintIssue> issues,
        out ToolConfig? toolConfig)
    {
        var commands = new Dictionary<string, string>();
        var regexes = new Dictionary<string, string>();
        var hasCommand = false;
        var hasRegex = false;
        var toolIssuesBefore = issues.Count;

        foreach (var entry in toolNode.Children)
        {
            // All tool config keys must be scalar values
            if (entry.Key is not YamlScalarNode entryKeyNode)
            {
                issues.Add(CreateIssue(filePath, entry.Key, LintSeverity.Error, $"Tool '{toolName}' configuration keys must be scalar values"));
                continue;
            }

            // All tool config values must be scalar values
            if (entry.Value is not YamlScalarNode entryValueNode)
            {
                issues.Add(CreateIssue(filePath, entry.Value, LintSeverity.Error, $"Tool '{toolName}' configuration values must be scalar values"));
                continue;
            }

            var key = entryKeyNode.Value ?? string.Empty;
            var value = entryValueNode.Value ?? string.Empty;

            // Warn about unrecognized keys without failing validation
            if (!ValidToolKeys.Contains(key))
            {
                issues.Add(CreateIssue(filePath, entryKeyNode, LintSeverity.Warning, $"Tool '{toolName}' has unknown key '{key}'"));
            }

            // Validate default command
            if (key == "command")
            {
                hasCommand = true;
                if (string.IsNullOrWhiteSpace(value))
                {
                    issues.Add(CreateIssue(filePath, entryValueNode, LintSeverity.Error, $"Tool '{toolName}' 'command' must not be empty"));
                }
                else
                {
                    commands[string.Empty] = value;
                }
            }

            // Validate OS-specific command overrides
            else if (key is "command-win" or "command-linux" or "command-macos")
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    issues.Add(CreateIssue(filePath, entryValueNode, LintSeverity.Error, $"Tool '{toolName}' '{key}' must not be empty"));
                }
                else
                {
                    var os = key["command-".Length..];
                    commands[os] = value;
                }
            }

            // Validate default regex
            else if (key == "regex")
            {
                hasRegex = true;
                if (string.IsNullOrWhiteSpace(value))
                {
                    issues.Add(CreateIssue(filePath, entryValueNode, LintSeverity.Error, $"Tool '{toolName}' 'regex' must not be empty"));
                }
                else
                {
                    var compiled = TryCompileRegex(filePath, toolName, key, value, entryValueNode, issues);
                    if (compiled != null)
                    {
                        if (!compiled.GetGroupNames().Contains("version"))
                        {
                            issues.Add(CreateIssue(filePath, entryValueNode, LintSeverity.Error, $"Tool '{toolName}' 'regex' must contain a named 'version' capture group: (?<version>...)"));
                        }
                        else
                        {
                            regexes[string.Empty] = value;
                        }
                    }
                }
            }

            // Validate OS-specific regex overrides
            else if (key is "regex-win" or "regex-linux" or "regex-macos")
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    issues.Add(CreateIssue(filePath, entryValueNode, LintSeverity.Error, $"Tool '{toolName}' '{key}' must not be empty"));
                }
                else
                {
                    var compiled = TryCompileRegex(filePath, toolName, key, value, entryValueNode, issues);
                    if (compiled != null)
                    {
                        if (!compiled.GetGroupNames().Contains("version"))
                        {
                            issues.Add(CreateIssue(filePath, entryValueNode, LintSeverity.Error, $"Tool '{toolName}' '{key}' must contain a named 'version' capture group: (?<version>...)"));
                        }
                        else
                        {
                            var os = key["regex-".Length..];
                            regexes[os] = value;
                        }
                    }
                }
            }
        }

        // Report missing required fields after scanning all entries
        if (!hasCommand)
        {
            issues.Add(CreateIssue(filePath, toolNode, LintSeverity.Error, $"Tool '{toolName}' must have a 'command' field"));
        }

        if (!hasRegex)
        {
            issues.Add(CreateIssue(filePath, toolNode, LintSeverity.Error, $"Tool '{toolName}' must have a 'regex' field"));
        }

        // Only produce a ToolConfig when no new errors were added for this tool
        var hasNewErrors = issues.Skip(toolIssuesBefore).Any(i => i.Severity == LintSeverity.Error);
        toolConfig = hasNewErrors ? null : new ToolConfig(commands, regexes);
    }

    /// <summary>
    ///     Attempts to compile a regular expression, adding an error issue when compilation fails.
    /// </summary>
    /// <param name="filePath">Path to the configuration file (for issue location).</param>
    /// <param name="toolName">Name of the tool owning the regex (for error messages).</param>
    /// <param name="key">Configuration key of the regex field (for error messages).</param>
    /// <param name="value">Regex pattern to compile.</param>
    /// <param name="node">YAML node that holds the value (for source location).</param>
    /// <param name="issues">Shared list to which a compilation error is appended on failure.</param>
    /// <returns>The compiled <see cref="Regex"/>, or <see langword="null"/> if compilation failed.</returns>
    private static Regex? TryCompileRegex(
        string filePath,
        string toolName,
        string key,
        string value,
        YamlNode node,
        List<LintIssue> issues)
    {
        try
        {
            return new Regex(value, RegexOptions.Multiline | RegexOptions.IgnoreCase, RegexTimeout);
        }
        catch (ArgumentException ex)
        {
            issues.Add(CreateIssue(filePath, node, LintSeverity.Error, $"Tool '{toolName}' '{key}' contains an invalid regex: {ex.Message}"));
            return null;
        }
    }

    /// <summary>
    ///     Creates a <see cref="LintIssue"/> using the source location of a YAML node,
    ///     converting from the zero-based offsets reported by YamlDotNet to one-based display values.
    /// </summary>
    /// <param name="filePath">Path to the configuration file.</param>
    /// <param name="node">YAML node whose start position provides the line and column.</param>
    /// <param name="severity">Severity of the issue.</param>
    /// <param name="description">Human-readable description of the issue.</param>
    /// <returns>A new <see cref="LintIssue"/> with one-based line and column numbers.</returns>
    private static LintIssue CreateIssue(string filePath, YamlNode node, LintSeverity severity, string description)
        => new(filePath, node.Start.Line + 1, node.Start.Column + 1, severity, description);

    /// <summary>
    ///     Finds versions for the specified tools.
    /// </summary>
    /// <param name="toolNames">List of tool names to query.</param>
    /// <param name="jobId">Job ID for this version capture.</param>
    /// <returns>VersionInfo record containing the job ID and tool versions.</returns>
    public VersionInfo FindVersions(IEnumerable<string> toolNames, string jobId)
    {
        var versions = new Dictionary<string, string>();

        foreach (var toolName in toolNames)
        {
            if (!Tools.TryGetValue(toolName, out var toolConfig))
            {
                throw new ArgumentException($"Tool '{toolName}' not found in configuration");
            }

            var command = toolConfig.GetEffectiveCommand();
            var regex = toolConfig.GetEffectiveRegex();

            var output = RunCommand(command);
            var version = ExtractVersion(output, regex, toolName);

            versions[toolName] = version;
        }

        return new VersionInfo(jobId, versions);
    }

    /// <summary>
    ///     Runs a command and captures its output.
    /// </summary>
    /// <param name="command">The command to run.</param>
    /// <returns>The combined stdout and stderr output.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the command fails to execute.</exception>
    /// <remarks>
    ///     Commands are delegated to the OS shell (<c>cmd.exe /c</c> on Windows, <c>/bin/sh -c</c>
    ///     elsewhere) via <c>ArgumentList</c> to avoid escaping issues. This supports <c>.cmd</c>/<c>.bat</c>
    ///     files on Windows and shell features (pipes, redirects, built-ins) on all platforms.
    /// </remarks>
    private static string RunCommand(string command)
    {
        // To support .cmd/.bat files on Windows and shell features on all platforms,
        // we run commands through the appropriate shell using ArgumentList to avoid escaping issues
        var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        try
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = isWindows ? "cmd.exe" : "/bin/sh",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            processStartInfo.ArgumentList.Add(isWindows ? "/c" : "-c");
            processStartInfo.ArgumentList.Add(command);

            using var process = Process.Start(processStartInfo);
            if (process == null)
            {
                throw new InvalidOperationException($"Failed to start process for command: {command}");
            }

            // Read output asynchronously to prevent deadlock if pipes fill up
            var outputTask = process.StandardOutput.ReadToEndAsync();
            var errorTask = process.StandardError.ReadToEndAsync();
            process.WaitForExit();
            Task.WaitAll(outputTask, errorTask);
            var output = outputTask.Result;
            var error = errorTask.Result;

            // Check exit code - if non-zero, command failed
            if (process.ExitCode != 0)
            {
                var errorMessage = string.IsNullOrEmpty(error) ? output : error;
                throw new InvalidOperationException($"Failed to run command '{command}': {errorMessage}");
            }

            // Combine stdout and stderr with newline separator for better debuggability
            if (string.IsNullOrEmpty(error))
            {
                return output;
            }

            return string.IsNullOrEmpty(output) ? error : output + Environment.NewLine + error;
        }
        catch (Exception ex) when (ex is not InvalidOperationException)
        {
            throw new InvalidOperationException($"Failed to run command '{command}': {ex.Message}", ex);
        }
    }

    /// <summary>
    ///     Extracts version from command output using regex.
    /// </summary>
    /// <param name="output">The command output.</param>
    /// <param name="regexPattern">The regex pattern with a named 'version' capture group.</param>
    /// <param name="toolName">The tool name (for error messages).</param>
    /// <returns>The extracted version string.</returns>
    /// <exception cref="InvalidOperationException">Thrown when version cannot be extracted.</exception>
    private static string ExtractVersion(string output, string regexPattern, string toolName)
    {
        var regex = new Regex(
            regexPattern,
            RegexOptions.Multiline | RegexOptions.IgnoreCase,
            RegexTimeout);
        var match = regex.Match(output);

        if (!match.Success)
        {
            throw new InvalidOperationException($"Failed to extract version for tool '{toolName}' using regex: {regexPattern}");
        }

        var versionGroup = match.Groups["version"];
        if (!versionGroup.Success)
        {
            throw new InvalidOperationException($"Regex for tool '{toolName}' must contain a named 'version' capture group");
        }

        return versionGroup.Value;
    }
}
