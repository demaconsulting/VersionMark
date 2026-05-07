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
using DemaConsulting.VersionMark.Capture;
using DemaConsulting.VersionMark.Configuration;

namespace DemaConsulting.VersionMark.Tests.Configuration;

/// <summary>
///     Unit tests for the VersionMarkConfig class.
/// </summary>
public partial class VersionMarkConfigTests
{
    private static readonly string[] s_dotnetToolArray = ["dotnet"];
    private static readonly string[] s_dotnetGitToolArray = ["dotnet", "git"];
    private static readonly string[] s_nonexistentToolArray = ["nonexistent"];
    private static readonly string[] s_invalidToolArray = ["invalid"];

    [GeneratedRegex(@"\d+\.\d+\.\d+")]
    private static partial Regex VersionRegex();

    /// <summary>
    ///     Test internal constructor creates config with tools.
    /// </summary>
    [Fact]
    public void VersionMarkConfig_InternalConstructor_CreatesConfig()
    {
        // Arrange
        var tools = new Dictionary<string, ToolConfig>
        {
            ["dotnet"] = new ToolConfig(
                new Dictionary<string, string> { [string.Empty] = "dotnet --version" },
                new Dictionary<string, string> { [string.Empty] = @"(?<version>\d+\.\d+\.\d+)" }
            )
        };

        // Act
        var config = new VersionMarkConfig(tools);

        // Assert
        Assert.NotNull(config);
        Assert.Single(config.Tools);
        Assert.True(config.Tools.ContainsKey("dotnet"));
    }

