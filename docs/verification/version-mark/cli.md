## Cli Subsystem Verification

### Overview

The Cli subsystem is responsible for parsing command-line arguments, routing program flow
to the appropriate subsystem, and managing all output (console, error, and log file). It
consists of two units: `Program` (the entry point) and `Context` (the argument and output
container).

Subsystem-level integration tests are in `CliTests.cs` and cover the full CLI pipeline
with `Context` and `Program` working together. Unit-level verification for `Program` and
`Context` is in the chapters that follow.

### Verification Approach

Integration tests construct a `Context` with specific arguments and call `Program.Run`,
then assert on console output and exit code. `StringWriter` captures console output
without external mocks or file system interaction.

### Test Scenarios

The following integration test scenarios verify Cli subsystem requirements:

- **`Cli_Run_VersionFlag_ExitsCleanly`**: Context with `--version`; Program.Run exits with code 0.
- **`Cli_Run_SilentWithVersionFlag_SuppressesOutput`**: Context with `--silent --version`; output is suppressed.
- **`Cli_Run_HelpFlag_DisplaysUsageInformation`**: `--help` shows usage; exit code 0.
- **`Cli_Run_ValidateFlag_RunsValidation`**: `--validate --silent` exits 0.
- **`Cli_Run_InvalidArgs_ThrowsArgumentException`**: `--unknown-flag-xyz` throws ArgumentException.
- **`Cli_Run_LintFlag_ValidConfig_Succeeds`**: `--lint <file>` on a valid config exits 0.
- **`Cli_Run_ResultsFlag_WritesResultsFile`**: `--validate --results <file>` creates a TRX file.
- **`Cli_Run_LogFlag_WritesOutputToLogFile`**: `--version --log <file>` writes to log.

### Dependencies

No external mocks are required. Tests use `StringWriter` to capture console output.

### Requirements Coverage

The following list maps Cli subsystem requirements to test scenarios:

- **`VersionMark-CommandLine-Context`**: `Cli_Run_VersionFlag_ExitsCleanly`,
  `Cli_Run_SilentWithVersionFlag_SuppressesOutput`
- **`VersionMark-CommandLine-Version`**: `Cli_Run_VersionFlag_ExitsCleanly`
- **`VersionMark-CommandLine-Help`**: `Cli_Run_HelpFlag_DisplaysUsageInformation`
- **`VersionMark-CommandLine-Silent`**: `Cli_Run_SilentWithVersionFlag_SuppressesOutput`
- **`VersionMark-CommandLine-Validate`**: `Cli_Run_ValidateFlag_RunsValidation`
- **`VersionMark-CommandLine-Results`**: `Cli_Run_ResultsFlag_WritesResultsFile`
- **`VersionMark-CommandLine-Log`**: `Cli_Run_LogFlag_WritesOutputToLogFile`
- **`VersionMark-CommandLine-ErrorOutput`**: `Cli_Run_InvalidArgs_ThrowsArgumentException`
- **`VersionMark-CommandLine-InvalidArgs`**: `Cli_Run_InvalidArgs_ThrowsArgumentException`
- **`VersionMark-CommandLine-ExitCode`**: `Cli_Run_InvalidArgs_ThrowsArgumentException`
- **`VersionMark-CommandLine-Lint`**: `Cli_Run_LintFlag_ValidConfig_Succeeds`
