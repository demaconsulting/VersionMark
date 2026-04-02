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

using DemaConsulting.VersionMark.Cli;

namespace DemaConsulting.VersionMark.Configuration;

/// <summary>
///     Severity level for a lint issue.
/// </summary>
public enum LintSeverity
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
public sealed record LintIssue(
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

/// <summary>
///     Result returned by <see cref="VersionMarkConfig.Load"/>, containing both the loaded
///     configuration (when successful) and the full list of validation issues found during loading.
/// </summary>
/// <param name="Config">
///     The loaded <see cref="VersionMarkConfig"/>, or <see langword="null"/> when one or more
///     <see cref="LintSeverity.Error"/>-severity issues prevented the configuration from being built.
/// </param>
/// <param name="Issues">
///     All validation issues found during loading. Always populated; may contain warnings even
///     when <paramref name="Config"/> is non-<see langword="null"/>.
/// </param>
public sealed record VersionMarkLoadResult(
    VersionMarkConfig? Config,
    IReadOnlyList<LintIssue> Issues)
{
    /// <summary>
    ///     Writes all validation issues to the specified context, routing errors to the error
    ///     stream and warnings to the standard output stream.
    /// </summary>
    /// <param name="context">The context used to write output.</param>
    internal void ReportIssues(Context context)
    {
        ArgumentNullException.ThrowIfNull(context);

        foreach (var issue in Issues)
        {
            if (issue.Severity == LintSeverity.Error)
            {
                context.WriteError(issue.ToString());
            }
            else
            {
                context.WriteLine(issue.ToString());
            }
        }
    }
}
