### Program

#### Verification Approach

The `Program` unit is the entry point for the VersionMark tool. It dispatches to the
appropriate subsystem based on parsed context flags and exposes a static `Version` property
for the tool version string. Unit tests are in `ProgramTests.cs` and cover every dispatch
path including each supported flag, invalid argument handling, capture, publish, and lint
workflows. Tests construct a `Context` with specific arguments and call `Program.Run`, then
assert on console output and exit code. All console output is captured using `StringWriter`
so no real I/O is required for most scenarios. Capture and publish tests use temporary
directories and configuration files created during test setup.

#### Test Environment

N/A - standard test environment. Tests that require file I/O use temporary directories
created during test setup. The `FindVersions` scenarios require the `dotnet` command to be
available on `PATH`.

#### Acceptance Criteria

- All unit tests for `Program` pass with zero failures across all supported OS and .NET
  version matrix combinations (Windows, Linux, macOS × .NET 8, .NET 9, .NET 10).
- Every requirement for the `Program` unit is covered by at least one named test scenario.

#### Test Scenarios

**Program_Version_ReturnsNonEmptyString**: `Program.Version` returns a non-empty version
string. This scenario is tested by `Program_Version_ReturnsNonEmptyString`.

**Program_Run_WithVersionFlag_DisplaysVersionOnly**: A `Context` with `--version` calls
`Program.Run`, which writes the version string only (no copyright) and exits with code 0.
This scenario is tested by `Program_Run_WithVersionFlag_DisplaysVersionOnly`.

**Program_Run_WithHelpFlag_DisplaysUsageInformation**: A `Context` with `--help` calls
`Program.Run`, which writes usage information and exits with code 0. This scenario is
tested by `Program_Run_WithHelpFlag_DisplaysUsageInformation`.

**Program_Run_WithValidateFlag_RunsValidation**: A `Context` with `--validate` calls
`Program.Run`, which writes output containing "Total Tests:" and exits with code 0. This
scenario is tested by `Program_Run_WithValidateFlag_RunsValidation`.

**Program_Run_NoArguments_DisplaysDefaultBehavior**: A `Context` with no arguments calls
`Program.Run`, which writes the banner with version and copyright and exits with code 0.
This scenario is tested by `Program_Run_NoArguments_DisplaysDefaultBehavior`.

**Program_Run_WithCaptureCommand_CapturesToolVersions**: A `Context` with `--capture
--job-id --output -- dotnet` calls `Program.Run`, which creates the output JSON file
containing the captured version. This scenario is tested by
`Program_Run_WithCaptureCommand_CapturesToolVersions`.

**Program_Run_WithCaptureCommandNoToolFilter_CapturesAllConfiguredTools**: A `Context` with
`--capture` and no tool filter patterns calls `Program.Run`, which captures all tools
defined in the configuration. This scenario is tested by
`Program_Run_WithCaptureCommandNoToolFilter_CapturesAllConfiguredTools`.

**Program_Run_WithCaptureCommandWithoutJobId_ReturnsError**: A `Context` with `--capture`
but no `--job-id` calls `Program.Run`, which returns a non-zero exit code. This scenario
is tested by `Program_Run_WithCaptureCommandWithoutJobId_ReturnsError`.

**Program_Run_WithCaptureCommandWithMissingConfig_ReturnsError**: A `Context` with
`--capture` pointing to a missing `.versionmark.yaml` calls `Program.Run`, which returns
a non-zero exit code. This scenario is tested by
`Program_Run_WithCaptureCommandWithMissingConfig_ReturnsError`.

**Program_Run_WithPublishCommandWithoutReport_ReturnsError**: A `Context` with `--publish`
but no `--report` calls `Program.Run`, which returns a non-zero exit code. This scenario
is tested by `Program_Run_WithPublishCommandWithoutReport_ReturnsError`.

**Program_Run_WithPublishCommandNoMatchingFiles_ReturnsError**: A `Context` with `--publish`
and a glob pattern matching no files calls `Program.Run`, which returns a non-zero exit
code. This scenario is tested by `Program_Run_WithPublishCommandNoMatchingFiles_ReturnsError`.

**Program_Run_WithPublishCommandInvalidJson_ReturnsError**: A `Context` with `--publish`
pointing to an invalid JSON file calls `Program.Run`, which returns a non-zero exit code.
This scenario is tested by `Program_Run_WithPublishCommandInvalidJson_ReturnsError`.

**Program_Run_WithPublishCommand_GeneratesMarkdownReport**: A full publish invocation calls
`Program.Run`, which generates a markdown report file at the specified path. This scenario
is tested by `Program_Run_WithPublishCommand_GeneratesMarkdownReport`.

**Program_Run_WithPublishCommandCustomDepth_AdjustsHeadingLevels**: A publish invocation
with a custom `--depth` value calls `Program.Run`, which adjusts heading levels in the
report accordingly. This scenario is tested by
`Program_Run_WithPublishCommandCustomDepth_AdjustsHeadingLevels`.

**Program_Run_WithLintFlag_ValidConfig_ReturnsSuccess**: A `Context` with `--lint <file>`
pointing to a valid configuration calls `Program.Run`, which exits with code 0. This
scenario is tested by `Program_Run_WithLintFlag_ValidConfig_ReturnsSuccess`.

**Program_Run_WithLintFlag_ValidConfig_SuppressesBanner**: A `Context` with `--lint <file>`
on a valid configuration calls `Program.Run`, which suppresses the banner output. This
scenario is tested by `Program_Run_WithLintFlag_ValidConfig_SuppressesBanner`.

**Program_Run_WithLintFlag_InvalidConfig_ReturnsError**: A `Context` with `--lint <file>`
pointing to an invalid configuration calls `Program.Run`, which reports an error and
returns a non-zero exit code. This scenario is tested by
`Program_Run_WithLintFlag_InvalidConfig_ReturnsError`.

**Program_Run_WithLintFlag_NoFile_UsesDefaultConfigFile**: A `Context` with `--lint` and
no file argument calls `Program.Run`, which uses `.versionmark.yaml` as the default
configuration file path. This scenario is tested by
`Program_Run_WithLintFlag_NoFile_UsesDefaultConfigFile`.

**Program_Run_WithHelpFlag_IncludesLintInformation**: A `Context` with `--help` calls
`Program.Run`, which includes lint command information in the usage output. This scenario
is tested by `Program_Run_WithHelpFlag_IncludesLintInformation`.
