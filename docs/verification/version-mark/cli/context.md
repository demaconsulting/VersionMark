### Context Unit Verification

#### Overview

The `Context` unit parses command-line arguments and manages all output routing (console,
error stream, and log file). Unit tests are in `Cli/ContextTests.cs`. Each test
constructs a `Context` via `Context.Create()` with specific argument arrays and asserts on
the resulting property values. `StringWriter` captures console output.

#### Test Scenarios

The following test scenarios verify `Context` unit requirements:

- **`Context_Create_NoArguments_ReturnsDefaultContext`**: No arguments creates a default context with all flags false.
- **`Context_Create_VersionFlag_SetsVersionTrue`**: `--version` sets `Version = true`.
- **`Context_Create_ShortVersionFlag_SetsVersionTrue`**: `-v` sets `Version = true`.
- **`Context_Create_HelpFlag_SetsHelpTrue`**: `--help` sets `Help = true`.
- **`Context_Create_ShortHelpFlag_H_SetsHelpTrue`**: `-h` sets `Help = true`.
- **`Context_Create_ShortHelpFlag_Question_SetsHelpTrue`**: `-?` sets `Help = true`.
- **`Context_Create_SilentFlag_SetsSilentTrue`**: `--silent` sets `Silent = true`.
- **`Context_Create_ValidateFlag_SetsValidateTrue`**: `--validate` sets `Validate = true`.
- **`Context_Create_ResultsFlag_SetsResultsFile`**: `--results file.trx` sets `ResultsFile`.
- **`Context_Create_ResultFlag_SetsResultsFile`**: `--result file.trx` (alias) sets `ResultsFile`.
- **`Context_Create_LogFlag_OpensLogFile`**: `--log file.log` opens a log file writer.
- **`Context_Create_UnknownArgument_ThrowsArgumentException`**: Unknown argument throws ArgumentException.
- **`Context_Create_LogFlag_WithoutValue_ThrowsArgumentException`**: `--log` without a value throws ArgumentException.
- **`Context_Create_ResultsFlag_WithoutValue_ThrowsArgumentException`**: `--results` without a value throws ArgumentException.
- **`Context_Create_PublishFlag_SetsPublishTrue`**: `--publish` sets `Publish = true`.
- **`Context_Create_ReportParameter_SetsReportFile`**: `--report file.md` sets `ReportFile`.
- **`Context_Create_ReportDepthParameter_SetsReportDepth`**: `--report-depth 2` sets `ReportDepth` to 2.
- **`Context_Create_NoReportDepth_DefaultsToDepthOne`**: Default `ReportDepth` is 1 when not specified.
- **`Context_Create_ReportDepthZero_ThrowsArgumentException`**: `--report-depth 0` throws ArgumentException.
- **`Context_Create_ReportDepthNegative_ThrowsArgumentException`**: Negative `--report-depth` throws ArgumentException.
- **`Context_Create_ReportDepthSeven_ThrowsArgumentException`**: `--report-depth 7` throws ArgumentException.
- **`Context_Create_DepthParameter_SetsDepth`**: `--depth 2` sets the depth.
- **`Context_Create_NoDepth_DefaultsToOne`**: Default depth is 1 when not specified.
- **`Context_Create_DepthParameter_SetsDefaultReportDepth`**: `--depth` sets the default report depth.
- **`Context_Create_ExplicitReportDepthOverridesDepth`**: Explicit `--report-depth` overrides the value set by `--depth`.
- **`Context_Create_DepthZero_ThrowsArgumentException`**: `--depth 0` throws ArgumentException.
- **`Context_Create_DepthNegative_ThrowsArgumentException`**: Negative `--depth` throws ArgumentException.
- **`Context_Create_DepthSeven_ThrowsArgumentException`**: `--depth 7` throws ArgumentException.
- **`Context_Create_GlobPatternsAfterSeparator_CapturesPatterns`**:
  Patterns after `--` separator are captured in GlobPatterns.
- **`Context_Create_PublishWithoutReport_ParsesSuccessfully`**: `--publish` without `--report` parses without error.
- **`Context_Create_NoGlobPatterns_EmptyArray`**: No `--` separator produces an empty GlobPatterns array.
- **`Context_Create_LintFlag_SetsLintTrue`**: `--lint` sets `Lint = true`.
- **`Context_Create_LintFlag_WithFile_SetsLintFile`**: `--lint file.yaml` sets the lint config file path.
- **`Context_Create_LintFlag_FollowedByFlag_DoesNotConsumeFlagAsFile`**:
  `--lint --version` does not consume `--version` as the lint file.
