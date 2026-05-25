## Utilities

### Overview

The Utilities subsystem provides general-purpose helper classes used by other subsystems
within VersionMark. It consists of two units: `GlobMatcher`, which implements glob-pattern
file matching for publish mode, and `PathHelpers`, which provides safe path combination to
protect against path-traversal attacks. The subsystem has no dependencies on other
VersionMark subsystems.

### Interfaces

**`GlobMatcher.FindMatchingFiles(string[] globPatterns)`**: Resolves an array of glob
patterns into a sorted, deduplicated list of full file paths.

- *Type*: In-process .NET public API (internal static method).
- *Role*: Provider.
- *Contract*: Accepts an array of glob patterns (relative or absolute). Relative patterns
  are evaluated against `Directory.GetCurrentDirectory()`; absolute patterns are each
  evaluated against their own root directory. Returns a sorted, deduplicated
  `List<string>` of full file paths. Deduplication uses a `HashSet<string>` with a
  file-system-appropriate comparer (ordinal ignore-case on Windows, ordinal elsewhere).
- *Constraints*: No explicit error handling; invalid patterns or inaccessible directories
  are handled by the BCL and FileSystemGlobbing. Callers must handle an empty result.

**`GlobMatcher.SplitAbsolutePattern(string absolutePattern)`**: Splits an absolute glob
pattern into its root directory and relative pattern components.

- *Type*: In-process .NET public API (internal static method).
- *Role*: Provider.
- *Contract*: Returns `(string rootDir, string relativePattern)` by locating the first
  wildcard character and finding the last directory separator before it.
- *Constraints*: Input must be an absolute (rooted) path.

**`PathHelpers.SafePathCombine(string basePath, string relativePath)`**: Combines two paths
while verifying the result remains within the base directory.

- *Type*: In-process .NET public API (internal static method).
- *Role*: Provider.
- *Contract*: Combines `basePath` and `relativePath` using `Path.Combine`, resolves both to
  absolute form, then uses `Path.GetRelativePath` to verify the result lies within
  `basePath`. Returns the combined path on success.
- *Constraints*: Throws `ArgumentNullException` for null inputs. Throws `ArgumentException`
  (identifying `relativePath`) when the combined path escapes the base directory.

### Design

The Utilities subsystem contains two independent, stateless units with no dependency on
each other or on any other VersionMark subsystem:

- **`GlobMatcher`** — delegates pattern evaluation to
  `Microsoft.Extensions.FileSystemGlobbing`. Relative patterns are batched into a single
  `Matcher` run against `Directory.GetCurrentDirectory()`; absolute patterns are each
  evaluated against their own root directory obtained via `SplitAbsolutePattern`. Results
  are deduplicated with a `HashSet<string>` using a file-system-appropriate comparer
  and returned sorted.
  `GlobMatcher.FindMatchingFiles` is called by `Program.RunPublish` to resolve command-line
  glob patterns into a concrete list of JSON capture files.

- **`PathHelpers`** — uses `Path.GetFullPath` to canonicalize both the base path and the
  combined candidate, then uses `Path.GetRelativePath` to verify the candidate lies within
  the base. The containment check treats only a relative result of exactly `".."` or one
  starting with `".." + separator` (or a rooted result) as an escape, avoiding false
  positives for legitimate in-base names that begin with `..`.
  `PathHelpers.SafePathCombine` is called by `Validation.TemporaryDirectory` (SelfTest
  subsystem) when constructing paths inside temporary directories.
