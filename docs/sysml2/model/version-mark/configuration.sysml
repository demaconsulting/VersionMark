package VersionMark {
    private import SoftwareItemMetadata::*;

    part def ConfigurationSubsystem {
        metadata info : SoftwareInfo {
            description = "YAML configuration loading, validation, and version capture execution.";
            testFiles = ("test/DemaConsulting.VersionMark.Tests/Configuration/ConfigurationTests.cs");
            designFiles = ("docs/design/version-mark/configuration.md");
            verificationFiles = ("docs/verification/version-mark/configuration.md");
            requirementFiles = ("docs/reqstream/version-mark/configuration.yaml");
        }

        part versionMarkConfig : VersionMarkConfig;
        part toolConfig : ToolConfig;
        part lintIssue : LintIssue;
    }
}
