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

using DemaConsulting.VersionMark.Configuration;

namespace DemaConsulting.VersionMark.Tests.Configuration;

/// <summary>
///     Subsystem tests for the Configuration subsystem (VersionMarkConfig and ToolConfig working together).
/// </summary>
public class ConfigurationTests
{
    /// <summary>
    ///     Test that reading a multi-tool configuration file produces all tools with usable commands and regexes.
    /// </summary>
    [Fact]
    public void Configuration_ReadFromFile_MultipleTools_AllToolsAccessible()
    {
        // Arrange - Write a valid multi-tool config to a temp file
        var tempFile = Path.GetTempFileName();
        try
        {
            const string yaml = """
                ---
                tools:
                  dotnet:
                    command: dotnet --version
                    regex: '(?<version>\d+\.\d+\.\d+)'
                  git:
                    command: git --version
                    regex: 'git version (?<version>[\d\.]+)'
                """;
            File.WriteAllText(tempFile, yaml);

            // Act - Read the config through the full Configuration subsystem pipeline
            var config = VersionMarkConfig.ReadFromFile(tempFile);

            // Assert - Both tools should be accessible with valid commands and regexes
            Assert.NotNull(config);
            Assert.Equal(2, config.Tools.Count);
            Assert.True(config.Tools.ContainsKey("dotnet"), "dotnet tool should be present");
            Assert.True(config.Tools.ContainsKey("git"), "git tool should be present");
            Assert.False(string.IsNullOrEmpty(config.Tools["dotnet"].GetEffectiveCommand("linux")),
                "dotnet command should be accessible");
            Assert.False(string.IsNullOrEmpty(config.Tools["git"].GetEffectiveRegex("linux")),
                "git regex should be accessible");
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    /// <summary>
    ///     Test that reading a configuration file with OS-specific overrides selects the correct command.
    /// </summary>
    [Fact]
    public void Configuration_ReadFromFile_WithOsOverrides_SelectsAppropriateCommand()
    {
        // Arrange - Write a config with OS-specific overrides to a temp file
        var tempFile = Path.GetTempFileName();
        try
        {
            const string yaml = """
                ---
                tools:
                  dotnet:
                    command: dotnet --version
                    command-win: dotnet.exe --version
                    command-linux: dotnet-linux --version
                    regex: '(?<version>\d+\.\d+\.\d+)'
                """;
            File.WriteAllText(tempFile, yaml);

            // Act - Read the config and get the effective command for each OS
            var config = VersionMarkConfig.ReadFromFile(tempFile);
            var dotnet = config.Tools["dotnet"];

            // Assert - Each OS should return the appropriate override or default
            Assert.Equal("dotnet.exe --version", dotnet.GetEffectiveCommand("win"));
            Assert.Equal("dotnet-linux --version", dotnet.GetEffectiveCommand("linux"));
            Assert.Equal("dotnet --version", dotnet.GetEffectiveCommand("macos"));
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    /// <summary>
    ///     Test that reading a configuration from a missing file throws an ArgumentException.
    /// </summary>
    [Fact]
    public void Configuration_ReadFromFile_MissingFile_ThrowsArgumentException()
    {
        // Arrange - Use a path that does not exist
        var nonExistentFile = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.yaml");

        // Act & Assert
        Assert.Throws<ArgumentException>(() => VersionMarkConfig.ReadFromFile(nonExistentFile));
    }

    /// <summary>
    ///     Test that reading a configuration with an OS-specific regex override returns the appropriate regex.
    /// </summary>
    [Fact]
    public void Configuration_ReadFromFile_OsRegexOverride_SelectsAppropriateRegex()
    {
        // Arrange
        var tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, """
            tools:
              dotnet:
                command: dotnet --version
                regex: '(?<version>\d+\.\d+\.\d+)'
                regex-win: '(?<version>\d+\.\d+\.\d+)-win'
                regex-linux: '(?<version>\d+\.\d+\.\d+)-linux'
            """);

        try
        {
            // Act
            var config = VersionMarkConfig.ReadFromFile(tempFile);
            var tool = config.Tools["dotnet"];

            // Assert - Each OS should return the appropriate override or default
            Assert.Equal(@"(?<version>\d+\.\d+\.\d+)-win", tool.GetEffectiveRegex("win"));
            Assert.Equal(@"(?<version>\d+\.\d+\.\d+)-linux", tool.GetEffectiveRegex("linux"));
            Assert.Equal(@"(?<version>\d+\.\d+\.\d+)", tool.GetEffectiveRegex("macos"));
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    /// <summary>
    ///     Test that reading a configuration with an empty tools section throws an ArgumentException.
    /// </summary>
    [Fact]
    public void Configuration_ReadFromFile_EmptyTools_ThrowsArgumentException()
    {
        // Arrange
        var tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, """
            tools:
            """);

        try
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => VersionMarkConfig.ReadFromFile(tempFile));
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    /// <summary>
    ///     Test that reading a configuration with invalid YAML throws an ArgumentException.
    /// </summary>
    [Fact]
    public void Configuration_ReadFromFile_InvalidYaml_ThrowsArgumentException()
    {
        // Arrange
        var tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, "invalid: yaml: content: [[[");

        try
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => VersionMarkConfig.ReadFromFile(tempFile));
        }
        finally
        {
            File.Delete(tempFile);
        }
    }
}
