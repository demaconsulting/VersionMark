# VersionMark

## Verification Approach

VersionMark is a .NET global tool that captures tool version information from CI/CD job
environments and publishes consolidated version reports as markdown. The verification
strategy is organized around six subsystems: Cli, Configuration, Capture, Publishing,
SelfTest, and Utilities. Each subsystem is verified through a combination of integration
tests (at the subsystem level) and unit tests (at the unit level). All tests are
implemented using xUnit and are located under `test/DemaConsulting.VersionMark.Tests/`.

Tests are executed using `dotnet test` across multiple operating systems (Windows, Linux,
macOS) and multiple .NET versions (8, 9, 10). Each test run produces a TRX results file
which serves as compliance evidence.

The built-in `--validate` mode exercises capture, publish, and lint workflows end-to-end
and produces a results file that can be used as post-deployment verification evidence.

## Test Environment

System-level verification is performed in the GitHub Actions CI/CD environment. Each
matrix job runs on a specific platform and .NET version combination, producing named TRX
result files. The file naming convention (`artifacts/validation-{os}-{dotnet}.trx`) and
test names provide the platform linkage used by ReqStream filters.

## Acceptance Criteria

- All automated tests pass with zero failures across all supported operating system and
  .NET version matrix combinations (Windows, Linux, macOS × .NET 8, .NET 9, .NET 10).
- The built-in self-validation (`--validate`) exits with code 0 on each supported platform.
- No unresolved anomalies of severity "error" or above remain open at the time of release.

## Test Scenarios

**VersionMark-Validate-Full**: Invoke `--validate --silent` to run all internal self-test
scenarios. This exercises the `VersionMark_CapturesVersions`,
`VersionMark_GeneratesMarkdownReport`, `VersionMark_LintPassesForValidConfig`, and
`VersionMark_LintReportsErrorsForInvalidConfig` scenarios internally and asserts that the
process exits with code 0. A non-zero exit code or any internal scenario failure
constitutes a test failure. This scenario is tested by
`IntegrationTest_ValidateFlag_RunsValidation`.

**VersionMark-CapturePublishCycle**: Verify capture mode and publish mode independently.
The capture scenario invokes the capture workflow and asserts that a JSON output file is
produced with the expected tool name and version entries. The publish scenario writes known
JSON capture files and invokes the publish workflow, asserting that the markdown report
contains the expected content. This scenario confirms that the Capture and Publishing
subsystems each operate correctly in isolation; end-to-end integration is verified through
the self-validation suite (see VersionMark-Validate-Full). This scenario is tested by
`IntegrationTest_CaptureCommand_CapturesToolVersions`.

**VersionMark-CLI-ErrorHandling**: Submit an unknown argument, omit a required flag, and
supply a missing configuration file path in separate invocations. Assert that each
invocation produces a non-zero exit code and emits a descriptive error message. This
scenario confirms that the Cli subsystem correctly rejects invalid inputs at the
assembled-system level. This scenario is tested by
`IntegrationTest_UnknownArgument_ReturnsError`,
`IntegrationTest_CaptureCommandWithoutJobId_ReturnsError`,
`IntegrationTest_CaptureCommandWithMissingConfig_ReturnsError`, and
`IntegrationTest_LintFlag_MissingConfig_ReturnsError`.

**VersionMark-MultiPlatform**: Each of the above scenarios (VersionMark-Validate-Full,
VersionMark-CapturePublishCycle, and VersionMark-CLI-ErrorHandling) is repeated on
Windows, Linux, and macOS across .NET 8, .NET 9, and .NET 10. Successful execution on all
matrix combinations confirms that there are no platform-specific regressions in the
assembled system. This scenario is verified by the full CI matrix run.
