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
    /// <returns>A new ToolConfig instance.</returns>
    /// <exception cref="ArgumentException">Thrown when required fields are missing or node types are invalid.</exception>
    internal static ToolConfig FromYamlNode(YamlMappingNode node)
    {
        var commands = new Dictionary<string, string>();
        var regexes = new Dictionary<string, string>();

        foreach (var entry in node.Children)
        {
            if (entry.Key is not YamlScalarNode keyNode || entry.Value is not YamlScalarNode valueNode)
            {
                throw new ArgumentException("Tool configuration entries must be scalar key-value pairs");
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

        if (!commands.ContainsKey(string.Empty))
        {
            throw new ArgumentException("Tool configuration must contain a default 'command' field");
        }

        if (!regexes.ContainsKey(string.Empty))
        {
            throw new ArgumentException("Tool configuration must contain a default 'regex' field");
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
            using var reader = new StreamReader(filePath);
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
                tools[toolName] = ToolConfig.FromYamlNode(toolNode);
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
}
