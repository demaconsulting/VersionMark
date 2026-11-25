---
title: Capture tool versions
summary: Run the configured commands in a CI job and save the tool versions as JSON.
aliases:
  - job-id
  - json
tags:
  - reference
order: 4
see-also:
  - configuration
  - publish
examples:
  - name: all
    kind: command
    language: shell
    description: Capture every tool in .versionmark.yaml for the job 'build'.
    content: '{{tool}} --capture --job-id build'
    expect: 'Captured versions:'
  - name: some
    kind: command
    language: shell
    description: Capture only the tools named after --.
    content: '{{tool}} --capture --job-id build -- dotnet git'
    expect: 'Capturing 2 tool(s)...'
  - name: output
    kind: command
    language: shell
    description: Save the capture under another file name.
    content: '{{tool}} --capture --job-id build --output versions.json'
    expect: 'Version information saved to versions.json'
  - name: no-job-id
    kind: command
    language: shell
    description: A capture without a job id. Exits 1.
    content: '{{tool}} --capture'
    exit: 1
    expect: 'Error: --job-id is required for capture mode'
  - name: unknown-tool
    kind: command
    language: shell
    description: A tool name that the configuration does not list. Exits 1.
    content: '{{tool}} --capture --job-id build -- node'
    exit: 1
    expect: "Error: Tool 'node' not found in configuration"
---
`--capture` reads `.versionmark.yaml` in the current folder (see [[configuration]]),
runs the command of each tool, and finds its version with the regular expression. Issues in the file are printed
first, and errors stop the capture.
The command that [[#all]] prints captures every tool.

**Tools.** Tool names after `--` limit the capture to those tools,
as [[#some]] shows. A name that the file does not list is an error,
as [[#unknown-tool]] shows. When one tool fails, nothing is saved.

**Output.** The versions are saved as indented JSON with two properties: `JobId`, the value of `--job-id`, and
`Versions`, which maps each tool name to its version. The default file name is `versionmark-<job-id>.json` in the
current folder; `--output <file>` chooses another, as [[#output]] shows.
An existing file is overwritten, and the folder must exist. The console lists the captured versions and ends with
`Version information saved to <file>`.

**Jobs.** Give every CI job its own job id, such as the operating system or the build configuration, and upload
the capture file as a build artifact. A later job downloads all of them and publishes the report;
see [[publish]].

**Errors.** `--job-id` is required, as [[#no-job-id]] shows.
A command that fails, or output that the expression does not match, stops the capture with an error;
see [[troubleshooting]].
