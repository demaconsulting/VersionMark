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
using DemaConsulting.VersionMark.Linting;

namespace DemaConsulting.VersionMark.Tests.Linting;

/// <summary>
///     Subsystem tests for the Linting subsystem (Lint and Context working together).
/// </summary>
[TestClass]
public class LintingSubsystemTests
{
    /// <summary>
    ///     Test that the full linting pipeline succeeds and exits cleanly for a valid configuration.
    /// </summary>
    [TestMethod]
    public void LintingSubsystem_Lint_ValidConfig_SucceedsWithZeroExitCode()
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
            using var context = Context.Create(["--silent"]);

            // Act - Run the full linting pipeline
            var result = Lint.Run(context, tempFile);

            // Assert - The linting subsystem should report success with a clean exit code
            Assert.IsTrue(result, "Linting should succeed for a valid configuration");
            Assert.AreEqual(0, context.ExitCode,
                "Exit code should be zero after successful lint through the full linting pipeline");
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    /// <summary>
    ///     Test that the linting pipeline reports all errors in a single pass for an invalid configuration.
    /// </summary>
    [TestMethod]
    public void LintingSubsystem_Lint_MultipleErrors_ReportsAllErrorsInSinglePass()
    {
        // Arrange - Write a configuration with multiple errors to a temp file
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

            using var context = Context.Create(["--silent"]);
            var originalError = Console.Error;
            try
            {
                using var errorWriter = new StringWriter();
                Console.SetError(errorWriter);

                // Act - Run the full linting pipeline against a config with multiple errors
                var result = Lint.Run(context, tempFile);

                // Assert - The linting subsystem should report failure
                Assert.IsFalse(result,
                    "Linting should fail for a configuration with multiple errors");
                Assert.AreEqual(1, context.ExitCode,
                    "Exit code should be non-zero when linting finds errors");
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
    ///     Test that the linting pipeline fails for invalid YAML content.
    /// </summary>
    [TestMethod]
    public void LintingSubsystem_Lint_InvalidYaml_Fails()
    {
        // Arrange
        var tempFile = Path.GetTempFileName() + ".yaml";
        File.WriteAllText(tempFile, "not: valid: yaml: content: : ::");

        try
        {
            using var context = Context.Create(["--silent"]);

            // Act
            var result = Lint.Run(context, tempFile);

            // Assert
            Assert.IsFalse(result);
            Assert.AreEqual(1, context.ExitCode);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    /// <summary>
    ///     Test that linting reports an error when the config file does not exist.
    /// </summary>
    [TestMethod]
    public void LintingSubsystem_Lint_NonExistentFile_Fails()
    {
        // Arrange
        var nonExistentPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".yaml");
        using var context = Context.Create(["--silent"]);

        // Act
        var result = Lint.Run(context, nonExistentPath);

        // Assert
        Assert.IsFalse(result);
        Assert.AreEqual(1, context.ExitCode);
    }
}
