# Introduction

This document describes the internal design of the VersionMark .NET tool. It provides a
structured account of the key components, their responsibilities, and how they interact to
deliver the tool's capabilities.

## Purpose

The purpose of this document is to:

- Describe the design decisions and structure of the VersionMark tool
- Provide a reference for developers contributing to or reviewing the tool
- Establish traceability between requirements and the components that fulfil them
- Document each conceptual group in sufficient detail to support code review

## Scope

This document covers the design of five primary components within VersionMark:

- The **command-line layer**: the `Program` entry point and `Context` class that handle
  argument parsing, output routing, and program flow control
- The **utilities layer**: the `PathHelpers` path-safety utility and `MarkdownFormatter`
  report generator
- The **configuration layer**: the `VersionMarkConfig` and `ToolConfig` classes that
  read and interpret `.versionmark.yaml` configuration files
- The **version information layer**: the `VersionInfo` record that serializes and
  deserializes captured version data to and from JSON
- The **self-validation layer**: the `Validation` class that provides built-in
  verification of the tool's core functionality

This document does not cover installation, end-user usage patterns, or the CI/CD pipeline
configuration. Those topics are addressed in the [User Guide][user-guide] and the
[Requirements document][requirements-doc].

## Software Architecture

The following tree shows the VersionMark software system, its software subsystems, and their
software units. Each node has a corresponding requirements file in `docs/reqstream/`.

```text
VersionMark (Software System)                      versionmark-system.yaml
├── Command-Line Interface Subsystem               subsystem-command-line.yaml
│   ├── Program (Software Unit)                    unit-program.yaml
│   └── Context (Software Unit)                    unit-context.yaml
├── Capture Subsystem                              subsystem-capture.yaml
├── Publish Subsystem                              subsystem-publish.yaml
├── Configuration Subsystem                        subsystem-configuration.yaml
│   ├── VersionMarkConfig (Software Unit)          unit-version-mark-config.yaml
│   └── ToolConfig (Software Unit)                 unit-tool-config.yaml
├── Lint Subsystem                                 subsystem-lint.yaml
│   └── Lint (Software Unit)                       unit-lint.yaml
├── MarkdownFormatter (Software Unit)              unit-formatter.yaml
├── VersionInfo (Software Unit)                    unit-version-info.yaml
├── PathHelpers (Software Unit)                    unit-path-helpers.yaml
└── Validation (Software Unit)                     unit-validation.yaml
```

Platform support requirements are in `platform-requirements.yaml`. Each off-the-shelf
component used by the tool has its own requirements file (`ots-mstest.yaml`,
`ots-reqstream.yaml`, `ots-buildmark.yaml`, `ots-sarifmark.yaml`, `ots-sonarmark.yaml`,
`ots-reviewmark.yaml`).

## Audience

This document is intended for:

- Software developers implementing features or fixing defects in the tool
- Reviewers conducting formal design and code reviews
- Quality assurance engineers tracing requirements to implementation

Readers are assumed to be familiar with C# and .NET development and general concepts of
command-line tool design.

## Relationship to Requirements and Code

Each component described here corresponds to one or more requirements defined in the
`docs/reqstream/` files. Requirements identifiers are referenced inline where relevant to
make traceability explicit.

The source code in `src/DemaConsulting.VersionMark/` is the authoritative implementation.
This document describes the intent and structure of that code; any discrepancy between
this document and the code should be resolved by updating this document to reflect the
actual implementation, or by raising a defect against the code.

[user-guide]: https://github.com/demaconsulting/VersionMark
[requirements-doc]: https://github.com/demaconsulting/VersionMark
