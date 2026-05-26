## Microsoft.Extensions.FileSystemGlobbing

### Purpose

`Microsoft.Extensions.FileSystemGlobbing` is a glob-pattern file matching
library provided by Microsoft as part of the ASP.NET Core extensions ecosystem.
VersionMark uses it inside `GlobMatcher.FindMatchingFiles` to evaluate relative
and absolute glob patterns against the file system and return the matching file
paths. It is chosen because it handles cross-platform path separators correctly
and supports the standard `*`, `**`, and `?` wildcard syntax expected by CI/CD
pipeline authors.

### Features Used

| Feature              | Usage in VersionMark                                               |
|----------------------|--------------------------------------------------------------------|
| `Matcher`            | Accumulate one or more patterns and execute the match             |
| `Matcher.AddInclude` | Register each glob pattern supplied by the caller                 |
| `Matcher.Execute`    | Run the match against a `DirectoryInfoWrapper` rooted at a base path |
| `PatternMatchingResult.Files` | Enumerate matched relative file paths and resolve to absolute paths |

Only `AddInclude` and `Execute` are used; exclude patterns are not consumed by
VersionMark.

### Integration Pattern

`Microsoft.Extensions.FileSystemGlobbing` is consumed entirely within
`GlobMatcher.FindMatchingFiles`. No library types are exposed in public
signatures.

1. Each pattern supplied by the caller is inspected to determine whether it is
   absolute or relative. `Path.IsPathRooted` is used for this check.
2. For relative patterns, a single `Matcher` is created, all relative patterns
   are added via `AddInclude`, and `Execute` is called with a
   `DirectoryInfoWrapper` wrapping `new DirectoryInfo(Environment.CurrentDirectory)`.
3. For absolute patterns, the pattern is split at the last path separator before
   the first wildcard character (`SplitAbsolutePattern`). A separate `Matcher` is
   created for each distinct root directory, the relative portion is added via
   `AddInclude`, and `Execute` is called with a `DirectoryInfoWrapper` wrapping
   the absolute root directory.
4. Results from all `Matcher` instances are collected into a `HashSet<string>`
   with a file-system-appropriate comparer (to deduplicate overlapping
   patterns), then returned as a sorted `List<string>`.
5. No `Dispose` is required; `Matcher` does not hold unmanaged resources.
