package VersionMark {
    package Configuration {
        private import SoftwareItemMetadata::*;

        part def ToolConfig {
            metadata info : SoftwareInfo {
                description = "Per-tool capture configuration (command, regex, OS-specific overrides).";
                sourceFiles = ("src/DemaConsulting.VersionMark/Configuration/VersionMarkConfig.cs");
                testFiles = ("test/DemaConsulting.VersionMark.Tests/Configuration/VersionMarkConfigTests.cs");
                designFiles = ("docs/design/version-mark/configuration/tool-config.md");
                verificationFiles = ("docs/verification/version-mark/configuration/tool-config.md");
                requirementFiles = ("docs/reqstream/version-mark/configuration/tool-config.yaml");
            }
        }
    }
}
