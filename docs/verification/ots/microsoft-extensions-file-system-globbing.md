## Microsoft.Extensions.FileSystemGlobbing

### Verification Approach

`Microsoft.Extensions.FileSystemGlobbing` is an OTS library from Microsoft that provides
the `Matcher` class for glob-pattern file matching. The Utilities subsystem uses it inside
`GlobMatcher.FindMatchingFiles` to evaluate both relative and absolute glob patterns
against the file system and return the matching file paths.

`Microsoft.Extensions.FileSystemGlobbing` is verified through the GlobMatcher unit tests.
Tests supply known directory structures and glob patterns to `GlobMatcher.FindMatchingFiles`
and assert on the returned file list. Scenarios cover relative patterns, absolute patterns,
overlapping patterns (deduplication), and patterns that match no files. Successful
execution of these tests across all supported OS and .NET version matrix combinations
confirms that the `Matcher` class is evaluating patterns correctly in each environment.

### Test Scenarios

**FileSystemGlobbing_MatchesRelativePattern**: `GlobMatcher.FindMatchingFiles` with a
relative glob pattern returns the expected matching files from the current directory. This
scenario is verified by `GlobMatcher_FindMatchingFiles_RelativePattern_ReturnsMatchingFiles`.

**FileSystemGlobbing_MatchesAbsolutePattern**: `GlobMatcher.FindMatchingFiles` with an
absolute glob pattern returns the expected matching files from the rooted directory. This
scenario is verified by `GlobMatcher_FindMatchingFiles_AbsolutePattern_ReturnsMatchingFiles`.

**FileSystemGlobbing_DeduplicatesOverlappingPatterns**: Two overlapping glob patterns
supplied to `GlobMatcher.FindMatchingFiles` return each matching file only once in the
sorted result. This scenario is verified by
`GlobMatcher_FindMatchingFiles_MixedPatterns_ReturnsCombinedFiles`.
