# Introduction

This document describes the verification design for the VersionMark .NET tool. It defines
how each requirement is verified through named test scenarios, providing traceability from
requirements to tests for regulatory review.

## Purpose

The purpose of this document is to:

- Define the verification approach for all in-house VersionMark software items
- Map every requirement to at least one named test scenario
- Identify what is tested at each level (system, subsystem, unit) and how
- Provide OTS verification evidence for all third-party components used
- Enable reviewers to confirm test completeness without reading implementation code

## Scope

This document covers verification of six in-house subsystems within the VersionMark system:

- The **Cli Subsystem**: argument parsing and program dispatch via `Program` and `Context`
- The **Configuration Subsystem**: YAML configuration loading and validation via
  `VersionMarkConfig`, `ToolConfig`, and `LintIssue`
- The **Capture Subsystem**: tool version capture and JSON serialization via `VersionInfo`
- The **Publishing Subsystem**: markdown report generation via `MarkdownFormatter`
- The **SelfTest Subsystem**: built-in self-validation via `Validation`
- The **Utilities Subsystem**: glob-pattern file matching and safe path combination via
  `GlobMatcher` and `PathHelpers`

It also covers verification evidence for the following Off-The-Shelf (OTS) components:

- **BuildMark** - build notes generation tool
- **DemaConsulting.TestResults** - TRX/JUnit result serialization library
- **FileAssert** - file content assertion tool
- **Microsoft.Extensions.FileSystemGlobbing** - glob-pattern file matching library
- **Pandoc** - document conversion tool
- **ReqStream** - requirements traceability tool
- **ReviewMark** - code review enforcement tool
- **SarifMark** - SARIF report tool
- **SonarMark** - SonarCloud report tool
- **WeasyPrint** - HTML-to-PDF conversion tool
- **xUnit** - unit testing framework
- **YamlDotNet** - YAML parsing library

This document does not cover installation, end-user usage patterns, or CI/CD pipeline
configuration. Those topics are addressed in the VersionMark User Guide and the
VersionMark Requirements documents.

## Software Structure

The following tree shows how the VersionMark software items are organized across the
system, subsystem, and unit levels:

```text
VersionMark (System)
├── Cli (Subsystem)
│   ├── Program (Unit)
│   └── Context (Unit)
├── Configuration (Subsystem)
│   ├── VersionMarkConfig (Unit)
│   ├── ToolConfig (Unit)
│   └── LintIssue (Unit)
├── Capture (Subsystem)
│   └── VersionInfo (Unit)
├── Publishing (Subsystem)
│   └── MarkdownFormatter (Unit)
├── SelfTest (Subsystem)
│   └── Validation (Unit)
└── Utilities (Subsystem)
    ├── GlobMatcher (Unit)
    └── PathHelpers (Unit)
```

## Companion Artifact Structure

In-house items have parallel artifacts in the following locations:

- Requirements: `docs/reqstream/version-mark.yaml` for the system, with subsystem and unit
  requirements in paths such as `docs/reqstream/version-mark/cli.yaml` and
  `docs/reqstream/version-mark/cli/context.yaml`
- Design: `docs/design/version-mark.md` for the system, with subsystem and unit design
  documents in paths such as `docs/design/version-mark/cli.md` and
  `docs/design/version-mark/cli/context.md`
- Verification: `docs/verification/version-mark.md` for the system, with subsystem and unit
  verification documents in paths such as `docs/verification/version-mark/cli.md` and
  `docs/verification/version-mark/cli/context.md`
- Source: implementation files under `src/DemaConsulting.VersionMark/`, such as
  `src/DemaConsulting.VersionMark/Program.cs`,
  `src/DemaConsulting.VersionMark/Configuration/VersionMarkConfig.cs`, and
  `src/DemaConsulting.VersionMark/Utilities/PathHelpers.cs`
- Tests: verification evidence under `test/DemaConsulting.VersionMark.Tests/`, such as
  `test/DemaConsulting.VersionMark.Tests/ProgramTests.cs`,
  `test/DemaConsulting.VersionMark.Tests/Configuration/VersionMarkConfigTests.cs`, and
  `test/DemaConsulting.VersionMark.Tests/Utilities/PathHelpersTests.cs`

OTS items (no design documentation) have artifacts in these locations:

- Requirements: `docs/reqstream/ots/`, for example `docs/reqstream/ots/buildmark.yaml`
- Verification: `docs/verification/ots/`, for example `docs/verification/ots/buildmark.md`

Review-sets are defined in `.reviewmark.yaml`.

## References

- [VersionMark releases](https://github.com/demaconsulting/VersionMark/releases)
