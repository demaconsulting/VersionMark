# Introduction

VersionMark is a .NET global tool that captures tool version information from CI/CD job
environments and publishes consolidated version reports as markdown. This document describes
its internal design, organized as a single system with six subsystems — Cli, Configuration,
Capture, Publishing, SelfTest, and Utilities — each containing one or more units.

## Purpose

This document defines the full architectural and detailed design for every local software
item in VersionMark — the VersionMark system, its six subsystems, and their units. A
reviewer should be able to understand how each item fulfills its requirements without
reading source code. This document is intended for software developers implementing features
or fixing defects, reviewers conducting formal design and code reviews, and quality assurance
engineers tracing requirements to implementation. Readers are assumed to be familiar with C#
and .NET development and general concepts of command-line tool design.

## Scope

This document covers the design of the VersionMark system and its six subsystems:

- The **Cli Subsystem**: the `Program` entry point and `Context` class that handle argument
  parsing, output routing, and program flow control
- The **Configuration Subsystem**: the `VersionMarkConfig`, `ToolConfig`, and `LintIssue`
  classes that read, validate, and interpret `.versionmark.yaml` configuration files
- The **Capture Subsystem**: the `VersionInfo` record that serializes and deserializes
  captured version data to and from JSON
- The **Publishing Subsystem**: the `MarkdownFormatter` class that generates the markdown
  version report from captured data
- The **SelfTest Subsystem**: the `Validation` class that provides built-in verification
  of the tool's core functionality
- The **Utilities Subsystem**: the `GlobMatcher` class that provides glob-pattern file
  matching and the `PathHelpers` class that provides safe path combination

This document does not cover installation, end-user usage patterns, CI/CD pipeline
configuration, or the internal design of OTS dependencies. Those topics are addressed in
the *VersionMark User Guide*, the *VersionMark Requirements Document*, and the respective
OTS package documentation.

Each component described here corresponds to one or more requirements defined in the
`docs/reqstream/` files. The source code in `src/DemaConsulting.VersionMark/` is the
authoritative implementation. Any discrepancy between this document and the code should be
resolved by updating this document to reflect the actual implementation, or by raising a
defect against the code.

## Software Structure

The following tree shows how the VersionMark software items are organized across the
system, subsystem, and unit levels:

```text
VersionMark (System)                        Version capture/publish tool
├── Cli (Subsystem)                         Argument parsing and dispatch
│   ├── Program (Unit)                      Tool entry point
│   └── Context (Unit)                      Command-line state container
├── Configuration (Subsystem)               YAML configuration loading and validation
│   ├── VersionMarkConfig (Unit)            Top-level config container and validator
│   ├── ToolConfig (Unit)                   Per-tool config record
│   └── LintIssue (Unit)                    Lint severity, issue record, and load result
├── Capture (Subsystem)                     Tool version capture
│   └── VersionInfo (Unit)                  JSON version data record
├── Publishing (Subsystem)                  Markdown report publishing
│   └── MarkdownFormatter (Unit)            Version report formatter
├── SelfTest (Subsystem)                    Built-in self-validation
│   └── Validation (Unit)                   Self-validation runner
└── Utilities (Subsystem)                   General-purpose helper utilities
    ├── GlobMatcher (Unit)                  Glob-pattern file matching
    └── PathHelpers (Unit)                  Safe path combination
```

## Folder Layout

The source code folder structure mirrors the top-level subsystem breakdown above, giving
reviewers an explicit navigation aid from design to code:

```text
src/DemaConsulting.VersionMark/
├── Program.cs                              — entry point and execution orchestrator
├── Cli/
│   └── Context.cs                          — command-line argument parser and I/O owner
├── Configuration/
│   ├── LintIssue.cs                        — lint issue record and severity enum
│   └── VersionMarkConfig.cs                — YAML configuration, tool definitions, and validation
├── Capture/
│   └── VersionInfo.cs                      — captured version data record
├── Publishing/
│   └── MarkdownFormatter.cs                — markdown report generation
├── SelfTest/
│   └── Validation.cs                       — self-validation test runner
└── Utilities/
    ├── GlobMatcher.cs                      — glob-pattern file matching
    └── PathHelpers.cs                      — safe path utilities
```

`Program.cs` resides at the project root rather than inside `Cli/` because .NET uses the
presence of a top-level `Program.cs` at the project root as the conventional entry-point
file. It is conceptually part of the Cli Subsystem, as shown in the Software Structure tree
above. The test project mirrors the same layout under `test/DemaConsulting.VersionMark.Tests/`.

## Companion Artifact Structure

Each local software item has corresponding artifacts in parallel directory trees:

- Requirements: `docs/reqstream/version-mark.yaml`,
  `docs/reqstream/version-mark/{subsystem}/{unit}.yaml`
- Design: `docs/design/version-mark.md`,
  `docs/design/version-mark/{subsystem}/{unit}.md`
- Verification: `docs/verification/version-mark.md`,
  `docs/verification/version-mark/{subsystem}/{unit}.md`
- Source: `src/DemaConsulting.VersionMark/{Subsystem}/{Unit}.cs`
- Tests: `test/DemaConsulting.VersionMark.Tests/{Subsystem}/{Unit}Tests.cs`

Review-sets: defined in `.reviewmark.yaml`

## References

- [VersionMark releases](https://github.com/demaconsulting/VersionMark/releases)
