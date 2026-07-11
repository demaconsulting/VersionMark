### Validation

![SelfTest Structure](SelfTestView.svg)

#### Purpose

`Validation` (`Validation.cs`) implements the built-in self-validation suite for
VersionMark. It is invoked when `--validate` is passed to the tool. It runs four
functional tests — capture, publish, lint-valid, and lint-invalid — each in an isolated
temporary directory using a re-entrant call to `Program.Run`. Results are printed to the
context output and optionally written to a TRX or JUnit XML file.

#### Data Model

`Validation` is a static class with no instance state. Each test is named and tracked in a
`TestResults` collection (from `DemaConsulting.TestResults`) named
`"VersionMark Self-Validation"`.

Test names:

| Test name                                          | Validates                        |
|----------------------------------------------------|----------------------------------|
| `VersionMark_CapturesVersions`                     | Capture mode end-to-end          |
| `VersionMark_GeneratesMarkdownReport`              | Publish mode end-to-end          |
| `VersionMark_LintPassesForValidConfig`             | Lint mode: valid config exit 0   |
| `VersionMark_LintReportsErrorsForInvalidConfig`    | Lint mode: invalid config exit 1 |

#### Key Methods

**`Run(Context context)` (static)** — Orchestrates the self-validation sequence:

1. Calls `PrintValidationHeader` to emit a markdown heading and environment info table.
2. Creates a `TestResults` collection.
3. Calls all four test helpers in order.
4. Prints a pass/fail summary; calls `context.WriteError` if any tests failed.
5. If `context.ResultsFile` is set, calls `WriteResultsFile`.

**`RunCaptureTest(Context context, TestResults results)` (private static)** — Creates a
`TemporaryDirectory`, writes a minimal `.versionmark.yaml` containing only `dotnet`,
constructs a fresh `Context` with `--silent`, `--capture`, `--job-id test-job`, and
`--output`, changes the current directory to the temp directory, calls `Program.Run`, then
verifies exit code is 0, the output file exists, `JobId` equals `"test-job"`, and the
`dotnet` version is non-empty.

**`RunPublishTest(Context context, TestResults results)` (private static)** — Creates a
`TemporaryDirectory`, writes two `VersionInfo` JSON files with known content, constructs a
fresh `Context` with `--silent`, `--publish`, `--report`, and `-- versionmark-*.json`,
calls `Program.Run`, then verifies exit code is 0, the report file exists, and contains
`## Tool Versions`, `**dotnet**`, `**node**`, `8.0.0`, and `20.0.0`.

**`RunLintValidTest(Context context, TestResults results)` (private static)** — Creates a
`TemporaryDirectory`, writes a minimal `.versionmark.yaml` with a valid `dotnet` tool
entry (including a `(?<version>...)` capture group), constructs a fresh `Context` with
`--silent` and `--lint <file>`, calls `Program.Run`, and verifies exit code is 0.

**`RunLintInvalidTest(Context context, TestResults results)` (private static)** — Creates
a `TemporaryDirectory`, writes a `bad.versionmark.yaml` with a tool entry that has only a
`command` field and no `regex`, constructs a fresh `Context`, calls `Program.Run`, and
verifies exit code is non-zero.

**`WriteResultsFile(Context context, TestResults results)` (private static)** — Inspects
the extension of `context.ResultsFile`: `.trx` → `TrxSerializer.Serialize`; `.xml` →
`JUnitSerializer.Serialize`; other → `context.WriteError`.

#### Error Handling

| Condition                                     | Behavior                                         |
|-----------------------------------------------|--------------------------------------------------|
| Any test assertion fails                      | Failure recorded in `TestResults`; `context.WriteError` called; `ExitCode` set to 1 |
| `TemporaryDirectory` creation fails           | `InvalidOperationException` wrapping the original exception |
| `context.ResultsFile` extension unrecognized  | `context.WriteError`; no file written            |
| `Dispose` on `TemporaryDirectory` fails       | `IOException`/`UnauthorizedAccessException` silently suppressed |

#### Dependencies

- `Context` (Cli subsystem) — output routing and parsed flag values.
- `Program` (Cli subsystem) — called re-entrantly for each functional test.
- `VersionInfo` (Capture subsystem) — constructs and reads JSON data in publish test.
- `PathHelpers` (Utilities subsystem) — used by `TemporaryDirectory` to construct safe
  paths under `Path.GetTempPath()`.
- `DemaConsulting.TestResults` (OTS) — `TestResults`, `TrxSerializer`, `JUnitSerializer`.

#### Callers

- `Program.Run` (validate dispatch, priority 3) — calls `Validation.Run` when `--validate` is specified.
