### GlobMatcher

#### Purpose

`GlobMatcher` (`GlobMatcher.cs`) is a static utility class that resolves an array of glob
patterns into a sorted, deduplicated list of full file paths. It supports both relative
patterns (evaluated against the current working directory) and absolute patterns (evaluated
from their own root directory). `GlobMatcher.FindMatchingFiles` is called by
`Program.RunPublish` to resolve command-line glob patterns into a concrete list of JSON
capture files.

#### Data Model

`GlobMatcher` is a static class with no instance state. Internally it accumulates
absolute patterns into separate `Matcher` invocations and collects relative patterns for a
single batched `Matcher` run. Results are deduplicated using a case-insensitive
`HashSet<string>`.

#### Key Methods

**`FindMatchingFiles(string[] globPatterns)` (internal static)** — Resolves all patterns
and returns a sorted `List<string>` of full file paths.

Processing steps:

1. Iterate each pattern in `globPatterns`.
2. For rooted patterns, call `SplitAbsolutePattern` to obtain `(rootDir,
   relativePattern)`, then run a `Matcher` against that `rootDir`.
3. Collect non-rooted patterns into a list.
4. If any relative patterns were collected, run a single `Matcher` against
   `Directory.GetCurrentDirectory()` covering all relative patterns at once.
5. Combine all matches into a case-insensitive `HashSet<string>` for deduplication,
   then return the sorted result.

Batching relative patterns into one `Matcher` run reduces directory enumeration overhead
compared to one run per pattern. Sorted output makes results deterministic.

**`SplitAbsolutePattern(string absolutePattern)` (internal static)** — Splits an absolute
glob pattern into `(string rootDir, string relativePattern)`.

Algorithm:

1. Find the index of the first wildcard character (`*`, `?`, or `[`).
2. If no wildcard, return `(Path.GetDirectoryName, Path.GetFileName)`.
3. Find the last directory separator before the wildcard.
4. Split at that separator, handling edge cases where the separator is the first
   character (e.g., `/`) or where the root segment has no trailing separator (e.g.,
   `C:` on Windows).

#### Error Handling

`GlobMatcher` contains no explicit error handling. Invalid patterns or inaccessible
directories are handled by the BCL and `Microsoft.Extensions.FileSystemGlobbing`. Callers
must handle an empty result list.

#### Dependencies

- `Microsoft.Extensions.FileSystemGlobbing` (OTS) — `Matcher` for pattern evaluation.
- `System.IO` (BCL) — `Directory.GetCurrentDirectory`, `Path.IsPathRooted`, etc.

#### Callers

- `Program.RunPublish` — calls `GlobMatcher.FindMatchingFiles` with the glob patterns
  parsed from `context.GlobPatterns`.
