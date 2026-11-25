---
title: Getting started
summary: Install the tool, capture tool versions, and publish a version report.
aliases:
  - start
  - quickstart
  - examples
tags:
  - tutorial
order: 1
see-also:
  - configuration
  - capture
  - publish
examples:
  - name: version
    kind: command
    language: shell
    description: Print the version and exit.
    content: '{{tool}} --version'
  - name: lint
    kind: command
    language: shell
    description: Check .versionmark.yaml in the current folder. Prints nothing when it is valid.
    content: '{{tool}} --lint'
  - name: capture
    kind: command
    language: shell
    description: Run the configured commands and save the versions for the job 'build'.
    content: '{{tool}} --capture --job-id build'
    expect: 'Version information saved to versionmark-build.json'
  - name: publish
    kind: command
    language: shell
    description: Combine every versionmark-*.json file in the current folder into one report.
    content: '{{tool}} --publish --report versions.md'
    expect: 'Report successfully written to versions.md'
  - name: topic
    kind: command
    language: shell
    description: Print a help topic, here the capture topic.
    content: '{{tool}} help capture'
    expect: '--job-id'
---
{{tool}} {{version}} is a .NET tool.
It records which versions of the build tools each CI job used, and turns the records of all jobs into one Markdown
report for the build notes.

1. **Install it.** Install the tool from NuGet, globally or in a local tool manifest:

   ```bash
   dotnet tool install -g DemaConsulting.VersionMark
   ```

2. **Check it.** The command that [[#version]] prints shows the version.
   `{{tool}} --validate` runs the built-in self-tests; see [[self-validation]].
3. **Configure it.** Write `.versionmark.yaml` in the folder the tool runs in. It lists each tool with the
   command that prints its version and a regular expression that finds the version;
   see [[configuration]].
   The command that [[#lint]] prints checks the file,
   and prints nothing when the file is valid.
4. **Capture.** In each CI job, the command that [[#capture]] prints
   runs the commands and saves the versions to a JSON file named after the job id;
   see [[capture]].
5. **Publish.** Collect the JSON files of all jobs in one folder.
   The command that [[#publish]] prints combines them into one report;
   see [[publish]].

Print any help topic with `{{tool}} help <topic>`,
as in the command that [[#topic]] prints.
Add `--search <words>` to find a topic, and `--json` to get any help answer as JSON.
