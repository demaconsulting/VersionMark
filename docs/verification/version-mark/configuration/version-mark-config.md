### VersionMarkConfig

#### Verification Approach

The `VersionMarkConfig` unit is the top-level configuration container. It reads and
validates `.versionmark.yaml` files and provides `FindVersions` to execute tool commands
and extract version strings. Tests are split across two files:
`Configuration/VersionMarkConfigTests.cs` (covering `ReadFromFile` and `FindVersions`)
and `Configuration/VersionMarkConfigLoadTests.cs` (covering the `Load` method). Tests
write temporary YAML files to disk and call the relevant method, then assert on the returned
configuration object or exception. No external mocks are required.

#### Test Environment

N/A - standard test environment. Tests write temporary YAML files to disk during setup and
clean them up afterwards. No additional environment configuration is required.

#### Acceptance Criteria

- All unit tests for `VersionMarkConfig` pass with zero failures across all supported OS
  and .NET version matrix combinations (Windows, Linux, macOS x .NET 8, .NET 9, .NET 10).
- Every requirement for the `VersionMarkConfig` unit is covered by at least one named test
  scenario.

#### Test Scenarios

**VersionMarkConfig_ReadFromFile_ValidFile_ReturnsConfig**: A valid YAML file returns a
populated config object. This scenario is tested by
`VersionMarkConfig_ReadFromFile_ValidFile_ReturnsConfig`.

**VersionMarkConfig_ReadFromFile_WithAllOsOverrides_ReturnsConfig**: A file with all OS
overrides returns a config with the correct override values. This scenario is tested by
`VersionMarkConfig_ReadFromFile_WithAllOsOverrides_ReturnsConfig`.

**VersionMarkConfig_ReadFromFile_NonExistentFile_ThrowsArgumentException**: A path to a
non-existent file throws `ArgumentException`. This scenario is tested by
`VersionMarkConfig_ReadFromFile_NonExistentFile_ThrowsArgumentException`.

**VersionMarkConfig_ReadFromFile_InvalidYaml_ThrowsArgumentException**: A file containing
invalid YAML throws `ArgumentException`. This scenario is tested by
`VersionMarkConfig_ReadFromFile_InvalidYaml_ThrowsArgumentException`.

**VersionMarkConfig_ReadFromFile_NoTools_ThrowsArgumentException**: A YAML file with no
tools section throws `ArgumentException`. This scenario is tested by
`VersionMarkConfig_ReadFromFile_NoTools_ThrowsArgumentException`.

**VersionMarkConfig_FindVersions_DotnetCommand_ReturnsVersionInfo**: `dotnet --version`
executes and returns a `VersionInfo` entry. This scenario is tested by
`VersionMarkConfig_FindVersions_DotnetCommand_ReturnsVersionInfo`.

**VersionMarkConfig_FindVersions_MultipleTools_ReturnsAllVersions**: Multiple tools in
the config return all version entries. This scenario is tested by
`VersionMarkConfig_FindVersions_MultipleTools_ReturnsAllVersions`.

**VersionMarkConfig_FindVersions_NonExistentTool_ThrowsArgumentException**: A tool name
not present in the config throws `ArgumentException`. This scenario is tested by
`VersionMarkConfig_FindVersions_NonExistentTool_ThrowsArgumentException`.

**VersionMarkConfig_FindVersions_InvalidCommand_ThrowsInvalidOperationException**: An
invalid command throws `InvalidOperationException`. This scenario is tested by
`VersionMarkConfig_FindVersions_InvalidCommand_ThrowsInvalidOperationException`.

**VersionMarkConfig_FindVersions_RegexNoMatch_ThrowsInvalidOperationException**: A regex
that does not match the command output throws `InvalidOperationException`. This scenario
is tested by `VersionMarkConfig_FindVersions_RegexNoMatch_ThrowsInvalidOperationException`.

**VersionMarkConfig_FindVersions_RegexNoVersionGroup_ThrowsInvalidOperationException**: A
regex without a named version group throws `InvalidOperationException`. This scenario is
tested by
`VersionMarkConfig_FindVersions_RegexNoVersionGroup_ThrowsInvalidOperationException`.

**VersionMarkConfig_FindVersions_OsOnlyCommand_MatchingOs_ReturnsVersionInfo**: A tool
with only an OS-specific command succeeds when the matching OS is specified. This scenario
is tested by
`VersionMarkConfig_FindVersions_OsOnlyCommand_MatchingOs_ReturnsVersionInfo`.

**VersionMarkConfig_FindVersions_OsOnlyCommand_WrongOs_ThrowsInvalidOperationException**: A
tool with only an OS-specific command throws `InvalidOperationException` when a
non-matching OS is specified. This scenario is tested by
`VersionMarkConfig_FindVersions_OsOnlyCommand_WrongOs_ThrowsInvalidOperationException`.

**VersionMarkConfig_Load_ValidConfig_ReturnsConfig**: A valid config file returns a config
with no issues. This scenario is tested by `VersionMarkConfig_Load_ValidConfig_ReturnsConfig`.

