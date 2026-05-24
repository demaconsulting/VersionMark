### Program

#### Purpose

`Program` (`Program.cs`) is the top-level entry point for VersionMark. It owns the `Main`
method, constructs the `Context` from command-line arguments, dispatches to the appropriate
operational mode, and handles top-level exception translation. It also contains the private
helpers `RunCapture`, `RunPublish`, and `RunLint` that implement each operational mode.

#### Data Model

`Program` has no instance state. The single static `Version` property reads the
assembly's `AssemblyInformationalVersionAttribute` at runtime, falling back to
`AssemblyVersion` or `"0.0.0"` if neither is available.

#### Key Methods

**`Main(string[] args)` (static)** — Constructs a `Context` from command-line arguments,
calls `Run`, and returns `context.ExitCode`. `ArgumentException` and
`InvalidOperationException` are caught and written to `Console.Error`, returning exit code

1. Unexpected exceptions are re-thrown to generate event-log entries.

**`Run(Context context)` (static)** — Implements the following priority-ordered dispatch:

| Priority | Condition          | Action                                                         |
|----------|--------------------|----------------------------------------------------------------|
| 1        | `context.Version`  | Print version string and return                                |
| —        | Print banner       | Executed after priority 1; **skipped when lint is dispatched** |
| 2        | `context.Help`     | Print usage and return                                         |
| 3        | `context.Validate` | Run self-validation and return                                 |
| 4        | `context.Lint`     | Run lint mode and return                                       |
| 5        | `context.Capture`  | Run capture mode and return                                    |
| 6        | `context.Publish`  | Run publish mode and return                                    |
| 7        | Default            | Print placeholder message                                      |

**`RunCapture(Context context)` (private static)** — Validates required arguments
(`--job-id`), resolves the default output file name (`versionmark-<job-id>.json`), loads
the configuration via `VersionMarkConfig.Load`, reports lint issues, calls
`VersionMarkConfig.FindVersions`, and saves the result with `VersionInfo.SaveToFile`.

**`RunPublish(Context context)` (private static)** — Validates required arguments
(`--report`), resolves capture files via `GlobMatcher.FindMatchingFiles`, loads each with
`VersionInfo.LoadFromFile`, generates the report with `MarkdownFormatter.Format`, and
writes it to the report file.

**`RunLint(Context context)` (private static)** — Resolves the configuration file path
(defaulting to `.versionmark.yaml` when `context.LintFile` is null), calls
`VersionMarkConfig.Load`, and reports all discovered issues via `result.ReportIssues`.

#### Error Handling

| Condition                              | Behavior                                                           |
|----------------------------------------|--------------------------------------------------------------------|
| `ArgumentException` or `InvalidOperationException` from any mode | `context.WriteError`; `ExitCode` set to 1 |
| Unexpected exception from `Main`       | Re-thrown to propagate as unhandled exception                      |
| `--job-id` missing in capture mode     | `context.WriteError`; return                                       |
| `--report` missing in publish mode     | `context.WriteError`; return                                       |
| No files match glob patterns           | `context.WriteError` listing the patterns; return                  |
| Lint errors found                      | `context.WriteError` per issue; `ExitCode` set to 1                |

#### Dependencies

- `Context` (Cli subsystem) — command-line state and output routing.
- `VersionMarkConfig` (Configuration subsystem) — configuration loading and version capture.
- `VersionInfo` (Capture subsystem) — JSON serialization of captured versions.
- `MarkdownFormatter` (Publishing subsystem) — markdown report generation.
- `GlobMatcher` (Utilities subsystem) — glob-pattern resolution of capture files.
- `Validation` (SelfTest subsystem) — self-validation test runner.

#### Callers

- `Main` is called by the .NET runtime as the application entry point.
- `Validation.RunCaptureTest`, `Validation.RunPublishTest`, `Validation.RunLintValidTest`,
  and `Validation.RunLintInvalidTest` call `Program.Run` re-entrantly for self-validation.
