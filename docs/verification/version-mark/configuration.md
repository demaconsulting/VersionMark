## Configuration Subsystem Verification

### Overview

The Configuration subsystem is responsible for loading and validating the
`.versionmark.yaml` configuration file. It consists of three units: `VersionMarkConfig`
(the top-level configuration container), `ToolConfig` (per-tool configuration record), and
`LintIssue` (lint issue record and load result).

Subsystem-level integration tests are in `Configuration/ConfigurationTests.cs` and cover
the full configuration loading workflow via `VersionMarkConfig.ReadFromFile`. Unit-level
verification for each unit is in the chapters that follow.

### Verification Approach

Integration tests write temporary YAML files to disk and call `VersionMarkConfig.ReadFromFile`,
then assert on the returned configuration object. No external mocks or stubs are required.

### Test Scenarios

The following integration test scenarios verify Configuration subsystem requirements:

- **`Configuration_ReadFromFile_MultipleTools_AllToolsAccessible`**: Multiple tools all accessible after load.
- **`Configuration_ReadFromFile_WithOsOverrides_SelectsAppropriateCommand`**: OS overrides select correct command.
- **`Configuration_ReadFromFile_OsRegexOverride_SelectsAppropriateRegex`**: OS regex overrides select correct regex.
- **`Configuration_ReadFromFile_EmptyTools_ThrowsArgumentException`**: Empty tools section throws ArgumentException.
- **`Configuration_ReadFromFile_MissingFile_ThrowsArgumentException`**: Missing config file throws ArgumentException.
- **`Configuration_ReadFromFile_InvalidYaml_ThrowsArgumentException`**: Invalid YAML throws ArgumentException.

### Dependencies

No external mocks are required. Tests read from temporary YAML files written during the
test setup.

### Requirements Coverage

The following list maps Configuration subsystem requirements to test scenarios:

- **`VersionMark-Configuration-YamlConfig`**: `Configuration_ReadFromFile_MultipleTools_AllToolsAccessible`
- **`VersionMark-Configuration-ToolDefinition`**: `Configuration_ReadFromFile_MultipleTools_AllToolsAccessible`
- **`VersionMark-Configuration-OsCommandOverride`**:
  `Configuration_ReadFromFile_WithOsOverrides_SelectsAppropriateCommand`
- **`VersionMark-Configuration-OsRegexOverride`**:
  `Configuration_ReadFromFile_OsRegexOverride_SelectsAppropriateRegex`
- **`VersionMark-Configuration-ValidateTools`**: `Configuration_ReadFromFile_EmptyTools_ThrowsArgumentException`
- **`VersionMark-Configuration-ReadError`**: `Configuration_ReadFromFile_MissingFile_ThrowsArgumentException`
- **`VersionMark-Configuration-ParseError`**: `Configuration_ReadFromFile_InvalidYaml_ThrowsArgumentException`
