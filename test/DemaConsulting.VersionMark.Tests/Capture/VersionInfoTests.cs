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
///     Unit tests for the VersionInfo class.
/// </summary>
public class VersionInfoTests
{
    /// <summary>
    ///     Test creating a VersionInfo with constructor.
    /// </summary>
    [Fact]
    public void VersionInfo_Constructor_CreatesVersionInfo()
    {
        // Arrange
        var jobId = "job-123";
        var versions = new Dictionary<string, string>
        {
            ["dotnet"] = "8.0.0",
            ["git"] = "2.40.0"
        };

        // Act
        var versionInfo = new VersionInfo(jobId, versions);

        // Assert
        Assert.NotNull(versionInfo);
        Assert.Equal(jobId, versionInfo.JobId);
        Assert.Equal(2, versionInfo.Versions.Count);
        Assert.Equal("8.0.0", versionInfo.Versions["dotnet"]);
        Assert.Equal("2.40.0", versionInfo.Versions["git"]);
    }

    /// <summary>
    ///     Test SaveToFile creates a JSON file with expected content.
    /// </summary>
    [Fact]
    public void VersionInfo_SaveToFile_CreatesJsonFile()
    {
        // Arrange
        var tempFile = Path.GetTempFileName();
        try
        {
            var versionInfo = new VersionInfo(
                "job-456",
                new Dictionary<string, string>
                {
                    ["node"] = "18.0.0",
                    ["npm"] = "9.0.0"
                });

            // Act
            versionInfo.SaveToFile(tempFile);

            // Assert
            Assert.True(File.Exists(tempFile));

            // Verify by loading and comparing
            var loaded = VersionInfo.LoadFromFile(tempFile);
            Assert.Equal("job-456", loaded.JobId);
            Assert.Equal(2, loaded.Versions.Count);
            Assert.Equal("18.0.0", loaded.Versions["node"]);
            Assert.Equal("9.0.0", loaded.Versions["npm"]);
        }
        finally
        {
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
    }

    /// <summary>
    ///     Test LoadFromFile reads a JSON file and creates VersionInfo.
    /// </summary>
    [Fact]
    public void VersionInfo_LoadFromFile_ReadsJsonFile()
    {
        // Arrange
        var tempFile = Path.GetTempFileName();
        try
        {
            var json = @"{
  ""JobId"": ""job-789"",
  ""Versions"": {
    ""python"": ""3.11.0"",
    ""pip"": ""23.0.0""
  }
}";
            File.WriteAllText(tempFile, json);

            // Act
            var versionInfo = VersionInfo.LoadFromFile(tempFile);

            // Assert
            Assert.NotNull(versionInfo);
            Assert.Equal("job-789", versionInfo.JobId);
            Assert.Equal(2, versionInfo.Versions.Count);
            Assert.Equal("3.11.0", versionInfo.Versions["python"]);
            Assert.Equal("23.0.0", versionInfo.Versions["pip"]);
        }
        finally
        {
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
    }

    /// <summary>
    ///     Test SaveToFile and LoadFromFile round-trip preserves data.
    /// </summary>
    [Fact]
    public void VersionInfo_SaveAndLoad_RoundTripPreservesData()
    {
        // Arrange
        var tempFile = Path.GetTempFileName();
        try
        {
            var original = new VersionInfo(
                "job-roundtrip",
                new Dictionary<string, string>
                {
                    ["java"] = "17.0.0",
                    ["maven"] = "3.9.0",
                    ["gradle"] = "8.0.0"
                });

            // Act
            original.SaveToFile(tempFile);
            var loaded = VersionInfo.LoadFromFile(tempFile);

            // Assert
            Assert.Equal(original.JobId, loaded.JobId);
            Assert.Equal(original.Versions.Count, loaded.Versions.Count);
            foreach (var kvp in original.Versions)
            {
                Assert.True(loaded.Versions.TryGetValue(kvp.Key, out var value));
                Assert.Equal(kvp.Value, value);
            }
        }
        finally
        {
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
    }

    /// <summary>
    ///     Test LoadFromFile throws ArgumentException when file does not exist.
    /// </summary>
    [Fact]
    public void VersionInfo_LoadFromFile_NonExistentFile_ThrowsArgumentException()
    {
        // Arrange
        var nonExistentFile = Path.Combine(Path.GetTempPath(), "non-existent-file.json");

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => VersionInfo.LoadFromFile(nonExistentFile));
        Assert.Contains("not found", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    ///     Test LoadFromFile throws ArgumentException for invalid JSON.
    /// </summary>
    [Fact]
    public void VersionInfo_LoadFromFile_InvalidJson_ThrowsArgumentException()
    {
        // Arrange
        var tempFile = Path.GetTempFileName();
        try
        {
            File.WriteAllText(tempFile, "{ invalid json }");

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => VersionInfo.LoadFromFile(tempFile));
            Assert.Contains("parse", exception.Message, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
    }

    /// <summary>
    ///     Test LoadFromFile throws ArgumentException for empty JSON file.
    /// </summary>
    [Fact]
    public void VersionInfo_LoadFromFile_EmptyJson_ThrowsArgumentException()
    {
        // Arrange
        var tempFile = Path.GetTempFileName();
        try
        {
            File.WriteAllText(tempFile, "");

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => VersionInfo.LoadFromFile(tempFile));
            Assert.Contains("parse", exception.Message, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
    }

    /// <summary>
    ///     Test SaveToFile to invalid path throws InvalidOperationException.
    /// </summary>
    [Fact]
    public void VersionInfo_SaveToFile_InvalidPath_ThrowsInvalidOperationException()
    {
        // Arrange
        var versionInfo = new VersionInfo("job-123", []);
        var invalidPath = Path.Combine(Path.GetTempPath(), "non-existent-directory", "file.json");

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => versionInfo.SaveToFile(invalidPath));
        Assert.Contains("Failed to save", exception.Message);
    }

    /// <summary>
    ///     Test VersionInfo with empty versions dictionary.
    /// </summary>
    [Fact]
    public void VersionInfo_EmptyVersions_SavesAndLoadsCorrectly()
    {
        // Arrange
        var tempFile = Path.GetTempFileName();
        try
        {
            var original = new VersionInfo("job-empty", []);

            // Act
            original.SaveToFile(tempFile);
            var loaded = VersionInfo.LoadFromFile(tempFile);

            // Assert
            Assert.Equal(original.JobId, loaded.JobId);
            Assert.Empty(loaded.Versions);
        }
        finally
        {
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
    }

    /// <summary>
    ///     Test LoadFromFile throws ArgumentException when JSON deserializes to null (e.g., literal "null").
    /// </summary>
    [Fact]
    public void VersionInfo_LoadFromFile_NullJson_ThrowsArgumentException()
    {
        // Arrange
        var tempFile = Path.GetTempFileName();
        try
        {
            File.WriteAllText(tempFile, "null");

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => VersionInfo.LoadFromFile(tempFile));
            Assert.Contains("deserialize", exception.Message, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
    }

    /// <summary>
    ///     Test VersionInfo with special characters in values.
    /// </summary>
    [Fact]
    public void VersionInfo_SpecialCharacters_SavesAndLoadsCorrectly()
    {
        // Arrange
        var tempFile = Path.GetTempFileName();
        try
        {
            var original = new VersionInfo(
                "job-special-chars",
                new Dictionary<string, string>
                {
                    ["tool1"] = "1.0.0-beta+build.123",
                    ["tool2"] = "2.0.0 (x64)",
                    ["tool3"] = "3.0.0\nline2"
                });

            // Act
            original.SaveToFile(tempFile);
            var loaded = VersionInfo.LoadFromFile(tempFile);

            // Assert
            Assert.Equal(original.JobId, loaded.JobId);
            Assert.Equal(original.Versions.Count, loaded.Versions.Count);
            foreach (var kvp in original.Versions)
            {
                Assert.Equal(kvp.Value, loaded.Versions[kvp.Key]);
            }
        }
        finally
        {
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
    }

    /// <summary>
    ///     Test VersionInfo record creation with specific job-id and version values.
    /// </summary>
    [Fact]
    public void VersionInfo_Constructor_CreatesRecord()
    {
        // Arrange
        var jobId = "job-123";
        var versions = new Dictionary<string, string>
        {
            ["dotnet"] = "8.0.100",
            ["git"] = "2.43.0"
        };

        // Act
        var versionInfo = new VersionInfo(jobId, versions);

        // Assert
        Assert.NotNull(versionInfo);
        Assert.Equal("job-123", versionInfo.JobId);
        Assert.Equal(2, versionInfo.Versions.Count);
        Assert.Equal("8.0.100", versionInfo.Versions["dotnet"]);
        Assert.Equal("2.43.0", versionInfo.Versions["git"]);
    }
}
