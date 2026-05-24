### LintIssue

#### Verification Approach

The `LintIssue` unit represents a single lint issue produced during configuration
validation. The related `VersionMarkLoadResult` record holds the loaded configuration (or
null on failure) together with all discovered issues. Tests are in
`Configuration/LintIssueTests.cs` and use `StringWriter` to capture console and error
stream output. No file system access is required.

#### Test Environment

N/A - standard test environment. All tests run using `dotnet test` with no additional
environment setup required.

#### Acceptance Criteria

- All unit tests for `LintIssue` pass with zero failures across all supported OS and .NET
  version matrix combinations (Windows, Linux, macOS x .NET 8, .NET 9, .NET 10).
- Every requirement for the `LintIssue` unit is covered by at least one named test
  scenario.

#### Test Scenarios

**LintIssue_Constructor_AllFields_AreStoredCorrectly**: The constructor correctly stores
severity, file path, line, column, and message fields. This scenario is tested by
`LintIssue_Constructor_AllFields_AreStoredCorrectly`.

**LintIssue_ToString_Error_ProducesLowercaseSeverity**: `ToString` for an error issue
includes the lowercase string "error". This scenario is tested by
`LintIssue_ToString_Error_ProducesLowercaseSeverity`.

**LintIssue_ToString_Warning_ProducesLowercaseSeverity**: `ToString` for a warning issue
includes the lowercase string "warning". This scenario is tested by
`LintIssue_ToString_Warning_ProducesLowercaseSeverity`.

**VersionMarkLoadResult_Constructor_AllFields_AreStoredCorrectly**: The constructor
correctly stores the config and issue list. This scenario is tested by
`VersionMarkLoadResult_Constructor_AllFields_AreStoredCorrectly`.

**VersionMarkLoadResult_ReportIssues_Error_WritesToErrorStream**: Error-severity issues are
written to the error stream. This scenario is tested by
`VersionMarkLoadResult_ReportIssues_Error_WritesToErrorStream`.

**VersionMarkLoadResult_ReportIssues_Warning_WritesToStdOut**: Warning-severity issues are
written to standard output. This scenario is tested by
`VersionMarkLoadResult_ReportIssues_Warning_WritesToStdOut`.