- **`Context_Create_CaptureFlag_SetsCaptureTrue`**: `--capture` sets `Capture = true`.
- **`Context_Create_JobIdFlag_SetsJobId`**: `--job-id build-1` sets `JobId`.
- **`Context_Create_OutputFlag_SetsOutputFile`**: `--output file.json` sets `OutputFile`.
- **`Context_WriteLine_NotSilent_WritesToConsole`**: `WriteLine` writes to console when not in silent mode.
- **`Context_WriteLine_Silent_DoesNotWriteToConsole`**: `WriteLine` suppresses output in silent mode.
- **`Context_WriteError_NotSilent_WritesToConsole`**: `WriteError` writes to console when not in silent mode.
- **`Context_WriteError_Silent_DoesNotWriteToConsole`**: `WriteError` suppresses output in silent mode.
- **`Context_WriteError_WritesToLogFile`**: `WriteError` writes the message to the log file.
- **`Context_WriteError_SetsErrorExitCode`**: `WriteError` sets the exit code to 1.

#### Dependencies

Tests use `StringWriter` to capture console output. No file system access is required
except for `Context_Create_LogFlag_OpensLogFile`, which uses a temporary log file.

#### Requirements Coverage

The following list maps `Context` unit requirements to test scenarios:

- **`VersionMark-Context-Create`**: `Context_Create_NoArguments_ReturnsDefaultContext`,
  `Context_Create_VersionFlag_SetsVersionTrue`, `Context_Create_ShortVersionFlag_SetsVersionTrue`,
  `Context_Create_HelpFlag_SetsHelpTrue`, `Context_Create_ShortHelpFlag_H_SetsHelpTrue`,
  `Context_Create_ShortHelpFlag_Question_SetsHelpTrue`, `Context_Create_SilentFlag_SetsSilentTrue`,
  `Context_Create_ValidateFlag_SetsValidateTrue`, `Context_Create_ResultsFlag_SetsResultsFile`,
  `Context_Create_ResultFlag_SetsResultsFile`, `Context_Create_LogFlag_OpensLogFile`,
  `Context_Create_UnknownArgument_ThrowsArgumentException`,
  `Context_Create_LogFlag_WithoutValue_ThrowsArgumentException`,
  `Context_Create_ResultsFlag_WithoutValue_ThrowsArgumentException`,
  `Context_Create_PublishFlag_SetsPublishTrue`, `Context_Create_ReportParameter_SetsReportFile`,
  `Context_Create_ReportDepthParameter_SetsReportDepth`,
  `Context_Create_NoReportDepth_DefaultsToDepthOne`,
  `Context_Create_GlobPatternsAfterSeparator_CapturesPatterns`,
  `Context_Create_PublishWithoutReport_ParsesSuccessfully`,
  `Context_Create_NoGlobPatterns_EmptyArray`, `Context_Create_LintFlag_SetsLintTrue`,
  `Context_Create_LintFlag_WithFile_SetsLintFile`,
  `Context_Create_LintFlag_FollowedByFlag_DoesNotConsumeFlagAsFile`,
  `Context_Create_DepthParameter_SetsDepth`, `Context_Create_NoDepth_DefaultsToOne`,
  `Context_Create_DepthParameter_SetsDefaultReportDepth`,
  `Context_Create_ExplicitReportDepthOverridesDepth`,
  `Context_Create_DepthZero_ThrowsArgumentException`,
  `Context_Create_DepthNegative_ThrowsArgumentException`,
  `Context_Create_DepthSeven_ThrowsArgumentException`,
  `Context_Create_ReportDepthZero_ThrowsArgumentException`,
  `Context_Create_ReportDepthNegative_ThrowsArgumentException`,
  `Context_Create_ReportDepthSeven_ThrowsArgumentException`,
  `Context_Create_CaptureFlag_SetsCaptureTrue`, `Context_Create_JobIdFlag_SetsJobId`,
  `Context_Create_OutputFlag_SetsOutputFile`
- **`VersionMark-Context-WriteLine`**: `Context_WriteLine_NotSilent_WritesToConsole`,
  `Context_WriteLine_Silent_DoesNotWriteToConsole`
- **`VersionMark-Context-WriteError`**: `Context_WriteError_NotSilent_WritesToConsole`,
  `Context_WriteError_Silent_DoesNotWriteToConsole`, `Context_WriteError_WritesToLogFile`
- **`VersionMark-Context-WriteErrorExitCode`**: `Context_WriteError_SetsErrorExitCode`
