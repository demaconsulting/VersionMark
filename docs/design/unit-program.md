# Program Unit

## Overview

The `Program` class (`Program.cs`) is the top-level entry point for the tool. It owns the
`Main` method, constructs the `Context`, dispatches to the appropriate mode, and handles
top-level exception translation.

## Version Property

The static `Version` property reads the assembly's `AssemblyInformationalVersionAttribute`
at runtime and falls back to `AssemblyVersion` or `"0.0.0"` if neither is available. This
satisfies requirement `VersionMark-Cmd-Version`.

## Main Method

`Main` creates a `Context` from the command-line arguments, calls `Run`, and returns
`context.ExitCode`. `ArgumentException` and `InvalidOperationException` are caught and
written to `Console.Error`, returning exit code 1. Unexpected exceptions are re-thrown to
generate event-log entries. This satisfies requirements `VersionMark-Cmd-ExitCode` and
`VersionMark-Cmd-ErrorOutput`.

## Run Method

`Run` implements priority-ordered dispatch:

| Priority | Condition              | Action                          |
|----------|------------------------|---------------------------------|
| 1        | `context.Version`      | Print version string and return |
| —        | Print banner           | Always executed after priority 1|
| 2        | `context.Help`         | Print usage and return          |
| 3        | `context.Validate`     | Run self-validation and return  |
| 4        | `context.Lint`         | Run lint mode and return        |
| 5        | `context.Capture`      | Run capture mode and return     |
| 6        | `context.Publish`      | Run publish mode and return     |
| 7        | Default                | Run placeholder tool logic      |

This dispatch order satisfies requirements `VersionMark-Cmd-Version`, `VersionMark-Cmd-Help`,
`VersionMark-Cmd-Validate`, `VersionMark-Cmd-Lint`, `VersionMark-Cap-Capture`, and
`VersionMark-Pub-Publish`.

## Capture and Publish Orchestration

`RunCapture` and `RunPublish` are private helpers called from `Run`. They validate required
arguments, invoke configuration loading and version capture/report generation, and delegate
error handling to `context.WriteError`. These methods satisfy requirements
`VersionMark-Cap-JobId`, `VersionMark-Cap-Output`, `VersionMark-Pub-RequireReport`, and
`VersionMark-Pub-GlobPattern`.

## RunLint

`RunLint` is a private helper called from `Run`. It resolves the configuration file path,
defaulting to `.versionmark.yaml` when `context.LintFile` is `null`, then delegates to
`Lint.Run`. This satisfies requirement `VersionMark-Cmd-Lint`.