    /// <summary>
    ///     Test reading a valid YAML configuration file.
    /// </summary>
    [Fact]
    public void VersionMarkConfig_ReadFromFile_ValidFile_ReturnsConfig()
    {
        // Arrange
        var tempFile = Path.GetTempFileName();
        try
        {
            var yaml = @"tools:
  tool1:
    command: tool1 --version
    regex: 'Tool1\s+(?<version>[\d\.]+)'
  tool2:
    command: tool2 version --client
    command-win: tool2.cmd version --client
    regex: 'Tool2:""v(?<version>[\d\.]+)""'
    regex-linux: 'Tool2 Version: v(?<version>[\d\.]+)'
";
            File.WriteAllText(tempFile, yaml);

            // Act
            var config = VersionMarkConfig.ReadFromFile(tempFile);

            // Assert
            Assert.NotNull(config);
            Assert.Equal(2, config.Tools.Count);
            Assert.True(config.Tools.TryGetValue("tool1", out var tool1));
            Assert.True(config.Tools.TryGetValue("tool2", out var tool2));

            // Check tool1
            Assert.Equal("tool1 --version", tool1.Command[string.Empty]);
            Assert.Equal(@"Tool1\s+(?<version>[\d\.]+)", tool1.Regex[string.Empty]);

            // Check tool2
            Assert.Equal("tool2 version --client", tool2.Command[string.Empty]);
            Assert.Equal("tool2.cmd version --client", tool2.Command["win"]);
            Assert.Equal(@"Tool2:""v(?<version>[\d\.]+)""", tool2.Regex[string.Empty]);
            Assert.Equal(@"Tool2 Version: v(?<version>[\d\.]+)", tool2.Regex["linux"]);
        }
        finally
        {
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
    }

    /// <summary>
    ///     Test reading configuration with all OS overrides.
    /// </summary>
    [Fact]
    public void VersionMarkConfig_ReadFromFile_WithAllOsOverrides_ReturnsConfig()
    {
        // Arrange
        var tempFile = Path.GetTempFileName();
        try
        {
            var yaml = @"tools:
  gcc:
    command: gcc --version
    command-win: gcc.exe --version
    command-linux: gcc-13 --version
    command-macos: gcc-14 --version
    regex: 'gcc.*?(?<version>\d+\.\d+\.\d+)'
    regex-win: 'gcc\.exe.*?(?<version>\d+\.\d+\.\d+)'
    regex-linux: 'gcc-13.*?(?<version>\d+\.\d+\.\d+)'
    regex-macos: 'gcc-14.*?(?<version>\d+\.\d+\.\d+)'
";
            File.WriteAllText(tempFile, yaml);

            // Act
            var config = VersionMarkConfig.ReadFromFile(tempFile);

            // Assert
            Assert.NotNull(config);
            Assert.Single(config.Tools);
            Assert.True(config.Tools.TryGetValue("gcc", out var gcc));

            Assert.Equal("gcc --version", gcc.Command[string.Empty]);
            Assert.Equal("gcc.exe --version", gcc.Command["win"]);
            Assert.Equal("gcc-13 --version", gcc.Command["linux"]);
            Assert.Equal("gcc-14 --version", gcc.Command["macos"]);
            Assert.Equal(@"gcc.*?(?<version>\d+\.\d+\.\d+)", gcc.Regex[string.Empty]);
            Assert.Equal(@"gcc\.exe.*?(?<version>\d+\.\d+\.\d+)", gcc.Regex["win"]);
            Assert.Equal(@"gcc-13.*?(?<version>\d+\.\d+\.\d+)", gcc.Regex["linux"]);
            Assert.Equal(@"gcc-14.*?(?<version>\d+\.\d+\.\d+)", gcc.Regex["macos"]);
        }
        finally
        {
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
    }

    /// <summary>
    ///     Test reading from non-existent file throws ArgumentException.
    /// </summary>
    [Fact]
    public void VersionMarkConfig_ReadFromFile_NonExistentFile_ThrowsArgumentException()
    {
        // Arrange
        var nonExistentFile = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.yaml");

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            VersionMarkConfig.ReadFromFile(nonExistentFile));

        Assert.Contains("Configuration file not found", ex.Message);
    }

    /// <summary>
    ///     Test reading invalid YAML throws ArgumentException.
    /// </summary>
    [Fact]
    public void VersionMarkConfig_ReadFromFile_InvalidYaml_ThrowsArgumentException()
    {
        // Arrange
        var tempFile = Path.GetTempFileName();
        try
        {
            File.WriteAllText(tempFile, "invalid: yaml: content: [[[");

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() =>
                VersionMarkConfig.ReadFromFile(tempFile));

            Assert.Contains("Failed to parse YAML file", ex.Message);
        }
        finally
        {
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
    }

    /// <summary>
    ///     Test reading YAML with no tools throws ArgumentException.
    /// </summary>
    [Fact]
    public void VersionMarkConfig_ReadFromFile_NoTools_ThrowsArgumentException()
    {
        // Arrange
        var tempFile = Path.GetTempFileName();
        try
        {
            File.WriteAllText(tempFile, "tools: {}");

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() =>
                VersionMarkConfig.ReadFromFile(tempFile));

            Assert.Contains("must contain at least one tool", ex.Message);
        }
        finally
        {
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
    }

    /// <summary>
    ///     Test GetEffectiveCommand returns default command when no OS override exists.
    /// </summary>
    [Fact]
    public void ToolConfig_GetEffectiveCommand_NoOverride_ReturnsDefaultCommand()
    {
        // Arrange
        var tool = new ToolConfig(
            new Dictionary<string, string> { [string.Empty] = "tool --version" },
            new Dictionary<string, string> { [string.Empty] = @"(?<version>\d+\.\d+\.\d+)" }
        );

        // Act
        var command = tool.GetEffectiveCommand();

        // Assert
        Assert.Equal("tool --version", command);
    }

    /// <summary>
    ///     Test GetEffectiveRegex returns default regex when no OS override exists.
    /// </summary>
    [Fact]
    public void ToolConfig_GetEffectiveRegex_NoOverride_ReturnsDefaultRegex()
    {
        // Arrange
        var tool = new ToolConfig(
            new Dictionary<string, string> { [string.Empty] = "tool --version" },
            new Dictionary<string, string> { [string.Empty] = @"(?<version>\d+\.\d+\.\d+)" }
        );

        // Act
        var regex = tool.GetEffectiveRegex();

        // Assert
        Assert.Equal(@"(?<version>\d+\.\d+\.\d+)", regex);
    }

    /// <summary>
    ///     Test GetEffectiveCommand with explicit OS parameter.
    /// </summary>
    [Fact]
    public void ToolConfig_GetEffectiveCommand_WithExplicitOs_ReturnsCorrectCommand()
    {
        // Arrange
        var tool = new ToolConfig(
            new Dictionary<string, string>
            {
                [string.Empty] = "tool --version",
                ["win"] = "tool.exe --version",
                ["linux"] = "tool-linux --version",
                ["macos"] = "tool-macos --version"
            },
            new Dictionary<string, string> { [string.Empty] = @"(?<version>\d+\.\d+\.\d+)" }
        );

        // Act & Assert
        Assert.Equal("tool.exe --version", tool.GetEffectiveCommand("win"));
        Assert.Equal("tool-linux --version", tool.GetEffectiveCommand("linux"));
        Assert.Equal("tool-macos --version", tool.GetEffectiveCommand("macos"));
        Assert.Equal("tool --version", tool.GetEffectiveCommand("unknown"));
    }

    /// <summary>
    ///     Test GetEffectiveRegex with explicit OS parameter.
    /// </summary>
    [Fact]
    public void ToolConfig_GetEffectiveRegex_WithExplicitOs_ReturnsCorrectRegex()
    {
        // Arrange
        var tool = new ToolConfig(
            new Dictionary<string, string> { [string.Empty] = "tool --version" },
            new Dictionary<string, string>
            {
                [string.Empty] = @"(?<version>\d+\.\d+\.\d+)",
                ["win"] = @"Windows: (?<version>\d+\.\d+\.\d+)",
                ["linux"] = @"Linux: (?<version>\d+\.\d+\.\d+)",
                ["macos"] = @"macOS: (?<version>\d+\.\d+\.\d+)"
            }
        );

        // Act & Assert
        Assert.Equal(@"Windows: (?<version>\d+\.\d+\.\d+)", tool.GetEffectiveRegex("win"));
        Assert.Equal(@"Linux: (?<version>\d+\.\d+\.\d+)", tool.GetEffectiveRegex("linux"));
        Assert.Equal(@"macOS: (?<version>\d+\.\d+\.\d+)", tool.GetEffectiveRegex("macos"));
        Assert.Equal(@"(?<version>\d+\.\d+\.\d+)", tool.GetEffectiveRegex("unknown"));
    }

    /// <summary>
    ///     Test GetEffectiveCommand on Windows returns Windows override when available.
    /// </summary>
    [Fact]
    public void ToolConfig_GetEffectiveCommand_WindowsOverride_ReturnsWindowsCommand()
    {
        // Arrange
        var tool = new ToolConfig(
            new Dictionary<string, string>
            {
                [string.Empty] = "tool --version",
                ["win"] = "tool.exe --version"
            },
            new Dictionary<string, string> { [string.Empty] = @"(?<version>\d+\.\d+\.\d+)" }
        );

        // Act
        var command = tool.GetEffectiveCommand();

        // Assert
        // On Windows, should return Windows override; otherwise default
        if (OperatingSystem.IsWindows())
        {
            Assert.Equal("tool.exe --version", command);
        }
        else
        {
            Assert.Equal("tool --version", command);
        }
    }

    /// <summary>
    ///     Test GetEffectiveCommand on Linux returns Linux override when available.
    /// </summary>
    [Fact]
    public void ToolConfig_GetEffectiveCommand_LinuxOverride_ReturnsLinuxCommand()
    {
        // Arrange
        var tool = new ToolConfig(
            new Dictionary<string, string>
            {
                [string.Empty] = "tool --version",
                ["linux"] = "tool-linux --version"
            },
            new Dictionary<string, string> { [string.Empty] = @"(?<version>\d+\.\d+\.\d+)" }
        );

        // Act
        var command = tool.GetEffectiveCommand();

        // Assert
        // On Linux, should return Linux override; otherwise default
        if (OperatingSystem.IsLinux())
        {
            Assert.Equal("tool-linux --version", command);
        }
        else
        {
            Assert.Equal("tool --version", command);
        }
    }

    /// <summary>
    ///     Test GetEffectiveCommand on macOS returns macOS override when available.
    /// </summary>
    [Fact]
    public void ToolConfig_GetEffectiveCommand_MacOsOverride_ReturnsMacOsCommand()
    {
        // Arrange
        var tool = new ToolConfig(
            new Dictionary<string, string>
            {
                [string.Empty] = "tool --version",
                ["macos"] = "tool-macos --version"
            },
            new Dictionary<string, string> { [string.Empty] = @"(?<version>\d+\.\d+\.\d+)" }
        );

        // Act
        var command = tool.GetEffectiveCommand();

        // Assert
        // On macOS, should return macOS override; otherwise default
        if (OperatingSystem.IsMacOS())
        {
            Assert.Equal("tool-macos --version", command);
        }
        else
        {
            Assert.Equal("tool --version", command);
        }
    }

    /// <summary>
    ///     Test GetEffectiveRegex on Windows returns Windows override when available.
    /// </summary>
    [Fact]
    public void ToolConfig_GetEffectiveRegex_WindowsOverride_ReturnsWindowsRegex()
    {
        // Arrange
        var tool = new ToolConfig(
            new Dictionary<string, string> { [string.Empty] = "tool --version" },
            new Dictionary<string, string>
            {
                [string.Empty] = @"(?<version>\d+\.\d+\.\d+)",
                ["win"] = @"Windows: (?<version>\d+\.\d+\.\d+)"
            }
        );

        // Act
        var regex = tool.GetEffectiveRegex();

        // Assert
        // On Windows, should return Windows override; otherwise default
        if (OperatingSystem.IsWindows())
        {
            Assert.Equal(@"Windows: (?<version>\d+\.\d+\.\d+)", regex);
        }
        else
        {
            Assert.Equal(@"(?<version>\d+\.\d+\.\d+)", regex);
        }
    }

    /// <summary>
    ///     Test GetEffectiveRegex on Linux returns Linux override when available.
    /// </summary>
    [Fact]
    public void ToolConfig_GetEffectiveRegex_LinuxOverride_ReturnsLinuxRegex()
    {
        // Arrange
        var tool = new ToolConfig(
            new Dictionary<string, string> { [string.Empty] = "tool --version" },
            new Dictionary<string, string>
            {
                [string.Empty] = @"(?<version>\d+\.\d+\.\d+)",
                ["linux"] = @"Linux: (?<version>\d+\.\d+\.\d+)"
            }
        );

        // Act
        var regex = tool.GetEffectiveRegex();

        // Assert
        // On Linux, should return Linux override; otherwise default
        if (OperatingSystem.IsLinux())
        {
            Assert.Equal(@"Linux: (?<version>\d+\.\d+\.\d+)", regex);
        }
        else
        {
            Assert.Equal(@"(?<version>\d+\.\d+\.\d+)", regex);
        }
    }

    /// <summary>
    ///     Test GetEffectiveRegex on macOS returns macOS override when available.
    /// </summary>
    [Fact]
    public void ToolConfig_GetEffectiveRegex_MacOsOverride_ReturnsMacOsRegex()
    {
        // Arrange
        var tool = new ToolConfig(
            new Dictionary<string, string> { [string.Empty] = "tool --version" },
            new Dictionary<string, string>
            {
                [string.Empty] = @"(?<version>\d+\.\d+\.\d+)",
                ["macos"] = @"macOS: (?<version>\d+\.\d+\.\d+)"
            }
        );

        // Act
        var regex = tool.GetEffectiveRegex();

        // Assert
        // On macOS, should return macOS override; otherwise default
        if (OperatingSystem.IsMacOS())
        {
            Assert.Equal(@"macOS: (?<version>\d+\.\d+\.\d+)", regex);
        }
        else
        {
            Assert.Equal(@"(?<version>\d+\.\d+\.\d+)", regex);
        }
    }

    /// <summary>
    ///     Test GetEffectiveCommand throws InvalidOperationException when no default key is present.
    /// </summary>
    [Fact]
    public void ToolConfig_GetEffectiveCommand_NoDefaultKey_ThrowsInvalidOperationException()
    {
        // Arrange - a ToolConfig with only an OS-specific command and no default key
        var tool = new ToolConfig(
            new Dictionary<string, string> { ["win"] = "tool.exe --version" },
            new Dictionary<string, string> { [string.Empty] = @"(?<version>\d+\.\d+\.\d+)" }
        );

        // Act & Assert - requesting an OS with no matching key and no default should throw
        Assert.Throws<InvalidOperationException>(() => tool.GetEffectiveCommand("linux"));
    }

    /// <summary>
    ///     Test GetEffectiveRegex throws InvalidOperationException when no default key is present.
    /// </summary>
    [Fact]
    public void ToolConfig_GetEffectiveRegex_NoDefaultKey_ThrowsInvalidOperationException()
    {
        // Arrange - a ToolConfig with only an OS-specific regex and no default key
        var tool = new ToolConfig(
            new Dictionary<string, string> { [string.Empty] = "tool --version" },
            new Dictionary<string, string> { ["win"] = @"Windows: (?<version>\d+\.\d+\.\d+)" }
        );

        // Act & Assert - requesting an OS with no matching key and no default should throw
        Assert.Throws<InvalidOperationException>(() => tool.GetEffectiveRegex("linux"));
    }

    /// <summary>
    ///     Test FindVersions with dotnet command.
    /// </summary>
    [Fact]
    public void VersionMarkConfig_FindVersions_DotnetCommand_ReturnsVersionInfo()
    {
        // Arrange
        var tools = new Dictionary<string, ToolConfig>
        {
            ["dotnet"] = new ToolConfig(
                new Dictionary<string, string> { [string.Empty] = "dotnet --version" },
                new Dictionary<string, string> { [string.Empty] = @"(?<version>\d+\.\d+\.\d+)" }
            )
        };
        var config = new VersionMarkConfig(tools);

        // Act
        var versionInfo = config.FindVersions(s_dotnetToolArray, "test-job");

        // Assert
        Assert.NotNull(versionInfo);
        Assert.Equal("test-job", versionInfo.JobId);
        Assert.Single(versionInfo.Versions);
        Assert.True(versionInfo.Versions.TryGetValue("dotnet", out var dotnetVersion));
        Assert.Matches(VersionRegex(), dotnetVersion);
    }

    /// <summary>
    ///     Test FindVersions with multiple tools.
    /// </summary>
    [Fact]
    public void VersionMarkConfig_FindVersions_MultipleTools_ReturnsAllVersions()
    {
        // Arrange
        var tools = new Dictionary<string, ToolConfig>
        {
            ["dotnet"] = new ToolConfig(
                new Dictionary<string, string> { [string.Empty] = "dotnet --version" },
                new Dictionary<string, string> { [string.Empty] = @"(?<version>\d+\.\d+\.\d+)" }
            ),
            ["git"] = new ToolConfig(
                new Dictionary<string, string> { [string.Empty] = "git --version" },
                new Dictionary<string, string> { [string.Empty] = @"git version (?<version>\d+\.\d+\.\d+)" }
            )
        };
        var config = new VersionMarkConfig(tools);

        // Act
        var versionInfo = config.FindVersions(s_dotnetGitToolArray, "test-job");

        // Assert
        Assert.NotNull(versionInfo);
        Assert.Equal("test-job", versionInfo.JobId);
        Assert.Equal(2, versionInfo.Versions.Count);
        Assert.True(versionInfo.Versions.TryGetValue("dotnet", out var dotnetVersion));
        Assert.True(versionInfo.Versions.TryGetValue("git", out var gitVersion));
        Assert.Matches(VersionRegex(), dotnetVersion);
        Assert.Matches(VersionRegex(), gitVersion);
    }

    /// <summary>
    ///     Test FindVersions with non-existent tool throws ArgumentException.
    /// </summary>
    [Fact]
    public void VersionMarkConfig_FindVersions_NonExistentTool_ThrowsArgumentException()
    {
        // Arrange
        var tools = new Dictionary<string, ToolConfig>
        {
            ["dotnet"] = new ToolConfig(
                new Dictionary<string, string> { [string.Empty] = "dotnet --version" },
                new Dictionary<string, string> { [string.Empty] = @"(?<version>\d+\.\d+\.\d+)" }
            )
        };
        var config = new VersionMarkConfig(tools);

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            config.FindVersions(s_nonexistentToolArray, "test-job"));

        Assert.Contains("Tool 'nonexistent' not found in configuration", ex.Message);
    }

