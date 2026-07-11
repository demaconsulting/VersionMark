### Context

![Cli Structure](CliView.svg)

#### Purpose

`Context` (`Context.cs`) is a sealed, disposable container for all parsed command-line
state and output routing. It is constructed via the `Create` factory method, which
tokenizes the argument array and populates the relevant flag and value properties. All
other subsystems receive a `Context` instance from `Program` and use it to query parsed
flags and to write output or errors.

#### Data Model

| Property       | Type       | Default     | Description                                        |
|----------------|------------|-------------|----------------------------------------------------|
| `Version`      | `bool`     | `false`     | `-v` / `--version` flag                            |
| `Help`         | `bool`     | `false`     | `-?`, `-h`, `--help` flag                          |
| `Silent`       | `bool`     | `false`     | `--silent` flag                                    |
| `Validate`     | `bool`     | `false`     | `--validate` flag                                  |
| `ResultsFile`  | `string?`  | `null`      | `--results <file>` / `--result <file>` (legacy alias) |
| `Lint`         | `bool`     | `false`     | `--lint` flag                                      |
| `LintFile`     | `string?`  | `null`      | Optional file argument for `--lint`                |
| `Capture`      | `bool`     | `false`     | `--capture` flag                                   |
| `JobId`        | `string?`  | `null`      | `--job-id <id>`                                    |
| `OutputFile`   | `string?`  | `null`      | `--output <file>`                                  |
| `ToolNames`    | `string[]` | `[]`        | Tool names after `--` separator in capture mode    |
| `Publish`      | `bool`     | `false`     | `--publish` flag                                   |
| `ReportFile`   | `string?`  | `null`      | `--report <file>`                                  |
| `Depth`        | `int`      | `1`         | `--depth <depth>` (heading depth, default: 1)      |
| `ReportDepth`  | `int`      | `Depth`     | `--report-depth <depth>` (defaults to `Depth`)     |
| `GlobPatterns` | `string[]` | `[]`        | Patterns after `--` separator in publish mode      |
| `ExitCode`     | `int`      | `0` / `1`   | 0 for success; 1 if any errors have been reported  |

The private `_hasErrors` field, set by `WriteError`, controls whether `ExitCode` returns 1.
An optional `StreamWriter` opened by `OpenLogFile` is disposed when `Context` is disposed.

#### Key Methods

**`Create(string[] args)` (static factory)** — Constructs a `Context` by delegating to the
private `ArgumentParser` class, which performs token-by-token parsing. The `--` separator
switches subsequent tokens to either tool names (capture mode) or glob patterns (publish
mode); using `--` outside capture or publish mode throws `ArgumentException`. Both
`--results` and its legacy alias `--result` set `ResultsFile`; the factory throws
`InvalidOperationException` if the log file specified via `--log` cannot be opened. Returns
the populated `Context`.

**`OpenLogFile(string path)`** — Opens a `StreamWriter` with `AutoFlush = true` to `path`.
Subsequent `WriteLine` and `WriteError` calls also write to this stream. If opening fails,
`InvalidOperationException` is thrown with contextual information.

**`WriteLine(string message)`** — Writes to `Console.Out` unless `Silent` is set, and
also writes to the log file if one was opened.

**`WriteError(string message)`** — Sets `_hasErrors = true` (making `ExitCode` return 1),
and also writes to the log file if one was opened. Writes to `Console.Error` in red unless
`Silent` is set. In silent mode all output is suppressed and callers detect failures via the
process exit code. This supports self-validation scenarios where the tool deliberately
triggers errors without producing unwanted output.

#### Error Handling

| Condition                         | Behavior                                               |
|-----------------------------------|--------------------------------------------------------|
| `null` `args` passed to `Create`  | `Create` throws `ArgumentNullException`                |
| Unknown argument token            | `ArgumentParser` throws `ArgumentException`            |
| `--` used outside capture/publish | `ArgumentParser` throws `ArgumentException`            |
| Log file cannot be opened         | `OpenLogFile` throws `InvalidOperationException`       |

#### Dependencies

- `System.Console` (BCL) — output to stdout and stderr.
- `System.IO.StreamWriter` (BCL) — log file output.

#### Callers

- `Program.Main` — constructs `Context` via `Context.Create` and passes it to `Program.Run`.
- `Program.Run`, `Program.RunCapture`, `Program.RunPublish`, `Program.RunLint` — query
  flags and call `WriteLine` / `WriteError`.
- `Validation.Run` and test helpers — construct a fresh `Context` for each self-validation
  test.
- `VersionMarkLoadResult.ReportIssues` — calls `context.WriteLine` and `context.WriteError`
  to report lint issues.
