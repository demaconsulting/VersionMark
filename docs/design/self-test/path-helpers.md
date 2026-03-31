# PathHelpers Unit

## Overview

The `PathHelpers` class (`PathHelpers.cs`) provides a single static method,
`SafePathCombine`, designed to defend against path-traversal attacks when constructing
file paths from partially-trusted input.

## SafePathCombine Method

`SafePathCombine` takes a `basePath` and a `relativePath` and returns a combined path,
subject to two layers of validation:

1. **Pre-combination check**: rejects `relativePath` if it contains `".."` or is a rooted
   (absolute) path.
2. **Post-combination check**: resolves both paths with `Path.GetFullPath` and calls
   `Path.GetRelativePath` to verify the combined path still sits under `basePath`.

If either check fails, `ArgumentException` is thrown. This defense-in-depth approach
guards against edge-cases that might bypass the initial string check while remaining
straightforward to audit.

`PathHelpers` is used by `Validation` when constructing paths inside temporary directories
for self-validation tests. This satisfies requirement `VersionMark-PathHelpers-SafeCombine`.
