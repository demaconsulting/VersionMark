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
public class CliSubsystemTests
{
    /// <summary>
    ///     Test that the full CLI pipeline with --version flag exits cleanly.
    /// </summary>
    [TestMethod]
    public void CliSubsystem_Run_VersionFlag_ExitsCleanly()
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
    public void CliSubsystem_Run_SilentWithVersionFlag_SuppressesOutput()
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
}
