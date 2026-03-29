# Validation Subsystem

## Overview

The validation subsystem provides built-in verification of the tool's core functionality
and safe path construction for use within that verification. It consists of two units:
`Validation` (the self-validation test runner) and `PathHelpers` (a safe path combination
utility used internally by `Validation`).

The validation subsystem is invoked when the `--validate` flag is passed and can write
results to a TRX or JUnit XML file when `--results` is also provided. This satisfies
requirements `VersionMark-Cmd-Validate` and `VersionMark-Cmd-Results`.

## PathHelpers Class

The `PathHelpers` class (`PathHelpers.cs`) provides a single static method,
`SafePathCombine`, designed to defend against path-traversal attacks when constructing
file paths from partially-trusted input.

### SafePathCombine Method

`SafePathCombine` takes a `basePath` and a `relativePath` and returns a combined path,
subject to two layers of validation:

1. **Pre-combination check**: rejects `relativePath` if it contains `".."` or is a rooted
   (absolute) path.
2. **Post-combination check**: resolves both paths with `Path.GetFullPath` and calls
   `Path.GetRelativePath` to verify the combined path still sits under `basePath`.

If either check fails, `ArgumentException` is thrown. This defense-in-depth approach
guards against edge-cases that might bypass the initial string check while remaining
straightforward to audit.

`PathHelpers` is used by `Validation` when constructing paths inside temporary directories
for self-validation tests. This satisfies requirement `VersionMark-PathHelpers-SafeCombine`.

## Validation Class

The `Validation` class (`Validation.cs`) exposes a single public method, `Run`, and
organizes all test execution internally.

### Run Method

`Run` orchestrates the self-validation sequence:

1. Calls `PrintValidationHeader` to emit a markdown table with tool version, machine
   name, OS version, .NET runtime, and timestamp.
2. Creates a `TestResults` collection named `"VersionMark Self-Validation"`.
3. Calls `RunCaptureTest`, `RunPublishTest`, `RunLintValidTest`, and `RunLintInvalidTest`
   to execute the functional tests.
4. Prints a summary of passed and failed tests, calling `context.WriteError` for the
   failed count if any tests failed.
5. If `context.ResultsFile` is set, calls `WriteResultsFile` to persist the results.

### RunCaptureTest

`RunCaptureTest` verifies the capture mode end-to-end:

1. Creates a `TemporaryDirectory` (see below).
2. Writes a minimal `.versionmark.yaml` containing only the `dotnet` tool.
3. Constructs a `Context` with `--silent`, `--log <file>`, `--capture`, `--job-id test-job`,
   and `--output <file>`.
4. Changes the current directory to the temp directory and calls `Program.Run`.
5. Verifies exit code is 0, output file exists, `JobId` equals `"test-job"`, and `dotnet`
   version was captured and is non-empty.

The test name is `VersionMark_CapturesVersions`, satisfying `VersionMark-Cap-Capture`.

### RunPublishTest

`RunPublishTest` verifies the publish mode end-to-end:

1. Creates a `TemporaryDirectory`.
2. Writes two `VersionInfo` JSON files with known content.
3. Constructs a `Context` with `--silent`, `--log <file>`, `--publish`, `--report <file>`,
   `--report-depth 2`, and `-- versionmark-*.json`.
4. Changes the current directory to the temp directory and calls `Program.Run`.
5. Verifies exit code is 0, report file exists, and contains `## Tool Versions`,
   `**dotnet**`, `**node**`, `8.0.0`, and `20.0.0`.

The test name is `VersionMark_GeneratesMarkdownReport`, satisfying `VersionMark-Pub-Publish`.

### RunLintValidTest

`RunLintValidTest` verifies that lint mode exits successfully for a valid configuration file:

1. Creates a `TemporaryDirectory`.
2. Writes a minimal `.versionmark.yaml` containing the `dotnet` tool with a valid `command`
   and `regex` that includes the `(?<version>...)` capture group.
3. Constructs a `Context` with `--silent`, `--log <file>`, and `--lint <config-file>`.
4. Calls `Program.Run` and checks that the exit code is 0.

The test name is `VersionMark_LintPassesForValidConfig`, satisfying `VersionMark-Cmd-Lint`.

### RunLintInvalidTest

`RunLintInvalidTest` verifies that lint mode reports errors for an invalid configuration file:

1. Creates a `TemporaryDirectory`.
2. Writes a `bad.versionmark.yaml` containing a tool entry with only a `command` field and
   no `regex` field (deliberately invalid).
3. Constructs a `Context` with `--silent`, `--log <file>`, and `--lint <config-file>`.
4. Calls `Program.Run` and checks that the exit code is non-zero.

The test name is `VersionMark_LintReportsErrorsForInvalidConfig`, satisfying
`VersionMark-Cmd-Lint`.

### WriteResultsFile

`WriteResultsFile` inspects the file extension of `context.ResultsFile`:

- `.trx` → `TrxSerializer.Serialize`
- `.xml` → `JUnitSerializer.Serialize`
- Other → writes an error via `context.WriteError`

The serialized content is written with `File.WriteAllText`. This satisfies
`VersionMark-Cmd-Results`.

### TemporaryDirectory

`TemporaryDirectory` is a private nested class implementing `IDisposable`. It creates a
uniquely-named subdirectory under `Path.GetTempPath()` using `PathHelpers.SafePathCombine`
and a `Guid`-based name. On `Dispose`, it deletes the directory recursively, ignoring
`IOException` and `UnauthorizedAccessException` to allow graceful cleanup even in
constrained environments.
