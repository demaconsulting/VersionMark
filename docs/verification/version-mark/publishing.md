## Publishing

### Verification Approach

The Publishing subsystem is responsible for reading captured version JSON files and
generating a consolidated markdown report. It consists of one unit: `MarkdownFormatter`
(the version report formatter). Subsystem-level integration tests are in
`Publishing/PublishingTests.cs` and cover the full publish workflow including glob
pattern resolution, JSON file loading, report generation, and error handling. Tests use
temporary directories containing pre-built JSON capture files. No external mocks are
required.

### Test Environment

N/A - standard test environment. Tests create temporary directories and pre-built JSON
capture files during setup and clean them up afterwards.

### Acceptance Criteria

- All subsystem integration tests pass with zero failures across all supported OS and .NET
  version matrix combinations (Windows, Linux, macOS x .NET 8, .NET 9, .NET 10).
- Every requirement for the Publishing subsystem is covered by at least one named test
  scenario.

### Test Scenarios

**Publishing_Format_MultipleCaptureFiles_ProducesConsolidatedReport**: Multiple capture
files produce a single consolidated report. This scenario is tested by
`Publishing_Format_MultipleCaptureFiles_ProducesConsolidatedReport`.

**Publishing_Format_IdenticalVersionsAcrossJobs_ConsolidatesVersions**: Identical versions
across multiple jobs are consolidated into a single line. This scenario is tested by
`Publishing_Format_IdenticalVersionsAcrossJobs_ConsolidatesVersions`.

**Publishing_Format_ConflictingVersions_ShowsJobIds**: Conflicting versions across jobs
show individual job IDs in the report. This scenario is tested by
`Publishing_Format_ConflictingVersions_ShowsJobIds`.

**Publishing_Format_WithCustomDepth_UsesCorrectHeadingLevel**: A custom depth value
produces the correct markdown heading level. This scenario is tested by
`Publishing_Format_WithCustomDepth_UsesCorrectHeadingLevel`.

**Publishing_Run_WithoutReport_ReportsError**: Running publish without the `--report`
flag reports an error. This scenario is tested by
`Publishing_Run_WithoutReport_ReportsError`.

**Publishing_Run_WithGlobPattern_ReadsMatchingFiles**: A glob pattern reads all matching
capture files. This scenario is tested by
`Publishing_Run_WithGlobPattern_ReadsMatchingFiles`.

**Publishing_Run_WithGlobPatternMatchingNoFiles_ReportsError**: A glob pattern that matches
no files reports an error. This scenario is tested by
`Publishing_Run_WithGlobPatternMatchingNoFiles_ReportsError`.

**Publishing_Run_WithMalformedJsonFile_ReportsError**: A malformed JSON capture file
reports an error. This scenario is tested by
`Publishing_Run_WithMalformedJsonFile_ReportsError`.

**Publishing_Run_WithReportDepth_UsesCorrectDepth**: The `--report-depth` flag is applied
to the heading level in the generated report. This scenario is tested by
`Publishing_Run_WithReportDepth_UsesCorrectDepth`.

**Publishing_Run_WithAbsoluteGlobPattern_ReadsMatchingFiles**: An absolute glob pattern
reads all matching capture files even when the working directory differs. This scenario is
tested by `Publishing_Run_WithAbsoluteGlobPattern_ReadsMatchingFiles`.
