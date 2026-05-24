### Context

#### Verification Approach

The `Context` unit parses command-line arguments and manages all output routing (console,
error stream, and log file). Unit tests are in `Cli/ContextTests.cs`. Each test constructs
a `Context` via `Context.Create()` with specific argument arrays and asserts on the
resulting property values. `StringWriter` captures console output. The one test that
exercises file I/O (`Context_Create_LogFlag_OpensLogFile`) uses a temporary log file
created during test setup.

#### Test Environment

N/A - standard test environment. All tests run using `dotnet test` with no additional
environment setup required.

#### Acceptance Criteria

- All unit tests for `Context` pass with zero failures across all supported OS and .NET
  version matrix combinations (Windows, Linux, macOS x .NET 8, .NET 9, .NET 10).
- Every requirement for the `Context` unit is covered by at least one named test scenario.

#### Test Scenarios

**Context_Create_NoArguments_ReturnsDefaultContext**: No arguments creates a default context
with all flags false. This scenario is tested by
`Context_Create_NoArguments_ReturnsDefaultContext`.

**Context_Create_VersionFlag_SetsVersionTrue**: `--version` sets `Version = true`. This
scenario is tested by `Context_Create_VersionFlag_SetsVersionTrue`.

**Context_Create_ShortVersionFlag_SetsVersionTrue**: `-v` sets `Version = true`. This
scenario is tested by `Context_Create_ShortVersionFlag_SetsVersionTrue`.

**Context_Create_HelpFlag_SetsHelpTrue**: `--help` sets `Help = true`. This scenario is
tested by `Context_Create_HelpFlag_SetsHelpTrue`.

**Context_Create_ShortHelpFlag_H_SetsHelpTrue**: `-h` sets `Help = true`. This scenario
is tested by `Context_Create_ShortHelpFlag_H_SetsHelpTrue`.

**Context_Create_ShortHelpFlag_Question_SetsHelpTrue**: `-?` sets `Help = true`. This
scenario is tested by `Context_Create_ShortHelpFlag_Question_SetsHelpTrue`.

**Context_Create_SilentFlag_SetsSilentTrue**: `--silent` sets `Silent = true`. This
scenario is tested by `Context_Create_SilentFlag_SetsSilentTrue`.

**Context_Create_ValidateFlag_SetsValidateTrue**: `--validate` sets `Validate = true`.
This scenario is tested by `Context_Create_ValidateFlag_SetsValidateTrue`.

**Context_Create_ResultsFlag_SetsResultsFile**: `--results file.trx` sets
`ResultsFile`. This scenario is tested by `Context_Create_ResultsFlag_SetsResultsFile`.

**Context_Create_ResultFlag_SetsResultsFile**: `--result file.trx` (alias) sets
`ResultsFile`. This scenario is tested by `Context_Create_ResultFlag_SetsResultsFile`.

**Context_Create_LogFlag_OpensLogFile**: `--log file.log` opens a log file writer. This
scenario is tested by `Context_Create_LogFlag_OpensLogFile`.

**Context_Create_UnknownArgument_ThrowsArgumentException**: An unknown argument throws
`ArgumentException`. This scenario is tested by
`Context_Create_UnknownArgument_ThrowsArgumentException`.

**Context_Create_LogFlag_WithoutValue_ThrowsArgumentException**: `--log` without a value
throws `ArgumentException`. This scenario is tested by
`Context_Create_LogFlag_WithoutValue_ThrowsArgumentException`.

**Context_Create_ResultsFlag_WithoutValue_ThrowsArgumentException**: `--results` without
a value throws `ArgumentException`. This scenario is tested by
`Context_Create_ResultsFlag_WithoutValue_ThrowsArgumentException`.

**Context_Create_PublishFlag_SetsPublishTrue**: `--publish` sets `Publish = true`. This
scenario is tested by `Context_Create_PublishFlag_SetsPublishTrue`.

**Context_Create_ReportParameter_SetsReportFile**: `--report file.md` sets
`ReportFile`. This scenario is tested by `Context_Create_ReportParameter_SetsReportFile`.

**Context_Create_ReportDepthParameter_SetsReportDepth**: `--report-depth 2` sets
`ReportDepth` to 2. This scenario is tested by
`Context_Create_ReportDepthParameter_SetsReportDepth`.

**Context_Create_NoReportDepth_DefaultsToDepthOne**: Default `ReportDepth` is 1 when not
specified. This scenario is tested by `Context_Create_NoReportDepth_DefaultsToDepthOne`.

**Context_Create_ReportDepthZero_ThrowsArgumentException**: `--report-depth 0` throws
`ArgumentException`. This scenario is tested by
`Context_Create_ReportDepthZero_ThrowsArgumentException`.

