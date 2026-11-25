---
title: Command-line options
summary: Every option of the tool, and the order in which a run handles them.
aliases:
  - options
  - flags
tags:
  - reference
order: 2
see-also:
  - troubleshooting
  - configuration
examples:
  - name: usage
    kind: command
    language: shell
    description: Print the short usage text. It ends with a pointer to this help.
    content: '{{tool}} --help'
    expect: "Run '{{tool}} help' for topics and examples."
  - name: silent-log
    kind: command
    language: shell
    description: Hide the console output of a capture and keep it in a log file.
    content: '{{tool}} --silent --log versionmark.log --capture --job-id build'
  - name: missing-value
    kind: command
    language: shell
    description: An option without its value is an error. Exits 1.
    content: '{{tool}} --capture --job-id'
    exit: 1
    expect: '--job-id requires a job ID argument'
---
Options start with `--` (or a single character after `-`) and may be given in any order.

| Option | Meaning |
| --- | --- |
| `-v`, `--version` | Print only the version. |
| `-?`, `-h`, `--help` | Print the short usage text. |
| `--silent` | Hide console output. Errors still set exit code 1. |
| `--log <file>` | Also write the output to a file, replacing it. |
| `--lint [<file>]` | Check a configuration file; see [[configuration]]. |
| `--capture` | Capture tool versions; see [[capture]]. |
| `--job-id <id>` | The id of the CI job; required with `--capture`. |
| `--output <file>` | The capture file; default `versionmark-<job-id>.json`. |
| `--publish` | Write a version report; see [[publish]]. |
| `--report <file>` | The Markdown report; required with `--publish`. |
| `--report-depth <n>` | First heading level of the report, 1 to 6; default `--depth`. |
| `--validate` | Run the self-tests; see [[self-validation]]. |
| `--results <file>` | Write the self-test results as `.trx` or `.xml`. |
| `--depth <n>` | Heading level of the self-test report, 1 to 6; default 1. |
| `-- <names>` | Tool names to capture, or file patterns to publish. |

`--result` is an older name of `--results` and still works.

**Order.** `--version`, when all arguments are valid, prints only the version and skips everything else.
Otherwise the tool prints a banner (not
for a lint run), then handles the first of `--help`, `--validate`, `--lint`, `--capture`, and `--publish`. Without
any of them it prints only the banner and a placeholder text.
The command that [[#silent-log]] prints writes its output to the log file only.

**Values.** `--lint` takes the next argument as the file name unless it starts with `-`; without one it checks
`.versionmark.yaml`. `--` ends the options: the rest of the line is tool names after `--capture`, or file patterns
after `--publish`. Anywhere else `--` is an error.

**Errors.** An unknown argument, an option without its value, or a depth outside 1 to 6 prints `Error: ...` and
exits 1. These errors are found before `--silent` and `--log` take effect, so they are always printed and never
logged. The command that [[#missing-value]] prints shows this.

**Help.** `help` is a command, not an option, and must be the first argument. `--silent` and `--log` do not apply
to it, and an unknown topic exits 2.
