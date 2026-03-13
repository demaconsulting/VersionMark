# Utilities

## Overview

The utilities layer provides two internal support classes used by the rest of the tool:
`PathHelpers` for safe path operations, and `MarkdownFormatter` for generating the version
report markdown output.

## PathHelpers Class

The `PathHelpers` class (`PathHelpers.cs`) provides a single static method, `SafePathCombine`,
designed to defend against path-traversal attacks when constructing file paths from
partially-trusted input.

### SafePathCombine Method

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
for self-validation tests.

## MarkdownFormatter Class

The `MarkdownFormatter` class (`MarkdownFormatter.cs`) provides the `Format` static method
that converts a collection of `VersionInfo` records into a markdown string. This satisfies
requirements `VersionMark-Fmt-JsonStructure`, `VersionMark-Fmt-JsonJobId`,
`VersionMark-Fmt-JsonVersions`, `VersionMark-Fmt-MarkdownList`, and
`VersionMark-Fmt-MarkdownConsolidation`.

### Format Method

`Format` accepts an `IEnumerable<VersionInfo>` and an optional `reportDepth` (default 2),
and returns a markdown string.

The method delegates to two helpers:

1. **`BuildToolVersionsDictionary`**: iterates all `VersionInfo` records and builds a
   `Dictionary<string, List<(string JobId, string Version)>>` mapping each tool name to
   the list of `(jobId, version)` pairs seen across all input files.
2. **`GenerateMarkdown`**: writes the heading, sorts tools alphabetically, and calls
   `FormatVersionEntries` for each tool.

### Version Consolidation Logic

`FormatVersionEntries` implements the consolidation rule:

- If all `(jobId, version)` pairs for a tool share the **same** version, a single bullet
  is emitted: `- **tool**: version`
- If versions **differ**, one bullet per distinct version is emitted with the contributing
  job IDs in parentheses: `- **tool**: version (job1, job2)`

Both cases use bold tool names. In the multi-version case, each unique version appears on
its own line with the alphabetically-sorted job IDs that produced it enclosed in parentheses.

This behavior satisfies `VersionMark-Fmt-JsonJobId` (uniform versions collapse) and
`VersionMark-Fmt-JsonVersions` / `VersionMark-Fmt-MarkdownList` (differing versions show
job attribution).

### Heading Depth

The heading prefix is constructed as `new string('#', reportDepth)`, so `reportDepth = 2`
yields `## Tool Versions`, `reportDepth = 1` yields `# Tool Versions`, and so on. This
satisfies `VersionMark-Fmt-MarkdownConsolidation`.
