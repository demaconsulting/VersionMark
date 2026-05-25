### VersionMarkConfig

#### Purpose

`VersionMarkConfig` is the top-level record for a loaded `.versionmark.yaml` configuration
file. It holds the full set of tool definitions and exposes the `Load` method, which
parses and validates the YAML file in a single pass, accumulating all warnings and errors.
It also provides `FindVersions`, which executes the configured commands to capture tool
version strings for the current CI/CD job.

#### Data Model

| Property | Type                              | Description                                   |
|----------|-----------------------------------|-----------------------------------------------|
| `Tools`  | `Dictionary<string, ToolConfig>`  | Maps tool names to their per-OS configuration |

The dictionary is populated during `Load`. Tools with error-level issues are excluded;
tools with only warnings are included.

#### Key Methods

**`Load(string filePath)` (static)** — Primary entry point for loading and validating the
configuration file. Performs the following steps:

1. Checks file existence; adds an error issue if absent.
2. Parses the YAML stream; captures parse errors as error-level issues with source location.
3. Validates the root node is a mapping and locates the `tools` key.
4. Iterates all tool entries, calling the private `ValidateTool` for each.
5. Validates at least one tool is present.
6. Returns a `VersionMarkLoadResult` with the loaded `VersionMarkConfig` (or `null` if any
   errors were found) and the complete issue list.

The accumulate-and-continue strategy collects all issues in a single pass. A
`toolIssuesBefore` snapshot isolates per-tool validation: when any new error-severity
issues are added while processing a tool, that tool is excluded from the result rather
than contributing a broken entry.

**`ReadFromFile(string filePath)` (static)** — Backward-compatibility wrapper that calls
`Load` and throws `ArgumentException` if any error-level issues are present. Use `Load`
directly when access to lint issues is needed.

**`FindVersions(IEnumerable<string> toolNames, string jobId, string? os = null)`** —
Resolves the OS once (`os ?? ToolConfig.GetCurrentOs()`), then for each named tool:
looks up the `ToolConfig`, calls `GetEffectiveCommand` and `GetEffectiveRegex` with the
resolved OS, calls the private `RunCommand` helper, calls `ExtractVersion`, and stores
the result. Returns a `VersionInfo` record.

**`ValidateTool(string name, YamlMappingNode node, ...)` (private)** — Processes a single
tool's `YamlMappingNode`. Iterates all key-value pairs, populating command and regex
dictionaries. Reports unknown keys as warnings, empty values as errors, and calls
`TryCompileRegex` to validate each regex entry and verify the `version` named capture
group is present. Returns `null` for that tool if any new errors were added.

**`TryCompileRegex(string pattern, ...)` (private)** — Compiles the pattern with
`RegexOptions.Multiline | RegexOptions.IgnoreCase` and a one-second timeout. Appends an
error-level issue on compilation failure and returns `null`; returns the compiled `Regex`
on success.

**`RunCommand(string command)` (private)** — Runs the command through the OS shell
(`cmd.exe /c` on Windows, `/bin/sh -c` on other platforms) using `Process.Start` with
redirected stdout and stderr. Streams are read asynchronously to prevent pipe-deadlock.
Throws `InvalidOperationException` on non-zero exit code.

**`ExtractVersion(string output, string regexPattern, string toolName)` (private)** —
Compiles the regex, matches against the command output, and returns the value of the named
`version` capture group. The `toolName` parameter is included solely to produce actionable
error messages that identify which tool's version could not be extracted. Throws
`InvalidOperationException` when no match or group is found.

#### Error Handling

| Condition                                   | Behavior                                             |
|---------------------------------------------|------------------------------------------------------|
| File does not exist                         | Error `LintIssue` added; `Config` returns null       |
| YAML parse error                            | Error `LintIssue` with source location; parse stops  |
| Root node is not a mapping                  | Error `LintIssue`; `Config` returns null             |
| Missing `tools` key                         | Error `LintIssue`; `Config` returns null             |
| No tools defined                            | Error `LintIssue`; `Config` returns null             |
| Unknown YAML key in tool entry              | Warning `LintIssue`; tool otherwise valid            |
| Invalid regex pattern                       | Error `LintIssue`; tool excluded from result         |
| Missing `version` capture group in regex    | Error `LintIssue`; tool excluded from result         |
| Command exits with non-zero code            | `InvalidOperationException` in `FindVersions`        |
| Version group not found in command output   | `InvalidOperationException` in `FindVersions`        |
| Error-level issues present in `ReadFromFile`| `ArgumentException` thrown                           |

#### Dependencies

- `ToolConfig` (this unit's companion in the same file) — per-tool configuration.
- `LintIssue`, `VersionMarkLoadResult` (Configuration subsystem) — issue and result types.
- `VersionInfo` (Capture subsystem) — return type of `FindVersions`.
- `YamlDotNet` (OTS) — YAML parsing.
- `System.Text.RegularExpressions` (BCL) — regex compilation in `TryCompileRegex` and
  `ExtractVersion`.
- `System.Diagnostics.Process` (BCL) — shell command execution in `RunCommand`.

#### Callers

- `Program.RunCapture` — calls `VersionMarkConfig.Load` then `FindVersions`.
- `Program.RunLint` — calls `VersionMarkConfig.Load` to validate configuration.
- `Validation.RunCaptureTest`, `Validation.RunLintValidTest`,
  `Validation.RunLintInvalidTest` — exercise `Load` indirectly via `Program.Run`.
