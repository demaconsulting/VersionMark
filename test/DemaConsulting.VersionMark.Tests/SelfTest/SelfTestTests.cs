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
///     Subsystem tests for the SelfTest subsystem (Validation and PathHelpers working together).
/// </summary>
[TestClass]
public class SelfTestTests
{
    /// <summary>
    ///     Test that PathHelpers prevents path traversal attacks within the self-test subsystem context.
    /// </summary>
    [TestMethod]
    public void SelfTest_PathHelpers_PathTraversal_ThrowsArgumentException()
    {
        // Arrange - Define a base directory and an attacker-controlled traversal path
        var baseDir = AppContext.BaseDirectory;
        const string traversalPath = "../../../etc/passwd";

        // Act & Assert - The self-test subsystem path helper should reject traversal attempts
        Assert.ThrowsExactly<ArgumentException>(() =>
            PathHelpers.SafePathCombine(baseDir, traversalPath),
            "PathHelpers should reject path traversal attempts that escape the base directory");
    }

    /// <summary>
    ///     Test that PathHelpers correctly combines valid paths within the self-test subsystem context.
    /// </summary>
    [TestMethod]
    public void SelfTest_PathHelpers_ValidRelativePath_ProducesExpectedPath()
    {
        // Arrange - Use the application base directory as the root
        var baseDir = AppContext.BaseDirectory;
        const string relativePath = "test-results/output.trx";

        // Act - Combine the base directory with a valid relative path
        var result = PathHelpers.SafePathCombine(baseDir, relativePath);

        // Assert - The combined path should be under the base directory
        Assert.IsFalse(string.IsNullOrEmpty(result),
            "Valid path combination should produce a non-empty result");
        Assert.IsTrue(result.StartsWith(baseDir, StringComparison.OrdinalIgnoreCase) ||
                      Path.GetFullPath(result).StartsWith(Path.GetFullPath(baseDir), StringComparison.OrdinalIgnoreCase),
            "Combined path should be rooted within the base directory");
    }

    /// <summary>
    ///     Test that the self-test subsystem can locate the main DLL in the base directory.
    /// </summary>
    [TestMethod]
    public void SelfTest_FindsDllInBaseDirectory()
    {
        // Arrange
        var dllPath = PathHelpers.SafePathCombine(AppContext.BaseDirectory, "DemaConsulting.VersionMark.dll");

        // Act & Assert
        Assert.IsTrue(File.Exists(dllPath));
    }

    /// <summary>
    ///     Test that the self-validation pipeline writes results to a TRX file when --results is specified.
    /// </summary>
    [TestMethod]
    public void SelfTest_Run_WithResultsFlag_WritesResultsFile()
    {
        // Arrange - Set up a TRX results file path
        var resultsFile = Path.GetTempFileName() + ".trx";
        try
        {
            using var context = Context.Create(["--validate", "--silent", "--results", resultsFile]);

            // Act - Run self-validation with --results to write TRX output
            Program.Run(context);

            // Assert - The TRX file should exist and contain XML content
            Assert.AreEqual(0, context.ExitCode);
            Assert.IsTrue(File.Exists(resultsFile),
                "Self-validation should write results to the file specified by --results");
            var content = File.ReadAllText(resultsFile);
            Assert.IsTrue(content.Contains("TestRun") || content.Contains("testsuites"),
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
    ///     Test that the self-validation pipeline writes a ## heading when --depth 2 is specified.
    ///     What is tested: The --depth argument controls the heading level in the self-validation report
    ///     What the assertions prove: Output contains "## DEMA Consulting VersionMark" with depth 2
    /// </summary>
    [TestMethod]
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
            Program.Run(context);

            // Assert - Output should contain the ## heading for depth 2
            var output = writer.ToString();
            Assert.IsTrue(output.Contains("## DEMA Consulting VersionMark"),
                "Self-validation report should use ## heading when --depth 2 is specified");
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }
}
