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
[TestClass]
public class ConfigurationSubsystemTests
{
    /// <summary>
    ///     Test that reading a multi-tool configuration file produces all tools with usable commands and regexes.
    /// </summary>
    [TestMethod]
    public void ConfigurationSubsystem_ReadFromFile_MultipleTools_AllToolsAccessible()
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
            Assert.IsNotNull(config);
            Assert.HasCount(2, config.Tools);
            Assert.IsTrue(config.Tools.ContainsKey("dotnet"), "dotnet tool should be present");
            Assert.IsTrue(config.Tools.ContainsKey("git"), "git tool should be present");
            Assert.IsFalse(string.IsNullOrEmpty(config.Tools["dotnet"].GetEffectiveCommand()),
                "dotnet command should be accessible");
            Assert.IsFalse(string.IsNullOrEmpty(config.Tools["git"].GetEffectiveRegex()),
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
    [TestMethod]
    public void ConfigurationSubsystem_ReadFromFile_WithOsOverrides_SelectsAppropriateCommand()
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

            // Act - Read the config and get the effective command for the current OS
            var config = VersionMarkConfig.ReadFromFile(tempFile);
            var effectiveCommand = config.Tools["dotnet"].GetEffectiveCommand();

            // Assert - The effective command should match the OS-specific override for the current platform
            if (OperatingSystem.IsWindows())
            {
                Assert.AreEqual("dotnet.exe --version", effectiveCommand,
                    "On Windows the Windows override should be selected");
            }
            else if (OperatingSystem.IsLinux())
            {
                Assert.AreEqual("dotnet-linux --version", effectiveCommand,
                    "On Linux the Linux override should be selected");
            }
            else
            {
                Assert.AreEqual("dotnet --version", effectiveCommand,
                    "On other platforms the default command should be selected");
            }
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    /// <summary>
    ///     Test that reading a configuration with OS-specific regex overrides returns the appropriate regex.
    /// </summary>
    [TestMethod]
    public void ConfigurationSubsystem_ReadFromFile_OsRegexOverride_SelectsAppropriateRegex()
    {
        // Arrange
        var tempFile = Path.GetTempFileName() + ".yaml";
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
            var effectiveRegex = tool.GetEffectiveRegex();

            // Assert - The effective regex should match the OS-specific override for the current platform
            if (OperatingSystem.IsWindows())
            {
                Assert.AreEqual(@"(?<version>\d+\.\d+\.\d+)-win", effectiveRegex,
                    "On Windows the Windows regex override should be selected");
            }
            else if (OperatingSystem.IsLinux())
            {
                Assert.AreEqual(@"(?<version>\d+\.\d+\.\d+)-linux", effectiveRegex,
                    "On Linux the Linux regex override should be selected");
            }
            else
            {
                Assert.AreEqual(@"(?<version>\d+\.\d+\.\d+)", effectiveRegex,
                    "On other platforms the default regex should be selected");
            }
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    /// <summary>
    ///     Test that reading a configuration with an empty tools section throws an ArgumentException.
    /// </summary>
    [TestMethod]
    public void ConfigurationSubsystem_ReadFromFile_EmptyTools_ThrowsArgumentException()
    {
        // Arrange
        var tempFile = Path.GetTempFileName() + ".yaml";
        File.WriteAllText(tempFile, """
            tools:
            """);

        try
        {
            // Act & Assert
            Assert.ThrowsExactly<ArgumentException>(() => VersionMarkConfig.ReadFromFile(tempFile));
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    /// <summary>
    ///     Test that reading a configuration with invalid YAML throws an ArgumentException.
    /// </summary>
    [TestMethod]
    public void ConfigurationSubsystem_ReadFromFile_InvalidYaml_ThrowsArgumentException()
    {
        // Arrange
        var tempFile = Path.GetTempFileName() + ".yaml";
        File.WriteAllText(tempFile, "invalid: yaml: content: [[[");

        try
        {
            // Act & Assert
            Assert.ThrowsExactly<ArgumentException>(() => VersionMarkConfig.ReadFromFile(tempFile));
        }
        finally
        {
            File.Delete(tempFile);
        }
    }
}
