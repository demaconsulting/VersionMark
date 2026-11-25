---
name: versionmark
schema: 2
title: VersionMark
summary: Built-in help of VersionMark, which captures tool versions and publishes version reports.
---
{{tool}} runs the commands listed in a configuration file to capture the versions of the tools that a CI job used,
and combines the captures of all jobs into one Markdown version report.

Every operation is an option. The only command is `{{tool}} help`, which must be the first argument and takes topic
names, such as `{{tool}} help command-line`.
