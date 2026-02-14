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
///     Unit tests for the Context class.
/// </summary>
[TestClass]
public class ContextTests
{
    /// <summary>
    ///     Test creating a context with no arguments.
    /// </summary>
    [TestMethod]
    public void Context_Create_NoArguments_ReturnsDefaultContext()
    {
        // Arrange & Act - Create context with no arguments
        using var context = Context.Create([]);

        // Assert - Verify default context state
        Assert.IsFalse(context.Version);
        Assert.IsFalse(context.Help);
        Assert.IsFalse(context.Silent);
        Assert.IsFalse(context.Validate);
        Assert.AreEqual(0, context.ExitCode);
    }

    /// <summary>
    ///     Test creating a context with the version flag.
    /// </summary>
    [TestMethod]
    public void Context_Create_VersionFlag_SetsVersionTrue()
    {
        // Arrange & Act - Create context with --version flag
        using var context = Context.Create(["--version"]);

        // Assert - Verify version flag is set
        Assert.IsTrue(context.Version);
        Assert.IsFalse(context.Help);
        Assert.AreEqual(0, context.ExitCode);
    }

    /// <summary>
    ///     Test creating a context with the short version flag.
    /// </summary>
    [TestMethod]
    public void Context_Create_ShortVersionFlag_SetsVersionTrue()
    {
        // Arrange & Act - Create context with -v flag
        using var context = Context.Create(["-v"]);

        // Assert - Verify version flag is set
        Assert.IsTrue(context.Version);
        Assert.IsFalse(context.Help);
        Assert.AreEqual(0, context.ExitCode);
    }

    /// <summary>
    ///     Test creating a context with the help flag.
    /// </summary>
    [TestMethod]
    public void Context_Create_HelpFlag_SetsHelpTrue()
    {
        // Arrange & Act - Create context with --help flag
        using var context = Context.Create(["--help"]);

        // Assert - Verify help flag is set
        Assert.IsFalse(context.Version);
        Assert.IsTrue(context.Help);
        Assert.AreEqual(0, context.ExitCode);
    }

    /// <summary>
    ///     Test creating a context with the short help flag -h.
    /// </summary>
    [TestMethod]
    public void Context_Create_ShortHelpFlag_H_SetsHelpTrue()
    {
        // Arrange & Act - Create context with -h flag
        using var context = Context.Create(["-h"]);

        // Assert - Verify help flag is set
        Assert.IsFalse(context.Version);
        Assert.IsTrue(context.Help);
        Assert.AreEqual(0, context.ExitCode);
    }

    /// <summary>
    ///     Test creating a context with the short help flag -?.
    /// </summary>
    [TestMethod]
    public void Context_Create_ShortHelpFlag_Question_SetsHelpTrue()
    {
        // Arrange & Act - Create context with -? flag
        using var context = Context.Create(["-?"]);

        // Assert - Verify help flag is set
        Assert.IsFalse(context.Version);
        Assert.IsTrue(context.Help);
        Assert.AreEqual(0, context.ExitCode);
    }

    /// <summary>
    ///     Test creating a context with the silent flag.
    /// </summary>
    [TestMethod]
    public void Context_Create_SilentFlag_SetsSilentTrue()
    {
        // Arrange & Act - Create context with --silent flag
        using var context = Context.Create(["--silent"]);

        // Assert - Verify silent flag is set
        Assert.IsTrue(context.Silent);
        Assert.AreEqual(0, context.ExitCode);
    }

    /// <summary>
    ///     Test creating a context with the validate flag.
    /// </summary>
    [TestMethod]
    public void Context_Create_ValidateFlag_SetsValidateTrue()
    {
        // Arrange & Act - Create context with --validate flag
        using var context = Context.Create(["--validate"]);

        // Assert - Verify validate flag is set
        Assert.IsTrue(context.Validate);
        Assert.AreEqual(0, context.ExitCode);
    }

