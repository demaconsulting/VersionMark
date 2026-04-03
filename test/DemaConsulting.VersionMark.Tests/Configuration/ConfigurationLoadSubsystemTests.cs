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

using DemaConsulting.VersionMark.Cli;

namespace DemaConsulting.VersionMark.Tests.Configuration;

/// <summary>
///     Subsystem tests for configuration loading via <see cref="Program.Run"/> with the <c>--lint</c> flag.
/// </summary>
[TestClass]
public class ConfigurationLoadSubsystemTests
{
    /// <summary>
    ///     Test that the full configuration load pipeline succeeds and exits cleanly for a valid configuration.
    /// </summary>
    [TestMethod]
    public void ConfigurationLoad_ValidConfig_SucceedsWithZeroExitCode()
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
    public void ConfigurationLoad_MultipleErrors_ReportsAllErrorsInSinglePass()
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
    public void ConfigurationLoad_InvalidYaml_Fails()
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
    public void ConfigurationLoad_NonExistentFile_Fails()
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
    public void ConfigurationLoad_InvalidRegex_ReportsError()
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
    public void ConfigurationLoad_RegexWithoutVersionGroup_ReportsError()
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
    public void ConfigurationLoad_EmptyOsSpecificOverride_ReportsError()
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
    public void ConfigurationLoad_UnknownKey_IsWarningNotError()
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
    public void ConfigurationLoad_Error_IncludesFileAndLineInfo()
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
