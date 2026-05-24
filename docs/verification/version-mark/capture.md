## Capture

### Verification Approach

The Capture subsystem is responsible for executing tool version commands, extracting
version strings, and serializing the results to JSON. It consists of one unit:
`VersionInfo` (the JSON version data record). Subsystem-level integration tests are in
`Capture/CaptureTests.cs` and cover the full capture workflow including configuration
loading, command execution, output file writing, and loading the saved data. Tests use a
temporary directory containing a `.versionmark.yaml` configuration file. No external API
mocks are required.

### Test Environment

N/A - standard test environment. Tests create temporary directories and configuration files
during setup and clean them up afterwards.

### Acceptance Criteria

- All subsystem integration tests pass with zero failures across all supported OS and .NET
  version matrix combinations (Windows, Linux, macOS x .NET 8, .NET 9, .NET 10).
- Every requirement for the Capture subsystem is covered by at least one named test
  scenario.

### Test Scenarios

**Capture_Context_CaptureFlag_SetsCaptureMode**: `--capture` sets capture mode in the
context. This scenario is tested by `Capture_Context_CaptureFlag_SetsCaptureMode`.

**Capture_Context_WithJobId_SetsJobId**: `--job-id` sets the job ID in the context. This
scenario is tested by `Capture_Context_WithJobId_SetsJobId`.

**Capture_Run_NoOutputFlagSpecified_UsesDefaultFilename**: When no `--output` flag is
specified, the default output filename is derived from the job ID. This scenario is tested
by `Capture_Run_NoOutputFlagSpecified_UsesDefaultFilename`.

**Capture_Context_WithToolFilter_SetsToolNames**: Tool filter patterns after `--` are
captured in the context. This scenario is tested by
`Capture_Context_WithToolFilter_SetsToolNames`.

**Capture_Run_NoToolFilter_CapturesAllConfiguredTools**: When no tool filter is specified,
all configured tools are captured. This scenario is tested by
`Capture_Run_NoToolFilter_CapturesAllConfiguredTools`.

**Capture_Config_ReadFromFile_LoadsToolDefinitions**: The config file loads all tool
definitions correctly. This scenario is tested by
`Capture_Config_ReadFromFile_LoadsToolDefinitions`.

**Capture_FindVersions_ExecutesCommandAndExtractsVersion**: The tool command is executed and
the version string is extracted via the configured regex. This scenario is tested by
`Capture_FindVersions_ExecutesCommandAndExtractsVersion`.

**Capture_Run_DisplaysCapturedVersionsAfterCapture**: Captured versions are displayed in
the output after capture completes. This scenario is tested by
`Capture_Run_DisplaysCapturedVersionsAfterCapture`.

**Capture_Run_MissingConfig_ReportsError**: A missing config file reports an error. This
scenario is tested by `Capture_Run_MissingConfig_ReportsError`.

**Capture_SaveAndLoad_PreservesAllVersionData**: A save and load cycle preserves all
version data. This scenario is tested by `Capture_SaveAndLoad_PreservesAllVersionData`.

**Capture_MultipleCaptures_EachFileHasDistinctJobId**: Multiple capture files each have a
distinct job ID. This scenario is tested by
`Capture_MultipleCaptures_EachFileHasDistinctJobId`.
