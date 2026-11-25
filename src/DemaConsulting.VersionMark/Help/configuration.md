---
title: Configuration file
summary: Write .versionmark.yaml, the list of tools to capture, and check it with --lint.
aliases:
  - config
  - versionmark-yaml
  - lint
  - regex
tags:
  - reference
order: 3
see-also:
  - capture
  - troubleshooting
examples:
  - name: lint
    kind: command
    language: shell
    description: Check .versionmark.yaml in the current folder. Prints nothing when it is valid.
    content: '{{tool}} --lint'
  - name: lint-missing
    kind: command
    language: shell
    description: Check a file that does not exist. Exits 1.
    content: '{{tool}} --lint missing.yaml'
    exit: 1
    expect: 'missing.yaml(1,1): error: Configuration file not found'
---
`--capture` always reads `.versionmark.yaml` in the current folder. `--lint <file>` checks any file.

A configuration that captures the .NET SDK, Git, and the operating system version:

```yaml
---
tools:
  dotnet:
    command: dotnet --version
    regex: '(?<version>\d+\.\d+\.\d+)'
  git:
    command: git --version
    regex: 'git version (?<version>\d+\.\d+\.\d+)'
  os:
    command: uname -r
    command-win: ver
    regex: '(?<version>\d+\.\d+\.\d+)'
```

**Keys.** `tools` maps each tool name to its settings, and needs at least one tool. A tool needs a `command`
that prints its version and a `regex` that finds the version in that output. The tool names are the keys of the
capture file and the names in the report.

**Operating systems.** `command-win`, `command-linux`, and `command-macos` replace `command` on that operating
system, and `regex-win`, `regex-linux`, and `regex-macos` replace `regex`. In the example, `os` runs `ver` on
Windows and `uname -r` elsewhere. A tool without `command` or `regex` must have the system key in its place
for every system it is captured on.

**Regex.** The expression must contain a group named `version`, as in `(?<version>\d+\.\d+\.\d+)`. It is matched
without regard to case, `^` and `$` match at every line, and the first match wins. Single quotes in YAML keep the
backslashes as they are.

**Commands.** A command runs through `cmd.exe /c` on Windows and `/bin/sh -c` elsewhere, so pipes and shell
built-ins work. Its standard output and standard error are both searched. A command that exits with a code other
than 0 fails the capture. On Windows, quote an executable path that contains spaces.

**Lint.** `--lint` prints no banner, and nothing at all when the file is valid;
see [[#lint]]. Each issue is one line:

```text
<file>(<line>,<column>): <warning|error>: <message>
```

All issues are reported in one pass. Unknown keys are warnings. Missing or empty commands and expressions, an
invalid expression, or one without a `version` group are errors. The run exits 1 only when there are errors,
as [[#lint-missing]] shows.
A capture checks the file the same way first, and stops on errors.
