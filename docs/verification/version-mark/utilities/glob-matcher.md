### GlobMatcher Unit Verification

#### Overview

The `GlobMatcher` unit provides `FindMatchingFiles` and `SplitAbsolutePattern` methods for
glob-pattern file matching. It supports relative and absolute patterns and returns a sorted,
deduplicated list of full paths. Tests are in `Utilities/GlobMatcherTests.cs`.

#### Test Scenarios

The following test scenarios verify `GlobMatcher`:

- **`GlobMatcher_FindMatchingFiles_EmptyPatterns_ReturnsEmptyList`**:
  An empty pattern array returns an empty list.
- **`GlobMatcher_FindMatchingFiles_PatternMatchingNoFiles_ReturnsEmptyList`**:
  A pattern that matches no files returns an empty list.
- **`GlobMatcher_FindMatchingFiles_RelativePattern_ReturnsMatchingFiles`**:
  A relative glob pattern is matched against the current directory.
- **`GlobMatcher_FindMatchingFiles_AbsolutePattern_ReturnsMatchingFiles`**:
  An absolute glob pattern is matched from its root directory.
- **`GlobMatcher_FindMatchingFiles_SingleFileAbsolutePath_ReturnsSingleFile`**:
  An absolute path with no wildcard returns that single file.
- **`GlobMatcher_FindMatchingFiles_MixedPatterns_ReturnsCombinedFiles`**:
  Mixed absolute and relative patterns produce a combined deduplicated result.
- **`GlobMatcher_SplitAbsolutePattern_PatternWithWildcard_SplitsCorrectly`**:
  A pattern with a wildcard is split at the last separator before the wildcard.
- **`GlobMatcher_SplitAbsolutePattern_PatternWithoutWildcard_SplitsAtLastSeparator`**:
  A pattern without a wildcard is split at the final separator.
- **`GlobMatcher_SplitAbsolutePattern_UnixRootPattern_SplitsToRootAndRelative`**:
  A Unix root-level pattern (e.g. `/*.json`) splits to `/` root and relative pattern (non-Windows only).
- **`GlobMatcher_SplitAbsolutePattern_WindowsDriveRootPattern_SplitsToDriveRootAndRelative`**:
  A Windows drive-root pattern (e.g. `C:\*.json`) splits to `C:\` root and relative pattern (Windows only).

#### Dependencies

Tests use temporary directories created with `Path.GetTempPath()` for all file-system
scenarios. No external mocks are required. Tests call `GlobMatcher` methods directly.

#### Requirements Coverage

The following list maps `GlobMatcher` unit requirements to test scenarios:

- **`VersionMark-GlobMatcher-FindFiles`**: `GlobMatcher_FindMatchingFiles_RelativePattern_ReturnsMatchingFiles`,
  `GlobMatcher_FindMatchingFiles_EmptyPatterns_ReturnsEmptyList`,
  `GlobMatcher_FindMatchingFiles_PatternMatchingNoFiles_ReturnsEmptyList`,
  `GlobMatcher_FindMatchingFiles_MixedPatterns_ReturnsCombinedFiles`
- **`VersionMark-GlobMatcher-AbsolutePaths`**: `GlobMatcher_FindMatchingFiles_AbsolutePattern_ReturnsMatchingFiles`,
  `GlobMatcher_FindMatchingFiles_SingleFileAbsolutePath_ReturnsSingleFile`,
  `GlobMatcher_FindMatchingFiles_MixedPatterns_ReturnsCombinedFiles`,
  `GlobMatcher_SplitAbsolutePattern_PatternWithWildcard_SplitsCorrectly`,
  `GlobMatcher_SplitAbsolutePattern_PatternWithoutWildcard_SplitsAtLastSeparator`,
  `GlobMatcher_SplitAbsolutePattern_UnixRootPattern_SplitsToRootAndRelative`,
  `GlobMatcher_SplitAbsolutePattern_WindowsDriveRootPattern_SplitsToDriveRootAndRelative`
