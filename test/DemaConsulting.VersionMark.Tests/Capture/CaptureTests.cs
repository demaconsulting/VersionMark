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

using DemaConsulting.VersionMark.Capture;
using DemaConsulting.VersionMark.Cli;
using DemaConsulting.VersionMark.Configuration;
using DemaConsulting.VersionMark.SelfTest;

namespace DemaConsulting.VersionMark.Tests.Capture;

/// <summary>
///     Subsystem tests for the Capture subsystem (version capture and persistence pipeline).
/// </summary>
[TestClass]
public class CaptureTests
{
    /// <summary>
    ///     Test that the full capture pipeline saves and loads version data without data loss.
    /// </summary>
    [TestMethod]
    public void Capture_SaveAndLoad_PreservesAllVersionData()
    {
        // Arrange - Create version info representing a complete capture result
        var tempFile = Path.GetTempFileName();
        try
        {
            var originalVersionInfo = new VersionInfo(
                "ci-job-42",
                new Dictionary<string, string>
                {
                    ["dotnet"] = "8.0.100",
                    ["git"] = "2.43.0",
                    ["node"] = "20.11.0"
                });

            // Act - Execute the full capture persistence pipeline (save then load)
            originalVersionInfo.SaveToFile(tempFile);
            var loadedVersionInfo = VersionInfo.LoadFromFile(tempFile);

            // Assert - All version data should survive the save/load cycle
            Assert.IsNotNull(loadedVersionInfo);
            Assert.AreEqual(originalVersionInfo.JobId, loadedVersionInfo.JobId,
                "Job ID should be preserved through the capture pipeline");
            Assert.HasCount(3, loadedVersionInfo.Versions);
            Assert.AreEqual("8.0.100", loadedVersionInfo.Versions["dotnet"]);
            Assert.AreEqual("2.43.0", loadedVersionInfo.Versions["git"]);
            Assert.AreEqual("20.11.0", loadedVersionInfo.Versions["node"]);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    /// <summary>
    ///     Test that the capture subsystem correctly handles multiple capture files from the same job.
    /// </summary>
    [TestMethod]
    public void Capture_MultipleCaptures_EachFileHasDistinctJobId()
    {
        // Arrange - Create two capture files representing different CI jobs
        var tempFile1 = Path.GetTempFileName();
        var tempFile2 = Path.GetTempFileName();
        try
        {
            var capture1 = new VersionInfo("job-build-linux",
                new Dictionary<string, string> { ["dotnet"] = "8.0.100" });
            var capture2 = new VersionInfo("job-build-windows",
                new Dictionary<string, string> { ["dotnet"] = "8.0.100" });

            // Act - Save both captures and reload them
            capture1.SaveToFile(tempFile1);
            capture2.SaveToFile(tempFile2);
            var loaded1 = VersionInfo.LoadFromFile(tempFile1);
            var loaded2 = VersionInfo.LoadFromFile(tempFile2);

            // Assert - Each file should have its own distinct job ID
            Assert.AreEqual("job-build-linux", loaded1.JobId);
            Assert.AreEqual("job-build-windows", loaded2.JobId);
            Assert.AreNotEqual(loaded1.JobId, loaded2.JobId,
                "Different capture jobs should have distinct job IDs");
        }
        finally
        {
            File.Delete(tempFile1);
            File.Delete(tempFile2);
        }
    }

    /// <summary>
    ///     Test that loading a version info file that does not exist throws an ArgumentException.
    /// </summary>
    [TestMethod]
    public void Capture_LoadFromFile_NonExistentFile_ThrowsArgumentException()
    {
        // Arrange
        var nonExistentPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".json");

        // Act & Assert
        Assert.ThrowsExactly<ArgumentException>(() => VersionInfo.LoadFromFile(nonExistentPath));
    }

    /// <summary>
    ///     Test that Context correctly sets the capture mode flag when --capture is specified.
    /// </summary>
    [TestMethod]
    public void Capture_Context_CaptureFlag_SetsCaptureMode()
    {
        // Arrange & Act - Create a context with --capture and required --job-id
        using var context = Context.Create(["--capture", "--job-id", "test-job"]);

        // Assert - The capture flag should be set
        Assert.IsTrue(context.Capture,
            "Context should indicate capture mode when --capture flag is specified");
    }

    /// <summary>
    ///     Test that Context correctly stores the job ID from --job-id parameter.
    /// </summary>
    [TestMethod]
    public void Capture_Context_WithJobId_SetsJobId()
    {
        // Arrange & Act - Create a context with --capture and a specific job ID
        using var context = Context.Create(["--capture", "--job-id", "my-build-job"]);

        // Assert - The job ID should be stored on the context
        Assert.AreEqual("my-build-job", context.JobId,
            "Context should store the job ID specified via --job-id");
    }

    /// <summary>
    ///     Test that when --output is not specified, the default filename includes the job ID.
    /// </summary>
    [TestMethod]
    public void Capture_Run_NoOutputFlagSpecified_UsesDefaultFilename()
    {
        // Arrange - Set up temp directory with config; run without --output so default filename is used
        var currentDir = Directory.GetCurrentDirectory();
        var tempDir = PathHelpers.SafePathCombine(Path.GetTempPath(), Path.GetRandomFileName());
        try
        {
            Directory.CreateDirectory(tempDir);
            File.WriteAllText(
                PathHelpers.SafePathCombine(tempDir, ".versionmark.yaml"),
                """
                tools:
                  dotnet:
                    command: dotnet --version
                    regex: '(?<version>\d+\.\d+\.\d+)'
                """);
            Directory.SetCurrentDirectory(tempDir);

            using var context = Context.Create(["--capture", "--job-id", "default-job", "--silent"]);

            // Act - Run capture without specifying --output
            Program.Run(context);

            // Assert - The default output file versionmark-<job-id>.json should exist
            var defaultFile = PathHelpers.SafePathCombine(tempDir, "versionmark-default-job.json");
            Assert.IsTrue(File.Exists(defaultFile),
                "Default output file 'versionmark-<job-id>.json' should be created when --output is not specified");
        }
        finally
        {
            Directory.SetCurrentDirectory(currentDir);
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, recursive: true);
            }
        }
    }

    /// <summary>
    ///     Test that Context correctly stores tool names from the -- separator.
    /// </summary>
    [TestMethod]
    public void Capture_Context_WithToolFilter_SetsToolNames()
    {
        // Arrange & Act - Create a context with --capture and tool names after --
        using var context = Context.Create(["--capture", "--job-id", "x", "--", "dotnet", "git"]);

        // Assert - The tool names should be stored
        Assert.AreEqual(2, context.ToolNames.Length,
            "Context should store tool names specified after the -- separator");
        Assert.IsTrue(context.ToolNames.Contains("dotnet"));
        Assert.IsTrue(context.ToolNames.Contains("git"));
    }

    /// <summary>
    ///     Test that capture without a tool filter captures all tools defined in configuration.
    /// </summary>
    [TestMethod]
    public void Capture_Run_NoToolFilter_CapturesAllConfiguredTools()
    {
        // Arrange - Set up temp directory with a two-tool config; no tool filter specified
        var currentDir = Directory.GetCurrentDirectory();
        var tempDir = PathHelpers.SafePathCombine(Path.GetTempPath(), Path.GetRandomFileName());
        var outputFile = PathHelpers.SafePathCombine(tempDir, "output.json");
        try
        {
            Directory.CreateDirectory(tempDir);
            File.WriteAllText(
                PathHelpers.SafePathCombine(tempDir, ".versionmark.yaml"),
                """
                tools:
                  dotnet:
                    command: dotnet --version
                    regex: '(?<version>\d+\.\d+\.\d+)'
                  git:
                    command: git --version
                    regex: 'git version (?<version>[\d\.]+)'
                """);
            Directory.SetCurrentDirectory(tempDir);

            using var context = Context.Create([
                "--capture", "--job-id", "all-tools-job", "--output", outputFile, "--silent"
            ]);

            // Act - Run capture without any tool filter
            Program.Run(context);

            // Assert - Both tools should appear in the saved output
            Assert.AreEqual(0, context.ExitCode);
            var versionInfo = VersionInfo.LoadFromFile(outputFile);
            Assert.IsTrue(versionInfo.Versions.ContainsKey("dotnet"),
                "All configured tools should be captured when no tool filter is specified");
            Assert.IsTrue(versionInfo.Versions.ContainsKey("git"),
                "All configured tools should be captured when no tool filter is specified");
        }
        finally
        {
            Directory.SetCurrentDirectory(currentDir);
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, recursive: true);
            }
        }
    }

    /// <summary>
    ///     Test that VersionMarkConfig.ReadFromFile correctly loads tool definitions from a YAML file.
    /// </summary>
    [TestMethod]
    public void Capture_Config_ReadFromFile_LoadsToolDefinitions()
    {
        // Arrange - Write a .versionmark.yaml file to a temp path
        var tempFile = Path.GetTempFileName() + ".yaml";
        try
        {
            File.WriteAllText(tempFile, """
                tools:
                  dotnet:
                    command: dotnet --version
                    regex: '(?<version>\d+\.\d+\.\d+)'
                  git:
                    command: git --version
                    regex: 'git version (?<version>[\d\.]+)'
                """);

            // Act - Read the configuration from the file (simulates reading .versionmark.yaml)
            var config = VersionMarkConfig.ReadFromFile(tempFile);

            // Assert - All tool definitions should be loaded
            Assert.IsNotNull(config);
            Assert.IsTrue(config.Tools.ContainsKey("dotnet"),
                "ReadFromFile should load all tools from the configuration file");
            Assert.IsTrue(config.Tools.ContainsKey("git"),
                "ReadFromFile should load all tools from the configuration file");
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    /// <summary>
    ///     Test that FindVersions executes the configured command and extracts the version via regex.
    /// </summary>
    [TestMethod]
    public void Capture_FindVersions_ExecutesCommandAndExtractsVersion()
    {
        // Arrange - Create a configuration for dotnet (always available in the build environment)
        var tempFile = Path.GetTempFileName() + ".yaml";
        try
        {
            File.WriteAllText(tempFile, """
                tools:
                  dotnet:
                    command: dotnet --version
                    regex: '(?<version>\d+\.\d+\.\d+)'
                """);
            var config = VersionMarkConfig.ReadFromFile(tempFile);

            // Act - Run the capture pipeline for the dotnet tool
            var versionInfo = config.FindVersions(["dotnet"], "test-capture-job");

            // Assert - A version string should have been extracted
            Assert.IsNotNull(versionInfo);
            Assert.IsTrue(versionInfo.Versions.ContainsKey("dotnet"),
                "FindVersions should capture the dotnet version");
            Assert.IsFalse(string.IsNullOrEmpty(versionInfo.Versions["dotnet"]),
                "Captured version should be a non-empty string");
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    /// <summary>
    ///     Test that the capture pipeline displays captured tool versions to the user.
    /// </summary>
    [TestMethod]
    public void Capture_Run_DisplaysCapturedVersionsAfterCapture()
    {
        // Arrange - Set up temp directory with config and redirect console output to capture it
        var currentDir = Directory.GetCurrentDirectory();
        var tempDir = PathHelpers.SafePathCombine(Path.GetTempPath(), Path.GetRandomFileName());
        var outputFile = PathHelpers.SafePathCombine(tempDir, "output.json");
        try
        {
            Directory.CreateDirectory(tempDir);
            File.WriteAllText(
                PathHelpers.SafePathCombine(tempDir, ".versionmark.yaml"),
                """
                tools:
                  dotnet:
                    command: dotnet --version
                    regex: '(?<version>\d+\.\d+\.\d+)'
                """);
            Directory.SetCurrentDirectory(tempDir);

            var originalOut = Console.Out;
            try
            {
                using var outWriter = new StringWriter();
                Console.SetOut(outWriter);
                using var context = Context.Create([
                    "--capture", "--job-id", "display-job", "--output", outputFile
                ]);

                // Act - Run the full capture pipeline
                Program.Run(context);

                // Assert - Tool names and versions should appear in the output
                var output = outWriter.ToString();
                Assert.AreEqual(0, context.ExitCode);
                Assert.IsTrue(output.Contains("dotnet"),
                    "Capture output should display captured tool names to the user");
            }
            finally
            {
                Console.SetOut(originalOut);
            }
        }
        finally
        {
            Directory.SetCurrentDirectory(currentDir);
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, recursive: true);
            }
        }
    }
}
