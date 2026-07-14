package VersionMark {
    package Configuration {
        private import SoftwareItemMetadata::*;

        part def LintIssue {
            metadata info : SoftwareInfo {
                description = "Represents a single configuration validation issue with source location.";
                sourceFiles = ("src/DemaConsulting.VersionMark/Configuration/LintIssue.cs");
                testFiles = ("test/DemaConsulting.VersionMark.Tests/Configuration/LintIssueTests.cs");
                designFiles = ("docs/design/version-mark/configuration/lint-issue.md");
                verificationFiles = ("docs/verification/version-mark/configuration/lint-issue.md");
                requirementFiles = ("docs/reqstream/version-mark/configuration/lint-issue.yaml");
            }
        }
    }
}
