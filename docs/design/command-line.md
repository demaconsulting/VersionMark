# Command Line

## Overview

The command-line layer is responsible for parsing command-line arguments, routing program
flow to the appropriate subsystem, and managing all output (console, error, and log file).
It consists of two classes: `Program` (the entry point) and `Context` (the argument and
output container).

## Program Class

The `Program` class (`Program.cs`) is the top-level entry point for the tool. It owns the
`Main` method, constructs the `Context`, dispatches to the appropriate mode, and handles
top-level exception translation.

### Version Property

The static `Version` property reads the assembly's `AssemblyInformationalVersionAttribute`
at runtime and falls back to `AssemblyVersion` or `"0.0.0"` if neither is available. This
satisfies requirement `VersionMark-Cmd-Version`.

### Main Method

`Main` creates a `Context` from the command-line arguments, calls `Run`, and returns
`context.ExitCode`. `ArgumentException` and `InvalidOperationException` are caught and
written to `Console.Error`, returning exit code 1. Unexpected exceptions are re-thrown to
generate event-log entries. This satisfies requirements `VersionMark-Cmd-ExitCode` and
`VersionMark-Cmd-ErrorOutput`.

### Run Method

`Run` implements priority-ordered dispatch:

| Priority | Condition              | Action                          |
|----------|------------------------|---------------------------------|
| 1        | `context.Version`      | Print version string and return |
| —        | Print banner           | Always executed after priority 1|
| 2        | `context.Help`         | Print usage and return          |
| 3        | `context.Validate`     | Run self-validation and return  |
| 4        | `context.Capture`      | Run capture mode and return     |
| 4.5      | `context.Publish`      | Run publish mode and return     |
| 5        | Default                | Run placeholder tool logic      |

This dispatch order satisfies requirements `VersionMark-Cmd-Version`, `VersionMark-Cmd-Help`,
`VersionMark-Cmd-Validate`, `VersionMark-Cap-Capture`, and `VersionMark-Pub-Publish`.

### Capture and Publish Orchestration

`RunCapture` and `RunPublish` are private helpers called from `Run`. They validate required
arguments, invoke configuration loading and version capture/report generation, and delegate
error handling to `context.WriteError`. These methods satisfy requirements
`VersionMark-Cap-JobId`, `VersionMark-Cap-Output`, `VersionMark-Pub-RequireReport`, and
`VersionMark-Pub-GlobPattern`.

## Context Class

The `Context` class (`Context.cs`) is a sealed, disposable container for all parsed
command-line state and output routing. It is constructed via the `Create` factory method.

### Properties

| Property      | Type       | Default | Description                               |
|---------------|------------|---------|-------------------------------------------|
| `Version`     | `bool`     | `false` | `-v` / `--version` flag                   |
| `Help`        | `bool`     | `false` | `-?`, `-h`, `--help` flag                 |
| `Silent`      | `bool`     | `false` | `--silent` flag                           |
| `Validate`    | `bool`     | `false` | `--validate` flag                         |
| `ResultsFile` | `string?`  | `null`  | `--results <file>`                        |
| `Capture`     | `bool`     | `false` | `--capture` flag                          |
| `JobId`       | `string?`  | `null`  | `--job-id <id>`                           |
| `OutputFile`  | `string?`  | `null`  | `--output <file>`                         |
| `ToolNames`   | `string[]` | `[]`    | Tool names after `--` separator in capture|
| `Publish`     | `bool`     | `false` | `--publish` flag                          |
| `ReportFile`  | `string?`  | `null`  | `--report <file>`                         |
| `ReportDepth` | `int`      | `2`     | `--report-depth <depth>`                  |
| `GlobPatterns`| `string[]` | `[]`    | Patterns after `--` separator in publish  |
| `ExitCode`    | `int`      | `0`/`1` | 0 for success, 1 if errors reported       |

This satisfies requirements `VersionMark-Cmd-Context`, `VersionMark-Cmd-Version`,
`VersionMark-Cmd-Help`, `VersionMark-Cmd-Silent`, `VersionMark-Cmd-Validate`,
`VersionMark-Cmd-Results`, `VersionMark-Cmd-Log`, `VersionMark-Cmd-ExitCode`.

### ArgumentParser

The private `ArgumentParser` class performs the actual token-by-token parsing. It handles
the `--` separator, which switches subsequent tokens to either tool names (capture mode) or
glob patterns (publish mode). Unknown arguments throw `ArgumentException`, satisfying
`VersionMark-Cmd-InvalidArgs`.

### WriteLine and WriteError

`WriteLine` writes to `Console.Out` unless `Silent` is set, and also writes to the log
file if one was opened. `WriteError` additionally sets `_hasErrors = true` (making
`ExitCode` return 1) and writes to `Console.Error` in red. This satisfies
`VersionMark-Cmd-Silent`, `VersionMark-Cmd-ErrorOutput`, and `VersionMark-Cmd-ExitCode`.

### Log File

The `OpenLogFile` method opens a `StreamWriter` with `AutoFlush = true`. If opening fails,
an `InvalidOperationException` is thrown with contextual information. The writer is
disposed when `Context` is disposed, satisfying `VersionMark-Cmd-Log`.
