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

This document covers verification of five in-house subsystems within the VersionMark system:

- The **Cli Subsystem**: argument parsing and program dispatch via `Program` and `Context`
- The **Configuration Subsystem**: YAML configuration loading and validation via
  `VersionMarkConfig`, `ToolConfig`, and `LintIssue`
- The **Capture Subsystem**: tool version capture and JSON serialization via `VersionInfo`
- The **Publishing Subsystem**: markdown report generation via `MarkdownFormatter`
- The **SelfTest Subsystem**: built-in self-validation via `Validation` and `PathHelpers`

It also covers verification evidence for the following Off-The-Shelf (OTS) components:

- **BuildMark** - build notes generation tool
- **FileAssert** - file content assertion tool
- **MSTest** - unit testing framework
- **Pandoc** - document conversion tool
- **ReqStream** - requirements traceability tool
- **ReviewMark** - code review enforcement tool
- **SarifMark** - SARIF report tool
- **SonarMark** - SonarCloud report tool
- **WeasyPrint** - HTML-to-PDF conversion tool

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
└── SelfTest (Subsystem)
    ├── Validation (Unit)
    └── PathHelpers (Unit)
```

## Companion Artifact Structure

In-house items have parallel artifacts in the following locations:

- Requirements: `docs/reqstream/version-mark.yaml`,
  `docs/reqstream/version-mark/{subsystem}/{item}.yaml`
- Design: `docs/design/version-mark.md`,
  `docs/design/version-mark/{subsystem}/{item}.md`
- Verification: `docs/verification/version-mark.md`,
  `docs/verification/version-mark/{subsystem}/{item}.md`
- Source: `src/DemaConsulting.VersionMark/{Subsystem}/{Unit}.cs`
- Tests: `test/DemaConsulting.VersionMark.Tests/{Subsystem}/{Unit}Tests.cs`

OTS items (no design documentation) have artifacts in these locations:

- Requirements: `docs/reqstream/ots/{ots-name}.yaml`
- Verification: `docs/verification/ots/{ots-name}.md`

Review-sets are defined in `.reviewmark.yaml`.

## References

- [REF-1] VersionMark Software Design Document, DEMA Consulting
- [REF-2] VersionMark Requirements Document, DEMA Consulting