    /// <summary>
    ///     Test creating a context with the results flag.
    /// </summary>
    [TestMethod]
    public void Context_Create_ResultsFlag_SetsResultsFile()
    {
        // Arrange & Act - Create context with --results flag
        using var context = Context.Create(["--results", "test.trx"]);

        // Assert - Verify results file is set
        Assert.AreEqual("test.trx", context.ResultsFile);
        Assert.AreEqual(0, context.ExitCode);
    }

    /// <summary>
    ///     Test creating a context with the log flag.
    /// </summary>
    [TestMethod]
    public void Context_Create_LogFlag_OpensLogFile()
    {
        var logFile = Path.GetTempFileName();
        try
        {
            using (var context = Context.Create(["--log", logFile]))
            {
                context.WriteLine("Test message");
                Assert.AreEqual(0, context.ExitCode);
            }

            // Verify log file was written
            Assert.IsTrue(File.Exists(logFile));
            var logContent = File.ReadAllText(logFile);
            Assert.Contains("Test message", logContent);
        }
        finally
        {
            if (File.Exists(logFile))
            {
                File.Delete(logFile);
            }
        }
    }

    /// <summary>
    ///     Test creating a context with an unknown argument throws exception.
    /// </summary>
    [TestMethod]
    public void Context_Create_UnknownArgument_ThrowsArgumentException()
    {
        // Arrange & Act - Create context with unknown argument
        var exception = Assert.Throws<ArgumentException>(() => Context.Create(["--unknown"]));

        // Assert - Verify exception is thrown with correct message
        Assert.Contains("Unsupported argument", exception.Message);
    }

