## SelfTest

### Verification Approach

The SelfTest subsystem provides built-in self-validation for the VersionMark tool. It
consists of one unit: `Validation` (the self-validation test runner). Subsystem-level
integration tests are in `SelfTest/SelfTestTests.cs` and cover the full self-validation
workflow including TRX and JUnit results file writing and heading depth handling. Tests use
a temporary directory for results files. No external mocks are required.

### Test Environment

N/A - standard test environment. Tests create temporary directories during setup and clean
them up afterwards.

### Acceptance Criteria

- All subsystem integration tests pass with zero failures across all supported OS and .NET
  version matrix combinations (Windows, Linux, macOS x .NET 8, .NET 9, .NET 10).
- Every requirement for the SelfTest subsystem is covered by at least one named test
  scenario.

### Test Scenarios

**SelfTest_Run_WithResultsFlag_WritesResultsFile**: The `--results` flag causes a TRX
results file to be written at the specified path. This scenario is tested by
`SelfTest_Run_WithResultsFlag_WritesResultsFile`.

**SelfTest_Run_WithResultsXmlFlag_WritesJUnitResultsFile**: The `--results-xml` flag
causes a JUnit-format results file to be written at the specified path. This scenario is
tested by `SelfTest_Run_WithResultsXmlFlag_WritesJUnitResultsFile`.

**SelfTest_Run_WithDepthTwo_WritesHashHashHeader**: Running with depth 2 produces a `##`
heading level in the output. This scenario is tested by
`SelfTest_Run_WithDepthTwo_WritesHashHashHeader`.

**SelfTest_Run_Capture_CapturesToolVersions**: The capture sub-test within self-validation
runs successfully. This scenario is tested by `SelfTest_Run_Capture_CapturesToolVersions`.

**SelfTest_Run_Publish_GeneratesMarkdownReport**: The publish sub-test within
self-validation runs successfully. This scenario is tested by
`SelfTest_Run_Publish_GeneratesMarkdownReport`.

**SelfTest_Run_LintValid_PassesForValidConfig**: The lint-valid sub-test within
self-validation accepts a valid configuration with exit code 0. This scenario is tested by
`SelfTest_Run_LintValid_PassesForValidConfig`.

**SelfTest_Run_LintInvalid_RejectsInvalidConfig**: The lint-invalid sub-test within
self-validation rejects an invalid configuration with a non-zero exit code. This scenario
is tested by `SelfTest_Run_LintInvalid_RejectsInvalidConfig`.
