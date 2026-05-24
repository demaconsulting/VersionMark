## Configuration

### Overview

The Configuration subsystem reads and interprets the `.versionmark.yaml` file that defines
which tools to capture and how to extract their versions, and reports any validation issues
found during loading. It consists of three units: `LintIssue` (the shared validation types),
`ToolConfig` (per-tool settings), and `VersionMarkConfig` (the top-level configuration
container and validation entry point).

### Interfaces

**`VersionMarkConfig.Load(string filePath)`**: Reads and validates a `.versionmark.yaml`
file, returning a compound result containing the parsed configuration and all validation
issues.

- *Type*: In-process .NET public API (static method).
- *Role*: Provider.
- *Contract*: Checks file existence; parses YAML via YamlDotNet; validates the root node
  is a mapping; locates the `tools` key; calls `ValidateTool` for each entry; returns a
  `VersionMarkLoadResult` containing the parsed `VersionMarkConfig` (or `null` on error)
  and all `LintIssue` records. Uses an accumulate-and-continue strategy — all issues are
  collected before returning.
- *Constraints*: `Result.Config` is `null` when any error-level issue exists. Callers must
  check `Result.Config` before proceeding with capture or other operations.

**`VersionMarkConfig.FindVersions(IEnumerable<string> toolNames, string jobId, string? os)`**:
Executes the configured shell commands for the specified tools and returns a `VersionInfo`
record.

- *Type*: In-process .NET public API (instance method).
- *Role*: Provider.
- *Contract*: Resolves the OS once upfront (`os ?? ToolConfig.GetCurrentOs()`); for each
  named tool calls `GetEffectiveCommand`, `GetEffectiveRegex`, executes the command in a
  shell, and applies the regex to extract the version string. Returns a `VersionInfo` record
  with `JobId` and a `Versions` dictionary.
- *Constraints*: Throws `ArgumentException` for unknown tool names. Passing an OS for which
  no command or regex is defined raises `InvalidOperationException`.

**`VersionMarkLoadResult.ReportIssues(Context context)`**: Writes all collected validation
issues to the context.

- *Type*: In-process .NET public API (instance method).
- *Role*: Provider.
- *Contract*: Routes `LintSeverity.Error` issues to `context.WriteError` and
  `LintSeverity.Warning` issues to `context.WriteLine`.
- *Constraints*: Internal method; called only from `Program.RunLint` and `Program.RunCapture`.

**`LintIssue`**: Record carrying a single validation issue found during configuration loading.

- *Type*: In-process .NET public API (record type).
- *Role*: Provider.
- *Contract*: Carries `FilePath`, `Line`, `Column`, `Severity`, and `Description`.
  `ToString()` formats as `"file(line,col): severity: description"` where severity is
  lowercase (`warning` or `error`).
- *Constraints*: Immutable record; no failure-prone logic.

### Design

The Configuration subsystem contains three collaborating units. The dependency direction
is `VersionMarkConfig` → `ToolConfig` and `VersionMarkConfig` → `LintIssue`; `LintIssue`
and `ToolConfig` have no dependencies on each other or on other VersionMark subsystems.

1. **`LintIssue`** — defines the shared data types (`LintSeverity`, `LintIssue`,
   `VersionMarkLoadResult`) used throughout the loading pipeline.

2. **`ToolConfig`** — an immutable record representing a single tool's command and regex
   settings with OS-specific overrides. Constructed by `VersionMarkConfig.ValidateTool`.
   Exposes `GetEffectiveCommand` and `GetEffectiveRegex` for OS-resolved lookup.

3. **`VersionMarkConfig`** — the orchestrator. Its `Load` method drives YAML parsing using
   YamlDotNet, calls `ValidateTool` for each tool entry, and accumulates all `LintIssue`
   records into a single `VersionMarkLoadResult`. Its `FindVersions` method iterates the
   loaded `ToolConfig` entries, calls `GetEffectiveCommand` and `GetEffectiveRegex`,
   executes each command in an OS shell, and returns a `VersionInfo` record.
