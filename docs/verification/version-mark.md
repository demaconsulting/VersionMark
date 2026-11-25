# VersionMark

## Verification Approach

VersionMark is a .NET global tool that captures tool version information from CI/CD job
environments and publishes consolidated version reports as markdown. The verification
strategy is organized around six subsystems: Cli, Configuration, Capture, Publishing,
SelfTest, and Utilities. Each subsystem is verified through a combination of integration
tests (at the subsystem level) and unit tests (at the unit level). All tests are
implemented using xUnit and are located under `test/DemaConsulting.VersionMark.Tests/`.

Tests are executed using `dotnet test` across multiple operating systems (Windows, Linux,
macOS) and multiple .NET versions (10, 11). Each test run produces a TRX results file
which serves as compliance evidence.

The built-in `--validate` mode exercises capture, publish, and lint workflows end-to-end
and produces a results file that can be used as post-deployment verification evidence.

The built-in `help` command is verified by running the compiled tool with `help` arguments.
The command examples of the help cards are verified with the example runner of
`DemaConsulting.HelpCards`: it gives each card a fresh scratch folder and calls a test delegate
for each command example. The delegate copies the configuration and capture files that the
examples read from `test/DemaConsulting.VersionMark.Tests/TestData/HelpExamples/` into that
folder, runs the compiled tool there with `Runner.RunInDirectory`, and the runner compares the
exit code and the output with the `exit` and `expect` values of the example.

## Test Environment

System-level verification is performed in the GitHub Actions CI/CD environment. Each
matrix job runs on a specific platform and .NET version combination, producing named TRX
result files. The file naming convention (`artifacts/validation-{os}-{dotnet}.trx`) and
test names provide the platform linkage used by ReqStream filters.

The help example test runs the commands of the example configuration, so `dotnet`, `git`, and `uname`
(`ver` on Windows) must be available on the PATH of the test machine.

## Acceptance Criteria

- All automated tests pass with zero failures across all supported operating system and
  .NET version matrix combinations (Windows, Linux, macOS × .NET 10, .NET 11).
- The built-in self-validation (`--validate`) exits with code 0 on each supported platform.
- The `help` command lists the topics with exit code 0, and exits with code 2 for an unknown topic.
- Every command example in the help cards gives the exit code and output that its card states.
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

**VersionMark-HelpTopics**: Invoke the tool with `help` as the sole argument; assert exit code
is 0, the output lists every topic with its command, and no banner is printed. Invoke it with
`help no-such-topic`; assert exit code is 2 and the output names the topic. This scenario is
tested by `VersionMark_HelpTopics_HelpVerb_ListsTopics` and
`VersionMark_HelpTopics_UnknownTopic_ReturnsUsageError`.

**VersionMark-HelpExamples**: Build the catalog that the tool ships and run every command
example of every card against the compiled tool in a scratch folder that holds the example
configuration and capture files; assert at least one command example exists and each gives the
exit code and output fragments that its card states. This scenario is tested by
`VersionMark_HelpExamples_AllCommandExamples_GiveStatedResults`.

**VersionMark-MultiPlatform**: Each of the above scenarios (VersionMark-Validate-Full,
VersionMark-CapturePublishCycle, VersionMark-CLI-ErrorHandling, VersionMark-HelpTopics, and
VersionMark-HelpExamples) is repeated on
Windows, Linux, and macOS across .NET 10 and .NET 11. Successful execution on all
matrix combinations confirms that there are no platform-specific regressions in the
assembled system. This scenario is verified by the full CI matrix run.
