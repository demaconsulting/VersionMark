# VersionMark User Guide

VersionMark is a tool for capturing and publishing tool version information across CI/CD
environments. It helps track which versions of build tools, compilers, and dependencies are
used in different jobs and environments.

## Installation

Install the tool globally using the .NET CLI:

```bash
dotnet tool install -g DemaConsulting.VersionMark
```

## Quick Start

### Step 1: Create Configuration File

Create a `.versionmark.yaml` file in your repository root:

```yaml
tools:
  - name: dotnet
    command: dotnet --version
    regex: '(\d+\.\d+\.\d+)'
  
  - name: node
    command: node --version
    regex: 'v(\d+\.\d+\.\d+)'
  
  - name: gcc
    command: gcc --version
    regex: 'gcc \(.*\) (\d+\.\d+\.\d+)'
```

### Step 2: Capture Tool Versions in CI/CD

In each CI/CD job, capture tool versions with a unique job identifier:

```bash
versionmark capture --job-id "windows-net8"
```

This creates a JSON file (e.g., `versionmark-windows-net8.json`) containing the captured versions.

### Step 3: Publish Versions to Documentation

After all jobs complete, publish the captured versions:

```bash
versionmark publish --output versions.md
```

This generates a markdown file consolidating versions from all jobs.

## Command-Line Reference

### Global Options

These options are available for all commands:

| Option               | Description                                                  |
| -------------------- | ------------------------------------------------------------ |
| `-v`, `--version`    | Display version information                                  |
| `-?`, `-h`, `--help` | Display help message                                         |
| `--silent`           | Suppress console output                                      |
| `--log <file>`       | Write output to log file                                     |

### Capture Command

Capture tool versions from the current environment:

```bash
versionmark capture --job-id <job-identifier> [options]
```

#### Options

| Option                    | Description                                                                         |
| ------------------------- | ----------------------------------------------------------------------------------- |
| `--job-id <id>`           | **(Required)** Unique identifier for this CI/CD job. This is used to differentiate  |
|                           | versions captured in different environments or configurations.                      |
| `--config <file>`         | Path to configuration file (default: `.versionmark.yaml`)                           |
| `--output <file>`         | Path to output JSON file (default: `versionmark-<job-id>.json`)                     |

#### Example

```bash
# Capture versions with a descriptive job-id
versionmark capture --job-id "windows-dotnet8-release"

# Use a custom config file
versionmark capture --job-id "linux-gcc" --config custom-config.yaml

# Specify output file
versionmark capture --job-id "macos" --output versions/macos.json
```

### Publish Command

Publish captured versions to markdown documentation:

```bash
versionmark publish [options]
```

#### Publish Options

| Option              | Description                                                       |
| ------------------- | ----------------------------------------------------------------- |
| `--input <pattern>` | Glob pattern for input JSON files (default: `versionmark-*.json`) |
| `--output <file>`   | Path to output markdown file (default: `versions.md`)             |
| `--config <file>`   | Path to configuration file (default: `.versionmark.yaml`)         |

#### Publish Example

```bash
# Publish all captured versions
versionmark publish

# Use custom input pattern
versionmark publish --input "captured/versionmark-*.json"

# Specify output file
versionmark publish --output docs/tool-versions.md

# Use custom config
versionmark publish --config .versionmark.yaml --output versions.md
```

## Configuration File Format

The `.versionmark.yaml` file defines which tools to capture and how to extract version information.

### Basic Configuration

```yaml
tools:
  - name: dotnet
    command: dotnet --version
    regex: '(\d+\.\d+\.\d+)'
  
  - name: node
    command: node --version
    regex: 'v(\d+\.\d+\.\d+)'
  
  - name: python
    command: python --version
    regex: 'Python (\d+\.\d+\.\d+)'
```

### Configuration Properties

Each tool definition supports the following properties:

