# SelfTest Subsystem

## Overview

The SelfTest subsystem provides built-in verification of the tool's core functionality
and safe path construction for use within that verification. It consists of two units:
`Validation` (the self-validation test runner) and `PathHelpers` (a safe path combination
utility used internally by `Validation`).

The validation subsystem is invoked when the `--validate` flag is passed and can write
results to a TRX or JUnit XML file when `--results` is also provided. This satisfies
requirements `VersionMark-CommandLine-Validate` and `VersionMark-CommandLine-Results`.

## Units

### Validation

The `Validation` class (`Validation.cs`) is the self-validation test runner. It exposes a
single public method, `Run`, which orchestrates all internal self-tests against the tool's
core modes (capture, publish, lint), collects results, prints a summary, and optionally
writes a structured results file.

See [validation.md](validation.md) for the full unit design.

### PathHelpers

The `PathHelpers` class (`PathHelpers.cs`) provides a single static method,
`SafePathCombine`, used internally by `Validation` when constructing paths inside temporary
directories. It protects against path-traversal attacks by ensuring the resolved combined
path stays within the intended base directory.

See [path-helpers.md](path-helpers.md) for the full unit design.

## Subsystem Interactions

`Validation.Run` creates temporary directories via the private `TemporaryDirectory` helper
class and uses `PathHelpers.SafePathCombine` for all path construction within those
directories. `PathHelpers` has no dependency on `Validation` and may be considered a pure
utility within the subsystem.

The subsystem depends on:

- `DemaConsulting.VersionMark.Cli.Context` — command-line arguments and output
- `DemaConsulting.VersionMark.Capture.VersionInfo` — capture output model
- `DemaConsulting.VersionMark.Program` — re-entrant entry point for internal test runs
- `DemaConsulting.TestResults` — test result collection and serialization
