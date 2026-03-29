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

This document covers the design of six subsystems within VersionMark:

- The **Cli Subsystem**: the `Program` entry point and `Context` class
  that handle argument parsing, output routing, and program flow control
- The **Configuration Subsystem**: the `VersionMarkConfig` and `ToolConfig` classes that
  read and interpret `.versionmark.yaml` configuration files
- The **Capture Subsystem**: the `VersionInfo` record that serializes and deserializes
  captured version data to and from JSON
- The **Publishing Subsystem**: the `MarkdownFormatter` class that generates the markdown
  version report from captured data
- The **Linting Subsystem**: the `Lint` class that validates `.versionmark.yaml` configuration
  files and reports issues with precise source locations
- The **SelfTest Subsystem**: the `Validation` class and `PathHelpers` utility that
  together provide built-in verification of the tool's core functionality

This document does not cover installation, end-user usage patterns, or the CI/CD pipeline
configuration. Those topics are addressed in the [User Guide][user-guide] and the
[Requirements document][requirements-doc].

## Software Architecture

The following tree shows the VersionMark software system, its software subsystems, and their
software units, with a short description of each node's role.

```text
VersionMark (Software System)               Version capture/publish tool
├── Cli Subsystem                           Argument parsing and dispatch
│   ├── Program (Software Unit)             Tool entry point
│   └── Context (Software Unit)             Command-line state container
├── Configuration Subsystem                 YAML configuration loading
│   ├── VersionMarkConfig (Software Unit)   Top-level config container
│   └── ToolConfig (Software Unit)          Per-tool config record
├── Capture Subsystem                       Tool version capture
│   └── VersionInfo (Software Unit)         JSON version data record
├── Publishing Subsystem                    Markdown report publishing
│   └── MarkdownFormatter (Software Unit)   Version report formatter
├── Linting Subsystem                       Configuration file lint
│   └── Lint (Software Unit)                YAML configuration validator
└── SelfTest Subsystem                      Built-in self-validation
    ├── Validation (Software Unit)          Self-validation runner
    └── PathHelpers (Software Unit)         Safe path combination
```

## Folder Layout

The source files are arranged in subsystem-aligned subdirectories beneath the main project
folder. Each directory corresponds to one subsystem described above, making it
straightforward to locate the implementation for any given component.

```text
src/DemaConsulting.VersionMark/
├── Program.cs                          — entry point and execution orchestrator
├── Cli/
│   └── Context.cs                      — command-line argument parser and I/O owner
├── Configuration/
│   └── VersionMarkConfig.cs            — YAML configuration and tool definitions
├── Capture/
│   └── VersionInfo.cs                  — captured version data record
├── Publishing/
│   └── MarkdownFormatter.cs            — markdown report generation
├── Linting/
│   └── Lint.cs                         — YAML configuration linter
└── SelfTest/
    ├── Validation.cs                   — self-validation test runner
    └── PathHelpers.cs                  — safe path utilities
```

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
