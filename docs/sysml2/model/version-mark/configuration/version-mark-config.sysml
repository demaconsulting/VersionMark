package VersionMark {
    private import SoftwareItemMetadata::*;
    import OtsDependencies::*;

    part def VersionMarkConfig {
        metadata info : SoftwareInfo {
            description = "Loads, validates, and executes `.versionmark.yaml` tool version capture.";
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
        part tools : ToolConfig [0..*];
    }

    // Load validates and produces LintIssue/ToolConfig records; FindVersions returns VersionInfo.
    dependency VersionMarkConfig to ToolConfig;
    dependency VersionMarkConfig to LintIssue;
    dependency VersionMarkConfig to VersionInfo;
    dependency VersionMarkConfig to YamlDotNet;
}
