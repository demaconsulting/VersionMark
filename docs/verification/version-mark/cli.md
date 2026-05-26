## Cli

### Verification Approach

The Cli subsystem is responsible for parsing command-line arguments, routing program flow
to the appropriate subsystem, and managing all output (console, error, and log file). It
consists of two units: `Program` (the entry point) and `Context` (the argument and output
container). Subsystem-level integration tests are in `CliTests.cs` and cover the full CLI
pipeline with `Context` and `Program` working together. Tests construct a `Context` with
specific arguments and call `Program.Run`, then assert on console output and exit code.
`StringWriter` captures console output without external mocks or file system interaction.
The one test that exercises file I/O uses a temporary file created during test setup.

### Test Environment

N/A - standard test environment. All tests run using `dotnet test` with no additional
environment setup required beyond the standard .NET test runner. Console output is
captured using `StringWriter`. The one test that exercises file I/O
(`Cli_Run_LogFlag_WritesOutputToLogFile`) uses a temporary file created during test setup.

### Acceptance Criteria

- All subsystem integration tests pass with zero failures across all supported OS and .NET
  version matrix combinations (Windows, Linux, macOS x .NET 8, .NET 9, .NET 10).
- Every requirement for the Cli subsystem is covered by at least one named test scenario.

### Test Scenarios

**Cli_Run_VersionFlag_ExitsCleanly**: A `Context` constructed with `--version` calls
`Program.Run`, which exits with code 0. This scenario is tested by
`Cli_Run_VersionFlag_ExitsCleanly`.

**Cli_Run_SilentWithVersionFlag_SuppressesOutput**: A `Context` constructed with
`--silent --version` verifies that all console output is suppressed during the run. This
scenario is tested by `Cli_Run_SilentWithVersionFlag_SuppressesOutput`.

**Cli_Run_HelpFlag_DisplaysUsageInformation**: A `Context` constructed with `--help`
verifies that usage information is displayed and the exit code is 0. This scenario is
tested by `Cli_Run_HelpFlag_DisplaysUsageInformation`.

**Cli_Run_ValidateFlag_RunsValidation**: A `Context` constructed with `--validate
--silent` verifies that validation runs successfully and exits with code 0. This scenario
is tested by `Cli_Run_ValidateFlag_RunsValidation`.

**Cli_Run_InvalidArgs_ThrowsArgumentException**: A `Context` constructed with an
unrecognized flag such as `--unknown-flag-xyz` verifies that an ArgumentException is
thrown. This scenario is tested by `Cli_Run_InvalidArgs_ThrowsArgumentException`.

**Cli_Run_LintFlag_ValidConfig_Succeeds**: A `Context` constructed with `--lint <file>`
pointing to a valid configuration file verifies that the run exits with code 0. This
scenario is tested by `Cli_Run_LintFlag_ValidConfig_Succeeds`.

**Cli_Run_ResultsFlag_WritesResultsFile**: A `Context` constructed with `--validate
--results <file>` verifies that a TRX results file is created at the specified path. This
scenario is tested by `Cli_Run_ResultsFlag_WritesResultsFile`.

**Cli_Run_LogFlag_WritesOutputToLogFile**: A `Context` constructed with `--version --log
<file>` verifies that program output is written to the specified log file. This scenario
is tested by `Cli_Run_LogFlag_WritesOutputToLogFile`.
