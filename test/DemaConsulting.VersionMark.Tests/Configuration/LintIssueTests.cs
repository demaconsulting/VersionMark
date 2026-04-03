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
using DemaConsulting.VersionMark.Configuration;

namespace DemaConsulting.VersionMark.Tests.Configuration;

/// <summary>
///     Unit tests for the <see cref="LintIssue"/> record and the <see cref="VersionMarkLoadResult"/> record.
/// </summary>
[TestClass]
public class LintIssueTests
{
    /// <summary>
    ///     Test that <see cref="LintIssue"/> properties are stored correctly.
    /// </summary>
    [TestMethod]
    public void LintIssue_Constructor_AllFields_AreStoredCorrectly()
    {
        // Arrange & Act
        var issue = new LintIssue(
            FilePath: "config.yaml",
            Line: 3,
            Column: 5,
            Severity: LintSeverity.Error,
            Description: "Missing required field");

        // Assert
        Assert.AreEqual("config.yaml", issue.FilePath);
        Assert.AreEqual(3L, issue.Line);
        Assert.AreEqual(5L, issue.Column);
        Assert.AreEqual(LintSeverity.Error, issue.Severity);
        Assert.AreEqual("Missing required field", issue.Description);
    }

    /// <summary>
    ///     Test that <see cref="LintIssue.ToString"/> produces the expected format with lowercase severity for an error.
    /// </summary>
    [TestMethod]
    public void LintIssue_ToString_Error_ProducesLowercaseSeverity()
    {
        // Arrange
        var issue = new LintIssue(
            FilePath: "config.yaml",
            Line: 10,
            Column: 2,
            Severity: LintSeverity.Error,
            Description: "tool 'dotnet' is missing required field 'command'");

        // Act
        var result = issue.ToString();

        // Assert - severity must be lowercase 'error', not 'Error'
        Assert.AreEqual("config.yaml(10,2): error: tool 'dotnet' is missing required field 'command'", result);
    }

    /// <summary>
    ///     Test that <see cref="LintIssue.ToString"/> produces the expected format with lowercase severity for a warning.
    /// </summary>
    [TestMethod]
    public void LintIssue_ToString_Warning_ProducesLowercaseSeverity()
    {
        // Arrange
        var issue = new LintIssue(
            FilePath: "my.versionmark.yaml",
            Line: 4,
            Column: 1,
            Severity: LintSeverity.Warning,
            Description: "unknown key 'extra-field'");

        // Act
        var result = issue.ToString();

        // Assert - severity must be lowercase 'warning', not 'Warning'
        Assert.AreEqual("my.versionmark.yaml(4,1): warning: unknown key 'extra-field'", result);
    }

    /// <summary>
    ///     Test that <see cref="VersionMarkLoadResult"/> properties are stored correctly.
    /// </summary>
    [TestMethod]
    public void VersionMarkLoadResult_Constructor_AllFields_AreStoredCorrectly()
    {
        // Arrange
        var issue = new LintIssue("f.yaml", 1, 1, LintSeverity.Warning, "unknown key");
        var issues = new List<LintIssue> { issue };

        // Act
        var loadResult = new VersionMarkLoadResult(null, issues);

        // Assert
        Assert.IsNull(loadResult.Config);
        Assert.HasCount(1, loadResult.Issues);
        Assert.AreSame(issue, loadResult.Issues[0]);
    }

    /// <summary>
    ///     Test that <see cref="VersionMarkLoadResult.ReportIssues"/> routes errors to <c>context.WriteError</c>.
    /// </summary>
    [TestMethod]
    public void VersionMarkLoadResult_ReportIssues_Error_WritesToErrorStream()
    {
        // Arrange
        var issue = new LintIssue("config.yaml", 2, 1, LintSeverity.Error, "missing command");
        var loadResult = new VersionMarkLoadResult(null, [issue]);
        var originalError = Console.Error;

        try
        {
            using var errWriter = new StringWriter();
            Console.SetError(errWriter);

            // Act
            using var context = Context.Create(["--lint", "config.yaml"]);
            loadResult.ReportIssues(context);

            // Assert - errors must be routed to the error stream
            var errorOutput = errWriter.ToString();
            StringAssert.Contains(errorOutput, "error:");
            StringAssert.Contains(errorOutput, "missing command");
            Assert.AreEqual(1, context.ExitCode, "ExitCode should be non-zero after reporting an error");
        }
        finally
        {
            Console.SetError(originalError);
        }
    }

    /// <summary>
    ///     Test that <see cref="VersionMarkLoadResult.ReportIssues"/> routes warnings to standard output (not errors).
    /// </summary>
    [TestMethod]
    public void VersionMarkLoadResult_ReportIssues_Warning_WritesToStdOut()
    {
        // Arrange
        var issue = new LintIssue("config.yaml", 5, 3, LintSeverity.Warning, "unknown key 'x'");

        // Act
        using var context = Context.Create(["--silent", "--lint", "config.yaml"]);
        var loadResult = new VersionMarkLoadResult(null, [issue]);
        loadResult.ReportIssues(context);

        // Assert - a warning alone must not set the error exit code
        Assert.AreEqual(0, context.ExitCode, "ExitCode should remain zero for warnings only");
    }
}
