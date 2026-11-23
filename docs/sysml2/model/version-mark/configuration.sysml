package VersionMark_ConfigurationSubsystem {
    public import VersionMark_VersionMarkConfig::*;
    public import VersionMark_ToolConfig::*;
    public import VersionMark_LintIssue::*;
    private import SoftwareItemMetadata::*;

    part def ConfigurationSubsystem {
        doc /* YAML configuration loading, validation, and version capture execution. */

        metadata info : SoftwareInfo {
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
