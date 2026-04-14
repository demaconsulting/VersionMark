# VersionMark

[![GitHub forks][badge-forks]][link-forks]
[![GitHub stars][badge-stars]][link-stars]
[![GitHub contributors][badge-contributors]][link-contributors]
[![License][badge-license]][link-license]
[![Build][badge-build]][link-build]
[![Quality Gate][badge-quality]][link-quality]
[![Security][badge-security]][link-security]
[![NuGet][badge-nuget]][link-nuget]

VersionMark is a tool for capturing and publishing tool version information across CI/CD
environments. It helps track which versions of build tools, compilers, and dependencies are
used in different jobs and environments.

## Features

- **Version Capture**: Captures tool versions from CI/CD jobs and saves them to JSON files
- **Version Publishing**: Publishes captured versions to markdown documentation
- **Configuration Linting**: Validates `.versionmark.yaml` files with precise error locations
- **Job-ID Tracking**: Associates captured versions with specific CI/CD job identifiers
- **Version Consolidation**: Collapses common versions across jobs while highlighting conflicts
- **OS-Specific Overrides**: Supports platform-specific version capture commands
- **Configurable**: Uses `.versionmark.yaml` config file to define tools and capture methods
- **Multi-Platform Support**: Runs on Windows, Linux, and macOS
- **Multi-Runtime Support**: Targets .NET 8, 9, and 10
- **Continuous Compliance**: Compliance evidence generated automatically on every CI run, following
  the [Continuous Compliance][link-continuous-compliance] methodology

## Installation

Install the tool globally using the .NET CLI:

```bash
dotnet tool install -g DemaConsulting.VersionMark
```

## Quick Start

### 1. Create Configuration File

Create a `.versionmark.yaml` file in your repository:

```yaml
tools:
  dotnet:
    command: dotnet --version
    regex: '(?<version>\d+\.\d+\.\d+)'
  
  node:
    command: node --version
    regex: 'v(?<version>\d+\.\d+\.\d+)'
```

### 2. Capture Tool Versions

In your CI/CD job, capture tool versions with a job identifier:

```bash
versionmark --capture --job-id "windows-net8"
```

This creates a JSON file with captured versions (e.g., `versionmark-windows-net8.json`).

### 3. Publish to Documentation

After all jobs complete, publish the captured versions to markdown:

```bash
versionmark --publish --report versions.md
```

This consolidates versions from all jobs and generates a markdown report.

## Command-Line Options

```text
Usage: versionmark [options]
       versionmark --lint [<config-file>]
       versionmark --capture --job-id <id> [options] [-- tool1 tool2 ...]
       versionmark --publish --report <file> [options] [-- pattern1 pattern2 ...]
```

| Option                    | Description                                                      |
| ------------------------- | ---------------------------------------------------------------- |
| **General**               |                                                                  |
| `-v`, `--version`         | Display version information                                      |
| `-?`, `-h`, `--help`      | Display help message                                             |
| `--silent`                | Suppress console output                                          |
| `--log <file>`            | Write output to log file                                         |
| `--depth <depth>`         | Heading depth for validation and `--report-depth` (default: 1)   |
| **Lint Mode**             |                                                                  |
| `--lint [<config-file>]`  | Check configuration file (default: `.versionmark.yaml`)          |
| **Capture Mode**          |                                                                  |
| `--capture`               | Enable capture mode                                              |
| `--job-id <id>`           | **(Required)** Unique identifier for this CI/CD job              |
| `--output <file>`         | Output JSON file (default: `versionmark-<job-id>.json`)          |
| `-- <tools...>`           | List of tool names to capture (default: all tools in config)     |
| **Publish Mode**          |                                                                  |
| `--publish`               | Enable publish mode                                              |
| `--report <file>`         | **(Required)** Output markdown file path                         |
| `--report-depth <depth>`  | Heading depth for markdown output (default: `--depth` value)     |
| `-- <patterns...>`        | Glob patterns for JSON files (default: `versionmark-*.json`)     |
| **Self-Validation**       |                                                                  |
| `--validate`              | Run self-validation tests                                        |
| `--results <file>`        | Write validation results to file (`.trx` or `.xml`)              |

## Configuration File

The `.versionmark.yaml` file defines which tools to capture and how to extract their versions:

```yaml
tools:
  # Basic tool definition
  dotnet:
    command: dotnet --version
    regex: '(?<version>\d+\.\d+\.\d+)'
  
  # Tool with OS-specific overrides
  gcc:
    command: gcc --version
    command-win: gcc.exe --version
    command-linux: gcc-13 --version
    command-macos: gcc-14 --version
    regex: 'gcc \(.*\) (?<version>\d+\.\d+\.\d+)'
    regex-win: 'gcc\.exe \(.*\) (?<version>\d+\.\d+\.\d+)'
    regex-linux: 'gcc-13 \(.*\) (?<version>\d+\.\d+\.\d+)'
  
  # Tool with custom output parsing
  cmake:
    command: cmake --version
    regex: 'cmake version (?<version>\d+\.\d+\.\d+)'
```

