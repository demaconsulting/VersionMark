### PathHelpers

![Utilities Structure](UtilitiesView.svg)

#### Purpose

`PathHelpers` (`PathHelpers.cs`) is a static utility class that provides safe path
combination, protecting callers against path-traversal attacks. It verifies that the
resolved combined path remains within the specified base directory before returning it.
`PathHelpers.SafePathCombine` is used by `Validation.TemporaryDirectory` when constructing
file paths inside isolated temporary directories for self-validation tests.

**Note**: `Path.GetFullPath` normalizes `.`/`..` segments but does not resolve symlinks
or reparse points, so this check guards against string-level traversal only.

#### Data Model

`PathHelpers` is a static class with no instance state.

#### Key Methods

**`SafePathCombine(string basePath, string relativePath)` (internal static)** — Combines
`basePath` and `relativePath` safely, verifying the result lies within `basePath`.

Validation steps:

1. Reject null inputs via `ArgumentNullException.ThrowIfNull`.
2. Combine the paths with `Path.Combine` to produce the candidate path.
3. Resolve both `basePath` and the candidate to absolute form with `Path.GetFullPath`.
4. Compute `Path.GetRelativePath(absoluteBase, absoluteCombined)` and reject the input
   when the result is `".."`, starts with `".."` followed by a directory separator, or
   is itself rooted — all of which indicate the combined path escapes the base directory.
5. Return the combined path on success.

Using `Path.GetRelativePath` for the containment check handles root paths, platform
case-sensitivity, and directory-separator normalization natively. The test is applied only
to `".."` as the entire result or followed by a directory separator, avoiding false
positives for valid in-base names that begin with `..` (such as `..data`). Resolving paths
after combining handles all traversal patterns — `../`, embedded `/../`, absolute-path
overrides, and platform edge cases — without fragile pre-combine string inspection.

#### Error Handling

| Condition                                    | Behavior                                                    |
|----------------------------------------------|-------------------------------------------------------------|
| `basePath` is null                           | `ArgumentNullException` thrown                              |
| `relativePath` is null                       | `ArgumentNullException` thrown                              |
| Combined path escapes the base directory     | `ArgumentException` thrown, identifying `relativePath`      |

#### Dependencies

- `System.IO` (BCL) — `Path.Combine`, `Path.GetFullPath`, `Path.GetRelativePath`,
  `Path.DirectorySeparatorChar`, `Path.AltDirectorySeparatorChar`.

#### Callers

- `Validation.TemporaryDirectory` (SelfTest subsystem) — calls `SafePathCombine` to
  construct paths inside temporary directories, using a `Guid`-based relative name under
  `Path.GetTempPath()`.
