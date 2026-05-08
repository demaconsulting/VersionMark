### GlobMatcher Unit

#### Overview

`GlobMatcher` is a static utility class that provides glob-pattern file matching. It
supports both relative patterns (evaluated against the current directory) and absolute
patterns (evaluated from their own root directory), and returns a sorted, deduplicated
list of full file paths. It uses `Microsoft.Extensions.FileSystemGlobbing` for pattern
evaluation.

#### FindMatchingFiles Method

```csharp
internal static List<string> FindMatchingFiles(string[] globPatterns)
```

Finds all files matching the specified glob patterns and returns them as a sorted list of
full paths.

**Processing steps:**

1. Iterate over each pattern in `globPatterns`.
2. If a pattern is rooted (`Path.IsPathRooted`), call `SplitAbsolutePattern` to obtain the
   root directory and relative pattern, then use a `Matcher` against that directory.
3. If the pattern is relative, collect it into a separate list.
4. After iterating, if any relative patterns were collected, run a single `Matcher` against
   `Directory.GetCurrentDirectory()` covering all relative patterns.
5. Combine all matches into a `HashSet<string>` (case-insensitive) to deduplicate, then
   return the sorted result.

#### SplitAbsolutePattern Helper

```csharp
internal static (string rootDir, string relativePattern) SplitAbsolutePattern(string absolutePattern)
```

Splits an absolute glob pattern into its root directory and the relative pattern to be
passed to the `Matcher`.

**Algorithm:**

1. Determine the path root via `Path.GetPathRoot`.
2. Find the index of the first wildcard character (`*`, `?`, or `[`).
3. If no wildcard is found, return `(Path.GetDirectoryName, Path.GetFileName)`.
4. Find the last directory separator before the wildcard using `LastIndexOfAny` searching
   backwards from the wildcard position.
5. Split at that separator, handling the drive-root edge case where the separator is the
   first character (e.g. `/`) or where the root segment lacks a trailing separator (e.g.
   `C:` on Windows).

#### Design Decisions

- **Separate absolute and relative handling**: Absolute patterns are rooted at a specific
  directory and must be evaluated there, while relative patterns are evaluated relative to
  the current directory. Separating the two cases avoids incorrect matches.
- **Single Matcher for relative patterns**: Collecting all relative patterns into one
  `Matcher` run reduces directory enumeration overhead compared to one run per pattern.
- **Case-insensitive deduplication**: Using a case-insensitive `HashSet` prevents
  duplicates when patterns overlap or when the file system is case-insensitive.
- **Sorted output**: Returning a sorted list makes the output deterministic, simplifying
  testing and producing a consistent report order.

`GlobMatcher` is used by `Program.RunPublish` to resolve command-line glob patterns into
a concrete file list. This satisfies requirements `VersionMark-GlobMatcher-FindFiles` and
`VersionMark-GlobMatcher-AbsolutePaths`.
