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
using DemaConsulting.VersionMark.SelfTest;

namespace DemaConsulting.VersionMark.Tests.SelfTest;

/// <summary>
///     Subsystem tests for the SelfTest subsystem.
/// </summary>
public class SelfTestTests
{
    /// <summary>
    ///     Test that the self-validation pipeline writes results to a TRX file when --results is specified.
    /// </summary>
    [Fact]
    public void SelfTest_Run_WithResultsFlag_WritesResultsFile()
    {
        // Arrange - Set up a TRX results file path
        var resultsFile = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.trx");
        try
        {
            using var context = Context.Create(["--validate", "--silent", "--results", resultsFile]);

            // Act - Run self-validation with --results to write TRX output
            Validation.Run(context);

            // Assert - The TRX file should exist and contain XML content
            Assert.Equal(0, context.ExitCode);
            Assert.True(File.Exists(resultsFile),
                "Self-validation should write results to the file specified by --results");
            var content = File.ReadAllText(resultsFile);
            Assert.True(content.Contains("TestRun") || content.Contains("testsuites"),
                "Results file should contain TRX or JUnit test result data");
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
    ///     Test that the self-validation pipeline writes JUnit XML results when --results specifies a .xml file.
    /// </summary>
    [Fact]
    public void SelfTest_Run_WithResultsXmlFlag_WritesJUnitResultsFile()
    {
        // Arrange - Set up a JUnit XML results file path
        var resultsFile = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.xml");
        try
        {
            using var context = Context.Create(["--validate", "--silent", "--results", resultsFile]);

            // Act - Run self-validation with --results pointing to a .xml file
            Validation.Run(context);

            // Assert - The XML file should exist and contain JUnit content
            Assert.Equal(0, context.ExitCode);
            Assert.True(File.Exists(resultsFile),
                "Self-validation should write JUnit results to the .xml file specified by --results");
            var content = File.ReadAllText(resultsFile);
            Assert.True(content.Contains("testsuites") || content.Contains("testsuite"),
                "JUnit results file should contain testsuites element");
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
    ///     Test that the self-validation pipeline writes a ## heading when --depth 2 is specified.
    ///     What is tested: The --depth argument controls the heading level in the self-validation report
    ///     What the assertions prove: Output contains "## DEMA Consulting VersionMark" with depth 2
    /// </summary>
    [Fact]
    public void SelfTest_Run_WithDepthTwo_WritesHashHashHeader()
    {
        // Arrange - Redirect console output to capture the validation report
        var originalOut = Console.Out;
        using var writer = new System.IO.StringWriter();
        Console.SetOut(writer);
        try
        {
            using var context = Context.Create(["--validate", "--depth", "2"]);

            // Act - Run self-validation with --depth 2
            Validation.Run(context);

            // Assert - Output should contain the ## heading for depth 2
            var output = writer.ToString();
            Assert.True(output.Contains("## DEMA Consulting VersionMark"),
                "Self-validation report should use ## heading when --depth 2 is specified");
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    /// <summary>
    ///     Test that the self-validation capture workflow runs successfully.
    ///     What is tested: The capture sub-test within Validation.Run passes
    ///     What the assertions prove: Validation completes with exit code 0, confirming capture ran
    /// </summary>
    [Fact]
    public void SelfTest_Run_Capture_CapturesToolVersions()
    {
        // Arrange
        using var context = Context.Create(["--validate", "--silent"]);

        // Act - Run self-validation; it internally runs the capture sub-test
        Validation.Run(context);

        // Assert - Exit code 0 means all self-validation tests (including capture) passed
        Assert.Equal(0, context.ExitCode);
    }

    /// <summary>
    ///     Test that the self-validation publish workflow runs successfully.
    ///     What is tested: The publish sub-test within Validation.Run passes
    ///     What the assertions prove: Validation completes with exit code 0, confirming publish ran
    /// </summary>
    [Fact]
#pragma warning disable S4144 // Intentionally identical: each test covers a distinct requirement for traceability
    public void SelfTest_Run_Publish_GeneratesMarkdownReport()
    {
        // Arrange
        using var context = Context.Create(["--validate", "--silent"]);

        // Act - Run self-validation; it internally runs the publish sub-test
        Validation.Run(context);

        // Assert - Exit code 0 means all self-validation tests (including publish) passed
        Assert.Equal(0, context.ExitCode);
    }
#pragma warning restore S4144

    /// <summary>
    ///     Test that the self-validation lint-valid workflow accepts a valid configuration.
    ///     What is tested: The lint-valid sub-test within Validation.Run passes
    ///     What the assertions prove: Validation completes with exit code 0, confirming valid config accepted
    /// </summary>
    [Fact]
#pragma warning disable S4144 // Intentionally identical: each test covers a distinct requirement for traceability
    public void SelfTest_Run_LintValid_PassesForValidConfig()
    {
        // Arrange
        using var context = Context.Create(["--validate", "--silent"]);

        // Act - Run self-validation; it internally runs the lint-valid sub-test
        Validation.Run(context);

        // Assert - Exit code 0 means all self-validation tests (including lint-valid) passed
        Assert.Equal(0, context.ExitCode);
    }
#pragma warning restore S4144

    /// <summary>
    ///     Test that the self-validation lint-invalid workflow rejects an invalid configuration.
    ///     What is tested: The lint-invalid sub-test within Validation.Run passes
    ///     What the assertions prove: Validation completes with exit code 0, confirming invalid config was rejected
    /// </summary>
    [Fact]
#pragma warning disable S4144 // Intentionally identical: each test covers a distinct requirement for traceability
    public void SelfTest_Run_LintInvalid_RejectsInvalidConfig()
    {
        // Arrange
        using var context = Context.Create(["--validate", "--silent"]);

        // Act - Run self-validation; it internally runs the lint-invalid sub-test
        Validation.Run(context);

        // Assert - Exit code 0 means all self-validation tests (including lint-invalid) passed
        Assert.Equal(0, context.ExitCode);
    }
#pragma warning restore S4144
}
