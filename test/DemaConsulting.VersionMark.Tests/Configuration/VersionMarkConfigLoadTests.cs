// Copyright (c) 2025 DEMA Consulting
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
///     Unit tests for the <see cref="VersionMarkConfig.Load"/> method.
/// </summary>
[TestClass]
public class VersionMarkConfigLoadTests
{
    /// <summary>
    ///     Test that a valid configuration file returns a non-null config with no errors.
    /// </summary>
    [TestMethod]
    public void VersionMarkConfig_Load_ValidConfig_ReturnsConfig()
    {
        // Arrange - Create a well-formed .versionmark.yaml config file
        var tempFile = Path.GetTempFileName();
        try
        {
            const string yaml = """
                ---
                tools:
                  dotnet:
                    command: dotnet --version
                    regex: '(?<version>\d+\.\d+\.\d+)'
                """;
            File.WriteAllText(tempFile, yaml);

            // Act - Load the config from the valid file
            var (config, issues) = VersionMarkConfig.Load(tempFile);

            // Assert - Config should be returned with no error issues
            Assert.IsNotNull(config);
            Assert.IsFalse(issues.Any(i => i.Severity == LintSeverity.Error));
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    /// <summary>
    ///     Test that a non-existent file returns null config with an error issue.
    /// </summary>
    [TestMethod]
    public void VersionMarkConfig_Load_MissingFile_ReturnsNullConfig()
    {
        // Arrange - Use a path that does not exist
        var nonExistentFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".yaml");

        // Act - Attempt to load config from a missing file
        var (config, issues) = VersionMarkConfig.Load(nonExistentFile);

        // Assert - Config should be null and issues should contain an error
        Assert.IsNull(config);
        Assert.IsTrue(issues.Any(i => i.Severity == LintSeverity.Error));
    }

    /// <summary>
    ///     Test that a file containing invalid YAML returns null config with an error issue.
    /// </summary>
    [TestMethod]
    public void VersionMarkConfig_Load_InvalidYaml_ReturnsNullConfig()
    {
        // Arrange - Write syntactically broken YAML to a temp file
        var tempFile = Path.GetTempFileName();
        try
        {
            const string invalidYaml = """
                tools:
                  dotnet:
                    command: [unclosed bracket
                """;
            File.WriteAllText(tempFile, invalidYaml);

            // Act - Attempt to load config from malformed YAML
            var (config, issues) = VersionMarkConfig.Load(tempFile);

            // Assert - Config should be null and issues should contain a parse error
            Assert.IsNull(config);
            Assert.IsTrue(issues.Any(i => i.Severity == LintSeverity.Error));
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    /// <summary>
    ///     Test that a config without a 'tools' section returns null config with an error issue.
    /// </summary>
    [TestMethod]
    public void VersionMarkConfig_Load_MissingToolsSection_ReturnsNullConfig()
    {
        // Arrange - Write a YAML file that has no 'tools' key at the root
        var tempFile = Path.GetTempFileName();
        try
        {
            const string yaml = """
                ---
                version: 1
                """;
            File.WriteAllText(tempFile, yaml);

            // Act - Attempt to load config that lacks a 'tools' section
            var (config, issues) = VersionMarkConfig.Load(tempFile);

            // Assert - Config should be null because 'tools' is required
            Assert.IsNull(config);
            Assert.IsTrue(issues.Any(i => i.Severity == LintSeverity.Error));
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    /// <summary>
    ///     Test that a config with an empty 'tools' section returns null config with an error issue.
    /// </summary>
    [TestMethod]
    public void VersionMarkConfig_Load_EmptyToolsSection_ReturnsNullConfig()
    {
        // Arrange - Write a YAML file that has a 'tools' mapping with no entries
        var tempFile = Path.GetTempFileName();
        try
        {
            const string yaml = """
                ---
                tools: {}
                """;
            File.WriteAllText(tempFile, yaml);

            // Act - Attempt to load config with an empty tools section
            var (config, issues) = VersionMarkConfig.Load(tempFile);

            // Assert - Config should be null because at least one tool is required
            Assert.IsNull(config);
            Assert.IsTrue(issues.Any(i => i.Severity == LintSeverity.Error));
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    /// <summary>
    ///     Test that a tool without a 'command' field returns null config with an error issue.
    /// </summary>
    [TestMethod]
    public void VersionMarkConfig_Load_MissingCommand_ReturnsNullConfig()
    {
        // Arrange - Write a YAML file where the tool entry has no 'command' key
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

            // Act - Attempt to load config with a tool missing its 'command' field
            var (config, issues) = VersionMarkConfig.Load(tempFile);

            // Assert - Config should be null because 'command' is required
            Assert.IsNull(config);
            Assert.IsTrue(issues.Any(i => i.Severity == LintSeverity.Error));
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    /// <summary>
    ///     Test that a tool with an empty 'command' field returns null config with an error issue.
    /// </summary>
    [TestMethod]
    public void VersionMarkConfig_Load_EmptyCommand_ReturnsNullConfig()
    {
        // Arrange - Write a YAML file where the tool has an empty 'command' value
        var tempFile = Path.GetTempFileName();
        try
        {
            const string yaml = """
                ---
                tools:
                  dotnet:
                    command: ''
                    regex: '(?<version>\d+\.\d+\.\d+)'
                """;
            File.WriteAllText(tempFile, yaml);

            // Act - Attempt to load config with an empty command
            var (config, issues) = VersionMarkConfig.Load(tempFile);

            // Assert - Config should be null because an empty command is invalid
            Assert.IsNull(config);
            Assert.IsTrue(issues.Any(i => i.Severity == LintSeverity.Error));
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    /// <summary>
    ///     Test that a tool without a 'regex' field returns null config with an error issue.
    /// </summary>
    [TestMethod]
    public void VersionMarkConfig_Load_MissingRegex_ReturnsNullConfig()
    {
        // Arrange - Write a YAML file where the tool entry has no 'regex' key
        var tempFile = Path.GetTempFileName();
        try
        {
            const string yaml = """
                ---
                tools:
                  dotnet:
                    command: dotnet --version
                """;
            File.WriteAllText(tempFile, yaml);

            // Act - Attempt to load config with a tool missing its 'regex' field
            var (config, issues) = VersionMarkConfig.Load(tempFile);

            // Assert - Config should be null because 'regex' is required
            Assert.IsNull(config);
            Assert.IsTrue(issues.Any(i => i.Severity == LintSeverity.Error));
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    /// <summary>
    ///     Test that a tool with an empty 'regex' field returns null config with an error issue.
    /// </summary>
    [TestMethod]
    public void VersionMarkConfig_Load_EmptyRegex_ReturnsNullConfig()
    {
        // Arrange - Write a YAML file where the tool has an empty 'regex' value
        var tempFile = Path.GetTempFileName();
        try
        {
            const string yaml = """
                ---
                tools:
                  dotnet:
                    command: dotnet --version
                    regex: ''
                """;
            File.WriteAllText(tempFile, yaml);

            // Act - Attempt to load config with an empty regex
            var (config, issues) = VersionMarkConfig.Load(tempFile);

            // Assert - Config should be null because an empty regex is invalid
            Assert.IsNull(config);
            Assert.IsTrue(issues.Any(i => i.Severity == LintSeverity.Error));
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    /// <summary>
    ///     Test that a tool with an invalid 'regex' value returns null config with an error issue.
    /// </summary>
    [TestMethod]
    public void VersionMarkConfig_Load_InvalidRegex_ReturnsNullConfig()
    {
        // Arrange - Write a YAML file with a syntactically broken regex (unclosed group)
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

            // Act - Attempt to load config with a regex that cannot be compiled
            var (config, issues) = VersionMarkConfig.Load(tempFile);

            // Assert - Config should be null because the regex is invalid
            Assert.IsNull(config);
            Assert.IsTrue(issues.Any(i => i.Severity == LintSeverity.Error));
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    /// <summary>
    ///     Test that a tool with a regex missing the 'version' group returns null config with an error issue.
    /// </summary>
    [TestMethod]
    public void VersionMarkConfig_Load_RegexMissingVersionGroup_ReturnsNullConfig()
    {
        // Arrange - Write a YAML file with a valid regex that lacks the required 'version' named group
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

            // Act - Attempt to load config with a regex that has no 'version' group
            var (config, issues) = VersionMarkConfig.Load(tempFile);

            // Assert - Config should be null because the 'version' capture group is required
            Assert.IsNull(config);
            Assert.IsTrue(issues.Any(i => i.Severity == LintSeverity.Error));
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    /// <summary>
    ///     Test that an unknown top-level key produces a warning but config is still returned.
    /// </summary>
    [TestMethod]
    public void VersionMarkConfig_Load_UnknownTopLevelKey_ReturnsConfig()
    {
        // Arrange - Write a YAML file with a valid tool plus an unknown top-level key
        var tempFile = Path.GetTempFileName();
        try
        {
            const string yaml = """
                ---
                unknown-top-level-key: some-value
                tools:
                  dotnet:
                    command: dotnet --version
                    regex: '(?<version>\d+\.\d+\.\d+)'
                """;
            File.WriteAllText(tempFile, yaml);

            // Act - Load config that has an unknown top-level key
            var (config, issues) = VersionMarkConfig.Load(tempFile);

            // Assert - Config should be returned; unknown keys produce warnings, not errors
            Assert.IsNotNull(config);
            Assert.IsTrue(issues.Any(i => i.Severity == LintSeverity.Warning));
            Assert.IsFalse(issues.Any(i => i.Severity == LintSeverity.Error));
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    /// <summary>
    ///     Test that an unknown tool key produces a warning but config is still returned.
    /// </summary>
    [TestMethod]
    public void VersionMarkConfig_Load_UnknownToolKey_ReturnsConfig()
    {
        // Arrange - Write a YAML file with a valid tool plus an unknown key inside the tool
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

            // Act - Load config containing an unknown tool key
            var (config, issues) = VersionMarkConfig.Load(tempFile);

            // Assert - Config should be returned; unknown tool keys produce warnings, not errors
            Assert.IsNotNull(config);
            Assert.IsTrue(issues.Any(i => i.Severity == LintSeverity.Warning));
            Assert.IsFalse(issues.Any(i => i.Severity == LintSeverity.Error));
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    /// <summary>
    ///     Test that an empty OS-specific command override returns null config with an error issue.
    /// </summary>
    [TestMethod]
    public void VersionMarkConfig_Load_OsSpecificEmptyCommand_ReturnsNullConfig()
    {
        // Arrange - Write a YAML file with an empty command-win override
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

            // Act - Attempt to load config with an empty OS-specific command override
            var (config, issues) = VersionMarkConfig.Load(tempFile);

            // Assert - Config should be null because empty OS-specific overrides are not allowed
            Assert.IsNull(config);
            Assert.IsTrue(issues.Any(i => i.Severity == LintSeverity.Error));
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    /// <summary>
    ///     Test that an empty OS-specific regex override returns null config with an error issue.
    /// </summary>
    [TestMethod]
    public void VersionMarkConfig_Load_OsSpecificEmptyRegex_ReturnsNullConfig()
    {
        // Arrange - Write a YAML file with an empty regex-linux override
        var tempFile = Path.GetTempFileName();
        try
        {
            const string yaml = """
                ---
                tools:
                  dotnet:
                    command: dotnet --version
                    regex: '(?<version>\d+\.\d+\.\d+)'
                    regex-linux: ''
                """;
            File.WriteAllText(tempFile, yaml);

            // Act - Attempt to load config with an empty OS-specific regex override
            var (config, issues) = VersionMarkConfig.Load(tempFile);

            // Assert - Config should be null because empty OS-specific regex overrides are not allowed
            Assert.IsNull(config);
            Assert.IsTrue(issues.Any(i => i.Severity == LintSeverity.Error));
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    /// <summary>
    ///     Test that an OS-specific regex missing the 'version' group returns null config with an error issue.
    /// </summary>
    [TestMethod]
    public void VersionMarkConfig_Load_OsSpecificRegexMissingVersionGroup_ReturnsNullConfig()
    {
        // Arrange - Write a YAML file with an OS-specific regex that has no 'version' named group
        var tempFile = Path.GetTempFileName();
        try
        {
            const string yaml = """
                ---
                tools:
                  dotnet:
                    command: dotnet --version
                    regex: '(?<version>\d+\.\d+\.\d+)'
                    regex-macos: '\d+\.\d+\.\d+'
                """;
            File.WriteAllText(tempFile, yaml);

            // Act - Attempt to load config with an OS-specific regex missing the 'version' group
            var (config, issues) = VersionMarkConfig.Load(tempFile);

            // Assert - Config should be null because the 'version' group is required in all regexes
            Assert.IsNull(config);
            Assert.IsTrue(issues.Any(i => i.Severity == LintSeverity.Error));
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    /// <summary>
    ///     Test that an OS-specific regex that is invalid returns null config with an error issue.
    /// </summary>
    [TestMethod]
    public void VersionMarkConfig_Load_OsSpecificInvalidRegex_ReturnsNullConfig()
    {
        // Arrange - Write a YAML file with a broken OS-specific regex (unclosed group)
        var tempFile = Path.GetTempFileName();
        try
        {
            const string yaml = """
                ---
                tools:
                  dotnet:
                    command: dotnet --version
                    regex: '(?<version>\d+\.\d+\.\d+)'
                    regex-win: '(?<version'
                """;
            File.WriteAllText(tempFile, yaml);

            // Act - Attempt to load config with an invalid OS-specific regex
            var (config, issues) = VersionMarkConfig.Load(tempFile);

            // Assert - Config should be null because the OS-specific regex cannot be compiled
            Assert.IsNull(config);
            Assert.IsTrue(issues.Any(i => i.Severity == LintSeverity.Error));
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    /// <summary>
    ///     Test that multiple errors in different tools are all reported in a single Load call.
    /// </summary>
    [TestMethod]
    public void VersionMarkConfig_Load_MultipleErrors_ReportsAll()
    {
        // Arrange - Write a config where tool1 is missing 'regex' and tool2 is missing 'command'
        var tempFile = Path.GetTempFileName();
        try
        {
            const string yaml = """
                ---
                tools:
                  tool1:
                    command: tool1 --version
                  tool2:
                    regex: '(?<version>\d+)'
                """;
            File.WriteAllText(tempFile, yaml);

            // Act - Load a config containing errors in multiple tools
            var (config, issues) = VersionMarkConfig.Load(tempFile);

            // Assert - Config should be null and issues should reference both tool1 and tool2
            Assert.IsNull(config);
            Assert.IsTrue(
                issues.Any(i => i.Severity == LintSeverity.Error && i.Description.Contains("tool1")),
                "Issues should contain an error mentioning tool1 (missing regex)");
            Assert.IsTrue(
                issues.Any(i => i.Severity == LintSeverity.Error && i.Description.Contains("tool2")),
                "Issues should contain an error mentioning tool2 (missing command)");
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    /// <summary>
    ///     Test that all issues include the file path of the configuration file.
    /// </summary>
    [TestMethod]
    public void VersionMarkConfig_Load_IssuesContainFilePath()
    {
        // Arrange - Write a config with a missing required field to force an error issue
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

            // Act - Load the config and inspect the returned issues
            var (config, issues) = VersionMarkConfig.Load(tempFile);

            // Assert - All issues should reference the path of the config file that was loaded
            Assert.IsNull(config);
            Assert.IsTrue(
                issues.Any(i => i.FilePath == tempFile),
                "At least one issue should contain the config file path");
        }
        finally
        {
            File.Delete(tempFile);
        }
    }
}
