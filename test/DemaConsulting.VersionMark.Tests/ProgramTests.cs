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
///     Unit tests for the Program class.
/// </summary>
[TestClass]
public class ProgramTests
{
    /// <summary>
    ///     Test that Run with version flag displays version only.
    /// </summary>
    [TestMethod]
    public void Program_Run_WithVersionFlag_DisplaysVersionOnly()
    {
        var originalOut = Console.Out;
        try
        {
            using var outWriter = new StringWriter();
            Console.SetOut(outWriter);
            using var context = Context.Create(["--version"]);

            Program.Run(context);

            var output = outWriter.ToString();
            Assert.DoesNotContain("Copyright", output);
            Assert.DoesNotContain("Template DotNet Tool version", output);
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    /// <summary>
    ///     Test that Run with help flag displays usage information.
    /// </summary>
    [TestMethod]
    public void Program_Run_WithHelpFlag_DisplaysUsageInformation()
    {
        var originalOut = Console.Out;
        try
        {
            using var outWriter = new StringWriter();
            Console.SetOut(outWriter);
            using var context = Context.Create(["--help"]);

            Program.Run(context);

            var output = outWriter.ToString();
            Assert.Contains("Usage:", output);
            Assert.Contains("Options:", output);
            Assert.Contains("--version", output);
            Assert.Contains("--help", output);
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    /// <summary>
    ///     Test that Run with validate flag runs validation.
    /// </summary>
    [TestMethod]
    public void Program_Run_WithValidateFlag_RunsValidation()
    {
        var originalOut = Console.Out;
        try
        {
            using var outWriter = new StringWriter();
            Console.SetOut(outWriter);
            using var context = Context.Create(["--validate"]);

            Program.Run(context);

            var output = outWriter.ToString();
            Assert.Contains("Total Tests:", output);
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    /// <summary>
    ///     Test that Run with no arguments displays default behavior.
    /// </summary>
    [TestMethod]
    public void Program_Run_NoArguments_DisplaysDefaultBehavior()
    {
        var originalOut = Console.Out;
        try
        {
            using var outWriter = new StringWriter();
            Console.SetOut(outWriter);
            using var context = Context.Create([]);

            Program.Run(context);

            var output = outWriter.ToString();
            Assert.Contains("Template DotNet Tool version", output);
            Assert.Contains("Copyright", output);
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    /// <summary>
    ///     Test that version property returns non-empty version string.
    /// </summary>
    [TestMethod]
    public void Program_Version_ReturnsNonEmptyString()
    {
        var version = Program.Version;
        Assert.IsFalse(string.IsNullOrWhiteSpace(version));
    }

    /// <summary>
    ///     Test that Run with capture command captures tool versions.
    /// </summary>
    [TestMethod]
    public void Program_Run_WithCaptureCommand_CapturesToolVersions()
    {
        // Arrange - Set up unique temp directory with config file and redirect console output
        var currentDir = Directory.GetCurrentDirectory();
        var tempDir = PathHelpers.SafePathCombine(Path.GetTempPath(), Path.GetRandomFileName());
        var tempConfigFile = PathHelpers.SafePathCombine(tempDir, ".versionmark.yaml");
        var outputFile = PathHelpers.SafePathCombine(tempDir, "output.json");

        try
        {
            // Create unique temp directory for this test
            Directory.CreateDirectory(tempDir);

            // Create a temporary config file in temp directory
            File.WriteAllText(tempConfigFile, @"
tools:
  dotnet:
    command: dotnet --version
    regex: '(?<version>\d+\.\d+\.\d+)'
");

            // Change to temp directory so it finds the config
            Directory.SetCurrentDirectory(tempDir);

            var originalOut = Console.Out;
            try
            {
                using var outWriter = new StringWriter();
                Console.SetOut(outWriter);
                using var context = Context.Create([
                    "--capture",
                    "--job-id", "test-job",
                    "--output", outputFile,
                    "--", "dotnet"
                ]);

                // Act - Execute the capture command via Program.Run
                Program.Run(context);

                // Assert - Verify the command output, exit code, and captured data
                var output = outWriter.ToString();
                Assert.Contains("Capturing tool versions", output);
                Assert.Contains("test-job", output);
                Assert.Contains("dotnet", output);
                Assert.AreEqual(0, context.ExitCode);

                // Verify output file was created
                Assert.IsTrue(File.Exists(outputFile), "Output file was not created");

                // Verify output file contains expected data
                var versionInfo = VersionInfo.LoadFromFile(outputFile);
                Assert.AreEqual("test-job", versionInfo.JobId);
                Assert.IsTrue(versionInfo.Versions.ContainsKey("dotnet"));
            }
            finally
            {
                Console.SetOut(originalOut);
            }
        }
        finally
        {
            // Restore original directory
            Directory.SetCurrentDirectory(currentDir);

            // Clean up temp directory and all its contents
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, recursive: true);
            }
        }
    }

    /// <summary>
    ///     Test that Run with capture command without job ID fails.
    /// </summary>
    [TestMethod]
    public void Program_Run_WithCaptureCommandWithoutJobId_ReturnsError()
    {
        // Arrange - Set up context with capture flag but no job-id
        var originalOut = Console.Out;
        try
        {
            using var outWriter = new StringWriter();
            Console.SetOut(outWriter);
            using var context = Context.Create(["--capture"]);

            // Act - Run the program with incomplete capture arguments
            Program.Run(context);

            // Assert - Verify error message and non-zero exit code
            var output = outWriter.ToString();
            Assert.Contains("--job-id is required", output);
            Assert.AreEqual(1, context.ExitCode);
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    /// <summary>
    ///     Test that Run with capture command with missing config file fails.
    /// </summary>
    [TestMethod]
    public void Program_Run_WithCaptureCommandWithMissingConfig_ReturnsError()
    {
        // Arrange - Set up context in current directory (no .versionmark.yaml expected here)
        var originalOut = Console.Out;
        try
        {
            using var outWriter = new StringWriter();
            Console.SetOut(outWriter);
            using var context = Context.Create([
                "--capture",
                "--job-id", "test-job"
            ]);

            // Act - Run capture command without available config file
            Program.Run(context);

            // Assert - Verify error is reported and exit code indicates failure
            var output = outWriter.ToString();
            Assert.Contains("Error:", output);
            Assert.AreEqual(1, context.ExitCode);
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }
}