    /// <summary>
    ///     Test WriteLine writes to console output when not silent.
    /// </summary>
    [TestMethod]
    public void Context_WriteLine_NotSilent_WritesToConsole()
    {
        // Arrange - Redirect console output
        var originalOut = Console.Out;
        try
        {
            using var outWriter = new StringWriter();
            Console.SetOut(outWriter);

            // Act - Create context and write message
            using var context = Context.Create([]);
            context.WriteLine("Test message");

            // Assert - Verify message was written to console
            var output = outWriter.ToString();
            Assert.Contains("Test message", output);
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    /// <summary>
    ///     Test WriteLine does not write to console when silent.
    /// </summary>
    [TestMethod]
    public void Context_WriteLine_Silent_DoesNotWriteToConsole()
    {
        // Arrange - Redirect console output
        var originalOut = Console.Out;
        try
        {
            using var outWriter = new StringWriter();
            Console.SetOut(outWriter);

            // Act - Create silent context and write message
            using var context = Context.Create(["--silent"]);
            context.WriteLine("Test message");

            // Assert - Verify message was not written to console
            var output = outWriter.ToString();
            Assert.DoesNotContain("Test message", output);
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    /// <summary>
    ///     Test creating a context with the publish flag.
    ///     What is tested: --publish flag parsing sets Publish property to true
    ///     What the assertions prove: The Publish flag is correctly parsed and stored
    /// </summary>
    [TestMethod]
    public void Context_Create_PublishFlag_SetsPublishTrue()
    {
        // Arrange & Act - Create context with --publish and --report flags
        using var context = Context.Create(["--publish", "--report", "output.md"]);

        // Assert - Verify publish mode is enabled
        // What is proved: --publish flag is correctly recognized and sets Publish property
        Assert.IsTrue(context.Publish);
        Assert.AreEqual(0, context.ExitCode);
    }

    /// <summary>
    ///     Test creating a context with the report parameter.
    ///     What is tested: --report parameter parsing captures the output file path
    ///     What the assertions prove: The report file path is correctly parsed and stored
    /// </summary>
    [TestMethod]
    public void Context_Create_ReportParameter_SetsReportFile()
    {
        // Arrange & Act - Create context with --publish and --report flags
        using var context = Context.Create(["--publish", "--report", "output.md"]);

        // Assert - Verify report file path is captured
        // What is proved: --report parameter value is correctly captured
        Assert.AreEqual("output.md", context.ReportFile);
        Assert.AreEqual(0, context.ExitCode);
    }

    /// <summary>
    ///     Test creating a context with the report-depth parameter.
    ///     What is tested: --report-depth parameter parsing captures the depth value
    ///     What the assertions prove: The report depth is correctly parsed as an integer
    /// </summary>
    [TestMethod]
    public void Context_Create_ReportDepthParameter_SetsReportDepth()
    {
        // Arrange & Act - Create context with --publish, --report, and --report-depth flags
        using var context = Context.Create(["--publish", "--report", "output.md", "--report-depth", "3"]);

        // Assert - Verify report depth is captured
        // What is proved: --report-depth parameter value is correctly parsed as an integer
        Assert.AreEqual(3, context.ReportDepth);
        Assert.AreEqual(0, context.ExitCode);
    }

    /// <summary>
    ///     Test creating a context with default report-depth.
    ///     What is tested: Default report-depth value when not specified
    ///     What the assertions prove: The default report depth is 2
    /// </summary>
    [TestMethod]
    public void Context_Create_NoReportDepth_DefaultsToTwo()
    {
        // Arrange & Act - Create context without --report-depth
        using var context = Context.Create(["--publish", "--report", "output.md"]);

        // Assert - Verify default report depth is 2
        // What is proved: Report depth defaults to 2 when not specified
        Assert.AreEqual(2, context.ReportDepth);
    }

    /// <summary>
    ///     Test creating a context with glob patterns after -- separator.
    ///     What is tested: Glob patterns after -- are captured in GlobPatterns array
    ///     What the assertions prove: Multiple glob patterns are correctly parsed and stored
    /// </summary>
    [TestMethod]
    public void Context_Create_GlobPatternsAfterSeparator_CapturesPatterns()
    {
        // Arrange & Act - Create context with glob patterns after --
        using var context = Context.Create([
            "--publish",
            "--report", "output.md",
            "--",
            "versionmark-*.json",
            "results/*.json"
        ]);

        // Assert - Verify glob patterns are captured
        // What is proved: Arguments after -- are correctly captured in GlobPatterns array
        Assert.HasCount(2, context.GlobPatterns);
        Assert.AreEqual("versionmark-*.json", context.GlobPatterns[0]);
        Assert.AreEqual("results/*.json", context.GlobPatterns[1]);
    }

    /// <summary>
    ///     Test that publish flag is set without --report parameter (validation happens in Program.Run).
    ///     What is tested: --publish flag can be parsed without --report in Context.Create
    ///     What the assertions prove: Context parsing accepts --publish without --report (error checked later)
    /// </summary>
    [TestMethod]
    public void Context_Create_PublishWithoutReport_ParsesSuccessfully()
    {
        // Arrange & Act - Create context with --publish but no --report
        using var context = Context.Create(["--publish"]);

        // Assert - Verify publish mode is enabled (validation happens in Program.Run)
        // What is proved: --publish flag is parsed successfully without --report
        Assert.IsTrue(context.Publish);
        Assert.IsNull(context.ReportFile);
        Assert.AreEqual(0, context.ExitCode);
    }

    /// <summary>
    ///     Test that glob patterns is empty when not specified (default applied in Program.RunPublish).
    ///     What is tested: GlobPatterns array when none provided after --
    ///     What the assertions prove: GlobPatterns is empty array when not specified
    /// </summary>
    [TestMethod]
    public void Context_Create_NoGlobPatterns_EmptyArray()
    {
        // Arrange & Act - Create context without glob patterns
        using var context = Context.Create(["--publish", "--report", "output.md"]);

        // Assert - Verify glob patterns array is empty (default applied in Program.RunPublish)
        // What is proved: When no glob patterns specified, GlobPatterns is an empty array
        Assert.HasCount(0, context.GlobPatterns);
    }
}
