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
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace DemaConsulting.TemplateDotNetTool;

/// <summary>
///     Custom YAML type converter for ToolConfig that handles OS-suffixed keys.
/// </summary>
internal sealed class ToolConfigConverter : IYamlTypeConverter
{
    /// <summary>
    ///     Determines if this converter can handle the specified type.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns>True if this converter can handle the type.</returns>
    public bool Accepts(Type type)
    {
        return type == typeof(ToolConfig);
    }

    /// <summary>
    ///     Reads a ToolConfig from YAML.
    /// </summary>
    /// <param name="parser">The YAML parser.</param>
    /// <param name="type">The type to deserialize.</param>
    /// <param name="rootDeserializer">The root deserializer.</param>
    /// <returns>The deserialized ToolConfig.</returns>
    public object? ReadYaml(IParser parser, Type type, ObjectDeserializer rootDeserializer)
    {
        var commands = new Dictionary<string, string>();
        var regexes = new Dictionary<string, string>();

        parser.Consume<MappingStart>();

        while (!parser.TryConsume<MappingEnd>(out _))
        {
            var key = parser.Consume<Scalar>().Value;
            var value = parser.Consume<Scalar>().Value;

            if (key == "command")
            {
                commands[string.Empty] = value;
            }
            else if (key == "command-win")
            {
                commands["win"] = value;
            }
            else if (key == "command-linux")
            {
                commands["linux"] = value;
            }
            else if (key == "command-macos")
            {
                commands["macos"] = value;
            }
            else if (key == "regex")
            {
                regexes[string.Empty] = value;
            }
            else if (key == "regex-win")
            {
                regexes["win"] = value;
            }
            else if (key == "regex-linux")
            {
                regexes["linux"] = value;
            }
            else if (key == "regex-macos")
            {
                regexes["macos"] = value;
            }
        }

        return new ToolConfig(commands, regexes);
    }

    /// <summary>
    ///     Writes a ToolConfig to YAML.
    /// </summary>
    /// <param name="emitter">The YAML emitter.</param>
    /// <param name="value">The value to serialize.</param>
    /// <param name="type">The type to serialize.</param>
    /// <param name="serializer">The object serializer.</param>
    /// <exception cref="NotImplementedException">Serialization is not supported.</exception>
    public void WriteYaml(IEmitter emitter, object? value, Type type, ObjectSerializer serializer)
    {
        throw new NotImplementedException("Serialization of ToolConfig is not supported. This converter is designed for deserialization only.");
    }
}

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

            // Create deserializer with custom converter
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(HyphenatedNamingConvention.Instance)
                .WithTypeConverter(new ToolConfigConverter())
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
