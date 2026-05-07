### Program Unit Verification

#### Overview

The `Program` unit is the entry point for the VersionMark tool. It dispatches to the
appropriate subsystem based on parsed context flags. Unit tests are in `ProgramTests.cs`.
Each test constructs a `Context` with specific arguments and calls `Program.Run`, then
asserts on console output and exit code. `StringWriter` captures console output.

#### Test Scenarios

The following test scenarios verify `Program` unit requirements:

- **`Program_Version_ReturnsNonEmptyString`**: `Program.Version` returns a non-empty version string.
- **`Program_Run_WithVersionFlag_DisplaysVersionOnly`**: `--version` writes version only (no copyright).
- **`Program_Run_WithHelpFlag_DisplaysUsageInformation`**: `--help` writes usage information.
- **`Program_Run_WithValidateFlag_RunsValidation`**: `--validate` writes output containing "Total Tests:".
- **`Program_Run_NoArguments_DisplaysDefaultBehavior`**: No arguments writes banner with version and copyright.
- **`Program_Run_WithCaptureCommand_CapturesToolVersions`**: `--capture --job-id --output -- dotnet` creates output file.
- **`Program_Run_WithCaptureCommandNoToolFilter_CapturesAllConfiguredTools`**:
  Capture without tool filter captures all configured tools.
- **`Program_Run_WithCaptureCommandWithoutJobId_ReturnsError`**:
  Missing `--job-id` returns a non-zero exit code.
- **`Program_Run_WithCaptureCommandWithMissingConfig_ReturnsError`**:
  Missing `.versionmark.yaml` returns a non-zero exit code.
- **`Program_Run_WithPublishCommandWithoutReport_ReturnsError`**:
  `--publish` without `--report` returns a non-zero exit code.
- **`Program_Run_WithPublishCommandNoMatchingFiles_ReturnsError`**: Glob matching no files returns a non-zero exit code.
- **`Program_Run_WithPublishCommandInvalidJson_ReturnsError`**: Invalid JSON file returns a non-zero exit code.
- **`Program_Run_WithPublishCommand_GeneratesMarkdownReport`**: Full publish generates a markdown report file.
- **`Program_Run_WithPublishCommandCustomDepth_AdjustsHeadingLevels`**: Custom depth adjusts heading
  levels in the report.
- **`Program_Run_WithLintFlag_ValidConfig_ReturnsSuccess`**: Valid config lint returns exit code 0.
- **`Program_Run_WithLintFlag_ValidConfig_SuppressesBanner`**: Lint mode suppresses the banner.
- **`Program_Run_WithLintFlag_InvalidConfig_ReturnsError`**: Invalid config lint reports error
  and returns non-zero exit code.
- **`Program_Run_WithLintFlag_NoFile_UsesDefaultConfigFile`**: No file argument uses `.versionmark.yaml` as default.
- **`Program_Run_WithHelpFlag_IncludesLintInformation`**: Help output includes lint command information.

#### Dependencies

Tests use `StringWriter` to capture console output. Capture and publish tests use
temporary directories and configuration files created during test setup.

#### Requirements Coverage

The following list maps `Program` unit requirements to test scenarios:

- **`VersionMark-Program-Version`**: `Program_Version_ReturnsNonEmptyString`
- **`VersionMark-Program-Dispatch`**: `Program_Run_WithVersionFlag_DisplaysVersionOnly`,
  `Program_Run_WithHelpFlag_DisplaysUsageInformation`,
  `Program_Run_WithValidateFlag_RunsValidation`, `Program_Run_NoArguments_DisplaysDefaultBehavior`
- **`VersionMark-Program-RunCapture`**: `Program_Run_WithCaptureCommand_CapturesToolVersions`,
  `Program_Run_WithCaptureCommandNoToolFilter_CapturesAllConfiguredTools`,
  `Program_Run_WithCaptureCommandWithoutJobId_ReturnsError`,
  `Program_Run_WithCaptureCommandWithMissingConfig_ReturnsError`
- **`VersionMark-Program-RunPublish`**: `Program_Run_WithPublishCommandWithoutReport_ReturnsError`,
  `Program_Run_WithPublishCommandNoMatchingFiles_ReturnsError`,
  `Program_Run_WithPublishCommandInvalidJson_ReturnsError`,
  `Program_Run_WithPublishCommand_GeneratesMarkdownReport`,
  `Program_Run_WithPublishCommandCustomDepth_AdjustsHeadingLevels`
- **`VersionMark-Program-RunLint`**: `Program_Run_WithLintFlag_ValidConfig_ReturnsSuccess`,
  `Program_Run_WithLintFlag_ValidConfig_SuppressesBanner`,
  `Program_Run_WithLintFlag_InvalidConfig_ReturnsError`,
  `Program_Run_WithLintFlag_NoFile_UsesDefaultConfigFile`,
  `Program_Run_WithHelpFlag_IncludesLintInformation`
