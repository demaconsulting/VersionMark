### MarkdownFormatter

#### Purpose

`MarkdownFormatter` (`MarkdownFormatter.cs`) converts a collection of `VersionInfo` records
into a markdown string. It consolidates version data across multiple CI/CD jobs: when all
jobs report the same version for a tool, a single bullet is emitted; when versions differ,
one bullet per distinct version is emitted with contributing job IDs. The result is written
to a file by `Program.RunPublish`.

#### Data Model

`MarkdownFormatter` is a static class with no instance state. It uses a
`Dictionary<string, List<(string JobId, string Version)>>` internally to accumulate
per-tool version observations before generating output.

#### Key Methods

**`Format(IEnumerable<VersionInfo> versionInfos, int reportDepth = 2)` (static)** —
Accepts a sequence of `VersionInfo` records and returns a complete markdown string. The
`Tool Versions` section heading level is controlled by `reportDepth` (e.g., `reportDepth =
2` produces `## Tool Versions`). Delegates to `BuildToolVersionsDictionary` and
`GenerateMarkdown`.

**`BuildToolVersionsDictionary(IEnumerable<VersionInfo> versionInfos)` (private static)**
— Iterates all records and builds a `Dictionary<string, List<(string JobId, string
Version)>>` mapping each tool name to its observed `(jobId, version)` pairs.

**`GenerateMarkdown(Dictionary<...>, int reportDepth)` (private static)** — Writes the
heading using `new string('#', reportDepth)`, sorts tool names alphabetically, then calls
`FormatVersionEntries` for each tool.

**`FormatVersionEntries(StringBuilder markdown, string tool, List<(string JobId, string Version)> versions)` (private static)** — Applies the
consolidation rule. Version groups are ordered alphabetically (case-insensitive) before
per-group bullet lines are emitted:

- If all entries share the same version: emit `- **tool**: version` (no job IDs).
- If versions differ: emit one `- **tool**: version (job1, job2)` per distinct version
  with alphabetically sorted contributing job IDs.

#### Error Handling

| Condition              | Behavior                                                     |
|------------------------|--------------------------------------------------------------|
| `reportDepth <= 0`     | `ArgumentOutOfRangeException` thrown before any output       |
| `versionInfos` is empty | Returns valid markdown with heading and no bullets          |
| Tool name is empty     | Emitted as `- ****: version`                                 |
| `JobId` is null/empty  | Appears as an empty string in the parenthesised list         |

#### Dependencies

- `VersionInfo` (Capture subsystem) — input data model.
- `System.Text.StringBuilder` (BCL) — output string construction.

#### Callers

- `Program.RunPublish` — calls `MarkdownFormatter.Format` with the loaded `VersionInfo`
  records and writes the returned string to the report file.
- `Validation.RunPublishTest` — indirectly exercises `MarkdownFormatter.Format` via
  `Program.Run`.
