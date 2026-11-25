---
title: Publish a version report
summary: Combine the JSON files of every job into one Markdown version report.
aliases:
  - report
  - markdown
tags:
  - reference
order: 5
see-also:
  - capture
  - troubleshooting
examples:
  - name: publish
    kind: command
    language: shell
    description: Combine every versionmark-*.json file in the current folder into one report.
    content: '{{tool}} --publish --report versions.md'
    expect: 'Found 2 JSON file(s)'
  - name: patterns
    kind: command
    language: shell
    description: Name the capture files with patterns after --.
    content: '{{tool}} --publish --report versions.md -- "versionmark-linux.json" "versionmark-windows.json"'
    expect: 'Loaded version information from 2 file(s)'
  - name: depth
    kind: command
    language: shell
    description: Start the report at heading level 2 to embed it in a larger document.
    content: '{{tool}} --publish --report versions.md --report-depth 2'
    expect: 'Report successfully written to versions.md'
  - name: no-report
    kind: command
    language: shell
    description: A publish without a report file. Exits 1.
    content: '{{tool}} --publish'
    exit: 1
    expect: 'Error: --report is required for publish mode'
  - name: no-files
    kind: command
    language: shell
    description: Patterns that match no file. Exits 1.
    content: '{{tool}} --publish --report versions.md -- "none-*.json"'
    exit: 1
    expect: 'No JSON files found matching patterns: none-*.json'
---
`--publish` reads the capture files that `--capture` wrote (see [[capture]])
and writes one Markdown report to the file of `--report`, which is required,
as [[#no-report]] shows.
The examples read `versionmark-linux.json` and `versionmark-windows.json`, the captures of two jobs named `linux`
and `windows`.

**Inputs.** Without patterns the tool reads every `versionmark-*.json` file in the current folder,
as [[#publish]] shows. Patterns after `--` choose other files,
as [[#patterns]] shows.
Patterns are relative to the current folder unless absolute, and support `*` within a folder and `**` for any
folders. A file that matches two patterns is read once. Quote patterns so that the shell does not expand them.
No matching file is an error, as [[#no-files]] shows.

**Report.** The report has a heading `Tool Versions` and one bullet per tool, sorted by name. When every job
captured the same version, the bullet shows only the version, such as `- **dotnet**: 10.0.100`. Otherwise there is
one bullet per version, with the jobs that captured it: `- **git**: 2.43.0 (linux)` and
`- **git**: 2.47.1 (windows)`. A tool that a job did not capture is simply not listed for that job.

**Depth.** `--report-depth` sets the level of the heading, 1 to 6, so the report can be included in a larger
document, as [[#depth]] shows.
Its default is the value of `--depth`, which is 1 when not given.

The report file is overwritten, and its folder must exist.
