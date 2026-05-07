### LintIssue Unit Verification

#### Overview

The `LintIssue` unit represents a single lint issue produced during configuration
validation. The related `VersionMarkLoadResult` record holds the loaded configuration (or
null on failure) together with all discovered issues. Tests are in
`Configuration/LintIssueTests.cs`.

#### Test Scenarios

The following test scenarios verify `LintIssue` and `VersionMarkLoadResult`:

- **`LintIssue_Constructor_AllFields_AreStoredCorrectly`**: Constructor stores severity, file path, line, column, and message.
- **`LintIssue_ToString_Error_ProducesLowercaseSeverity`**: `ToString` for an error issue includes lowercase "error".
- **`LintIssue_ToString_Warning_ProducesLowercaseSeverity`**: `ToString` for a warning issue includes lowercase "warning".
- **`VersionMarkLoadResult_Constructor_AllFields_AreStoredCorrectly`**: Constructor stores config and issue list correctly.
- **`VersionMarkLoadResult_ReportIssues_Error_WritesToErrorStream`**: Error-severity issues are written to the error stream.
- **`VersionMarkLoadResult_ReportIssues_Warning_WritesToStdOut`**: Warning-severity issues are written to standard output.

#### Dependencies

Tests use `StringWriter` to capture console and error stream output.

#### Requirements Coverage

The following list maps `LintIssue` and `VersionMarkLoadResult` unit requirements to
test scenarios:

- **`VersionMark-Load-LintIssue-Fields`**: `LintIssue_Constructor_AllFields_AreStoredCorrectly`
- **`VersionMark-Load-LintIssue-Format`**: `LintIssue_ToString_Error_ProducesLowercaseSeverity`,
  `LintIssue_ToString_Warning_ProducesLowercaseSeverity`
- **`VersionMark-Load-VersionMarkLoadResult`**: `VersionMarkLoadResult_Constructor_AllFields_AreStoredCorrectly`,
  `VersionMarkLoadResult_ReportIssues_Error_WritesToErrorStream`,
  `VersionMarkLoadResult_ReportIssues_Warning_WritesToStdOut`