    /// <summary>
    ///     Test FindVersions with invalid command throws InvalidOperationException.
    /// </summary>
    [Fact]
    public void VersionMarkConfig_FindVersions_InvalidCommand_ThrowsInvalidOperationException()
    {
        // Arrange
        var tools = new Dictionary<string, ToolConfig>
        {
            ["invalid"] = new ToolConfig(
                new Dictionary<string, string> { [string.Empty] = "nonexistent-command-xyz" },
                new Dictionary<string, string> { [string.Empty] = @"(?<version>\d+\.\d+\.\d+)" }
            )
        };
        var config = new VersionMarkConfig(tools);

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() =>
            config.FindVersions(s_invalidToolArray, "test-job"));

        Assert.Contains("Failed to run command", ex.Message);
    }

    /// <summary>
    ///     Test FindVersions with regex that doesn't match throws InvalidOperationException.
    /// </summary>
    [Fact]
    public void VersionMarkConfig_FindVersions_RegexNoMatch_ThrowsInvalidOperationException()
    {
        // Arrange
        var tools = new Dictionary<string, ToolConfig>
        {
            ["dotnet"] = new ToolConfig(
                new Dictionary<string, string> { [string.Empty] = "dotnet --version" },
                new Dictionary<string, string> { [string.Empty] = @"(?<version>NOMATCH\d+)" }
            )
        };
        var config = new VersionMarkConfig(tools);

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() =>
            config.FindVersions(s_dotnetToolArray, "test-job"));

        Assert.Contains("Failed to extract version for tool 'dotnet'", ex.Message);
    }

    /// <summary>
    ///     Test FindVersions with regex without version group throws InvalidOperationException.
    /// </summary>
    [Fact]
    public void VersionMarkConfig_FindVersions_RegexNoVersionGroup_ThrowsInvalidOperationException()
    {
        // Arrange
        var tools = new Dictionary<string, ToolConfig>
        {
            ["dotnet"] = new ToolConfig(
                new Dictionary<string, string> { [string.Empty] = "dotnet --version" },
                new Dictionary<string, string> { [string.Empty] = @"(\d+\.\d+\.\d+)" }
            )
        };
        var config = new VersionMarkConfig(tools);

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() =>
            config.FindVersions(s_dotnetToolArray, "test-job"));

        Assert.Contains("must contain a named 'version' capture group", ex.Message);
    }
}