### Configuration Options

Each tool in the `tools` dictionary has the following properties:

- **command**: Shell command to execute to get version information
- **regex**: Regular expression with a named 'version' capture group using .NET syntax `(?<version>...)`
- **command-win**: (Optional) Command override for Windows
- **command-linux**: (Optional) Command override for Linux
- **command-macos**: (Optional) Command override for macOS
- **regex-win**: (Optional) Regex override for Windows
- **regex-linux**: (Optional) Regex override for Linux
- **regex-macos**: (Optional) Regex override for macOS

## Output Format

### Capture Output (JSON)

Capture mode creates a JSON file with the following structure:

```json
{
  "JobId": "windows-net8",
  "Versions": {
    "dotnet": "8.0.100",
    "node": "20.11.0",
    "gcc": "13.2.0"
  }
}
```

### Publish Output (Markdown)

Publish mode generates a markdown list consolidating versions from all jobs:

```markdown
## Tool Versions

- **dotnet**: 8.0.100
- **gcc**: 11.4.0 (windows-net8)
- **gcc**: 13.2.0 (linux-net8)
- **node**: 20.11.0
```

When a tool has the same version across all jobs, it's shown without job identifiers. When versions
differ, each version is listed on a separate line with the jobs that use it shown in parentheses.

## Self Validation

VersionMark includes built-in self-validation tests that verify the tool is working correctly in
its current environment. Run with `--validate`:

```bash
versionmark --validate
```

Use `--depth` to control the heading depth of the report (default: 1):

```bash
versionmark --validate --depth 2
```

Example output:

```text
# DEMA Consulting VersionMark

| Information         | Value                                              |
| :------------------ | :------------------------------------------------- |
| Tool Version        | 1.2.3                                              |
| Machine Name        | build-agent-01                                     |
| OS Version          | Ubuntu 22.04.3 LTS                                 |
| DotNet Runtime      | .NET 10.0.0                                        |
| Time Stamp          | 2025-01-01 12:00:00 UTC                            |

✓ VersionMark_CapturesVersions - Passed
✓ VersionMark_GeneratesMarkdownReport - Passed
✓ VersionMark_LintPassesForValidConfig - Passed
✓ VersionMark_LintReportsErrorsForInvalidConfig - Passed

Total Tests: 4
Passed: 4
Failed: 0
```

To save results in TRX format (Visual Studio test results):

```bash
versionmark --validate --results results.trx
```

To save results in JUnit XML format:

```bash
versionmark --validate --results results.xml
```

If any tests fail, the exit code will be non-zero.

## Documentation

Generated documentation includes:

- **Build Notes**: Release information and changes
- **User Guide**: Comprehensive usage documentation
- **Code Quality Report**: CodeQL and SonarCloud analysis results
- **Requirements**: Functional and non-functional requirements
- **Requirements Justifications**: Detailed requirement rationale
- **Trace Matrix**: Requirements to test traceability

## License

Copyright (c) DEMA Consulting. Licensed under the MIT License. See [LICENSE][link-license] for details.

By contributing to this project, you agree that your contributions will be licensed under the MIT License.

<!-- Badge References -->
[badge-forks]: https://img.shields.io/github/forks/demaconsulting/VersionMark?style=plastic
[badge-stars]: https://img.shields.io/github/stars/demaconsulting/VersionMark?style=plastic
[badge-contributors]: https://img.shields.io/github/contributors/demaconsulting/VersionMark?style=plastic
[badge-license]: https://img.shields.io/github/license/demaconsulting/VersionMark?style=plastic
[badge-build]: https://img.shields.io/github/actions/workflow/status/demaconsulting/VersionMark/build_on_push.yaml?style=plastic
[badge-quality]: https://sonarcloud.io/api/project_badges/measure?project=demaconsulting_VersionMark&metric=alert_status
[badge-security]: https://sonarcloud.io/api/project_badges/measure?project=demaconsulting_VersionMark&metric=security_rating
[badge-nuget]: https://img.shields.io/nuget/v/DemaConsulting.VersionMark?style=plastic

<!-- Link References -->
[link-forks]: https://github.com/demaconsulting/VersionMark/network/members
[link-stars]: https://github.com/demaconsulting/VersionMark/stargazers
[link-contributors]: https://github.com/demaconsulting/VersionMark/graphs/contributors
[link-license]: https://github.com/demaconsulting/VersionMark/blob/main/LICENSE
[link-build]: https://github.com/demaconsulting/VersionMark/actions/workflows/build_on_push.yaml
[link-quality]: https://sonarcloud.io/dashboard?id=demaconsulting_VersionMark
[link-security]: https://sonarcloud.io/dashboard?id=demaconsulting_VersionMark
[link-nuget]: https://www.nuget.org/packages/DemaConsulting.VersionMark
[link-continuous-compliance]: https://github.com/demaconsulting/ContinuousCompliance
