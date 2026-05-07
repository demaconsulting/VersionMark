### Validation Unit Verification

#### Overview

The `Validation` unit implements the self-test suite that runs during `--validate`. It
exercises capture, publish, and lint workflows end-to-end inside a temporary directory and
writes results to a TRX or JUnit XML file. Tests are in `SelfTest/SelfTestTests.cs`.

The internal test names used by `Validation` are `VersionMark_CapturesVersions`,
`VersionMark_GeneratesMarkdownReport`, `VersionMark_LintPassesForValidConfig`, and
`VersionMark_LintReportsErrorsForInvalidConfig`. These names appear in TRX results files
and serve as platform-level traceability evidence.

#### Test Scenarios

The following test scenarios verify the `Validation` unit:

- **`SelfTest_Run_WithResultsFlag_WritesResultsFile`**: Full validation run with `--results`
  writes a TRX file containing all internal test results.
- **`SelfTest_Run_WithResultsXmlFlag_WritesJUnitResultsFile`**: Full validation run with a
  JUnit results flag writes a JUnit XML file.
- **`SelfTest_Run_WithDepthTwo_WritesHashHashHeader`**: Running with depth 2 produces a `##` heading in the validation output.

Note: `SelfTest_Run_WithResultsFlag_WritesResultsFile` verifies that the `Validation`
class internally exercises all four self-test scenarios: `VersionMark_CapturesVersions`,
`VersionMark_GeneratesMarkdownReport`, `VersionMark_LintPassesForValidConfig`, and
`VersionMark_LintReportsErrorsForInvalidConfig`.

#### Dependencies

Tests use a temporary directory for results files and intermediate capture and publish
artifacts. No external mocks are required.

#### Requirements Coverage

The following list maps `Validation` unit requirements to test scenarios:

- **`VersionMark-Validation-Capture`**: `SelfTest_Run_WithResultsFlag_WritesResultsFile`
- **`VersionMark-Validation-Publish`**: `SelfTest_Run_WithResultsFlag_WritesResultsFile`
- **`VersionMark-Validation-Lint`**: `SelfTest_Run_WithResultsFlag_WritesResultsFile`
- **`VersionMark-Validation-WriteResults`**: `SelfTest_Run_WithResultsFlag_WritesResultsFile`,
  `SelfTest_Run_WithResultsXmlFlag_WritesJUnitResultsFile`
- **`VersionMark-Validation-HeaderDepth`**: `SelfTest_Run_WithDepthTwo_WritesHashHashHeader`
