package VersionMark {
    package Configuration {
        private import SoftwareItemMetadata::*;

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
        }
    }
}
