## Capture Subsystem Verification

### Overview

The Capture subsystem is responsible for executing tool version commands, extracting
version strings, and serializing the results to JSON. It consists of one unit:
`VersionInfo` (the JSON version data record).

Subsystem-level integration tests are in `Capture/CaptureTests.cs` and cover the full
capture workflow including configuration loading, command execution, output file writing,
and loading the saved data. Unit-level verification for `VersionInfo` is in the chapter
that follows.

### Verification Approach

Integration tests use a temporary directory containing a `.versionmark.yaml` configuration
file. Tests invoke capture operations and assert on the written JSON files and displayed
output. No external API mocks are required.

### Test Scenarios

The following integration test scenarios verify Capture subsystem requirements:

- **`Capture_Context_CaptureFlag_SetsCaptureMode`**: `--capture` sets capture mode in context.
- **`Capture_Context_WithJobId_SetsJobId`**: `--job-id` sets the job ID in context.
- **`Capture_Run_NoOutputFlagSpecified_UsesDefaultFilename`**: Default output filename is derived from job ID.
- **`Capture_Context_WithToolFilter_SetsToolNames`**: Tool filter patterns after `--` are captured.
- **`Capture_Run_NoToolFilter_CapturesAllConfiguredTools`**: No tool filter captures all configured tools.
- **`Capture_Config_ReadFromFile_LoadsToolDefinitions`**: Config file loads all tool definitions.
- **`Capture_FindVersions_ExecutesCommandAndExtractsVersion`**: Command is executed and version extracted via regex.
- **`Capture_Run_DisplaysCapturedVersionsAfterCapture`**: Captured versions are displayed after capture.
- **`Capture_Run_MissingConfig_ReportsError`**: Missing config file reports an error.
- **`Capture_SaveAndLoad_PreservesAllVersionData`**: Save and load cycle preserves all version data.
- **`Capture_MultipleCaptures_EachFileHasDistinctJobId`**: Multiple capture files each have a distinct job ID.

### Dependencies

No external mocks are required. Tests use temporary directories and configuration files
created during test setup.

### Requirements Coverage

The following list maps Capture subsystem requirements to test scenarios:

- **`VersionMark-Capture-Capture`**: `Capture_Context_CaptureFlag_SetsCaptureMode`
- **`VersionMark-Capture-JobId`**: `Capture_Context_WithJobId_SetsJobId`
- **`VersionMark-Capture-Output`**: `Capture_SaveAndLoad_PreservesAllVersionData`
- **`VersionMark-Capture-DefaultOutput`**: `Capture_Run_NoOutputFlagSpecified_UsesDefaultFilename`
- **`VersionMark-Capture-ToolFilter`**: `Capture_Context_WithToolFilter_SetsToolNames`
- **`VersionMark-Capture-MultipleTools`**: `Capture_Run_NoToolFilter_CapturesAllConfiguredTools`
- **`VersionMark-Capture-Config`**: `Capture_Config_ReadFromFile_LoadsToolDefinitions`
- **`VersionMark-Capture-Command`**: `Capture_FindVersions_ExecutesCommandAndExtractsVersion`
- **`VersionMark-Capture-JsonOutput`**: `Capture_SaveAndLoad_PreservesAllVersionData`,
  `Capture_MultipleCaptures_EachFileHasDistinctJobId`
- **`VersionMark-Capture-Display`**: `Capture_Run_DisplaysCapturedVersionsAfterCapture`
- **`VersionMark-Capture-ConfigError`**: `Capture_Run_MissingConfig_ReportsError`
- **`VersionMark-Capture-CommandFailure`**: `Capture_FindVersions_ExecutesCommandAndExtractsVersion`
