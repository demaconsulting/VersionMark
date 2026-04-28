# MarkdownFormatter Unit

## Overview

The `MarkdownFormatter` class (`MarkdownFormatter.cs`) provides the `Format` static method
that converts a collection of `VersionInfo` records into a markdown string. This satisfies
requirements `VersionMark-Formatter-Structure`, `VersionMark-Formatter-JobId`,
`VersionMark-Formatter-Versions`, `VersionMark-Formatter-MarkdownList`, and
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

## Output Structure

The markdown output begins with a `Tool Versions` section heading whose level is
controlled by `reportDepth`. For example, with `reportDepth = 2` the heading is
`## Tool Versions`; with `reportDepth = 3` it is `### Tool Versions`.

Each tool is then listed as one or more markdown bullet items below the heading.

## Version Consolidation Logic

`FormatVersionEntries` implements the consolidation rule:

- If all `(jobId, version)` pairs for a tool share the **same** version, a single bullet
  is emitted: `- **tool**: version` (no job IDs shown). This satisfies
  `VersionMark-Formatter-JobId`.
- If versions **differ**, one bullet per distinct version is emitted with the contributing
  job IDs in parentheses: `- **tool**: version (job1, job2)`. This satisfies
  `VersionMark-Formatter-Versions` and `VersionMark-Formatter-MarkdownList`.

Both cases use bold tool names. In the multi-version case, each unique version appears on
its own line with the alphabetically-sorted job IDs that produced it enclosed in
parentheses.

## Heading Depth

The heading prefix is constructed as `new string('#', reportDepth)`, so `reportDepth = 2`
yields `## Tool Versions`, `reportDepth = 1` yields `# Tool Versions`, and so on. This
satisfies `VersionMark-Formatter-MarkdownConsolidation`.

`reportDepth` must be greater than zero. Passing `0` or a negative value causes
`ArgumentOutOfRangeException` to be thrown before any output is generated.

## Error Handling

| Input condition                  | Behavior                                          |
|----------------------------------|---------------------------------------------------|
| `reportDepth <= 0`               | `ArgumentOutOfRangeException` thrown              |
| `versionInfos` is empty          | Returns valid markdown with heading but no bullets|
| Tool name is empty string        | Tool appears in output as `- ****: version`       |
| `JobId` is null or empty         | Job ID appears as empty string in parentheses     |
