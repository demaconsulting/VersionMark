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

namespace DemaConsulting.TemplateDotNetTool.Tests;

/// <summary>
///     Unit tests for the VersionMarkConfig class.
/// </summary>
[TestClass]
public class VersionMarkConfigTests
{
    /// <summary>
    ///     Test internal constructor creates config with tools.
    /// </summary>
    [TestMethod]
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
        Assert.IsNotNull(config);
        Assert.AreEqual(1, config.Tools.Count);
        Assert.IsTrue(config.Tools.ContainsKey("dotnet"));
    }

    /// <summary>
    ///     Test reading a valid YAML configuration file.
    /// </summary>
    [TestMethod]
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
            Assert.IsNotNull(config);
            Assert.AreEqual(2, config.Tools.Count);
            Assert.IsTrue(config.Tools.ContainsKey("tool1"));
            Assert.IsTrue(config.Tools.ContainsKey("tool2"));

            // Check tool1
            Assert.AreEqual("tool1 --version", config.Tools["tool1"].Command[string.Empty]);
            Assert.AreEqual(@"Tool1\s+(?<version>[\d\.]+)", config.Tools["tool1"].Regex[string.Empty]);

            // Check tool2
            Assert.AreEqual("tool2 version --client", config.Tools["tool2"].Command[string.Empty]);
            Assert.AreEqual("tool2.cmd version --client", config.Tools["tool2"].Command["win"]);
            Assert.AreEqual(@"Tool2:""v(?<version>[\d\.]+)""", config.Tools["tool2"].Regex[string.Empty]);
            Assert.AreEqual(@"Tool2 Version: v(?<version>[\d\.]+)", config.Tools["tool2"].Regex["linux"]);
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
    [TestMethod]
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
            Assert.IsNotNull(config);
            Assert.AreEqual(1, config.Tools.Count);
            Assert.IsTrue(config.Tools.ContainsKey("gcc"));

            var gcc = config.Tools["gcc"];
            Assert.AreEqual("gcc --version", gcc.Command[string.Empty]);
            Assert.AreEqual("gcc.exe --version", gcc.Command["win"]);
            Assert.AreEqual("gcc-13 --version", gcc.Command["linux"]);
            Assert.AreEqual("gcc-14 --version", gcc.Command["macos"]);
            Assert.AreEqual(@"gcc.*?(?<version>\d+\.\d+\.\d+)", gcc.Regex[string.Empty]);
            Assert.AreEqual(@"gcc\.exe.*?(?<version>\d+\.\d+\.\d+)", gcc.Regex["win"]);
            Assert.AreEqual(@"gcc-13.*?(?<version>\d+\.\d+\.\d+)", gcc.Regex["linux"]);
            Assert.AreEqual(@"gcc-14.*?(?<version>\d+\.\d+\.\d+)", gcc.Regex["macos"]);
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
    [TestMethod]
    public void VersionMarkConfig_ReadFromFile_NonExistentFile_ThrowsArgumentException()
    {
        // Arrange
        var nonExistentFile = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.yaml");

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            VersionMarkConfig.ReadFromFile(nonExistentFile));

        Assert.IsTrue(ex.Message.Contains("Configuration file not found"));
    }

    /// <summary>
    ///     Test reading invalid YAML throws ArgumentException.
    /// </summary>
    [TestMethod]
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

            Assert.IsTrue(ex.Message.Contains("Failed to parse YAML file"));
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
    [TestMethod]
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

            Assert.IsTrue(ex.Message.Contains("must contain at least one tool"));
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
    [TestMethod]
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
        Assert.AreEqual("tool --version", command);
    }

    /// <summary>
    ///     Test GetEffectiveRegex returns default regex when no OS override exists.
    /// </summary>
    [TestMethod]
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
        Assert.AreEqual(@"(?<version>\d+\.\d+\.\d+)", regex);
    }

    /// <summary>
    ///     Test GetEffectiveCommand with explicit OS parameter.
    /// </summary>
    [TestMethod]
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
        Assert.AreEqual("tool.exe --version", tool.GetEffectiveCommand("win"));
        Assert.AreEqual("tool-linux --version", tool.GetEffectiveCommand("linux"));
        Assert.AreEqual("tool-macos --version", tool.GetEffectiveCommand("macos"));
        Assert.AreEqual("tool --version", tool.GetEffectiveCommand("unknown"));
    }

    /// <summary>
    ///     Test GetEffectiveRegex with explicit OS parameter.
    /// </summary>
    [TestMethod]
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
        Assert.AreEqual(@"Windows: (?<version>\d+\.\d+\.\d+)", tool.GetEffectiveRegex("win"));
        Assert.AreEqual(@"Linux: (?<version>\d+\.\d+\.\d+)", tool.GetEffectiveRegex("linux"));
        Assert.AreEqual(@"macOS: (?<version>\d+\.\d+\.\d+)", tool.GetEffectiveRegex("macos"));
        Assert.AreEqual(@"(?<version>\d+\.\d+\.\d+)", tool.GetEffectiveRegex("unknown"));
    }

    /// <summary>
    ///     Test GetEffectiveCommand on Windows returns Windows override when available.
    /// </summary>
    [TestMethod]
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
            Assert.AreEqual("tool.exe --version", command);
        }
        else
        {
            Assert.AreEqual("tool --version", command);
        }
    }

    /// <summary>
    ///     Test GetEffectiveCommand on Linux returns Linux override when available.
    /// </summary>
    [TestMethod]
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
            Assert.AreEqual("tool-linux --version", command);
        }
        else
        {
            Assert.AreEqual("tool --version", command);
        }
    }

    /// <summary>
    ///     Test GetEffectiveCommand on macOS returns macOS override when available.
    /// </summary>
    [TestMethod]
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
            Assert.AreEqual("tool-macos --version", command);
        }
        else
        {
            Assert.AreEqual("tool --version", command);
        }
    }

    /// <summary>
    ///     Test GetEffectiveRegex on Windows returns Windows override when available.
    /// </summary>
    [TestMethod]
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
            Assert.AreEqual(@"Windows: (?<version>\d+\.\d+\.\d+)", regex);
        }
        else
        {
            Assert.AreEqual(@"(?<version>\d+\.\d+\.\d+)", regex);
        }
    }

    /// <summary>
    ///     Test GetEffectiveRegex on Linux returns Linux override when available.
    /// </summary>
    [TestMethod]
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
            Assert.AreEqual(@"Linux: (?<version>\d+\.\d+\.\d+)", regex);
        }
        else
        {
            Assert.AreEqual(@"(?<version>\d+\.\d+\.\d+)", regex);
        }
    }

    /// <summary>
    ///     Test GetEffectiveRegex on macOS returns macOS override when available.
    /// </summary>
    [TestMethod]
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
            Assert.AreEqual(@"macOS: (?<version>\d+\.\d+\.\d+)", regex);
        }
        else
        {
            Assert.AreEqual(@"(?<version>\d+\.\d+\.\d+)", regex);
        }
    }

    /// <summary>
    ///     Test VersionInfo record creation.
    /// </summary>
    [TestMethod]
    public void VersionInfo_Constructor_CreatesRecord()
    {
        // Arrange
        var jobId = "job-123";
        var versions = new Dictionary<string, string>
        {
            ["dotnet"] = "8.0.100",
            ["git"] = "2.43.0"
        };

        // Act
        var versionInfo = new VersionInfo(jobId, versions);

        // Assert
        Assert.IsNotNull(versionInfo);
        Assert.AreEqual("job-123", versionInfo.JobId);
        Assert.AreEqual(2, versionInfo.Versions.Count);
        Assert.AreEqual("8.0.100", versionInfo.Versions["dotnet"]);
        Assert.AreEqual("2.43.0", versionInfo.Versions["git"]);
    }

    /// <summary>
    ///     Test FindVersions with dotnet command.
    /// </summary>
    [TestMethod]
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
        var versionInfo = config.FindVersions(new[] { "dotnet" }, "test-job");

        // Assert
        Assert.IsNotNull(versionInfo);
        Assert.AreEqual("test-job", versionInfo.JobId);
        Assert.AreEqual(1, versionInfo.Versions.Count);
        Assert.IsTrue(versionInfo.Versions.ContainsKey("dotnet"));
        Assert.IsTrue(Regex.IsMatch(versionInfo.Versions["dotnet"], @"\d+\.\d+\.\d+"));
    }

    /// <summary>
    ///     Test FindVersions with multiple tools.
    /// </summary>
    [TestMethod]
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
        var versionInfo = config.FindVersions(new[] { "dotnet", "git" }, "test-job");

        // Assert
        Assert.IsNotNull(versionInfo);
        Assert.AreEqual("test-job", versionInfo.JobId);
        Assert.AreEqual(2, versionInfo.Versions.Count);
        Assert.IsTrue(versionInfo.Versions.ContainsKey("dotnet"));
        Assert.IsTrue(versionInfo.Versions.ContainsKey("git"));
        Assert.IsTrue(Regex.IsMatch(versionInfo.Versions["dotnet"], @"\d+\.\d+\.\d+"));
        Assert.IsTrue(Regex.IsMatch(versionInfo.Versions["git"], @"\d+\.\d+\.\d+"));
    }

    /// <summary>
    ///     Test FindVersions with non-existent tool throws ArgumentException.
    /// </summary>
    [TestMethod]
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
            config.FindVersions(new[] { "nonexistent" }, "test-job"));

        Assert.IsTrue(ex.Message.Contains("Tool 'nonexistent' not found in configuration"));
    }

    /// <summary>
    ///     Test FindVersions with invalid command throws InvalidOperationException.
    /// </summary>
    [TestMethod]
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
            config.FindVersions(new[] { "invalid" }, "test-job"));

        Assert.IsTrue(ex.Message.Contains("Failed to run command"));
    }

    /// <summary>
    ///     Test FindVersions with regex that doesn't match throws InvalidOperationException.
    /// </summary>
    [TestMethod]
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
            config.FindVersions(new[] { "dotnet" }, "test-job"));

        Assert.IsTrue(ex.Message.Contains("Failed to extract version for tool 'dotnet'"));
    }

    /// <summary>
    ///     Test FindVersions with regex without version group throws InvalidOperationException.
    /// </summary>
    [TestMethod]
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
            config.FindVersions(new[] { "dotnet" }, "test-job"));

        Assert.IsTrue(ex.Message.Contains("must contain a named 'version' capture group"));
    }
}