| Property    | Required | Description                                                               |
| ----------- | -------- | ------------------------------------------------------------------------- |
| `name`      | Yes      | Display name for the tool (used in output)                                |
| `command`   | Yes      | Shell command to execute to get version information                       |
| `regex`     | Yes      | Regular expression to extract version. First capture group is the version |
| `overrides` | No       | Platform-specific command overrides (see below)                           |

### OS-Specific Overrides

You can provide platform-specific commands for tools that have different behavior on different operating systems:

```yaml
tools:
  - name: gcc
    command: gcc --version
    regex: 'gcc \(.*\) (\d+\.\d+\.\d+)'
    overrides:
      windows:
        command: gcc.exe --version
      linux:
        command: gcc --version
      macos:
        command: gcc --version
  
  - name: powershell
    command: pwsh --version
    regex: 'PowerShell (\d+\.\d+\.\d+)'
    overrides:
      windows:
        command: powershell -Command "$PSVersionTable.PSVersion.ToString()"
        regex: '(\d+\.\d+\.\d+)'
```

The `overrides` section can contain `windows`, `linux`, or `macos` subsections. Each override
can specify:

- `command`: Alternative command for that platform
- `regex`: Alternative regex for that platform (optional, uses parent regex if not specified)

### Regular Expression Tips

The regex should contain at least one capture group `(...)` that captures the version number. Examples:

- **Simple version**: `(\d+\.\d+\.\d+)` - Captures `1.2.3`
- **Prefixed version**: `Version (\d+\.\d+\.\d+)` - Captures `1.2.3` from `Version 1.2.3`
- **Multiline output**: `(?m)version (\d+\.\d+\.\d+)` - Uses multiline mode
- **Build metadata**: `(\d+\.\d+\.\d+[-+][a-zA-Z0-9.]+)` - Captures `1.2.3-beta.1`

## Output Formats

### Capture Output (JSON)

When you run the capture command, VersionMark creates a JSON file with the following structure:

```json
{
  "job-id": "windows-net8",
  "timestamp": "2024-03-15T10:30:00Z",
  "versions": {
    "dotnet": "8.0.100",
    "node": "20.11.0",
    "gcc": "13.2.0",
    "cmake": "3.28.0"
  }
}
```

#### JSON Structure

- **job-id**: The unique identifier provided via `--job-id`
- **timestamp**: ISO 8601 timestamp of when versions were captured
- **versions**: Object mapping tool names to their captured versions

### Publish Output (Markdown)

The publish command generates a markdown table consolidating versions from all captured JSON files:

#### Example: All Jobs Use Same Version

```markdown
| Tool   | Version | Jobs     |
| ------ | ------- | -------- |
| dotnet | 8.0.100 | All jobs |
| node   | 20.11.0 | All jobs |
```

When all jobs capture the same version of a tool, it's displayed as "All jobs".

#### Example: Different Versions Across Jobs

```markdown
| Tool   | Version | Jobs                             |
| ------ | ------- | -------------------------------- |
| dotnet | 8.0.100 | All jobs                         |
| gcc    | 13.2.0  | linux-net8, linux-net9           |
| gcc    | 11.4.0  | windows-net8, windows-net9       |
| node   | 20.11.0 | windows-net8, linux-net8         |
| node   | 21.0.0  | windows-net9, linux-net9         |
```

When a tool has different versions across jobs, each version is listed separately with the jobs that use it.

## CI/CD Integration

### GitHub Actions Example

Here's a complete example of using VersionMark in a GitHub Actions workflow:

