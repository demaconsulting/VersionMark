package VersionMark {
    private import SoftwareItemMetadata::*;

    part def PathHelpers {
        metadata info : SoftwareInfo {
            description = "Safe path combination helpers used to construct paths within temp directories.";
            sourceFiles = ("src/DemaConsulting.VersionMark/Utilities/PathHelpers.cs");
            testFiles = ("test/DemaConsulting.VersionMark.Tests/Utilities/PathHelpersTests.cs");
            designFiles = ("docs/design/version-mark/utilities/path-helpers.md");
            verificationFiles = ("docs/verification/version-mark/utilities/path-helpers.md");
            requirementFiles = ("docs/reqstream/version-mark/utilities/path-helpers.yaml");
        }
    }
}
