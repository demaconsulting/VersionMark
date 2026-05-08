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

using DemaConsulting.VersionMark.Utilities;

namespace DemaConsulting.VersionMark.Tests.Utilities;

/// <summary>
///     Tests for the GlobMatcher class.
/// </summary>
public class GlobMatcherTests
{
    /// <summary>
    ///     Test that FindMatchingFiles returns an empty list when given an empty pattern array.
    /// </summary>
    [Fact]
    public void GlobMatcher_FindMatchingFiles_EmptyPatterns_ReturnsEmptyList()
    {
        // Arrange
        var patterns = Array.Empty<string>();

        // Act
        var result = GlobMatcher.FindMatchingFiles(patterns);

        // Assert
        Assert.Empty(result);
    }

    /// <summary>
    ///     Test that FindMatchingFiles returns an empty list when no files match the pattern.
    /// </summary>
    [Fact]
    public void GlobMatcher_FindMatchingFiles_PatternMatchingNoFiles_ReturnsEmptyList()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(tempDir);
        try
        {
            var pattern = Path.Combine(tempDir, "*.nonexistent");

            // Act
            var result = GlobMatcher.FindMatchingFiles([pattern]);

            // Assert
            Assert.Empty(result);
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    /// <summary>
    ///     Test that FindMatchingFiles returns matching files when given a relative pattern.
    /// </summary>
    [Fact]
    public void GlobMatcher_FindMatchingFiles_RelativePattern_ReturnsMatchingFiles()
    {
        // Arrange
        var originalDir = Environment.CurrentDirectory;
        var tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(tempDir);
        try
        {
            File.WriteAllText(Path.Combine(tempDir, "test.json"), "{}");
            Environment.CurrentDirectory = tempDir;

            // Act
            var result = GlobMatcher.FindMatchingFiles(["*.json"]);

            // Assert
            Assert.Single(result);
            Assert.Contains(result, f => f.EndsWith("test.json", StringComparison.OrdinalIgnoreCase));
        }
        finally
        {
            Environment.CurrentDirectory = originalDir;
            Directory.Delete(tempDir, recursive: true);
        }
    }

    /// <summary>
    ///     Test that FindMatchingFiles returns matching files when given an absolute pattern.
    /// </summary>
    [Fact]
    public void GlobMatcher_FindMatchingFiles_AbsolutePattern_ReturnsMatchingFiles()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(tempDir);
        try
        {
            File.WriteAllText(Path.Combine(tempDir, "a.json"), "{}");
            File.WriteAllText(Path.Combine(tempDir, "b.json"), "{}");
            var pattern = Path.Combine(tempDir, "*.json");

            // Act
            var result = GlobMatcher.FindMatchingFiles([pattern]);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, f => f.EndsWith("a.json", StringComparison.OrdinalIgnoreCase));
            Assert.Contains(result, f => f.EndsWith("b.json", StringComparison.OrdinalIgnoreCase));
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    /// <summary>
    ///     Test that FindMatchingFiles returns a single file when given an absolute path without a wildcard.
    /// </summary>
    [Fact]
    public void GlobMatcher_FindMatchingFiles_SingleFileAbsolutePath_ReturnsSingleFile()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(tempDir);
        try
        {
            var filePath = Path.Combine(tempDir, "single.json");
            File.WriteAllText(filePath, "{}");

            // Act
            var result = GlobMatcher.FindMatchingFiles([filePath]);

            // Assert
            Assert.Single(result);
            Assert.Equal(Path.GetFullPath(filePath), result[0]);
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    /// <summary>
    ///     Test that FindMatchingFiles combines results from both absolute and relative patterns.
    /// </summary>
    [Fact]
    public void GlobMatcher_FindMatchingFiles_MixedPatterns_ReturnsCombinedFiles()
    {
        // Arrange
        var originalDir = Environment.CurrentDirectory;
        var tempDir1 = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        var tempDir2 = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(tempDir1);
        Directory.CreateDirectory(tempDir2);
        try
        {
            // Absolute pattern directory
            File.WriteAllText(Path.Combine(tempDir1, "abs.json"), "{}");

            // Relative pattern directory (becomes the working directory)
            File.WriteAllText(Path.Combine(tempDir2, "rel.json"), "{}");
            Environment.CurrentDirectory = tempDir2;

            var absolutePattern = Path.Combine(tempDir1, "*.json");

            // Act
            var result = GlobMatcher.FindMatchingFiles([absolutePattern, "*.json"]);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, f => f.EndsWith("abs.json", StringComparison.OrdinalIgnoreCase));
            Assert.Contains(result, f => f.EndsWith("rel.json", StringComparison.OrdinalIgnoreCase));
        }
        finally
        {
            Environment.CurrentDirectory = originalDir;
            Directory.Delete(tempDir1, recursive: true);
            Directory.Delete(tempDir2, recursive: true);
        }
    }

    /// <summary>
    ///     Test that SplitAbsolutePattern correctly splits a pattern with a wildcard at the last
    ///     separator before the wildcard.
    /// </summary>
    [Fact]
    public void GlobMatcher_SplitAbsolutePattern_PatternWithWildcard_SplitsCorrectly()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), "subdir");
        var pattern = Path.Combine(tempDir, "*.json");

        // Act
        var (rootDir, relativePattern) = GlobMatcher.SplitAbsolutePattern(pattern);

        // Assert
        Assert.Equal(tempDir, rootDir);
        Assert.Equal("*.json", relativePattern);
    }

    /// <summary>
    ///     Test that SplitAbsolutePattern correctly splits a pattern without a wildcard at the
    ///     final separator.
    /// </summary>
    [Fact]
    public void GlobMatcher_SplitAbsolutePattern_PatternWithoutWildcard_SplitsAtLastSeparator()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), "subdir");
        var pattern = Path.Combine(tempDir, "file.json");

        // Act
        var (rootDir, relativePattern) = GlobMatcher.SplitAbsolutePattern(pattern);

        // Assert
        Assert.Equal(tempDir, rootDir);
        Assert.Equal("file.json", relativePattern);
    }

    /// <summary>
    ///     Test that SplitAbsolutePattern correctly handles a root-relative pattern using a forward slash
    ///     (e.g. /*.json), returning the platform path root as the directory and "*.json" as the relative
    ///     pattern. This covers the empty-rootDir fallback branch and runs on all platforms.
    /// </summary>
    [Fact]
    public void GlobMatcher_SplitAbsolutePattern_ForwardSlashRootPattern_SplitsToRootAndRelative()
    {
        // Arrange
        const string pattern = "/*.json";

        // The path root is platform-dependent: "/" on Unix, "\" on Windows (where a leading forward
        // slash is treated as a drive-relative absolute path rooted at the current drive's root).
        var expectedRoot = OperatingSystem.IsWindows() ? @"\" : "/";

        // Act
        var (rootDir, relativePattern) = GlobMatcher.SplitAbsolutePattern(pattern);

        // Assert
        Assert.Equal(expectedRoot, rootDir);
        Assert.Equal("*.json", relativePattern);
    }

    /// <summary>
    ///     Test that SplitAbsolutePattern correctly handles a Windows drive-root pattern like C:\*.json,
    ///     returning "C:\" as the root directory and "*.json" as the relative pattern.
    /// </summary>
    [Fact]
    public void GlobMatcher_SplitAbsolutePattern_WindowsDriveRootPattern_SplitsToDriveRootAndRelative()
    {
        // Arrange
        Assert.SkipUnless(OperatingSystem.IsWindows(), "Windows drive-root paths are only applicable on Windows");
        const string pattern = @"C:\*.json";

        // Act
        var (rootDir, relativePattern) = GlobMatcher.SplitAbsolutePattern(pattern);

        // Assert
        Assert.Equal(@"C:\", rootDir);
        Assert.Equal("*.json", relativePattern);
    }
}
