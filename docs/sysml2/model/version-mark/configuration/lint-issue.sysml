package VersionMark {
    private import SoftwareItemMetadata::*;
    private import ScalarValues::Integer;
    private import ScalarValues::String;

    enum def LintSeverity {
        enum warning;
        enum error;
    }

    part def LintIssue {
        metadata info : SoftwareInfo {
            description = "Represents a single configuration validation issue with source location.";
            sourceFiles = ("src/DemaConsulting.VersionMark/Configuration/LintIssue.cs");
            testFiles = ("test/DemaConsulting.VersionMark.Tests/Configuration/LintIssueTests.cs");
            designFiles = ("docs/design/version-mark/configuration/lint-issue.md");
            verificationFiles = ("docs/verification/version-mark/configuration/lint-issue.md");
            requirementFiles = ("docs/reqstream/version-mark/configuration/lint-issue.yaml");
        }

        attribute filePath : String;
        attribute line : Integer;
        attribute column : Integer;
        attribute severity : LintSeverity;
        attribute description : String;
    }

    // ReportIssues writes discovered issues to the Cli Context.
    dependency LintIssue to Context;
}
