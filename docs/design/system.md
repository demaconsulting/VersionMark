# System Design

## Overview

VersionMark is a .NET global tool that captures tool version information from CI/CD job
environments and publishes consolidated version reports as markdown. It is designed to run
in any CI/CD pipeline that has the .NET runtime available.

The tool operates in four distinct modes, selected by command-line flags:

| Mode       | Flag        | Description                                              |
|------------|-------------|----------------------------------------------------------|
| Capture    | `--capture` | Runs configured commands and saves versions to JSON      |
| Publish    | `--publish` | Reads JSON files and generates a markdown version report |
| Lint       | `--lint`    | Validates the `.versionmark.yaml` configuration file     |
| Validate   | `--validate`| Runs built-in self-verification tests                    |

## Typical Workflow

In a multi-job CI/CD pipeline, VersionMark is used as follows:

1. Each build job runs `versionmark --capture --job-id <name>` to capture tool versions
   into a `versionmark-<name>.json` file and uploads it as an artifact.
2. A final reporting job downloads all capture artifacts and runs
   `versionmark --publish --report report.md -- versionmark-*.json` to generate the report.

The JSON capture files are the interface between the capture and publish modes. They are
human-readable and can be inspected or consumed by other tools.

## System Boundaries

VersionMark interacts with the following external entities:

- **File system**: reads `.versionmark.yaml`, reads and writes JSON version files, writes
  the markdown report, and optionally writes a log file and a TRX/JUnit results file.
- **Shell environment**: executes tool commands through the OS shell (`cmd.exe /c` on
  Windows, `/bin/sh -c` on Linux and macOS).
- **Console**: writes progress and error output to `stdout` and `stderr`.
- **CI/CD pipeline**: consumes the tool via `dotnet tool install` and the command-line
  interface; artifact upload/download is the pipeline's responsibility.

## Inter-Subsystem Interactions

The subsystems interact as follows during the four operational modes:

### Capture Mode

```text
Cli Subsystem → Configuration Subsystem → (shell)
                                        ↓
                               Capture Subsystem (VersionInfo.SaveToFile)
```

1. The Cli Subsystem (Program) parses arguments and calls `RunCapture`.
2. `RunCapture` uses the Configuration Subsystem to load `.versionmark.yaml` and call
   `FindVersions`, which executes shell commands and extracts version strings.
3. The result is saved to disk by `VersionInfo.SaveToFile` (Capture Subsystem).

### Publish Mode

```text
Cli Subsystem → Capture Subsystem (VersionInfo.LoadFromFile) → Publishing Subsystem
                                                                       ↓
                                                             markdown report file
```

1. The Cli Subsystem (Program) parses arguments and calls `RunPublish`.
2. `RunPublish` resolves glob patterns, then uses the Capture Subsystem to load each
   JSON file via `VersionInfo.LoadFromFile`.
3. The Publishing Subsystem (`MarkdownFormatter.Format`) converts the loaded records into
   a markdown string, which is written to the report file.

### Lint Mode

```text
Cli Subsystem → Linting Subsystem
```

1. The Cli Subsystem (Program) calls `RunLint`, which resolves the config file path.
2. The Linting Subsystem validates the YAML structure and reports all issues.

### Validate Mode

```text
Cli Subsystem → SelfTest Subsystem
                       ↓
             (exercises Capture, Publish, and Lint modes internally)
```

1. The Cli Subsystem (Program) calls `Validation.Run`.
2. The SelfTest Subsystem exercises capture, publish, and lint modes end-to-end,
   using `PathHelpers` to safely construct paths inside a temporary directory.

The self-validation suite includes the following named tests, which serve as evidence
that the tool is functioning correctly after installation:

| Test Name | What it Verifies |
| --------- | ---------------- |
| `VersionMark_CapturesVersions` | Capture mode correctly runs commands and saves version JSON |
| `VersionMark_GeneratesMarkdownReport` | Publish mode correctly reads JSON and produces a markdown report |
| `VersionMark_LintPassesForValidConfig` | Lint mode passes for a valid `.versionmark.yaml` configuration |
| `VersionMark_LintReportsErrorsForInvalidConfig` | Lint mode reports errors for an invalid configuration |

These test names appear in requirements files (e.g., `system.yaml`,
`platform-requirements.yaml`) as traceability evidence. When `--validate` is run in CI,
each matrix job runs on a specific platform/runtime and produces a TRX results file whose
filename and CI job context (e.g., `artifacts/validation-windows-latest-dotnet8.x.trx`)
provide the platform/runtime linkage used by filters, together with the test names, allowing
requirements to be verified per platform using source filters such as
`windows@VersionMark_CapturesVersions` or `dotnet8.x@VersionMark_GeneratesMarkdownReport`.

## Configuration File

The `.versionmark.yaml` configuration file defines which tools to capture and how to
extract their version strings:

```yaml
tools:
  dotnet:
    command: dotnet --version
    regex: '(?<version>\d+\.\d+[\d.]*)'
  node:
    command: node --version
    regex: 'v(?<version>\d+\.\d+[\d.]*)'
    command-win: node.exe --version
```

OS-specific overrides (`command-win`, `command-linux`, `command-macos`, `regex-win`,
`regex-linux`, `regex-macos`) allow platform-specific command variations. See the
Configuration Subsystem design for full details.

## Platform Support

VersionMark targets .NET 8, .NET 9, and .NET 10. It runs on Windows, Linux, and macOS.
OS-specific command execution uses `cmd.exe /c` on Windows and `/bin/sh -c` elsewhere.
Platform-specific requirements are documented in `platform-requirements.yaml`.
