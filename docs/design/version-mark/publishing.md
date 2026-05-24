## Publishing

### Overview

The Publishing subsystem is responsible for generating a human-readable markdown version
report from captured JSON files. It reads the version data produced by the Capture
subsystem and consolidates identical versions across jobs, flagging any conflicts. It
consists of a single unit: `MarkdownFormatter`.

### Interfaces

**`MarkdownFormatter.Format(IEnumerable<VersionInfo> versionInfos, int reportDepth = 2)`**:
Converts a collection of captured version records into a markdown string.

- *Type*: In-process .NET public API (static method).
- *Role*: Provider.
- *Contract*: Accepts a sequence of `VersionInfo` records and returns a complete markdown
  string containing a `Tool Versions` section. `reportDepth` controls the heading level:
  `reportDepth = 2` produces `## Tool Versions`, `reportDepth = 1` produces
  `# Tool Versions`, and so on. When all jobs report the same version for a tool, a single
  bullet is emitted with no job IDs; when versions differ, one bullet per distinct version
  is emitted with contributing job IDs in parentheses.
- *Constraints*: `reportDepth` must be greater than zero; passing `0` or a negative value
  throws `ArgumentOutOfRangeException`. An empty `versionInfos` sequence produces valid
  markdown with only the heading.

| Parameter      | Type                       | Description                                             |
|----------------|----------------------------|---------------------------------------------------------|
| `versionInfos` | `IEnumerable<VersionInfo>` | The captured version records to include in the report   |
| `reportDepth`  | `int`                      | Heading depth for the section title (default: 2)        |

### Design

The Publishing subsystem consists of the single `MarkdownFormatter` static class, which
implements a three-step pipeline:

1. **`BuildToolVersionsDictionary`** — iterates all `VersionInfo` records and builds a
   `Dictionary<string, List<(string JobId, string Version)>>` mapping tool names to the
   `(jobId, version)` pairs observed across all input records.

2. **`GenerateMarkdown`** — writes the `Tool Versions` heading (using
   `new string('#', reportDepth)`), sorts tool names alphabetically, then calls
   `FormatVersionEntries` for each.

3. **`FormatVersionEntries`** — applies the consolidation rule: if all job entries for a
   tool share the same version, emits a single `- **tool**: version` bullet; if versions
   differ, emits one bullet per distinct version with contributing job IDs in parentheses.

The subsystem depends on `VersionInfo` (Capture subsystem) as its input data model. The
following error conditions are handled before the pipeline runs:

- **`--report` not specified**: `context.WriteError` in `Program.RunPublish`; exit 1.
- **No files match the glob patterns**: `context.WriteError` listing the patterns; exit 1.
- **`VersionInfo.LoadFromFile` throws**: `context.WriteError` with the exception message;
  exit 1.
- **`reportDepth <= 0`**: `ArgumentOutOfRangeException` thrown by `MarkdownFormatter.Format`.