**Context_Create_ReportDepthNegative_ThrowsArgumentException**: A negative
`--report-depth` throws `ArgumentException`. This scenario is tested by
`Context_Create_ReportDepthNegative_ThrowsArgumentException`.

**Context_Create_ReportDepthSeven_ThrowsArgumentException**: `--report-depth 7` throws
`ArgumentException`. This scenario is tested by
`Context_Create_ReportDepthSeven_ThrowsArgumentException`.

**Context_Create_DepthParameter_SetsDepth**: `--depth 2` sets the depth. This scenario
is tested by `Context_Create_DepthParameter_SetsDepth`.

**Context_Create_NoDepth_DefaultsToOne**: Default depth is 1 when not specified. This
scenario is tested by `Context_Create_NoDepth_DefaultsToOne`.

**Context_Create_DepthParameter_SetsDefaultReportDepth**: `--depth` sets the default
report depth. This scenario is tested by
`Context_Create_DepthParameter_SetsDefaultReportDepth`.

**Context_Create_ExplicitReportDepthOverridesDepth**: Explicit `--report-depth` overrides
the value set by `--depth`. This scenario is tested by
`Context_Create_ExplicitReportDepthOverridesDepth`.

**Context_Create_DepthZero_ThrowsArgumentException**: `--depth 0` throws
`ArgumentException`. This scenario is tested by
`Context_Create_DepthZero_ThrowsArgumentException`.

**Context_Create_DepthNegative_ThrowsArgumentException**: A negative `--depth` throws
`ArgumentException`. This scenario is tested by
`Context_Create_DepthNegative_ThrowsArgumentException`.

**Context_Create_DepthSeven_ThrowsArgumentException**: `--depth 7` throws
`ArgumentException`. This scenario is tested by
`Context_Create_DepthSeven_ThrowsArgumentException`.

**Context_Create_GlobPatternsAfterSeparator_CapturesPatterns**: Patterns after the `--`
separator are captured in `GlobPatterns`. This scenario is tested by
`Context_Create_GlobPatternsAfterSeparator_CapturesPatterns`.

**Context_Create_PublishWithoutReport_ParsesSuccessfully**: `--publish` without
`--report` parses without error. This scenario is tested by
`Context_Create_PublishWithoutReport_ParsesSuccessfully`.

**Context_Create_NoGlobPatterns_EmptyArray**: No `--` separator produces an empty
`GlobPatterns` array. This scenario is tested by
`Context_Create_NoGlobPatterns_EmptyArray`.

**Context_Create_LintFlag_SetsLintTrue**: `--lint` sets `Lint = true`. This scenario is
tested by `Context_Create_LintFlag_SetsLintTrue`.

**Context_Create_LintFlag_WithFile_SetsLintFile**: `--lint file.yaml` sets the lint
config file path. This scenario is tested by
`Context_Create_LintFlag_WithFile_SetsLintFile`.

**Context_Create_LintFlag_FollowedByFlag_DoesNotConsumeFlagAsFile**: `--lint --version`
does not consume `--version` as the lint file. This scenario is tested by
`Context_Create_LintFlag_FollowedByFlag_DoesNotConsumeFlagAsFile`.

**Context_Create_CaptureFlag_SetsCaptureTrue**: `--capture` sets `Capture = true`. This
scenario is tested by `Context_Create_CaptureFlag_SetsCaptureTrue`.

**Context_Create_JobIdFlag_SetsJobId**: `--job-id build-1` sets `JobId`. This scenario
is tested by `Context_Create_JobIdFlag_SetsJobId`.

**Context_Create_OutputFlag_SetsOutputFile**: `--output file.json` sets `OutputFile`.
This scenario is tested by `Context_Create_OutputFlag_SetsOutputFile`.

**Context_WriteLine_NotSilent_WritesToConsole**: `WriteLine` writes to console when not
in silent mode. This scenario is tested by `Context_WriteLine_NotSilent_WritesToConsole`.

**Context_WriteLine_Silent_DoesNotWriteToConsole**: `WriteLine` suppresses output in
silent mode. This scenario is tested by
`Context_WriteLine_Silent_DoesNotWriteToConsole`.

**Context_WriteError_NotSilent_WritesToConsole**: `WriteError` writes to console when not
in silent mode. This scenario is tested by
`Context_WriteError_NotSilent_WritesToConsole`.

**Context_WriteError_Silent_DoesNotWriteToConsole**: `WriteError` suppresses output in
silent mode. This scenario is tested by
`Context_WriteError_Silent_DoesNotWriteToConsole`.

**Context_WriteError_WritesToLogFile**: `WriteError` writes the message to the log file.
This scenario is tested by `Context_WriteError_WritesToLogFile`.

**Context_WriteError_SetsErrorExitCode**: `WriteError` sets the exit code to 1. This
scenario is tested by `Context_WriteError_SetsErrorExitCode`.
