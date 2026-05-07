## SelfTest Subsystem Verification

### Overview

The SelfTest subsystem provides built-in self-validation for the VersionMark tool. It
consists of two units: `Validation` (the self-validation test runner) and `PathHelpers`
(the safe path combination utility).

Subsystem-level integration tests are in `SelfTest/SelfTestTests.cs` and cover the full
self-validation workflow including TRX/JUnit results file writing, heading depth handling,
and path safety verification. Unit-level verification for `Validation` and `PathHelpers`
is in the chapters that follow.

### Verification Approach

Integration tests invoke the SelfTest subsystem with various flag combinations and assert
on the written results files and output. Tests use a temporary directory for results
files. No external mocks are required.

### Test Scenarios

The following integration test scenarios verify SelfTest subsystem requirements:

- **`SelfTest_PathHelpers_PathTraversal_ThrowsArgumentException`**: Path traversal attempt is rejected with ArgumentException.
- **`SelfTest_PathHelpers_ValidRelativePath_ProducesExpectedPath`**: Valid relative path combines correctly.
- **`SelfTest_PathHelpers_FindsDllInBaseDirectory_FileExists`**: Tool DLL is found in the base directory.
- **`SelfTest_Run_WithResultsFlag_WritesResultsFile`**: `--results` flag writes a TRX results file.
- **`SelfTest_Run_WithResultsXmlFlag_WritesJUnitResultsFile`**: `--results-xml` flag writes a JUnit results file.
- **`SelfTest_Run_WithDepthTwo_WritesHashHashHeader`**: Depth 2 produces a `##` heading in the output.

### Dependencies

Tests use a temporary directory for results files. No external mocks are required.

### Requirements Coverage

The following list maps SelfTest subsystem requirements to test scenarios:

- **`VersionMark-Validate-Capture`**: `SelfTest_Run_WithResultsFlag_WritesResultsFile`
- **`VersionMark-Validate-Publish`**: `SelfTest_Run_WithResultsFlag_WritesResultsFile`
- **`VersionMark-Validate-Lint`**: `SelfTest_Run_WithResultsFlag_WritesResultsFile`
- **`VersionMark-Validate-Results`**: `SelfTest_Run_WithResultsFlag_WritesResultsFile`,
  `SelfTest_Run_WithResultsXmlFlag_WritesJUnitResultsFile`
- **`VersionMark-PathHelpers-SafeCombine`**: `SelfTest_PathHelpers_PathTraversal_ThrowsArgumentException`,
  `SelfTest_PathHelpers_ValidRelativePath_ProducesExpectedPath`,
  `SelfTest_PathHelpers_FindsDllInBaseDirectory_FileExists`
- **`VersionMark-Validation-HeaderDepth`**: `SelfTest_Run_WithDepthTwo_WritesHashHashHeader`
