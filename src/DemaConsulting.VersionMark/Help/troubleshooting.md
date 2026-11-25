---
title: Errors and exit codes
summary: What the exit codes mean, and how to fix the common error messages.
aliases:
  - errors
  - exit-codes
tags:
  - troubleshooting
order: 7
see-also:
  - command-line
  - configuration
  - capture
  - publish
examples:
  - name: unknown-option
    kind: command
    language: shell
    description: An unknown argument. Exits 1.
    content: '{{tool}} --unknown'
    exit: 1
    expect: "Error: Unsupported argument '--unknown'"
  - name: silent-error
    kind: command
    language: shell
    description: An argument error is printed even with --silent. Exits 1.
    content: '{{tool}} --silent --unknown'
    exit: 1
    expect: "Error: Unsupported argument '--unknown'"
  - name: separator
    kind: command
    language: shell
    description: The -- separator without --capture or --publish before it. Exits 1.
    content: '{{tool}} -- dotnet'
    exit: 1
    expect: 'The -- separator is only valid in capture or publish mode'
  - name: unknown-topic
    kind: command
    language: shell
    description: A help topic that does not exist. Exits 2.
    content: '{{tool}} help no-such-topic'
    exit: 2
    expect: "Unknown help card 'no-such-topic'"
---
**Exit codes.**

| Code | Meaning |
| --- | --- |
| 0 | Success, also for a lint run with warnings only. |
| 1 | An error, a lint error, or a failed self-test. |
| 2 | A wrong `help` argument or an unknown help topic. |

A line `Unexpected error: ...` means a defect in the tool; please report it.

Errors are printed to standard error and set the exit code. Errors found during a run, such as a failed command,
are hidden by `--silent` and kept in the `--log` file. Errors in the arguments, and a log file that cannot be
opened, are found before the run starts: they are always printed and never reach the log file.
The command that [[#silent-error]] prints shows one despite `--silent`.

**Common messages.**

- `--job-id is required for capture mode`: add `--job-id <id>`; see [[capture]].
- `Configuration file not found`: `--capture` reads `.versionmark.yaml` in the current folder. Run the tool there.
- `Failed to parse YAML file`: the file is not valid YAML. The message gives the line and column.
- `must contain a named 'version' capture group`: add `(?<version>...)` to the expression;
  see [[configuration]].
- `Tool '<name>' not found in configuration`: a name after `--` is not a key under `tools`.
- `Failed to run command`: the command exited with an error or could not start. Run it by hand in the same
  folder; the message shows its error output.
- `Failed to extract version`: the expression does not match the output of the command. Run the command by hand
  and compare.
- `No default command specified`: the tool has only commands for other operating systems. Add `command`.
- `No default regex specified`: the tool has only expressions for other operating systems. Add `regex`.
- `--report is required for publish mode`: add `--report <file>`; see [[publish]].
- `No JSON files found matching patterns`: no capture file is in the current folder, or the patterns are wrong.
- `Failed to parse JSON file`: a capture file is not valid JSON. Capture it again.
- `Failed to open log file`: the folder of `--log` does not exist or the file is locked.
- `Unsupported argument`: check the spelling against [[command-line]]. `help` must come first.
  The command that [[#unknown-option]] prints shows the error.
- `The -- separator is only valid in capture or publish mode`: put `--capture` or `--publish` before `--`, as
  the command that [[#separator]] prints shows.
- `Unknown help card`: the topic does not exist. The message lists the topics.
  The command that [[#unknown-topic]] prints shows the error.
