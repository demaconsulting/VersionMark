## Utilities Subsystem Verification

### Overview

The Utilities subsystem provides general-purpose helper classes for use within VersionMark.
It currently consists of one unit: `GlobMatcher` (the glob-pattern file matcher).

Unit-level verification for `GlobMatcher` is in the chapter that follows.

### Verification Approach

Unit tests invoke `GlobMatcher` directly with various pattern and file-system inputs and
assert on the returned file list. Tests use temporary directories for file-system
scenarios, ensuring isolation and repeatability across platforms. No external mocks are
required.

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
