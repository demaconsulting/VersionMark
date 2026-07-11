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

This document covers the following software items:

Local items:

- **VersionMark**: system, subsystem, and unit design for all local components.

OTS items:

- **DemaConsulting.TestResults**: integration and usage design.
- **FileSystemGlobbing**: integration and usage design.
- **SysML2Tools**: integration and usage design.
- **YamlDotNet**: integration and usage design.

The following topics are out of scope:

- Installation and end-user usage patterns
- CI/CD pipeline configuration
- External library internals
- Test projects

## Software Structure

The software structure is modeled in SysML2 under `docs/sysml2/` and rendered to the
diagram below by SysML2Tools as part of the build pipeline. The model is the authoritative,
machine-queryable source of structure; the diagram is a generated/derived artifact. AI
agents should query the SysML2 model directly (see the `sysml2tools-query` skill) rather
than parsing this diagram before deep-diving into source code.

![Software Structure](SoftwareStructureView.svg)

## Folder Layout

- **src/** - source files and projects
  - **DemaConsulting.VersionMark/** - VersionMark system source
    - **Cli/** - Cli subsystem
    - **Configuration/** - Configuration subsystem
    - **Capture/** - Capture subsystem
    - **Publishing/** - Publishing subsystem
    - **SelfTest/** - SelfTest subsystem
    - **Utilities/** - Utilities subsystem

## Companion Artifact Structure

Each local software item has corresponding artifacts in parallel directory trees:

- Requirements: `docs/reqstream/version-mark.yaml`,
  `docs/reqstream/version-mark/{subsystem}.yaml`,
  `docs/reqstream/version-mark/{subsystem}/{unit}.yaml`
- Design: `docs/design/version-mark.md`,
  `docs/design/version-mark/{subsystem}.md`,
  `docs/design/version-mark/{subsystem}/{unit}.md`
- Verification: `docs/verification/version-mark.md`,
  `docs/verification/version-mark/{subsystem}.md`,
  `docs/verification/version-mark/{subsystem}/{unit}.md`
- Source: `src/DemaConsulting.VersionMark/{Subsystem}/{Unit}.cs`
- Tests: `test/DemaConsulting.VersionMark.Tests/{Subsystem}/{Unit}Tests.cs`

Review-sets: defined in `.reviewmark.yaml`

## References

- [VersionMark releases](https://github.com/demaconsulting/VersionMark/releases)
