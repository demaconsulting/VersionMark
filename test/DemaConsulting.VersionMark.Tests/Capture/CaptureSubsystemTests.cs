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

namespace DemaConsulting.VersionMark.Tests.Capture;

/// <summary>
///     Subsystem tests for the Capture subsystem (version capture and persistence pipeline).
/// </summary>
[TestClass]
public class CaptureSubsystemTests
{
    /// <summary>
    ///     Test that the full capture pipeline saves and loads version data without data loss.
    /// </summary>
    [TestMethod]
    public void CaptureSubsystem_SaveAndLoad_PreservesAllVersionData()
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
    public void CaptureSubsystem_MultipleCaptures_EachFileHasDistinctJobId()
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
    public void CaptureSubsystem_LoadFromFile_NonExistentFile_ThrowsArgumentException()
    {
        // Arrange
        var nonExistentPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".json");

        // Act & Assert
        Assert.ThrowsExactly<ArgumentException>(() => VersionInfo.LoadFromFile(nonExistentPath));
    }
}
