# MarkdownFormatter Unit

## Overview

The `MarkdownFormatter` class (`MarkdownFormatter.cs`) provides the `Format` static method
that converts a collection of `VersionInfo` records into a markdown string. This satisfies
requirements `VersionMark-Formatter-JsonStructure`, `VersionMark-Formatter-JsonJobId`,
`VersionMark-Formatter-JsonVersions`, `VersionMark-Formatter-MarkdownList`, and
`VersionMark-Formatter-MarkdownConsolidation`.

## Format Method

`Format` accepts an `IEnumerable<VersionInfo>` and an optional `reportDepth` (default 2),
and returns a markdown string.

The method delegates to two helpers:

1. **`BuildToolVersionsDictionary`**: iterates all `VersionInfo` records and builds a
   `Dictionary<string, List<(string JobId, string Version)>>` mapping each tool name to
   the list of `(jobId, version)` pairs seen across all input files.
2. **`GenerateMarkdown`**: writes the heading, sorts tools alphabetically, and calls
   `FormatVersionEntries` for each tool.

## Version Consolidation Logic

`FormatVersionEntries` implements the consolidation rule:

- If all `(jobId, version)` pairs for a tool share the **same** version, a single bullet
  is emitted: `- **tool**: version`
- If versions **differ**, one bullet per distinct version is emitted with the contributing
  job IDs in parentheses: `- **tool**: version (job1, job2)`

Both cases use bold tool names. In the multi-version case, each unique version appears on
its own line with the alphabetically-sorted job IDs that produced it enclosed in
parentheses.

This behavior satisfies `VersionMark-Formatter-JsonJobId` (uniform versions collapse) and
`VersionMark-Formatter-JsonVersions` / `VersionMark-Formatter-MarkdownList` (differing versions show
job attribution).

## Heading Depth

The heading prefix is constructed as `new string('#', reportDepth)`, so `reportDepth = 2`
yields `## Tool Versions`, `reportDepth = 1` yields `# Tool Versions`, and so on. This
satisfies `VersionMark-Formatter-MarkdownConsolidation`.
