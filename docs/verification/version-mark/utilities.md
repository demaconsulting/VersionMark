## Utilities Subsystem Verification

### Overview

The Utilities subsystem provides general-purpose helper classes for use within VersionMark.
It consists of two units: `GlobMatcher` (the glob-pattern file matcher) and `PathHelpers`
(the safe path combination utility).

Unit-level verification for `GlobMatcher` and `PathHelpers` is in the chapters that follow.

### Verification Approach

Unit tests invoke `GlobMatcher` and `PathHelpers` directly with various inputs and assert
on the returned results. Tests use temporary directories for file-system scenarios,
ensuring isolation and repeatability across platforms. No external mocks are required.

### Test Scenarios

The following test scenarios verify Utilities subsystem requirements:

- **`GlobMatcher_FindMatchingFiles_RelativePattern_ReturnsMatchingFiles`**:
  Relative glob pattern matches files in the current directory.
- **`GlobMatcher_FindMatchingFiles_AbsolutePattern_ReturnsMatchingFiles`**:
  Absolute glob pattern matches files regardless of the working directory.
- **`GlobMatcher_FindMatchingFiles_SingleFileAbsolutePath_ReturnsSingleFile`**:
  Absolute path without wildcard returns that single file.
- **`GlobMatcher_FindMatchingFiles_MixedPatterns_ReturnsCombinedFiles`**:
  Mixed absolute and relative patterns are combined correctly.
- **`GlobMatcher_FindMatchingFiles_EmptyPatterns_ReturnsEmptyList`**:
  Empty pattern array returns an empty list.
- **`GlobMatcher_FindMatchingFiles_PatternMatchingNoFiles_ReturnsEmptyList`**:
  Pattern with no matches returns an empty list.
- **`GlobMatcher_SplitAbsolutePattern_PatternWithWildcard_SplitsCorrectly`**:
  Pattern with wildcard is split at the last separator before the wildcard.
- **`GlobMatcher_SplitAbsolutePattern_PatternWithoutWildcard_SplitsAtLastSeparator`**:
  Pattern without wildcard is split at the last separator.
- **`GlobMatcher_SplitAbsolutePattern_ForwardSlashRootPattern_SplitsToRootAndRelative`**:
  Root-relative forward-slash pattern (e.g. `/*.json`) splits to the platform path root (`/` on Unix,
  `\` on Windows) and relative pattern on all platforms.
- **`GlobMatcher_SplitAbsolutePattern_WindowsDriveRootPattern_SplitsToDriveRootAndRelative`**:
  Windows drive-root pattern (e.g. `C:\*.json`) splits to `C:\` root and relative pattern (Windows only).
- **`PathHelpers_SafePathCombine_ValidPaths_CombinesCorrectly`**: A simple relative path is combined with the base path.
- **`PathHelpers_SafePathCombine_PathTraversalWithDoubleDots_ThrowsArgumentException`**:
  A path beginning with `../` throws ArgumentException.
- **`PathHelpers_SafePathCombine_DoubleDotsInMiddle_ThrowsArgumentException`**:
  A path containing `..` in the middle throws ArgumentException.
- **`PathHelpers_SafePathCombine_AbsolutePath_ThrowsArgumentException`**:
  A rooted absolute path throws ArgumentException.
- **`PathHelpers_SafePathCombine_CurrentDirectoryReference_CombinesCorrectly`**:
  A path containing `.` (current directory) combines correctly.
- **`PathHelpers_SafePathCombine_NestedPaths_CombinesCorrectly`**: A nested relative path combines correctly.
- **`PathHelpers_SafePathCombine_EmptyRelativePath_ReturnsBasePath`**:
  An empty relative path returns the base path unchanged.
- **`PathHelpers_SafePathCombine_DotDotAsNamePrefix_CombinesCorrectly`**:
  A filename that starts with `..` but is not a traversal combines correctly.
- **`PathHelpers_SafePathCombine_NullBasePath_ThrowsArgumentNullException`**:
  A null base path throws ArgumentNullException.
- **`PathHelpers_SafePathCombine_NullRelativePath_ThrowsArgumentNullException`**:
  A null relative path throws ArgumentNullException.

### Dependencies

Tests use temporary directories for file-system scenarios. No external mocks are required.

### Requirements Coverage

The following list maps Utilities subsystem requirements to test scenarios:

- **`VersionMark-Utilities-GlobMatch`**: `GlobMatcher_FindMatchingFiles_RelativePattern_ReturnsMatchingFiles`,
  `GlobMatcher_FindMatchingFiles_AbsolutePattern_ReturnsMatchingFiles`,
  `GlobMatcher_FindMatchingFiles_SingleFileAbsolutePath_ReturnsSingleFile`,
  `GlobMatcher_FindMatchingFiles_MixedPatterns_ReturnsCombinedFiles`,
  `GlobMatcher_FindMatchingFiles_EmptyPatterns_ReturnsEmptyList`,
  `GlobMatcher_FindMatchingFiles_PatternMatchingNoFiles_ReturnsEmptyList`
- **`VersionMark-Utilities-SafePath`**: `PathHelpers_SafePathCombine_ValidPaths_CombinesCorrectly`,
  `PathHelpers_SafePathCombine_PathTraversalWithDoubleDots_ThrowsArgumentException`,
  `PathHelpers_SafePathCombine_DoubleDotsInMiddle_ThrowsArgumentException`,
  `PathHelpers_SafePathCombine_AbsolutePath_ThrowsArgumentException`,
  `PathHelpers_SafePathCombine_CurrentDirectoryReference_CombinesCorrectly`,
  `PathHelpers_SafePathCombine_NestedPaths_CombinesCorrectly`,
  `PathHelpers_SafePathCombine_EmptyRelativePath_ReturnsBasePath`,
  `PathHelpers_SafePathCombine_DotDotAsNamePrefix_CombinesCorrectly`,
  `PathHelpers_SafePathCombine_NullBasePath_ThrowsArgumentNullException`,
  `PathHelpers_SafePathCombine_NullRelativePath_ThrowsArgumentNullException`
