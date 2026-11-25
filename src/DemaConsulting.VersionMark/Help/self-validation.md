---
title: Self-validation
summary: Prove that the installed tool works, and save the evidence as test results.
aliases:
  - validate
tags:
  - reference
order: 6
see-also:
  - command-line
examples:
  - name: run
    kind: command
    language: shell
    description: Run the self-tests. Prints one line per test and exits 0 when all pass.
    content: '{{tool}} --validate'
    expect: 'Failed: 0'
  - name: trx
    kind: command
    language: shell
    description: Also write the results as a TRX file.
    content: '{{tool}} --validate --results validation.trx'
  - name: junit
    kind: command
    language: shell
    description: Write the results as JUnit XML instead.
    content: '{{tool}} --validate --results validation.xml'
  - name: depth
    kind: command
    language: shell
    description: Start the self-test report at heading level 2.
    content: '{{tool}} --validate --depth 2'
    expect: '## DEMA Consulting VersionMark'
  - name: bad-extension
    kind: command
    language: shell
    description: Any other extension is an error. Exits 1.
    content: '{{tool}} --validate --results validation.txt'
    exit: 1
    expect: 'Unsupported results file format'
---
`--validate` runs a short set of tests of the tool itself in a temporary folder, with configuration and capture
files it writes there, so a regulated project can show that the installed version works on its build machine.
The capture test runs `dotnet --version`, so `dotnet` must be on the path.

**Report.** The output is Markdown. It starts with a heading and a table of the tool version, machine name,
operating system, .NET runtime, and time stamp. Each test prints one line ending in `PASSED` or `FAILED` with the
reason, and the report ends with `Total Tests`, `Passed`, and `Failed` lines. The exit code is 0 only when every
test passes. `--depth` sets the level of the heading, as [[#depth]] shows.

**Results files.** `--results <file>` also writes the outcome; the extension chooses the format. `.trx` writes a
Visual Studio TRX file and `.xml` writes JUnit XML, both readable by CI test-report tasks. The folder of the file
must exist. Any other extension is an error after the tests ran,
as [[#bad-extension]] shows.

**Evidence.** The tests have stable names, `VersionMark_CapturesVersions`, `VersionMark_GeneratesMarkdownReport`,
`VersionMark_LintPassesForValidConfig`, and `VersionMark_LintReportsErrorsForInvalidConfig`, so a requirements file
can list them as the tests that prove the tool works in its environment.
