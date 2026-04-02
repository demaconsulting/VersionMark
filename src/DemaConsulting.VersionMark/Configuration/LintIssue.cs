// Copyright (c) 2025 DEMA Consulting
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

namespace DemaConsulting.VersionMark.Configuration;

/// <summary>
///     Severity level for a lint issue.
/// </summary>
internal enum LintSeverity
{
    /// <summary>
    ///     Non-fatal advisory message.
    /// </summary>
    Warning,

    /// <summary>
    ///     Fatal validation failure that prevents loading.
    /// </summary>
    Error
}

/// <summary>
///     Represents a single validation issue found while loading a configuration file.
/// </summary>
/// <param name="FilePath">Path to the file that contains the issue.</param>
/// <param name="Line">One-based line number of the issue.</param>
/// <param name="Column">One-based column number of the issue.</param>
/// <param name="Severity">Severity of the issue.</param>
/// <param name="Description">Human-readable description of the issue.</param>
internal sealed record LintIssue(
    string FilePath,
    long Line,
    long Column,
    LintSeverity Severity,
    string Description)
{
    /// <summary>
    ///     Returns a formatted string representation of the issue suitable for display to the user.
    /// </summary>
    /// <returns>
    ///     A string in the format <c>"{FilePath}({Line},{Column}): {severity}: {Description}"</c>.
    /// </returns>
    public override string ToString() =>
        $"{FilePath}({Line},{Column}): {Severity.ToString().ToLowerInvariant()}: {Description}";
}
