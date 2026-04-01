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

using DemaConsulting.VersionMark.SelfTest;

namespace DemaConsulting.VersionMark.Tests.SelfTest;

/// <summary>
///     Subsystem tests for the SelfTest subsystem (Validation and PathHelpers working together).
/// </summary>
[TestClass]
public class SelfTestSubsystemTests
{
    /// <summary>
    ///     Test that PathHelpers prevents path traversal attacks within the self-test subsystem context.
    /// </summary>
    [TestMethod]
    public void SelfTestSubsystem_PathHelpers_PathTraversal_ThrowsArgumentException()
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
    public void SelfTestSubsystem_PathHelpers_ValidRelativePath_ProducesExpectedPath()
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
}
