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

**VersionMark-CapturePublishCycle**: Capture one or more tools using the capture workflow,
then invoke the publish workflow to generate a markdown report. Assert that the output
markdown report contains the expected tool name and version entries. This scenario confirms
that the Capture and Publishing subsystems interoperate correctly when assembled. This
scenario is tested by `IntegrationTest_CaptureCommand_CapturesToolVersions`.

**VersionMark-CLI-ErrorHandling**: Submit an unknown argument, omit a required flag, and
supply a missing configuration file path in separate invocations. Assert that each
invocation produces a non-zero exit code and emits a descriptive error message. This
scenario confirms that the Cli subsystem correctly rejects invalid inputs at the
assembled-system level. This scenario is tested by
`IntegrationTest_LintFlag_ValidConfig_ReturnsSuccess`.

**VersionMark-MultiPlatform**: Each of the above scenarios (VersionMark-Validate-Full,
VersionMark-CapturePublishCycle, and VersionMark-CLI-ErrorHandling) is repeated on
Windows, Linux, and macOS across .NET 8, .NET 9, and .NET 10. Successful execution on all
matrix combinations confirms that there are no platform-specific regressions in the
assembled system. This scenario is verified by the full CI matrix run.
