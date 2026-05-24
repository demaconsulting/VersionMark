### GlobMatcher

#### Verification Approach

The `GlobMatcher` unit provides `FindMatchingFiles` and `SplitAbsolutePattern` methods
for glob-pattern file matching. It supports relative and absolute patterns and returns a
sorted, deduplicated list of full paths. Tests are in `Utilities/GlobMatcherTests.cs` and
call `GlobMatcher` methods directly. Tests use temporary directories created with
`Path.GetTempPath()` for all file-system scenarios. No external mocks are required.

#### Test Environment

N/A - standard test environment. Tests create temporary directories during setup and clean
them up afterwards. No additional environment configuration is required.

#### Acceptance Criteria

- All unit tests for `GlobMatcher` pass with zero failures across all supported OS and
  .NET version matrix combinations (Windows, Linux, macOS x .NET 8, .NET 9, .NET 10).
- Every requirement for the `GlobMatcher` unit is covered by at least one named test
  scenario.

#### Test Scenarios

**GlobMatcher_FindMatchingFiles_EmptyPatterns_ReturnsEmptyList**: An empty pattern array
returns an empty list. This scenario is tested by
`GlobMatcher_FindMatchingFiles_EmptyPatterns_ReturnsEmptyList`.

**GlobMatcher_FindMatchingFiles_PatternMatchingNoFiles_ReturnsEmptyList**: A pattern that
matches no files returns an empty list. This scenario is tested by
`GlobMatcher_FindMatchingFiles_PatternMatchingNoFiles_ReturnsEmptyList`.

**GlobMatcher_FindMatchingFiles_RelativePattern_ReturnsMatchingFiles**: A relative glob
pattern is matched against the current directory. This scenario is tested by
`GlobMatcher_FindMatchingFiles_RelativePattern_ReturnsMatchingFiles`.

**GlobMatcher_FindMatchingFiles_AbsolutePattern_ReturnsMatchingFiles**: An absolute glob
pattern is matched from its root directory. This scenario is tested by
`GlobMatcher_FindMatchingFiles_AbsolutePattern_ReturnsMatchingFiles`.

**GlobMatcher_FindMatchingFiles_SingleFileAbsolutePath_ReturnsSingleFile**: An absolute
path with no wildcard returns that single file. This scenario is tested by
`GlobMatcher_FindMatchingFiles_SingleFileAbsolutePath_ReturnsSingleFile`.

**GlobMatcher_FindMatchingFiles_MixedPatterns_ReturnsCombinedFiles**: Mixed absolute and
relative patterns produce a combined deduplicated result. This scenario is tested by
`GlobMatcher_FindMatchingFiles_MixedPatterns_ReturnsCombinedFiles`.

**GlobMatcher_SplitAbsolutePattern_PatternWithWildcard_SplitsCorrectly**: A pattern with a
wildcard is split at the last path separator before the wildcard. This scenario is tested
by `GlobMatcher_SplitAbsolutePattern_PatternWithWildcard_SplitsCorrectly`.

**GlobMatcher_SplitAbsolutePattern_PatternWithoutWildcard_SplitsAtLastSeparator**: A
pattern without a wildcard is split at the final path separator. This scenario is tested by
`GlobMatcher_SplitAbsolutePattern_PatternWithoutWildcard_SplitsAtLastSeparator`.

**GlobMatcher_SplitAbsolutePattern_ForwardSlashRootPattern_SplitsToRootAndRelative**: A
root-relative forward-slash pattern (e.g. `/*.json`) splits to the platform path root
(`/` on Unix, `\` on Windows) and the relative pattern on all platforms. This scenario
is tested by
`GlobMatcher_SplitAbsolutePattern_ForwardSlashRootPattern_SplitsToRootAndRelative`.

**GlobMatcher_SplitAbsolutePattern_WindowsDriveRootPattern_SplitsToDriveRootAndRelative**:
A Windows drive-root pattern (e.g. `C:\*.json`) splits to the `C:\` root and the
relative pattern (Windows only). This scenario is tested by
`GlobMatcher_SplitAbsolutePattern_WindowsDriveRootPattern_SplitsToDriveRootAndRelative`.
