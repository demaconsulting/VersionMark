package VersionMark_VersionMarkConfig {
    private import SoftwareItemMetadata::*;
    private import OtsDependencies::*;

    part def VersionMarkConfig {
        doc /* Loads, validates, and executes `.versionmark.yaml` tool version capture. */

        metadata info : SoftwareInfo {
            sourceFiles = ("src/DemaConsulting.VersionMark/Configuration/VersionMarkConfig.cs");
            testFiles = (
                "test/DemaConsulting.VersionMark.Tests/Configuration/VersionMarkConfigTests.cs",
                "test/DemaConsulting.VersionMark.Tests/Configuration/VersionMarkConfigLoadTests.cs"
            );
            designFiles = ("docs/design/version-mark/configuration/version-mark-config.md");
            verificationFiles = ("docs/verification/version-mark/configuration/version-mark-config.md");
            requirementFiles = (
                "docs/reqstream/version-mark/configuration/version-mark-config.yaml",
                "docs/reqstream/version-mark/configuration/load.yaml"
            );
        }

        // Tools is keyed by tool name; the key/value pairing is not representable here,
        // so this is a flat multiplicity of ToolConfig parts.
        part tools : VersionMark_ToolConfig::ToolConfig [0..*];
    }

    // Load validates and produces LintIssue/ToolConfig records; FindVersions returns VersionInfo.
    dependency VersionMarkConfig to VersionMark_ToolConfig::ToolConfig;
    dependency VersionMarkConfig to VersionMark_LintIssue::LintIssue;
    dependency VersionMarkConfig to VersionMark_VersionInfo::VersionInfo;
    dependency VersionMarkConfig to OtsDependencies::YamlDotNet;
}
