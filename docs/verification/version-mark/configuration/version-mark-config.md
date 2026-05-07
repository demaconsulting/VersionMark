### VersionMarkConfig Unit Verification

#### Overview

The `VersionMarkConfig` unit is the top-level configuration container. It reads and
validates `.versionmark.yaml` files and provides `FindVersions` to execute tool commands
and extract version strings. Tests are split across two files:
`Configuration/VersionMarkConfigTests.cs` (covering `ReadFromFile` and `FindVersions`)
and `Configuration/VersionMarkConfigLoadTests.cs` (covering the `Load` method).

#### Test Scenarios — ReadFromFile

The following test scenarios verify `VersionMarkConfig.ReadFromFile`:

- **`VersionMarkConfig_ReadFromFile_ValidFile_ReturnsConfig`**: Valid file returns a populated config object.
- **`VersionMarkConfig_ReadFromFile_WithAllOsOverrides_ReturnsConfig`**: File with all OS overrides returns config.
- **`VersionMarkConfig_ReadFromFile_NonExistentFile_ThrowsArgumentException`**: Non-existent file throws ArgumentException.
- **`VersionMarkConfig_ReadFromFile_InvalidYaml_ThrowsArgumentException`**: Invalid YAML throws ArgumentException.
- **`VersionMarkConfig_ReadFromFile_NoTools_ThrowsArgumentException`**: No tools section throws ArgumentException.

#### Test Scenarios — FindVersions

The following test scenarios verify `VersionMarkConfig.FindVersions`:

- **`VersionMarkConfig_FindVersions_DotnetCommand_ReturnsVersionInfo`**: `dotnet --version` executes and returns version.
- **`VersionMarkConfig_FindVersions_MultipleTools_ReturnsAllVersions`**: Multiple tools returns all version entries.
- **`VersionMarkConfig_FindVersions_NonExistentTool_ThrowsArgumentException`**: Tool not in config throws ArgumentException.
- **`VersionMarkConfig_FindVersions_InvalidCommand_ThrowsInvalidOperationException`**:
  Invalid command throws InvalidOperationException.
- **`VersionMarkConfig_FindVersions_RegexNoMatch_ThrowsInvalidOperationException`**:
  No regex match throws InvalidOperationException.
- **`VersionMarkConfig_FindVersions_RegexNoVersionGroup_ThrowsInvalidOperationException`**:
  Missing version group throws InvalidOperationException.

#### Test Scenarios — Load

The following test scenarios verify `VersionMarkConfig.Load`:

- **`VersionMarkConfig_Load_ValidConfig_ReturnsConfig`**: Valid config returns a config with no issues.
- **`VersionMarkConfig_Load_MissingFile_ReturnsNullConfig`**: Missing file returns a null config with an issue.
- **`VersionMarkConfig_Load_InvalidYaml_ReturnsNullConfig`**: Invalid YAML returns a null config with an issue.
- **`VersionMarkConfig_Load_MissingToolsSection_ReturnsNullConfig`**: Missing tools section returns a null config.
- **`VersionMarkConfig_Load_EmptyToolsSection_ReturnsNullConfig`**: Empty tools section returns a null config.
- **`VersionMarkConfig_Load_MissingCommand_ReturnsNullConfig`**: Missing command returns a null config with an issue.
- **`VersionMarkConfig_Load_EmptyCommand_ReturnsNullConfig`**: Empty command returns a null config with an issue.
- **`VersionMarkConfig_Load_MissingRegex_ReturnsNullConfig`**: Missing regex returns a null config with an issue.
- **`VersionMarkConfig_Load_EmptyRegex_ReturnsNullConfig`**: Empty regex returns a null config with an issue.
- **`VersionMarkConfig_Load_InvalidRegex_ReturnsNullConfig`**: Invalid regex returns a null config with an issue.
- **`VersionMarkConfig_Load_RegexMissingVersionGroup_ReturnsNullConfig`**: Regex without version group returns a null config.
- **`VersionMarkConfig_Load_UnknownTopLevelKey_ReturnsConfig`**: Unknown top-level key is tolerated; config is returned.
- **`VersionMarkConfig_Load_UnknownToolKey_ReturnsConfig`**: Unknown tool-level key is tolerated; config is returned.
- **`VersionMarkConfig_Load_OsSpecificEmptyCommand_ReturnsNullConfig`**: Empty OS-specific command returns a null config.
- **`VersionMarkConfig_Load_OsSpecificEmptyRegex_ReturnsNullConfig`**: Empty OS-specific regex returns a null config.
- **`VersionMarkConfig_Load_OsSpecificRegexMissingVersionGroup_ReturnsNullConfig`**:
  OS-specific regex without version group returns null.
