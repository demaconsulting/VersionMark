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

using System.Text.Json;

namespace DemaConsulting.VersionMark;

/// <summary>
///     Version information record containing Job ID and tool versions.
/// </summary>
/// <param name="JobId">The Job ID for this version capture.</param>
/// <param name="Versions">The dictionary of tool names to version strings.</param>
public sealed record VersionInfo(string JobId, Dictionary<string, string> Versions)
{
    /// <summary>
    ///     Shared JSON serialization options for writing indented JSON.
    /// </summary>
    private static readonly JsonSerializerOptions s_jsonOptions = new()
    {
        WriteIndented = true
    };

    /// <summary>
    ///     Saves the VersionInfo to a JSON file.
    /// </summary>
    /// <param name="filePath">Path to the JSON file to write.</param>
    /// <exception cref="InvalidOperationException">Thrown when the file cannot be written.</exception>
    public void SaveToFile(string filePath)
    {
        try
        {
            var json = JsonSerializer.Serialize(this, s_jsonOptions);
            File.WriteAllText(filePath, json, System.Text.Encoding.UTF8);
        }
        catch (Exception ex) when (ex is not InvalidOperationException)
        {
            throw new InvalidOperationException($"Failed to save version info to file '{filePath}': {ex.Message}", ex);
        }
    }

    /// <summary>
    ///     Loads a VersionInfo from a JSON file.
    /// </summary>
    /// <param name="filePath">Path to the JSON file to read.</param>
    /// <returns>The loaded VersionInfo instance.</returns>
    /// <exception cref="ArgumentException">Thrown when the file does not exist or cannot be parsed.</exception>
    public static VersionInfo LoadFromFile(string filePath)
    {
        // Check if file exists
        if (!File.Exists(filePath))
        {
            throw new ArgumentException($"Version info file not found: {filePath}");
        }

        try
        {
            var json = File.ReadAllText(filePath, System.Text.Encoding.UTF8);
            var versionInfo = JsonSerializer.Deserialize<VersionInfo>(json);

            if (versionInfo == null)
            {
                throw new ArgumentException($"Failed to deserialize version info from file '{filePath}'");
            }

            return versionInfo;
        }
        catch (JsonException ex)
        {
            throw new ArgumentException($"Failed to parse JSON file '{filePath}': {ex.Message}", ex);
        }
        catch (Exception ex) when (ex is not ArgumentException)
        {
            throw new ArgumentException($"Failed to read version info file '{filePath}': {ex.Message}", ex);
        }
    }
}
