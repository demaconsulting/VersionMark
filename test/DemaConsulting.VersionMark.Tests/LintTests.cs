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

namespace DemaConsulting.VersionMark.Tests;

/// <summary>
///     Unit tests for the Lint class.
/// </summary>
[TestClass]
public class LintTests
{
    /// <summary>
    ///     Test that a valid configuration file with command and regex returns true.
    /// </summary>
    [TestMethod]
    public void Lint_Run_ValidConfig_ReturnsTrue()
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
            using var context = Context.Create(["--silent"]);

            // Act - Run lint on a valid config
            var result = Lint.Run(context, tempFile);

            // Assert - Lint should succeed with no errors and exit code 0
            Assert.IsTrue(result);
            Assert.AreEqual(0, context.ExitCode);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    /// <summary>
    ///     Test that a non-existent file returns false with exit code 1.
    /// </summary>
    [TestMethod]
    public void Lint_Run_MissingFile_ReturnsFalse()
    {
        // Arrange - Use a path that does not exist
        var nonExistentFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".yaml");
        using var context = Context.Create(["--silent"]);

        // Act - Run lint against a missing file
        var result = Lint.Run(context, nonExistentFile);

        // Assert - Lint should fail and report an error
        Assert.IsFalse(result);
        Assert.AreEqual(1, context.ExitCode);
    }

    /// <summary>
    ///     Test that a file containing invalid YAML returns false with exit code 1.
    /// </summary>
    [TestMethod]
    public void Lint_Run_InvalidYaml_ReturnsFalse()
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
            using var context = Context.Create(["--silent"]);

            // Act - Run lint on malformed YAML
            var result = Lint.Run(context, tempFile);

            // Assert - Lint should fail with a parse error
            Assert.IsFalse(result);
            Assert.AreEqual(1, context.ExitCode);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    /// <summary>
    ///     Test that a config without a 'tools' section returns false with exit code 1.
    /// </summary>
    [TestMethod]
    public void Lint_Run_MissingToolsSection_ReturnsFalse()
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
            using var context = Context.Create(["--silent"]);

            // Act - Run lint on a config that lacks a 'tools' section
            var result = Lint.Run(context, tempFile);

            // Assert - Lint should fail because 'tools' is required
            Assert.IsFalse(result);
            Assert.AreEqual(1, context.ExitCode);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    /// <summary>
    ///     Test that a config with an empty 'tools' section returns false with exit code 1.
    /// </summary>
    [TestMethod]
    public void Lint_Run_EmptyToolsSection_ReturnsFalse()
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
            using var context = Context.Create(["--silent"]);

            // Act - Run lint on a config with an empty tools section
            var result = Lint.Run(context, tempFile);

            // Assert - Lint should fail because at least one tool is required
            Assert.IsFalse(result);
            Assert.AreEqual(1, context.ExitCode);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    /// <summary>
    ///     Test that a tool without a 'command' field returns false with exit code 1.
    /// </summary>
    [TestMethod]
    public void Lint_Run_MissingCommand_ReturnsFalse()
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
            using var context = Context.Create(["--silent"]);

            // Act - Run lint on a tool that is missing its required 'command' field
            var result = Lint.Run(context, tempFile);

            // Assert - Lint should fail because 'command' is required
            Assert.IsFalse(result);
            Assert.AreEqual(1, context.ExitCode);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    /// <summary>
    ///     Test that a tool with an empty 'command' field returns false with exit code 1.
    /// </summary>
    [TestMethod]
    public void Lint_Run_EmptyCommand_ReturnsFalse()
    {
        // Arrange - Write a YAML file where the tool's 'command' value is blank
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
            using var context = Context.Create(["--silent"]);

            // Act - Run lint on a tool with an empty command string
            var result = Lint.Run(context, tempFile);

            // Assert - Lint should fail because an empty command is invalid
            Assert.IsFalse(result);
            Assert.AreEqual(1, context.ExitCode);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    /// <summary>
    ///     Test that a tool without a 'regex' field returns false with exit code 1.
    /// </summary>
    [TestMethod]
    public void Lint_Run_MissingRegex_ReturnsFalse()
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
            using var context = Context.Create(["--silent"]);

            // Act - Run lint on a tool that is missing its required 'regex' field
            var result = Lint.Run(context, tempFile);

            // Assert - Lint should fail because 'regex' is required
            Assert.IsFalse(result);
            Assert.AreEqual(1, context.ExitCode);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    /// <summary>
    ///     Test that a tool with an empty 'regex' field returns false with exit code 1.
    /// </summary>
    [TestMethod]
    public void Lint_Run_EmptyRegex_ReturnsFalse()
    {
        // Arrange - Write a YAML file where the tool's 'regex' value is blank
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
            using var context = Context.Create(["--silent"]);

            // Act - Run lint on a tool with an empty regex string
            var result = Lint.Run(context, tempFile);

            // Assert - Lint should fail because an empty regex is invalid
            Assert.IsFalse(result);
            Assert.AreEqual(1, context.ExitCode);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    /// <summary>
    ///     Test that a tool with a regex that cannot be compiled returns false with exit code 1.
    /// </summary>
    [TestMethod]
    public void Lint_Run_InvalidRegex_ReturnsFalse()
    {
        // Arrange - Write a YAML file where the tool's 'regex' is a syntactically invalid pattern
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
            using var context = Context.Create(["--silent"]);

            // Act - Run lint on a tool whose regex cannot be compiled
            var result = Lint.Run(context, tempFile);

            // Assert - Lint should fail because the regex pattern is invalid
            Assert.IsFalse(result);
            Assert.AreEqual(1, context.ExitCode);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    /// <summary>
    ///     Test that a tool with a valid regex that has no 'version' capture group returns false.
    /// </summary>
    [TestMethod]
    public void Lint_Run_RegexMissingVersionGroup_ReturnsFalse()
    {
        // Arrange - Write a YAML file where the tool's regex is valid but lacks a 'version' named group
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
            using var context = Context.Create(["--silent"]);

            // Act - Run lint on a tool whose regex has no named 'version' capture group
            var result = Lint.Run(context, tempFile);

            // Assert - Lint should fail because the 'version' group is required
            Assert.IsFalse(result);
            Assert.AreEqual(1, context.ExitCode);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    /// <summary>
    ///     Test that an unknown top-level key produces a warning but returns true (non-fatal).
    /// </summary>
    [TestMethod]
    public void Lint_Run_UnknownTopLevelKey_ReturnsTrue()
    {
        // Arrange - Write a YAML file with a valid tools section plus an unknown top-level key
        var tempFile = Path.GetTempFileName();
        try
        {
            const string yaml = """
                ---
                tools:
                  dotnet:
                    command: dotnet --version
                    regex: '(?<version>\d+\.\d+\.\d+)'
                unknown-key: some-value
                """;
            File.WriteAllText(tempFile, yaml);
            using var context = Context.Create(["--silent"]);

            // Act - Run lint on a config containing an unknown top-level key
            var result = Lint.Run(context, tempFile);

            // Assert - Lint should succeed (warnings are non-fatal) with exit code 0
            Assert.IsTrue(result);
            Assert.AreEqual(0, context.ExitCode);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    /// <summary>
    ///     Test that an unknown per-tool key produces a warning but returns true (non-fatal).
    /// </summary>
    [TestMethod]
    public void Lint_Run_UnknownToolKey_ReturnsTrue()
    {
        // Arrange - Write a YAML file where a tool has an unknown configuration key
        var tempFile = Path.GetTempFileName();
        try
        {
            const string yaml = """
                ---
                tools:
                  dotnet:
                    command: dotnet --version
                    regex: '(?<version>\d+\.\d+\.\d+)'
                    unknown-tool-key: ignored
                """;
            File.WriteAllText(tempFile, yaml);
            using var context = Context.Create(["--silent"]);

            // Act - Run lint on a tool that contains an unknown configuration key
            var result = Lint.Run(context, tempFile);

            // Assert - Lint should succeed (unknown tool keys are non-fatal warnings) with exit code 0
            Assert.IsTrue(result);
            Assert.AreEqual(0, context.ExitCode);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    /// <summary>
    ///     Test that an OS-specific command override with an empty value returns false.
    /// </summary>
    [TestMethod]
    public void Lint_Run_OsSpecificEmptyCommand_ReturnsFalse()
    {
        // Arrange - Write a YAML file where command-win is present but empty
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
            using var context = Context.Create(["--silent"]);

            // Act - Run lint on a tool with an empty OS-specific command override
            var result = Lint.Run(context, tempFile);

            // Assert - Lint should fail because an empty command-win is invalid
            Assert.IsFalse(result);
            Assert.AreEqual(1, context.ExitCode);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    /// <summary>
    ///     Test that an OS-specific regex override with an empty value returns false.
    /// </summary>
    [TestMethod]
    public void Lint_Run_OsSpecificEmptyRegex_ReturnsFalse()
    {
        // Arrange - Write a YAML file where regex-win is present but empty
        var tempFile = Path.GetTempFileName();
        try
        {
            const string yaml = """
                ---
                tools:
                  dotnet:
                    command: dotnet --version
                    regex: '(?<version>\d+\.\d+\.\d+)'
                    regex-win: ''
                """;
            File.WriteAllText(tempFile, yaml);
            using var context = Context.Create(["--silent"]);

            // Act - Run lint on a tool with an empty OS-specific regex override
            var result = Lint.Run(context, tempFile);

            // Assert - Lint should fail because an empty regex-win is invalid
            Assert.IsFalse(result);
            Assert.AreEqual(1, context.ExitCode);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    /// <summary>
    ///     Test that an OS-specific regex override that lacks a 'version' capture group returns false.
    /// </summary>
    [TestMethod]
    public void Lint_Run_OsSpecificRegexMissingVersionGroup_ReturnsFalse()
    {
        // Arrange - Write a YAML file where regex-win is a valid pattern but has no 'version' named group
        var tempFile = Path.GetTempFileName();
        try
        {
            const string yaml = """
                ---
                tools:
                  dotnet:
                    command: dotnet --version
                    regex: '(?<version>\d+\.\d+\.\d+)'
                    regex-win: '\d+\.\d+\.\d+'
                """;
            File.WriteAllText(tempFile, yaml);
            using var context = Context.Create(["--silent"]);

            // Act - Run lint on a tool with an OS-specific regex that has no 'version' group
            var result = Lint.Run(context, tempFile);

            // Assert - Lint should fail because the 'version' group is required even for OS-specific overrides
            Assert.IsFalse(result);
            Assert.AreEqual(1, context.ExitCode);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    /// <summary>
    ///     Test that an OS-specific regex override that cannot be compiled returns false.
    /// </summary>
    [TestMethod]
    public void Lint_Run_OsSpecificInvalidRegex_ReturnsFalse()
    {
        // Arrange - Write a YAML file where regex-linux contains an invalid regex pattern
        var tempFile = Path.GetTempFileName();
        try
        {
            const string yaml = """
                ---
                tools:
                  dotnet:
                    command: dotnet --version
                    regex: '(?<version>\d+\.\d+\.\d+)'
                    regex-linux: '(?<version'
                """;
            File.WriteAllText(tempFile, yaml);
            using var context = Context.Create(["--silent"]);

            // Act - Run lint on a tool with an OS-specific regex that cannot be compiled
            var result = Lint.Run(context, tempFile);

            // Assert - Lint should fail because the OS-specific regex is invalid
            Assert.IsFalse(result);
            Assert.AreEqual(1, context.ExitCode);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    /// <summary>
    ///     Test that a config with multiple errors reports all of them (does not stop at the first).
    /// </summary>
    [TestMethod]
    public void Lint_Run_MultipleErrors_ReportsAll()
    {
        // Arrange - Redirect Console.Error to verify both error messages are emitted
        var previousError = Console.Error;
        var errorOutput = new System.Text.StringBuilder();
        var tempFile = Path.GetTempFileName();
        try
        {
            // A tool with neither 'command' nor 'regex' should produce two separate error messages
            const string yaml = """
                ---
                tools:
                  dotnet: {}
                """;
            File.WriteAllText(tempFile, yaml);

            using var captureWriter = new System.IO.StringWriter(errorOutput);
            Console.SetError(captureWriter);

            // Context must NOT be silent so that WriteError calls Console.Error.WriteLine
            using var context = Context.Create([]);

            // Act - Run lint on a tool that has no required fields at all
            var result = Lint.Run(context, tempFile);
            captureWriter.Flush();

            // Assert - Lint should fail (both errors accumulated)
            Assert.IsFalse(result);
            Assert.AreEqual(1, context.ExitCode);
        }
        finally
        {
            Console.SetError(previousError);
            File.Delete(tempFile);
        }

        // Assert - Both error messages must be present, confirming lint does not stop at the first
        var captured = errorOutput.ToString();
        Assert.IsTrue(
            captured.Contains("'command'"),
            $"Expected error about missing 'command' field, but got: {captured}");
        Assert.IsTrue(
            captured.Contains("'regex'"),
            $"Expected error about missing 'regex' field, but got: {captured}");
    }

    /// <summary>
    ///     Test that error messages contain the config file path.
    /// </summary>
    [TestMethod]
    public void Lint_Run_ErrorMessageContainsFileName()
    {
        // Arrange - Redirect Console.Error so we can inspect the messages that Lint emits.
        //           A missing-command error is straightforward to trigger reliably.
        var previousError = Console.Error;
        var errorOutput = new System.Text.StringBuilder();
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

            using var captureWriter = new System.IO.StringWriter(errorOutput);
            Console.SetError(captureWriter);

            // Context must NOT be silent so that WriteError calls Console.Error.WriteLine
            using var context = Context.Create([]);

            // Act - Run lint on a tool that is missing its required 'command' field
            Lint.Run(context, tempFile);
            captureWriter.Flush();
        }
        finally
        {
            Console.SetError(previousError);
            File.Delete(tempFile);
        }

        // Assert - The error message should embed the config file path so users know which file to fix
        var captured = errorOutput.ToString();
        Assert.IsTrue(
            captured.Contains(Path.GetFileName(tempFile)),
            $"Expected error output to contain the file name '{Path.GetFileName(tempFile)}', but got: {captured}");
    }
}
