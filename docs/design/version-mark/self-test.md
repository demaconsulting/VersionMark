## SelfTest

### Overview

The SelfTest subsystem provides built-in verification of the tool's core functionality.
It is invoked when the `--validate` flag is passed and can write results to a TRX or JUnit
XML file when `--results` is also provided. It consists of a single unit: `Validation`.

### Interfaces

**`Validation.Run(Context context)`**: Executes the full self-validation suite.

- *Type*: In-process .NET public API (static method).
- *Role*: Provider.
- *Contract*: Runs four functional tests exercising capture, publish, lint-valid, and
  lint-invalid modes. Prints a results summary via `context.WriteLine`. Writes individual
  test failures via `context.WriteError`, setting `ExitCode` to `1` if any tests fail.
  Optionally writes a TRX or JUnit results file when `context.ResultsFile` is set.
- *Constraints*: Each test runs in an isolated temporary directory using a real re-entrant
  call to `Program.Run`. Requires write access to `Path.GetTempPath()`.

### Design

The SelfTest subsystem consists of the single `Validation` class, which orchestrates four
independent functional tests against a real (re-entrant) `Program.Run` call inside isolated
temporary directories:

1. **`RunCaptureTest`** — verifies that `--capture` saves a valid JSON file with the
   expected `JobId` and a non-empty `dotnet` version.
2. **`RunPublishTest`** — verifies that `--publish` reads two pre-written JSON files and
   produces a markdown report containing the expected headings and version strings.
3. **`RunLintValidTest`** — verifies that `--lint` exits with code `0` for a valid config
   file.
4. **`RunLintInvalidTest`** — verifies that `--lint` exits with a non-zero code for an
   invalid config file.

Each test creates a `TemporaryDirectory` (a private disposable helper that cleans up after
itself), constructs a fresh `Context`, changes the current working directory into the temp
directory, and calls `Program.Run`. Results are accumulated in a `TestResults` collection
and serialized by `WriteResultsFile` if `context.ResultsFile` is set.

The subsystem depends on `Context` (Cli), `VersionInfo` (Capture), `Program` (Cli, called
re-entrantly), `PathHelpers` (Utilities), and `DemaConsulting.TestResults` (OTS).
