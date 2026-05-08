## SelfTest Subsystem

### Overview

The SelfTest subsystem provides built-in verification of the tool's core functionality.
It consists of one unit: `Validation` (the self-validation test runner).

The validation subsystem is invoked when the `--validate` flag is passed and can write
results to a TRX or JUnit XML file when `--results` is also provided. This satisfies
requirements `VersionMark-CommandLine-Validate` and `VersionMark-CommandLine-Results`.

### Units

#### Validation

The `Validation` class (`Validation.cs`) is the self-validation test runner. It exposes a
single public method, `Run`, which orchestrates all internal self-tests against the tool's
core modes (capture, publish, lint), collects results, prints a summary, and optionally
writes a structured results file.

See *Validation Unit Design* for the full unit design.

### Subsystem Interactions

`Validation.Run` creates temporary directories via the private `TemporaryDirectory` helper
class and uses `PathHelpers.SafePathCombine` from the Utilities subsystem for all path
construction within those directories.

The subsystem depends on:

- `DemaConsulting.VersionMark.Cli.Context` — command-line arguments and output
- `DemaConsulting.VersionMark.Capture.VersionInfo` — capture output model
- `DemaConsulting.VersionMark.Program` — re-entrant entry point for internal test runs
- `DemaConsulting.TestResults` — test result collection and serialization
