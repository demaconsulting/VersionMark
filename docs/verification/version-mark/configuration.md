## Configuration

### Verification Approach

The Configuration subsystem is responsible for loading and validating the
`.versionmark.yaml` configuration file. It consists of three units: `VersionMarkConfig`
(the top-level configuration container), `ToolConfig` (per-tool configuration record), and
`LintIssue` (lint issue record and load result). Subsystem-level integration tests are in
`Configuration/ConfigurationTests.cs` and cover the full configuration loading workflow via
`VersionMarkConfig.ReadFromFile`. Tests write temporary YAML files to disk and assert on
the returned configuration object. No external mocks or stubs are required.

### Test Environment

N/A - standard test environment. Tests write temporary YAML files to disk during setup and
clean them up afterwards. No additional environment configuration is required.

### Acceptance Criteria

- All subsystem integration tests pass with zero failures across all supported OS and .NET
  version matrix combinations (Windows, Linux, macOS x .NET 8, .NET 9, .NET 10).
- Every requirement for the Configuration subsystem is covered by at least one named test
  scenario.

### Test Scenarios

**Configuration_ReadFromFile_MultipleTools_AllToolsAccessible**: A YAML file defining
multiple tools is loaded and all tools are accessible in the returned config object. This
scenario is tested by `Configuration_ReadFromFile_MultipleTools_AllToolsAccessible`.

**Configuration_ReadFromFile_WithOsOverrides_SelectsAppropriateCommand**: A YAML file with
OS-specific command overrides is loaded and the correct command is selected for the current
OS. This scenario is tested by
`Configuration_ReadFromFile_WithOsOverrides_SelectsAppropriateCommand`.

**Configuration_ReadFromFile_OsRegexOverride_SelectsAppropriateRegex**: A YAML file with
OS-specific regex overrides is loaded and the correct regex is selected for the current OS.
This scenario is tested by
`Configuration_ReadFromFile_OsRegexOverride_SelectsAppropriateRegex`.

**Configuration_ReadFromFile_EmptyTools_ThrowsArgumentException**: A YAML file with an
empty tools section throws `ArgumentException` during loading. This scenario is tested by
`Configuration_ReadFromFile_EmptyTools_ThrowsArgumentException`.

**Configuration_ReadFromFile_MissingFile_ThrowsArgumentException**: A path pointing to a
non-existent file throws `ArgumentException`. This scenario is tested by
`Configuration_ReadFromFile_MissingFile_ThrowsArgumentException`.

**Configuration_ReadFromFile_InvalidYaml_ThrowsArgumentException**: A file containing
invalid YAML throws `ArgumentException` during loading. This scenario is tested by
`Configuration_ReadFromFile_InvalidYaml_ThrowsArgumentException`.
