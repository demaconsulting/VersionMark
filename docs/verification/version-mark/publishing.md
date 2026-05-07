## Publishing Subsystem Verification

### Overview

The Publishing subsystem is responsible for reading captured version JSON files and
generating a consolidated markdown report. It consists of one unit: `MarkdownFormatter`
(the version report formatter).

Subsystem-level integration tests are in `Publishing/PublishingTests.cs` and cover the
full publish workflow including glob pattern resolution, JSON file loading, report
generation, and error handling. Unit-level verification for `MarkdownFormatter` is in the
chapter that follows.

### Verification Approach

Integration tests use temporary directories containing pre-built JSON capture files.
Tests invoke publish operations via `Program.RunPublish` and assert on the contents of
the generated report or the error output. No external mocks are required.

### Test Scenarios

The following integration test scenarios verify Publishing subsystem requirements:

- **`Publishing_Format_MultipleCaptureFiles_ProducesConsolidatedReport`**: Multiple files produce a consolidated report.
- **`Publishing_Format_IdenticalVersionsAcrossJobs_ConsolidatesVersions`**: Identical versions across jobs are consolidated.
- **`Publishing_Format_ConflictingVersions_ShowsJobIds`**: Conflicting versions show individual job IDs.
- **`Publishing_Format_WithCustomDepth_UsesCorrectHeadingLevel`**: Custom depth produces the correct heading level.
- **`Publishing_Run_WithoutReport_ReportsError`**: Missing `--report` flag reports an error.
- **`Publishing_Run_WithGlobPattern_ReadsMatchingFiles`**: Glob pattern reads all matching files.
- **`Publishing_Run_WithGlobPatternMatchingNoFiles_ReportsError`**: No matching files reports an error.
- **`Publishing_Run_WithMalformedJsonFile_ReportsError`**: Malformed JSON file reports an error.
- **`Publishing_Run_WithReportDepth_UsesCorrectDepth`**: Report depth flag is applied to heading levels.

### Dependencies

No external mocks are required. Tests use temporary directories and pre-built JSON
capture files created during test setup.

### Requirements Coverage

The following list maps Publishing subsystem requirements to test scenarios:

- **`VersionMark-Publish-Publish`**: `Publishing_Format_MultipleCaptureFiles_ProducesConsolidatedReport`
- **`VersionMark-Publish-Report`**: `Publishing_Format_MultipleCaptureFiles_ProducesConsolidatedReport`
- **`VersionMark-Publish-ReportDepth`**: `Publishing_Format_WithCustomDepth_UsesCorrectHeadingLevel`,
  `Publishing_Run_WithReportDepth_UsesCorrectDepth`
- **`VersionMark-Publish-RequireReport`**: `Publishing_Run_WithoutReport_ReportsError`
- **`VersionMark-Publish-GlobPattern`**: `Publishing_Run_WithGlobPattern_ReadsMatchingFiles`
- **`VersionMark-Publish-Consolidate`**: `Publishing_Format_MultipleCaptureFiles_ProducesConsolidatedReport`,
  `Publishing_Format_IdenticalVersionsAcrossJobs_ConsolidatesVersions`
- **`VersionMark-Publish-ConflictReport`**: `Publishing_Run_WithGlobPatternMatchingNoFiles_ReportsError`
- **`VersionMark-Publish-ConflictDisplay`**: `Publishing_Format_ConflictingVersions_ShowsJobIds`
- **`VersionMark-Publish-FileError`**: `Publishing_Run_WithMalformedJsonFile_ReportsError`
