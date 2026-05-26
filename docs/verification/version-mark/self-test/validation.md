### Validation

#### Verification Approach

The `Validation` unit implements the self-test suite that runs during `--validate`. It
exercises capture, publish, and lint workflows end-to-end inside a temporary directory and
writes results to a TRX or JUnit XML file. Tests are in `SelfTest/SelfTestTests.cs`. The
internal test names used by `Validation` are `VersionMark_CapturesVersions`,
`VersionMark_GeneratesMarkdownReport`, `VersionMark_LintPassesForValidConfig`, and
`VersionMark_LintReportsErrorsForInvalidConfig`; these appear in results files and serve
as platform-level traceability evidence. Tests use a temporary directory for results files
and intermediate artifacts. No external mocks are required.

#### Test Environment

N/A - standard test environment. Tests create temporary directories during setup and clean
them up afterwards.

#### Acceptance Criteria

- All unit tests for `Validation` pass with zero failures across all supported OS and
  .NET version matrix combinations (Windows, Linux, macOS x .NET 8, .NET 9, .NET 10).
- Every requirement for the `Validation` unit is covered by at least one named test
  scenario.

#### Test Scenarios

**SelfTest_Run_WithResultsFlag_WritesResultsFile**: A full validation run with `--results`
writes a TRX file containing all internal test results, verifying that the `Validation`
class exercises all four self-test scenarios: `VersionMark_CapturesVersions`,
`VersionMark_GeneratesMarkdownReport`, `VersionMark_LintPassesForValidConfig`, and
`VersionMark_LintReportsErrorsForInvalidConfig`. This scenario is tested by
`SelfTest_Run_WithResultsFlag_WritesResultsFile`.

**SelfTest_Run_WithResultsXmlFlag_WritesJUnitResultsFile**: A full validation run with a
JUnit results flag writes a JUnit XML file containing all internal test results. This
scenario is tested by `SelfTest_Run_WithResultsXmlFlag_WritesJUnitResultsFile`.

**SelfTest_Run_WithDepthTwo_WritesHashHashHeader**: Running validation with depth 2 produces
a `##` heading level in the validation output. This scenario is tested by
`SelfTest_Run_WithDepthTwo_WritesHashHashHeader`.
