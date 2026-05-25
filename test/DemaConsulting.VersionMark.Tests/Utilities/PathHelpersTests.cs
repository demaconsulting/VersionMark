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
///     Tests for the PathHelpers class.
/// </summary>
public class PathHelpersTests
{
    /// <summary>
    ///     Test that SafePathCombine throws ArgumentNullException when basePath is null.
    /// </summary>
    [Fact]
    public void PathHelpers_SafePathCombine_NullBasePath_ThrowsArgumentNullException()
    {
        // Arrange
        string? basePath = null;
        var relativePath = "subfolder/file.txt";

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            PathHelpers.SafePathCombine(basePath!, relativePath));
    }

    /// <summary>
    ///     Test that SafePathCombine throws ArgumentNullException when relativePath is null.
    /// </summary>
    [Fact]
    public void PathHelpers_SafePathCombine_NullRelativePath_ThrowsArgumentNullException()
    {
        // Arrange
        var basePath = "/home/user/project";
        string? relativePath = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            PathHelpers.SafePathCombine(basePath, relativePath!));
    }

    /// <summary>
    ///     Test that SafePathCombine correctly combines valid paths.
    /// </summary>
    [Fact]
    public void PathHelpers_SafePathCombine_ValidPaths_CombinesCorrectly()
    {
        // Arrange
        var basePath = "/home/user/project";
        var relativePath = "subfolder/file.txt";

        // Act
        var result = PathHelpers.SafePathCombine(basePath, relativePath);

        // Assert
        Assert.Equal(Path.Combine(basePath, relativePath), result);
    }

    /// <summary>
    ///     Test that SafePathCombine throws ArgumentException for path traversal with double dots.
    /// </summary>
    [Fact]
    public void PathHelpers_SafePathCombine_PathTraversalWithDoubleDots_ThrowsArgumentException()
    {
        // Arrange
        var basePath = "/home/user/project";
        var relativePath = "../etc/passwd";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            PathHelpers.SafePathCombine(basePath, relativePath));
        Assert.Contains("Invalid path component", exception.Message);
    }

    /// <summary>
    ///     Test that SafePathCombine throws ArgumentException for path with double dots in middle.
    /// </summary>
    [Fact]
    public void PathHelpers_SafePathCombine_DoubleDotsInMiddle_ThrowsArgumentException()
    {
        // Arrange
        var basePath = "/home/user/project";
        var relativePath = "subfolder/../../../etc/passwd";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            PathHelpers.SafePathCombine(basePath, relativePath));
        Assert.Contains("Invalid path component", exception.Message);
    }

    /// <summary>
    ///     Test that SafePathCombine throws ArgumentException for absolute paths.
    /// </summary>
    [Fact]
    public void PathHelpers_SafePathCombine_AbsolutePath_ThrowsArgumentException()
    {
        // Arrange & Act - Test Unix absolute path
        var unixBasePath = "/home/user/project";
        var unixRelativePath = "/etc/passwd";
        var unixException = Assert.Throws<ArgumentException>(() =>
            PathHelpers.SafePathCombine(unixBasePath, unixRelativePath));

        // Assert - Verify exception is thrown for Unix absolute path
        Assert.Contains("Invalid path component", unixException.Message);

        // Test Windows absolute path (only on Windows since Windows paths may not be rooted on Unix)
        if (OperatingSystem.IsWindows())
        {
            var windowsBasePath = "C:\\Users\\project";
            var windowsRelativePath = "C:\\Windows\\System32\\file.txt";
            var windowsException = Assert.Throws<ArgumentException>(() =>
                PathHelpers.SafePathCombine(windowsBasePath, windowsRelativePath));
            Assert.Contains("Invalid path component", windowsException.Message);
        }
    }

    /// <summary>
    ///     Test that SafePathCombine correctly handles current directory reference.
    /// </summary>
    [Fact]
    public void PathHelpers_SafePathCombine_CurrentDirectoryReference_CombinesCorrectly()
    {
        // Arrange
        var basePath = "/home/user/project";
        var relativePath = "./subfolder/file.txt";

        // Act
        var result = PathHelpers.SafePathCombine(basePath, relativePath);

        // Assert
        Assert.Equal(Path.Combine(basePath, relativePath), result);
    }

    /// <summary>
    ///     Test that SafePathCombine correctly handles nested paths.
    /// </summary>
    [Fact]
    public void PathHelpers_SafePathCombine_NestedPaths_CombinesCorrectly()
    {
        // Arrange
        var basePath = "/home/user/project";
        var relativePath = "level1/level2/level3/file.txt";

        // Act
        var result = PathHelpers.SafePathCombine(basePath, relativePath);

        // Assert
        Assert.Equal(Path.Combine(basePath, relativePath), result);
    }

    /// <summary>
    ///     Test that SafePathCombine correctly handles empty relative path.
    /// </summary>
    [Fact]
    public void PathHelpers_SafePathCombine_EmptyRelativePath_ReturnsBasePath()
    {
        // Arrange
        var basePath = "/home/user/project";
        var relativePath = "";

        // Act
        var result = PathHelpers.SafePathCombine(basePath, relativePath);

        // Assert
        Assert.Equal(Path.Combine(basePath, relativePath), result);
    }

    /// <summary>
    ///     Test that SafePathCombine correctly handles paths where a directory name starts with "..".
    ///     Such names are valid and must not be rejected as false positives.
    /// </summary>
    [Fact]
    public void PathHelpers_SafePathCombine_DotDotAsNamePrefix_CombinesCorrectly()
    {
        // Arrange - "..data" is a valid directory name that starts with ".." but is not a traversal segment
        var basePath = Path.GetTempPath();
        var relativePath = "..data/file.txt";

        // Act
        var result = PathHelpers.SafePathCombine(basePath, relativePath);

        // Assert
        Assert.Equal(Path.Combine(basePath, relativePath), result);
    }

    /// <summary>
    ///     Test that SafePathCombine throws ArgumentException for a deep path traversal attempt.
    /// </summary>
    [Fact]
    public void PathHelpers_SafePathCombine_DeepPathTraversal_ThrowsArgumentException()
    {
        // Arrange - Define a base directory and an attacker-controlled traversal path
        var baseDir = AppContext.BaseDirectory;
        const string traversalPath = "../../../etc/passwd";

        // Act & Assert - The path helper should reject traversal attempts
        Assert.Throws<ArgumentException>(() =>
            PathHelpers.SafePathCombine(baseDir, traversalPath));
    }

    /// <summary>
    ///     Test that SafePathCombine correctly combines a valid multi-segment relative path.
    /// </summary>
    [Fact]
    public void PathHelpers_SafePathCombine_MultiSegmentRelativePath_ProducesExpectedPath()
    {
        // Arrange - Use the application base directory as the root
        var baseDir = AppContext.BaseDirectory;
        const string relativePath = "test-results/output.trx";

        // Act - Combine the base directory with a valid relative path
        var result = PathHelpers.SafePathCombine(baseDir, relativePath);

        // Assert - The combined path should equal the expected combined path exactly
        Assert.Equal(Path.Combine(baseDir, relativePath), result);
    }

    /// <summary>
    ///     Test that SafePathCombine correctly locates the main DLL in the base directory.
    /// </summary>
    [Fact]
    public void PathHelpers_SafePathCombine_DllInBaseDirectory_FileExists()
    {
        // Arrange
        var baseDir = AppContext.BaseDirectory;
        const string fileName = "DemaConsulting.VersionMark.dll";

        // Act
        var result = PathHelpers.SafePathCombine(baseDir, fileName);

        // Assert
        Assert.Equal(Path.Combine(baseDir, fileName), result);
        Assert.True(File.Exists(result));
    }
}
