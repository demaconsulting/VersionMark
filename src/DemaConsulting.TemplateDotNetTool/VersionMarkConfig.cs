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
using YamlDotNet.RepresentationModel;

namespace DemaConsulting.TemplateDotNetTool;

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
    ///     Creates a ToolConfig from a YAML mapping node.
    /// </summary>
    /// <param name="node">The YAML mapping node containing tool configuration.</param>
    /// <param name="toolName">The name of the tool (for error messages).</param>
    /// <returns>A new ToolConfig instance.</returns>
    /// <exception cref="ArgumentException">Thrown when required fields are missing or node types are invalid.</exception>
    /// <remarks>Unknown keys in the configuration are silently ignored to allow for future extensibility.</remarks>
    internal static ToolConfig FromYamlNode(YamlMappingNode node, string toolName = "")
    {
        var commands = new Dictionary<string, string>();
        var regexes = new Dictionary<string, string>();

        foreach (var entry in node.Children)
        {
            if (entry.Key is not YamlScalarNode keyNode || entry.Value is not YamlScalarNode valueNode)
            {
                var toolContext = string.IsNullOrEmpty(toolName) ? "Tool" : $"Tool '{toolName}'";
                throw new ArgumentException($"{toolContext} configuration entries must be scalar key-value pairs");
            }

            var key = keyNode.Value ?? string.Empty;
            var value = valueNode.Value ?? string.Empty;

            switch (key)
            {
                case "command":
                    commands[string.Empty] = value;
                    break;
                case "command-win":
                    commands["win"] = value;
                    break;
                case "command-linux":
                    commands["linux"] = value;
                    break;
                case "command-macos":
                    commands["macos"] = value;
                    break;
                case "regex":
                    regexes[string.Empty] = value;
                    break;
                case "regex-win":
                    regexes["win"] = value;
                    break;
                case "regex-linux":
                    regexes["linux"] = value;
                    break;
                case "regex-macos":
                    regexes["macos"] = value;
                    break;
                default:
                    // Ignore unknown keys to allow for future extensibility
                    break;
            }
        }

        var toolContext2 = string.IsNullOrEmpty(toolName) ? "Tool" : $"Tool '{toolName}'";

        if (!commands.ContainsKey(string.Empty))
        {
            throw new ArgumentException($"{toolContext2} configuration must contain a default 'command' field");
        }

        if (!regexes.ContainsKey(string.Empty))
        {
            throw new ArgumentException($"{toolContext2} configuration must contain a default 'regex' field");
        }

        return new ToolConfig(commands, regexes);
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
///     Version information record containing Job ID and tool versions.
/// </summary>
public sealed record VersionInfo
{
    /// <summary>
    ///     Gets the Job ID for this version capture.
    /// </summary>
    public required string JobId { get; init; }

    /// <summary>
    ///     Gets the dictionary of tool names to version strings.
    /// </summary>
    public required Dictionary<string, string> Versions { get; init; }
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
    ///     Parameterless constructor required for object initializer.
    /// </summary>
    public VersionMarkConfig()
    {
        Tools = new Dictionary<string, ToolConfig>();
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
    ///     Reads configuration from a .versionmark.yaml file.
    /// </summary>
    /// <param name="filePath">Path to the YAML configuration file.</param>
    /// <returns>Parsed configuration.</returns>
    /// <exception cref="ArgumentException">Thrown when the file cannot be read or parsed.</exception>
    public static VersionMarkConfig ReadFromFile(string filePath)
    {
        // Check if file exists
        if (!File.Exists(filePath))
        {
            throw new ArgumentException($"Configuration file not found: {filePath}");
        }

        try
        {
            // Read the YAML file and parse as document
            using var reader = new StreamReader(filePath, System.Text.Encoding.UTF8);
            var yaml = new YamlStream();
            yaml.Load(reader);

            // Validate document exists
            if (yaml.Documents.Count == 0)
            {
                throw new ArgumentException("YAML file contains no documents");
            }

            // Get root document and validate it's a mapping
            if (yaml.Documents[0].RootNode is not YamlMappingNode rootNode)
            {
                throw new ArgumentException("YAML root node must be a mapping");
            }

            // Get tools mapping
            if (!rootNode.Children.TryGetValue(new YamlScalarNode("tools"), out var toolsNode))
            {
                throw new ArgumentException("Configuration file must contain a 'tools' section");
            }

            // Validate tools node is a mapping
            if (toolsNode is not YamlMappingNode toolsMapping)
            {
                throw new ArgumentException("The 'tools' section must be a mapping");
            }

            var tools = new Dictionary<string, ToolConfig>();

            // Parse each tool
            foreach (var toolEntry in toolsMapping.Children)
            {
                if (toolEntry.Key is not YamlScalarNode keyNode)
                {
                    throw new ArgumentException("Tool names must be scalar values");
                }

                if (toolEntry.Value is not YamlMappingNode toolNode)
                {
                    throw new ArgumentException($"Tool '{keyNode.Value}' configuration must be a mapping");
                }

                var toolName = keyNode.Value ?? string.Empty;
                tools[toolName] = ToolConfig.FromYamlNode(toolNode, toolName);
            }

            // Validate configuration
            if (tools.Count == 0)
            {
                throw new ArgumentException("Configuration must contain at least one tool");
            }

            return new VersionMarkConfig(tools);
        }
        catch (YamlDotNet.Core.YamlException ex)
        {
            throw new ArgumentException($"Failed to parse YAML file '{filePath}': {ex.Message}", ex);
        }
        catch (Exception ex) when (ex is not ArgumentException)
        {
            throw new ArgumentException($"Failed to read configuration file '{filePath}': {ex.Message}", ex);
        }
    }

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

        return new VersionInfo
        {
            JobId = jobId,
            Versions = versions
        };
    }

    /// <summary>
    ///     Runs a command and captures its output.
    /// </summary>
    /// <param name="command">The command to run.</param>
    /// <returns>The combined stdout and stderr output.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the command fails to execute.</exception>
    /// <remarks>
    ///     Commands are split on the first space character to separate executable from arguments.
    ///     This does not handle quoted arguments containing spaces.
    /// </remarks>
    private static string RunCommand(string command)
    {
        // Split command into executable and arguments
        var parts = command.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0)
        {
            throw new InvalidOperationException("Command is empty");
        }

        var fileName = parts[0];
        var arguments = parts.Length > 1 ? parts[1] : string.Empty;

        try
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(processStartInfo);
            if (process == null)
            {
                throw new InvalidOperationException($"Failed to start process for command: {command}");
            }

            var output = process.StandardOutput.ReadToEnd();
            var error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            // Combine stdout and stderr with newline separator for better debuggability
            return string.IsNullOrEmpty(error) ? output : output + Environment.NewLine + error;
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
        var regex = new Regex(regexPattern, RegexOptions.Multiline | RegexOptions.IgnoreCase);
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