```yaml
name: Build

on: [push, pull_request]

jobs:
  build-windows:
    runs-on: windows-latest
    steps:
      - uses: actions/checkout@v4
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '8.0.x'
      
      - name: Install VersionMark
        run: dotnet tool install -g DemaConsulting.VersionMark
      
      - name: Capture tool versions
        run: versionmark capture --job-id "windows-net8"
      
      - name: Upload version capture
        uses: actions/upload-artifact@v4
        with:
          name: versions-windows-net8
          path: versionmark-windows-net8.json
  
  build-linux:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '8.0.x'
      
      - name: Install VersionMark
        run: dotnet tool install -g DemaConsulting.VersionMark
      
      - name: Capture tool versions
        run: versionmark capture --job-id "linux-net8"
      
      - name: Upload version capture
        uses: actions/upload-artifact@v4
        with:
          name: versions-linux-net8
          path: versionmark-linux-net8.json
  
  publish-versions:
    needs: [build-windows, build-linux]
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      
      - name: Download all version captures
        uses: actions/download-artifact@v4
        with:
          pattern: versions-*
          merge-multiple: true
      
      - name: Install VersionMark
        run: dotnet tool install -g DemaConsulting.VersionMark
      
      - name: Publish versions
        run: versionmark publish --output docs/tool-versions.md
      
      - name: Commit version documentation
        run: |
          git config user.name "github-actions[bot]"
          git config user.email "github-actions[bot]@users.noreply.github.com"
          git add docs/tool-versions.md
          git commit -m "Update tool versions" || echo "No changes"
          git push
```

### Key Integration Points

1. **Install VersionMark**: Install the tool in each job that needs to capture versions
2. **Capture per Job**: Run `versionmark capture` with a unique `--job-id` in each job
3. **Upload Artifacts**: Save the captured JSON files as artifacts
4. **Download Artifacts**: In the publish job, download all captured JSON files
5. **Publish**: Run `versionmark publish` to generate consolidated documentation

## Common Workflows

### Workflow 1: Track Build Environment Versions

Use VersionMark to document which tool versions are used in your build environment:

```yaml
# .versionmark.yaml
tools:
  - name: dotnet
    command: dotnet --version
    regex: '(\d+\.\d+\.\d+)'
  
  - name: msbuild
    command: msbuild -version
    regex: '(\d+\.\d+\.\d+\.\d+)'
```

### Workflow 2: Compare Versions Across Platforms

Track how tool versions differ between Windows, Linux, and macOS:

```yaml
# .versionmark.yaml
tools:
  - name: gcc
    command: gcc --version
    regex: 'gcc.*?(\d+\.\d+\.\d+)'
  
  - name: clang
    command: clang --version
    regex: 'clang version (\d+\.\d+\.\d+)'
```

### Workflow 3: Monitor Dependency Versions

Track versions of runtime dependencies and tools:

```yaml
# .versionmark.yaml
tools:
  - name: docker
    command: docker --version
    regex: 'Docker version (\d+\.\d+\.\d+)'
  
  - name: kubectl
    command: kubectl version --client --short
    regex: 'v(\d+\.\d+\.\d+)'
  
  - name: terraform
    command: terraform version
    regex: 'Terraform v(\d+\.\d+\.\d+)'
```

## Troubleshooting

### Tool Not Found

If a tool command fails because the tool is not installed:

- VersionMark will report an error for that tool
- The capture will continue for other tools
- The published output will note which tools failed to capture

### Version Not Matched

If the regex doesn't match the command output:

- Check the actual output of the command manually
- Adjust the regex to match the format
- Use online regex testers to validate your pattern
- Remember to escape special regex characters

### OS-Specific Issues

If a tool behaves differently on different platforms:

- Use the `overrides` section to provide platform-specific commands
- Test on each platform to ensure the commands work
- Consider using platform-specific tools in separate configurations

## Best Practices

1. **Use Descriptive Job IDs**: Make job-ids descriptive (e.g., `windows-net8-release` instead of `job1`)
2. **Version Control Config**: Commit `.versionmark.yaml` to version control
3. **Automate Publishing**: Integrate publishing into your CI/CD pipeline
4. **Regular Updates**: Update captured versions regularly to track changes
5. **Document Changes**: Review version changes in pull requests
6. **Test Locally**: Test capture commands locally before adding to CI/CD
