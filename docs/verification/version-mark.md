# VersionMark System Verification

## Overview

This section documents the verification design for the VersionMark system. VersionMark is
a .NET global tool that captures tool version information from CI/CD job environments and
publishes consolidated version reports as markdown.

The verification strategy is organized around six subsystems:

- **Cli** - command-line argument parsing and program dispatch
- **Configuration** - YAML configuration loading and validation
- **Capture** - tool version capture and JSON serialization
- **Publishing** - markdown report generation
- **SelfTest** - built-in self-validation
- **Utilities** - glob-pattern file matching

## Verification Approach

Each subsystem is verified through a combination of integration tests (at the subsystem
level) and unit tests (at the unit level). All tests are implemented using xUnit and
are located under `test/DemaConsulting.VersionMark.Tests/`.

Tests are executed using `dotnet test` across multiple operating systems (Windows, Linux,
macOS) and multiple .NET versions (8, 9, 10). Each test run produces a TRX results file
which serves as compliance evidence.

The built-in `--validate` mode exercises capture, publish, and lint workflows end-to-end
and produces a results file that can be used as post-deployment verification evidence.

## System-Level Test Environments

System-level verification is performed in the GitHub Actions CI/CD environment. Each matrix
job runs on a specific platform and .NET version combination, producing named TRX result
files. The file naming convention (`artifacts/validation-{os}-{dotnet}.trx`) and test names
provide the platform linkage used by ReqStream filters.

## Requirements Coverage Summary

The subsystem chapters that follow provide detailed test-scenario-to-requirement mappings.
Each requirement at every level is covered by at least one named test scenario.