- **`VersionMarkConfig_Load_OsSpecificInvalidRegex_ReturnsNullConfig`**: OS-specific invalid regex returns a null config.
- **`VersionMarkConfig_Load_MultipleErrors_ReportsAll`**: Multiple errors are all reported in the issue list.
- **`VersionMarkConfig_Load_IssuesContainFilePath`**: Issue records include the config file path.

#### Dependencies

No external mocks are required. Tests write temporary YAML files to disk.

#### Requirements Coverage

The following list maps `VersionMarkConfig` unit requirements to test scenarios:

- **`VersionMark-VersionMarkConfig-ReadFromFile`**: `VersionMarkConfig_ReadFromFile_ValidFile_ReturnsConfig`,
  `VersionMarkConfig_ReadFromFile_WithAllOsOverrides_ReturnsConfig`,
  `VersionMarkConfig_ReadFromFile_NonExistentFile_ThrowsArgumentException`,
  `VersionMarkConfig_ReadFromFile_InvalidYaml_ThrowsArgumentException`,
  `VersionMarkConfig_ReadFromFile_NoTools_ThrowsArgumentException`
- **`VersionMark-VersionMarkConfig-FindVersions`**: `VersionMarkConfig_FindVersions_DotnetCommand_ReturnsVersionInfo`,
  `VersionMarkConfig_FindVersions_MultipleTools_ReturnsAllVersions`,
  `VersionMarkConfig_FindVersions_NonExistentTool_ThrowsArgumentException`,
  `VersionMarkConfig_FindVersions_InvalidCommand_ThrowsInvalidOperationException`,
  `VersionMarkConfig_FindVersions_RegexNoMatch_ThrowsInvalidOperationException`,
  `VersionMarkConfig_FindVersions_RegexNoVersionGroup_ThrowsInvalidOperationException`
- **`VersionMark-VersionMarkConfig-Load`**: All `VersionMarkConfig_Load_*` test scenarios above
- **`VersionMark-Load-Method`**: `VersionMarkConfig_Load_ValidConfig_ReturnsConfig`
- **`VersionMark-Load-FileExistence`**: `VersionMarkConfig_Load_MissingFile_ReturnsNullConfig`
- **`VersionMark-Load-YamlParsing`**: `VersionMarkConfig_Load_InvalidYaml_ReturnsNullConfig`
- **`VersionMark-Load-ToolsSection`**: `VersionMarkConfig_Load_MissingToolsSection_ReturnsNullConfig`,
  `VersionMarkConfig_Load_EmptyToolsSection_ReturnsNullConfig`
- **`VersionMark-Load-ToolCommand`**: `VersionMarkConfig_Load_MissingCommand_ReturnsNullConfig`,
  `VersionMarkConfig_Load_EmptyCommand_ReturnsNullConfig`
- **`VersionMark-Load-ToolRegex`**: `VersionMarkConfig_Load_MissingRegex_ReturnsNullConfig`,
  `VersionMarkConfig_Load_EmptyRegex_ReturnsNullConfig`
- **`VersionMark-Load-RegexValid`**: `VersionMarkConfig_Load_InvalidRegex_ReturnsNullConfig`
- **`VersionMark-Load-RegexVersion`**: `VersionMarkConfig_Load_RegexMissingVersionGroup_ReturnsNullConfig`
- **`VersionMark-Load-OsOverrides`**: `VersionMarkConfig_Load_OsSpecificEmptyCommand_ReturnsNullConfig`,
  `VersionMarkConfig_Load_OsSpecificEmptyRegex_ReturnsNullConfig`,
  `VersionMarkConfig_Load_OsSpecificRegexMissingVersionGroup_ReturnsNullConfig`,
  `VersionMarkConfig_Load_OsSpecificInvalidRegex_ReturnsNullConfig`
- **`VersionMark-Load-UnknownKeys`**: `VersionMarkConfig_Load_UnknownTopLevelKey_ReturnsConfig`,
  `VersionMarkConfig_Load_UnknownToolKey_ReturnsConfig`
- **`VersionMark-Load-ErrorLocation`**: `VersionMarkConfig_Load_IssuesContainFilePath`
- **`VersionMark-Load-AllIssues`**: `VersionMarkConfig_Load_MultipleErrors_ReportsAll`
