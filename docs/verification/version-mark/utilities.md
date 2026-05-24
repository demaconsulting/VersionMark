## Utilities

### Verification Approach

The Utilities subsystem provides general-purpose helper classes for use within VersionMark.
It consists of two units: `GlobMatcher` (the glob-pattern file matcher) and
`PathHelpers` (the safe path combination utility). Unit tests invoke `GlobMatcher` and
`PathHelpers` directly with various inputs and assert on the returned results. Tests use
temporary directories for file-system scenarios, ensuring isolation and repeatability
across platforms. No external mocks are required.

### Test Environment

N/A - standard test environment. Tests create temporary directories during setup and clean
them up afterwards. No additional environment configuration is required.

### Acceptance Criteria

- All subsystem tests pass with zero failures across all supported OS and .NET version
  matrix combinations (Windows, Linux, macOS x .NET 8, .NET 9, .NET 10).
- Every requirement for the Utilities subsystem is covered by at least one named test
  scenario.

### Test Scenarios

**GlobMatcher_FindMatchingFiles_RelativePattern_ReturnsMatchingFiles**: A relative glob
pattern matches files in the current directory. This scenario is tested by
`GlobMatcher_FindMatchingFiles_RelativePattern_ReturnsMatchingFiles`.

**GlobMatcher_FindMatchingFiles_AbsolutePattern_ReturnsMatchingFiles**: An absolute glob
pattern matches files regardless of the working directory. This scenario is tested by
`GlobMatcher_FindMatchingFiles_AbsolutePattern_ReturnsMatchingFiles`.

**GlobMatcher_FindMatchingFiles_SingleFileAbsolutePath_ReturnsSingleFile**: An absolute
path without a wildcard returns that single file. This scenario is tested by
`GlobMatcher_FindMatchingFiles_SingleFileAbsolutePath_ReturnsSingleFile`.

**GlobMatcher_FindMatchingFiles_MixedPatterns_ReturnsCombinedFiles**: Mixed absolute and
relative patterns produce a combined deduplicated result. This scenario is tested by
`GlobMatcher_FindMatchingFiles_MixedPatterns_ReturnsCombinedFiles`.

**GlobMatcher_FindMatchingFiles_EmptyPatterns_ReturnsEmptyList**: An empty pattern array
returns an empty list. This scenario is tested by
`GlobMatcher_FindMatchingFiles_EmptyPatterns_ReturnsEmptyList`.

**GlobMatcher_FindMatchingFiles_PatternMatchingNoFiles_ReturnsEmptyList**: A pattern that
matches no files returns an empty list. This scenario is tested by
`GlobMatcher_FindMatchingFiles_PatternMatchingNoFiles_ReturnsEmptyList`.

**GlobMatcher_SplitAbsolutePattern_PatternWithWildcard_SplitsCorrectly**: A pattern with a
wildcard is split at the last path separator before the wildcard. This scenario is tested
by `GlobMatcher_SplitAbsolutePattern_PatternWithWildcard_SplitsCorrectly`.

**GlobMatcher_SplitAbsolutePattern_PatternWithoutWildcard_SplitsAtLastSeparator**: A
pattern without a wildcard is split at the last path separator. This scenario is tested by
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

**PathHelpers_SafePathCombine_ValidPaths_CombinesCorrectly**: A simple relative path is
combined with the base path. This scenario is tested by
`PathHelpers_SafePathCombine_ValidPaths_CombinesCorrectly`.

**PathHelpers_SafePathCombine_PathTraversalWithDoubleDots_ThrowsArgumentException**: A path
beginning with `../` throws `ArgumentException`. This scenario is tested by
`PathHelpers_SafePathCombine_PathTraversalWithDoubleDots_ThrowsArgumentException`.

**PathHelpers_SafePathCombine_DoubleDotsInMiddle_ThrowsArgumentException**: A path
containing `..` in the middle throws `ArgumentException`. This scenario is tested by
`PathHelpers_SafePathCombine_DoubleDotsInMiddle_ThrowsArgumentException`.

**PathHelpers_SafePathCombine_AbsolutePath_ThrowsArgumentException**: A rooted absolute
path throws `ArgumentException`. This scenario is tested by
`PathHelpers_SafePathCombine_AbsolutePath_ThrowsArgumentException`.

**PathHelpers_SafePathCombine_CurrentDirectoryReference_CombinesCorrectly**: A path
containing `.` (current directory) combines correctly. This scenario is tested by
`PathHelpers_SafePathCombine_CurrentDirectoryReference_CombinesCorrectly`.

**PathHelpers_SafePathCombine_NestedPaths_CombinesCorrectly**: A nested relative path
combines correctly. This scenario is tested by
`PathHelpers_SafePathCombine_NestedPaths_CombinesCorrectly`.

**PathHelpers_SafePathCombine_EmptyRelativePath_ReturnsBasePath**: An empty relative path
returns the base path unchanged. This scenario is tested by
`PathHelpers_SafePathCombine_EmptyRelativePath_ReturnsBasePath`.

**PathHelpers_SafePathCombine_DotDotAsNamePrefix_CombinesCorrectly**: A filename that
starts with `..` but is not a traversal combines correctly. This scenario is tested by
`PathHelpers_SafePathCombine_DotDotAsNamePrefix_CombinesCorrectly`.

**PathHelpers_SafePathCombine_NullBasePath_ThrowsArgumentNullException**: A null base path
throws `ArgumentNullException`. This scenario is tested by
`PathHelpers_SafePathCombine_NullBasePath_ThrowsArgumentNullException`.

**PathHelpers_SafePathCombine_NullRelativePath_ThrowsArgumentNullException**: A null
relative path throws `ArgumentNullException`. This scenario is tested by
`PathHelpers_SafePathCombine_NullRelativePath_ThrowsArgumentNullException`.
