# Context Unit

## Overview

The `Context` class (`Context.cs`) is a sealed, disposable container for all parsed
command-line state and output routing. It is constructed via the `Create` factory method.

## Properties

| Property      | Type       | Default | Description                               |
|---------------|------------|---------|-------------------------------------------|
| `Version`     | `bool`     | `false` | `-v` / `--version` flag                   |
| `Help`        | `bool`     | `false` | `-?`, `-h`, `--help` flag                 |
| `Silent`      | `bool`     | `false` | `--silent` flag                           |
| `Validate`    | `bool`     | `false` | `--validate` flag                         |
| `ResultsFile` | `string?`  | `null`  | `--results <file>`                        |
| `Lint`        | `bool`     | `false` | `--lint` flag                             |
| `LintFile`    | `string?`  | `null`  | Optional file argument for `--lint`       |
| `Capture`     | `bool`     | `false` | `--capture` flag                          |
| `JobId`       | `string?`  | `null`  | `--job-id <id>`                           |
| `OutputFile`  | `string?`  | `null`  | `--output <file>`                         |
| `ToolNames`   | `string[]` | `[]`    | Tool names after `--` separator in capture|
| `Publish`     | `bool`     | `false` | `--publish` flag                          |
| `ReportFile`  | `string?`  | `null`  | `--report <file>`                         |
| `Depth`       | `int`      | `1`     | `--depth <depth>` (heading depth, def: 1) |
| `ReportDepth` | `int`      | `Depth` | `--report-depth <depth>` (def: `Depth`)   |
| `GlobPatterns`| `string[]` | `[]`    | Patterns after `--` separator in publish  |
| `ExitCode`    | `int`      | `0`/`1` | 0 for success, 1 if errors reported       |

This satisfies requirements `VersionMark-CommandLine-Context`, `VersionMark-CommandLine-Version`,
`VersionMark-CommandLine-Help`, `VersionMark-CommandLine-Silent`, `VersionMark-CommandLine-Validate`,
`VersionMark-CommandLine-Results`, `VersionMark-CommandLine-Log`, `VersionMark-CommandLine-ExitCode`,
`VersionMark-CommandLine-Lint`, and `VersionMark-Context-Create`.

## ArgumentParser

The private `ArgumentParser` class performs the actual token-by-token parsing. It handles
the `--` separator, which switches subsequent tokens to either tool names (capture mode) or
glob patterns (publish mode). Unknown arguments throw `ArgumentException`, satisfying
`VersionMark-CommandLine-InvalidArgs`.

## WriteLine and WriteError

`WriteLine` writes to `Console.Out` unless `Silent` is set, and also writes to the log
file if one was opened. `WriteError` additionally sets `_hasErrors = true` (making
`ExitCode` return 1) and writes to `Console.Error` in red. This satisfies
`VersionMark-CommandLine-Silent`, `VersionMark-CommandLine-ErrorOutput`, `VersionMark-CommandLine-ExitCode`,
`VersionMark-Context-WriteLine`, `VersionMark-Context-WriteError`, and `VersionMark-Context-WriteErrorExitCode`.

## Log File

The `OpenLogFile` method opens a `StreamWriter` with `AutoFlush = true`. If opening fails,
an `InvalidOperationException` is thrown with contextual information. The writer is
disposed when `Context` is disposed, satisfying `VersionMark-CommandLine-Log`.
