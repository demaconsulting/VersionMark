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

using DemaConsulting.VersionMark.Cli;
using DemaConsulting.VersionMark.Configuration;

namespace DemaConsulting.VersionMark.Tests.Configuration;

/// <summary>
///     Subsystem tests for the Configuration subsystem (VersionMarkConfig and ToolConfig working together).
/// </summary>
[TestClass]
public class ConfigurationTests
{
    /// <summary>
    ///     Test that reading a multi-tool configuration file produces all tools with usable commands and regexes.
    /// </summary>
    [TestMethod]
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
    public void Configuration_ReadFromFile_OsRegexOverride_SelectsAppropriateRegex()
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
    public void Configuration_ReadFromFile_EmptyTools_ThrowsArgumentException()
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
    public void Configuration_ReadFromFile_InvalidYaml_ThrowsArgumentException()
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

    /// <summary>
    ///     Test that the full configuration load pipeline succeeds and exits cleanly for a valid configuration.
    /// </summary>
    [TestMethod]
    public void Configuration_Run_ValidConfig_SucceedsWithZeroExitCode()
    {
        // Arrange - Write a complete and valid configuration to a temp file
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

            // Act - Run the program with the --lint flag against the valid config
            using var context = Context.Create(["--silent", "--lint", tempFile]);
            Program.Run(context);

            // Assert - The program should report success with a clean exit code
            Assert.AreEqual(0, context.ExitCode,
                "Exit code should be zero after successful configuration load");
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    /// <summary>
    ///     Test that the configuration load pipeline reports all errors in a single pass for an invalid configuration.
    /// </summary>
    [TestMethod]
    public void Configuration_Run_MultipleErrors_ReportsAllErrorsInSinglePass()
    {
        // Arrange - Write a configuration with multiple errors to a temp file
        // tool1 is missing 'regex'; tool2 is missing 'command' and has a regex without a 'version' group
        var tempFile = Path.GetTempFileName();
        try
        {
            const string yaml = """
                ---
                tools:
                  tool1:
                    command: tool1 --version
                  tool2:
                    regex: 'no-version-group'
                """;
            File.WriteAllText(tempFile, yaml);

            var originalError = Console.Error;
            try
            {
                using var errorWriter = new StringWriter();
                Console.SetError(errorWriter);

                // Act - Run the program against a config with multiple errors
                using var context = Context.Create(["--lint", tempFile]);
                Program.Run(context);

                // Assert - The program should report failure and emit findings for both tools
                Assert.AreEqual(1, context.ExitCode,
                    "Exit code should be non-zero when configuration load finds errors");

                var errorOutput = errorWriter.ToString();
                StringAssert.Contains(errorOutput, "tool1",
                    "Error output should mention tool1 (missing regex)");
                StringAssert.Contains(errorOutput, "tool2",
                    "Error output should mention tool2 (missing command and invalid regex)");
            }
            finally
            {
                Console.SetError(originalError);
            }
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    /// <summary>
    ///     Test that the configuration load pipeline fails for invalid YAML content.
    /// </summary>
    [TestMethod]
    public void Configuration_Run_InvalidYaml_Fails()
    {
        // Arrange - Write syntactically broken YAML to a temp file
        var tempFile = Path.GetTempFileName() + ".yaml";
        File.WriteAllText(tempFile, "tools:\n  dotnet:\n    command: [unclosed bracket");

        try
        {
            // Act - Run the program against malformed YAML
            using var context = Context.Create(["--silent", "--lint", tempFile]);
            Program.Run(context);

            // Assert - The program should fail with a non-zero exit code
            Assert.AreEqual(1, context.ExitCode);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    /// <summary>
    ///     Test that the configuration load pipeline reports an error when the config file does not exist.
    /// </summary>
    [TestMethod]
    public void Configuration_Run_NonExistentFile_Fails()
    {
        // Arrange - Use a path that does not exist
        var nonExistentPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".yaml");

        // Act - Run the program against a missing file
        using var context = Context.Create(["--silent", "--lint", nonExistentPath]);
        Program.Run(context);

        // Assert - The program should fail with a non-zero exit code
        Assert.AreEqual(1, context.ExitCode);
    }

    /// <summary>
    ///     Test that the configuration load pipeline reports an error when a regex cannot be compiled.
    /// </summary>
    [TestMethod]
    public void Configuration_Run_InvalidRegex_ReportsError()
    {
        // Arrange - Write a config with a syntactically broken regex (unclosed group)
        var tempFile = Path.GetTempFileName();
        try
        {
            const string yaml = """
                ---
                tools:
                  dotnet:
                    command: dotnet --version
                    regex: '(?<version'
                """;
            File.WriteAllText(tempFile, yaml);

            // Act - Run the program against a tool with an invalid regex pattern
            using var context = Context.Create(["--silent", "--lint", tempFile]);
            Program.Run(context);

            // Assert - The program should fail because the regex cannot be compiled
            Assert.AreEqual(1, context.ExitCode,
                "Exit code should be non-zero when a regex value cannot be compiled");
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    /// <summary>
    ///     Test that the configuration load pipeline reports an error when a regex does not contain a named 'version' capture group.
    /// </summary>
    [TestMethod]
    public void Configuration_Run_RegexWithoutVersionGroup_ReportsError()
    {
        // Arrange - Write a config with a valid regex that lacks the required 'version' group
        var tempFile = Path.GetTempFileName();
        try
        {
            const string yaml = """
                ---
                tools:
                  dotnet:
                    command: dotnet --version
                    regex: '\d+\.\d+\.\d+'
                """;
            File.WriteAllText(tempFile, yaml);

            // Act - Run the program on a tool whose regex has no 'version' named capture group
            using var context = Context.Create(["--silent", "--lint", tempFile]);
            Program.Run(context);

            // Assert - The program should fail because the 'version' group is required
            Assert.AreEqual(1, context.ExitCode,
                "Exit code should be non-zero when a regex does not contain a named 'version' capture group");
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    /// <summary>
    ///     Test that the configuration load pipeline reports an error when an OS-specific command override is empty.
    /// </summary>
    [TestMethod]
    public void Configuration_Run_EmptyOsSpecificOverride_ReportsError()
    {
        // Arrange - Write a config with an empty command-win override
        var tempFile = Path.GetTempFileName();
        try
        {
            const string yaml = """
                ---
                tools:
                  dotnet:
                    command: dotnet --version
                    command-win: ''
                    regex: '(?<version>\d+\.\d+\.\d+)'
                """;
            File.WriteAllText(tempFile, yaml);

            // Act - Run the program on a tool with an empty OS-specific override
            using var context = Context.Create(["--silent", "--lint", tempFile]);
            Program.Run(context);

            // Assert - The program should fail because empty OS-specific overrides are not allowed
            Assert.AreEqual(1, context.ExitCode,
                "Exit code should be non-zero when an OS-specific command override is empty");
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    /// <summary>
    ///     Test that the configuration load pipeline treats unknown keys as non-fatal warnings and succeeds.
    /// </summary>
    [TestMethod]
    public void Configuration_Run_UnknownKey_IsWarningNotError()
    {
        // Arrange - Write a config with a valid tool plus an unknown key
        var tempFile = Path.GetTempFileName();
        try
        {
            const string yaml = """
                ---
                tools:
                  dotnet:
                    command: dotnet --version
                    regex: '(?<version>\d+\.\d+\.\d+)'
                    unknown-tool-key: should-not-fail
                """;
            File.WriteAllText(tempFile, yaml);

            // Act - Run the program on a config containing an unknown tool key
            using var context = Context.Create(["--silent", "--lint", tempFile]);
            Program.Run(context);

            // Assert - The program should succeed; unknown keys produce warnings, not errors
            Assert.AreEqual(0, context.ExitCode,
                "Exit code should be zero when only unknown keys are present (warnings are non-fatal)");
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    /// <summary>
    ///     Test that configuration load error messages include the filename and line/column location.
    /// </summary>
    [TestMethod]
    public void Configuration_Run_Error_IncludesFileAndLineInfo()
    {
        // Arrange - Write a config missing the required 'command' field and capture error output
        var tempFile = Path.GetTempFileName();
        try
        {
            const string yaml = """
                ---
                tools:
                  dotnet:
                    regex: '(?<version>\d+\.\d+\.\d+)'
                """;
            File.WriteAllText(tempFile, yaml);

            var originalError = Console.Error;
            try
            {
                using var errWriter = new StringWriter();
                Console.SetError(errWriter);

                // Act - Run the program on a config with a missing command field
                using var context = Context.Create(["--lint", tempFile]);
                Program.Run(context);

                // Assert - The error message should contain the filename and line/column info
                Assert.AreEqual(1, context.ExitCode);
                var errorOutput = errWriter.ToString();
                StringAssert.Contains(errorOutput, Path.GetFileName(tempFile),
                    "Error message should include the config filename");
                Assert.IsTrue(
                    errorOutput.Contains("(") && errorOutput.Contains(",") && errorOutput.Contains(")"),
                    "Error message should include line and column location information");
            }
            finally
            {
                Console.SetError(originalError);
            }
        }
        finally
        {
            File.Delete(tempFile);
        }
    }
}
