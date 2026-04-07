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

namespace DemaConsulting.VersionMark.Tests.Cli;

/// <summary>
///     Subsystem tests for the Cli subsystem (Program and Context working together).
/// </summary>
[TestClass]
public class CliTests
{
    /// <summary>
    ///     Test that the full CLI pipeline with --version flag exits cleanly.
    /// </summary>
    [TestMethod]
    public void Cli_Run_VersionFlag_ExitsCleanly()
    {
        // Arrange - Create a context with --version via the full CLI pipeline
        using var context = Context.Create(["--version"]);

        // Act - Run the program through the CLI subsystem
        Program.Run(context);

        // Assert - The CLI subsystem should exit with code 0
        Assert.AreEqual(0, context.ExitCode);
    }

    /// <summary>
    ///     Test that the full CLI pipeline with --silent flag suppresses standard output.
    /// </summary>
    [TestMethod]
    public void Cli_Run_SilentWithVersionFlag_SuppressesOutput()
    {
        // Arrange - Redirect console output to capture what the CLI writes
        var originalOut = Console.Out;
        try
        {
            using var outWriter = new StringWriter();
            Console.SetOut(outWriter);

            // Act - Run through the full Context + Program CLI pipeline with silent mode
            using var context = Context.Create(["--silent", "--version"]);
            Program.Run(context);

            // Assert - Silent mode should suppress all standard output
            Assert.AreEqual(0, context.ExitCode);
            Assert.IsTrue(string.IsNullOrEmpty(outWriter.ToString()),
                "Silent flag should suppress version output through the full CLI pipeline");
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    /// <summary>
    ///     Test that the full CLI pipeline with --help flag displays usage information.
    /// </summary>
    [TestMethod]
    public void Cli_Run_HelpFlag_DisplaysUsageInformation()
    {
        // Arrange
        var originalOut = Console.Out;
        var output = new System.IO.StringWriter();
        Console.SetOut(output);

        try
        {
            using var context = Context.Create(["--help"]);

            // Act
            Program.Run(context);

            // Assert
            Assert.AreEqual(0, context.ExitCode);
            var text = output.ToString();
            Assert.IsTrue(text.Length > 0);
            Assert.IsTrue(text.Contains("--capture"));
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    /// <summary>
    ///     Test that the full CLI pipeline with --validate flag runs self-validation.
    /// </summary>
    [TestMethod]
    public void Cli_Run_ValidateFlag_RunsValidation()
    {
        // Arrange
        using var context = Context.Create(["--validate", "--silent"]);

        // Act
        Program.Run(context);

        // Assert
        Assert.AreEqual(0, context.ExitCode);
    }

    /// <summary>
    ///     Test that the full CLI pipeline rejects unknown arguments by throwing ArgumentException.
    /// </summary>
    [TestMethod]
    public void Cli_Run_InvalidArgs_ThrowsArgumentException()
    {
        // Arrange - No setup required; unknown flags are rejected by Context.Create

        // Act & Assert - Context.Create itself throws for unrecognized flags
        Assert.ThrowsExactly<ArgumentException>(() =>
        {
            using var context = Context.Create(["--unknown-flag-xyz"]);
            Program.Run(context);
        });
    }

    /// <summary>
    ///     Test that the full CLI pipeline with --lint flag succeeds for a valid config file.
    /// </summary>
    [TestMethod]
    public void Cli_Run_LintFlag_ValidConfig_Succeeds()
    {
        // Arrange
        var tempFile = Path.GetTempFileName() + ".yaml";
        File.WriteAllText(tempFile, """
            tools:
              dotnet:
                command: dotnet --version
                regex: '(?<version>\d+\.\d+\.\d+)'
            """);

        try
        {
            using var context = Context.Create(["--lint", tempFile]);

            // Act
            Program.Run(context);

            // Assert
            Assert.AreEqual(0, context.ExitCode);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    /// <summary>
    ///     Test that the full CLI pipeline with --results flag writes validation results to a file.
    /// </summary>
    [TestMethod]
    public void Cli_Run_ResultsFlag_WritesResultsFile()
    {
        // Arrange - Set up a results file path that should be written during --validate
        var resultsFile = Path.GetTempFileName() + ".trx";
        try
        {
            using var context = Context.Create(["--validate", "--silent", "--results", resultsFile]);

            // Act - Run the full CLI pipeline with both --validate and --results
            Program.Run(context);

            // Assert - The results file should exist and contain TRX content
            Assert.AreEqual(0, context.ExitCode);
            Assert.IsTrue(File.Exists(resultsFile),
                "Results file should be written when --results flag is specified");
            var content = File.ReadAllText(resultsFile);
            Assert.IsFalse(string.IsNullOrWhiteSpace(content),
                "Results file should contain test result data");
        }
        finally
        {
            if (File.Exists(resultsFile))
            {
                File.Delete(resultsFile);
            }
        }
    }

    /// <summary>
    ///     Test that the full CLI pipeline with --log flag writes output to a log file.
    /// </summary>
    [TestMethod]
    public void Cli_Run_LogFlag_WritesOutputToLogFile()
    {
        // Arrange - Set up a log file that should be written with version output
        var logFile = Path.GetTempFileName();
        try
        {
            string logContent;
            using (var context = Context.Create(["--version", "--log", logFile]))
            {
                // Act - Run the full CLI pipeline with --log
                Program.Run(context);

                // Assert - Exit code should be zero
                Assert.AreEqual(0, context.ExitCode);
            }

            // Assert - The log file should contain the version output (after context is disposed)
            logContent = File.ReadAllText(logFile);
            Assert.IsFalse(string.IsNullOrWhiteSpace(logContent),
                "Log file should contain output when --log flag is specified");
        }
        finally
        {
            if (File.Exists(logFile))
            {
                File.Delete(logFile);
            }
        }
    }
}