**VersionMarkConfig_Load_MissingFile_ReturnsNullConfig**: A missing file returns a null
config with an issue. This scenario is tested by
`VersionMarkConfig_Load_MissingFile_ReturnsNullConfig`.

**VersionMarkConfig_Load_InvalidYaml_ReturnsNullConfig**: Invalid YAML returns a null
config with an issue. This scenario is tested by
`VersionMarkConfig_Load_InvalidYaml_ReturnsNullConfig`.

**VersionMarkConfig_Load_MissingToolsSection_ReturnsNullConfig**: A missing tools section
returns a null config. This scenario is tested by
`VersionMarkConfig_Load_MissingToolsSection_ReturnsNullConfig`.

**VersionMarkConfig_Load_EmptyToolsSection_ReturnsNullConfig**: An empty tools section
returns a null config. This scenario is tested by
`VersionMarkConfig_Load_EmptyToolsSection_ReturnsNullConfig`.

**VersionMarkConfig_Load_MissingCommand_ReturnsNullConfig**: A missing command field
returns a null config with an issue. This scenario is tested by
`VersionMarkConfig_Load_MissingCommand_ReturnsNullConfig`.

**VersionMarkConfig_Load_EmptyCommand_ReturnsNullConfig**: An empty command field returns
a null config with an issue. This scenario is tested by
`VersionMarkConfig_Load_EmptyCommand_ReturnsNullConfig`.

**VersionMarkConfig_Load_MissingRegex_ReturnsNullConfig**: A missing regex field returns
a null config with an issue. This scenario is tested by
`VersionMarkConfig_Load_MissingRegex_ReturnsNullConfig`.

**VersionMarkConfig_Load_EmptyRegex_ReturnsNullConfig**: An empty regex field returns a
null config with an issue. This scenario is tested by
`VersionMarkConfig_Load_EmptyRegex_ReturnsNullConfig`.

**VersionMarkConfig_Load_InvalidRegex_ReturnsNullConfig**: An invalid regex returns a null
config with an issue. This scenario is tested by
`VersionMarkConfig_Load_InvalidRegex_ReturnsNullConfig`.

**VersionMarkConfig_Load_RegexMissingVersionGroup_ReturnsNullConfig**: A regex without a
version capture group returns a null config. This scenario is tested by
`VersionMarkConfig_Load_RegexMissingVersionGroup_ReturnsNullConfig`.

**VersionMarkConfig_Load_UnknownTopLevelKey_ReturnsConfig**: An unknown top-level key is
tolerated and the config is returned. This scenario is tested by
`VersionMarkConfig_Load_UnknownTopLevelKey_ReturnsConfig`.

**VersionMarkConfig_Load_UnknownToolKey_ReturnsConfig**: An unknown tool-level key is
tolerated and the config is returned. This scenario is tested by
`VersionMarkConfig_Load_UnknownToolKey_ReturnsConfig`.

**VersionMarkConfig_Load_OsSpecificEmptyCommand_ReturnsNullConfig**: An empty OS-specific
command field returns a null config. This scenario is tested by
`VersionMarkConfig_Load_OsSpecificEmptyCommand_ReturnsNullConfig`.

**VersionMarkConfig_Load_OsSpecificEmptyRegex_ReturnsNullConfig**: An empty OS-specific
regex field returns a null config. This scenario is tested by
`VersionMarkConfig_Load_OsSpecificEmptyRegex_ReturnsNullConfig`.

**VersionMarkConfig_Load_OsSpecificRegexMissingVersionGroup_ReturnsNullConfig**: An
OS-specific regex without a version group returns a null config. This scenario is tested by
`VersionMarkConfig_Load_OsSpecificRegexMissingVersionGroup_ReturnsNullConfig`.

**VersionMarkConfig_Load_OsSpecificInvalidRegex_ReturnsNullConfig**: An OS-specific invalid
regex returns a null config. This scenario is tested by
`VersionMarkConfig_Load_OsSpecificInvalidRegex_ReturnsNullConfig`.

**VersionMarkConfig_Load_OsOnlyCommand_ReturnsConfig**: A tool with only OS-specific
commands (no default command) is valid and returns a config. This scenario is tested by
`VersionMarkConfig_Load_OsOnlyCommand_ReturnsConfig`.

**VersionMarkConfig_Load_OsOnlyRegex_ReturnsConfig**: A tool with only OS-specific regex
(no default regex) is valid and returns a config. This scenario is tested by
`VersionMarkConfig_Load_OsOnlyRegex_ReturnsConfig`.

**VersionMarkConfig_Load_OsOnlyCommandAndRegex_ReturnsConfig**: A tool with only
OS-specific commands and regex (no defaults) is valid and returns a config. This scenario
is tested by `VersionMarkConfig_Load_OsOnlyCommandAndRegex_ReturnsConfig`.

**VersionMarkConfig_Load_MultipleErrors_ReportsAll**: Multiple validation errors are all
reported in the issue list. This scenario is tested by
`VersionMarkConfig_Load_MultipleErrors_ReportsAll`.

**VersionMarkConfig_Load_IssuesContainFilePath**: Issue records include the config file
path. This scenario is tested by `VersionMarkConfig_Load_IssuesContainFilePath`.
