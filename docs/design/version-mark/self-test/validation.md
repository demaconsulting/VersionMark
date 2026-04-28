# Validation Unit

## Overview

The `Validation` class (`Validation.cs`) exposes a single public method, `Run`, and
organizes all test execution internally.

## Run Method

`Run` orchestrates the self-validation sequence:

1. Calls `PrintValidationHeader` to emit a markdown heading (using `context.Depth` to set
   the heading level) followed by a table with tool version, machine name, OS version,
   .NET runtime, and timestamp.
2. Creates a `TestResults` collection named `"VersionMark Self-Validation"`.
3. Calls `RunCaptureTest`, `RunPublishTest`, `RunLintValidTest`, and `RunLintInvalidTest`
   to execute the functional tests.
4. Prints a summary of passed and failed tests, calling `context.WriteError` for the
   failed count if any tests failed (which also sets the process exit code to 1).
5. If `context.ResultsFile` is set, calls `WriteResultsFile` to persist the results.

## RunCaptureTest

`RunCaptureTest` verifies the capture mode end-to-end:

1. Creates a `TemporaryDirectory` (see below).
2. Writes a minimal `.versionmark.yaml` containing only the `dotnet` tool.
3. Constructs a `Context` with `--silent`, `--log <file>`, `--capture`, `--job-id test-job`,
   and `--output <file>`.
4. Changes the current directory to the temp directory and calls `Program.Run`.
5. Verifies exit code is 0, output file exists, `JobId` equals `"test-job"`, and `dotnet`
   version was captured and is non-empty.

The test name is `VersionMark_CapturesVersions`, satisfying `VersionMark-Validation-Capture`.

## RunPublishTest

`RunPublishTest` verifies the publish mode end-to-end:

1. Creates a `TemporaryDirectory`.
2. Writes two `VersionInfo` JSON files with known content.
3. Constructs a `Context` with `--silent`, `--log <file>`, `--publish`, `--report <file>`,
   `--report-depth 2`, and `-- versionmark-*.json`.
4. Changes the current directory to the temp directory and calls `Program.Run`.
5. Verifies exit code is 0, report file exists, and contains `## Tool Versions`,
   `**dotnet**`, `**node**`, `8.0.0`, and `20.0.0`.

The test name is `VersionMark_GeneratesMarkdownReport`, satisfying `VersionMark-Validation-Publish`.

## RunLintValidTest

`RunLintValidTest` verifies that lint mode exits successfully for a valid configuration file:

1. Creates a `TemporaryDirectory`.
2. Writes a minimal `.versionmark.yaml` containing the `dotnet` tool with a valid `command`
   and `regex` that includes the `(?<version>...)` capture group.
3. Constructs a `Context` with `--silent`, `--log <file>`, and `--lint <config-file>`.
4. Calls `Program.Run` and checks that the exit code is 0.

The test name is `VersionMark_LintPassesForValidConfig`, satisfying `VersionMark-Validation-Lint`.

## RunLintInvalidTest

`RunLintInvalidTest` verifies that lint mode reports errors for an invalid configuration file:

1. Creates a `TemporaryDirectory`.
2. Writes a `bad.versionmark.yaml` containing a tool entry with only a `command` field and
   no `regex` field (deliberately invalid).
3. Constructs a `Context` with `--silent`, `--log <file>`, and `--lint <config-file>`.
4. Calls `Program.Run` and checks that the exit code is non-zero.

The test name is `VersionMark_LintReportsErrorsForInvalidConfig`, satisfying
`VersionMark-Validation-Lint`.

## WriteResultsFile

`WriteResultsFile` inspects the file extension of `context.ResultsFile`:

- `.trx` → `TrxSerializer.Serialize`
- `.xml` → `JUnitSerializer.Serialize`
- Other → writes an error via `context.WriteError`

The method is only called when `context.ResultsFile` is non-null. The null check at the
start of the method is a defensive guard that prevents errors if the call contract changes
in the future.

This satisfies requirement `VersionMark-Validation-WriteResults`.

## TemporaryDirectory

`TemporaryDirectory` is a private nested class that implements `IDisposable`. It creates
a uniquely named directory under `Path.GetTempPath()` using `PathHelpers.SafePathCombine`
with a `Guid`-based name.

**Constructor**: Creates the directory with `Directory.CreateDirectory`. Any
`IOException`, `UnauthorizedAccessException`, or `ArgumentException` thrown during
creation is caught and re-thrown as `InvalidOperationException` with a descriptive
message, so callers receive a consistent exception type regardless of the underlying cause.

**Dispose**: Deletes the directory tree with `Directory.Delete(path, recursive: true)`,
silently suppressing `IOException` and `UnauthorizedAccessException` to avoid exceptions
during finalization or test teardown.

The serialized content is written with `File.WriteAllText`. This satisfies
`VersionMark-Validate-Results`.
