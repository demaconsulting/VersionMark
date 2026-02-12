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
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace DemaConsulting.TemplateDotNetTool;

/// <summary>
///     Configuration for a single tool in .versionmark.yaml file.
/// </summary>
public sealed record ToolConfig
{
    /// <summary>
    ///     Gets the default command to execute to get the tool version.
    /// </summary>
    [YamlMember(Alias = "command")]
    public required string Command { get; init; }

    /// <summary>
    ///     Gets the Windows-specific command override.
    /// </summary>
    [YamlMember(Alias = "command-win")]
    public string? CommandWin { get; init; }

    /// <summary>
    ///     Gets the Linux-specific command override.
    /// </summary>
    [YamlMember(Alias = "command-linux")]
    public string? CommandLinux { get; init; }

    /// <summary>
    ///     Gets the macOS-specific command override.
    /// </summary>
    [YamlMember(Alias = "command-macos")]
    public string? CommandMacOs { get; init; }

    /// <summary>
    ///     Gets the default regular expression to extract the version from command output.
    /// </summary>
    [YamlMember(Alias = "regex")]
    public required string Regex { get; init; }

    /// <summary>
    ///     Gets the Windows-specific regex override.
    /// </summary>
    [YamlMember(Alias = "regex-win")]
    public string? RegexWin { get; init; }

    /// <summary>
    ///     Gets the Linux-specific regex override.
    /// </summary>
    [YamlMember(Alias = "regex-linux")]
    public string? RegexLinux { get; init; }

    /// <summary>
    ///     Gets the macOS-specific regex override.
    /// </summary>
    [YamlMember(Alias = "regex-macos")]
    public string? RegexMacOs { get; init; }

    /// <summary>
    ///     Gets the effective command for the current operating system.
    /// </summary>
    /// <returns>The command to execute based on the current OS.</returns>
    public string GetEffectiveCommand()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && CommandWin != null)
        {
            return CommandWin;
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && CommandLinux != null)
        {
            return CommandLinux;
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) && CommandMacOs != null)
        {
            return CommandMacOs;
        }

        return Command;
    }

    /// <summary>
    ///     Gets the effective regex for the current operating system.
    /// </summary>
    /// <returns>The regex to use based on the current OS.</returns>
    public string GetEffectiveRegex()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && RegexWin != null)
        {
            return RegexWin;
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && RegexLinux != null)
        {
            return RegexLinux;
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) && RegexMacOs != null)
        {
            return RegexMacOs;
        }

        return Regex;
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
    [YamlMember(Alias = "tools")]
    public required Dictionary<string, ToolConfig> Tools { get; init; }

    /// <summary>
    ///     Parameterless constructor required for YAML deserialization.
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
            // Read the YAML file
            var yaml = File.ReadAllText(filePath);

            // Create deserializer
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(HyphenatedNamingConvention.Instance)
                .Build();

            // Deserialize the YAML
            var config = deserializer.Deserialize<VersionMarkConfig>(yaml);

            // Validate configuration
            if (config.Tools.Count == 0)
            {
                throw new ArgumentException("Configuration must contain at least one tool");
            }

            return config;
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
