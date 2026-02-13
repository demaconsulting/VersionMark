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
- **Job-ID Tracking**: Associates captured versions with specific CI/CD job identifiers
- **Version Consolidation**: Collapses common versions across jobs while highlighting conflicts
- **OS-Specific Overrides**: Supports platform-specific version capture commands
- **Configurable**: Uses `.versionmark.yaml` config file to define tools and capture methods
- **Multi-Platform Support**: Runs on Windows and Linux
- **Multi-Runtime Support**: Targets .NET 8, 9, and 10

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
versionmark publish --output versions.md
```

This consolidates versions from all jobs and generates a markdown table.

## Command-Line Options

### Global Options

| Option               | Description                                                  |
| -------------------- | ------------------------------------------------------------ |
| `-v`, `--version`    | Display version information                                  |
| `-?`, `-h`, `--help` | Display help message                                         |
| `--silent`           | Suppress console output                                      |
| `--log <file>`       | Write output to log file                                     |

### Capture Mode

Capture tool versions from the current environment:

```bash
versionmark --capture --job-id <job-identifier> [options] [-- tool1 tool2 ...]
```

| Option                    | Description                                                  |
| ------------------------- | ------------------------------------------------------------ |
| `--capture`               | Enable capture mode                                          |
| `--job-id <id>`           | **(Required)** Unique identifier for this CI/CD job          |
| `--output <file>`         | Output JSON file (default: `versionmark-<job-id>.json`)      |
| `-- <tools...>`           | List of tool names to capture (default: all tools in config) |

**Example:** Capture specific tools only:

```bash
versionmark --capture --job-id "windows-build" -- dotnet node npm
```

### Publish Mode

Publish captured versions to markdown documentation:

```bash
versionmark publish [options]
```

| Option                    | Description                                                  |
| ------------------------- | ------------------------------------------------------------ |
| `--input <pattern>`       | Input JSON file pattern (default: `versionmark-*.json`)      |
| `--output <file>`         | Output markdown file (default: `versions.md`)                |
| `--config <file>`         | Configuration file path (default: `.versionmark.yaml`)       |

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
  "job-id": "windows-net8",
  "timestamp": "2024-03-15T10:30:00Z",
  "versions": {
    "dotnet": "8.0.100",
    "node": "20.11.0",
    "gcc": "13.2.0"
  }
}
```

### Publish Output (Markdown)

Publish mode generates a markdown table consolidating versions from all jobs:

```markdown
| Tool   | Version | Jobs                    |
| ------ | ------- | ----------------------- |
| dotnet | 8.0.100 | All jobs                |
| node   | 20.11.0 | windows-net8, linux-net8|
| gcc    | 13.2.0  | linux-net8              |
| gcc    | 11.4.0  | windows-net8            |
```

When a tool has the same version across all jobs, it's shown as "All jobs". When versions
differ, each version is listed with the jobs that use it.

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
