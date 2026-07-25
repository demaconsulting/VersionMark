package VersionMark {
    private import SoftwareItemMetadata::*;
    private import ScalarValues::String;

    part def VersionInfo {
        metadata info : SoftwareInfo {
            description = "Captured version record; JSON serialization for capture/publish handoff.";
            sourceFiles = ("src/DemaConsulting.VersionMark/Capture/VersionInfo.cs");
            testFiles = ("test/DemaConsulting.VersionMark.Tests/Capture/VersionInfoTests.cs");
            designFiles = ("docs/design/version-mark/capture/version-info.md");
            verificationFiles = ("docs/verification/version-mark/capture/version-info.md");
            requirementFiles = ("docs/reqstream/version-mark/capture/version-info.yaml");
        }

        attribute jobId : String;

        // Captured tool versions dictionary (Dictionary<string, string> in code), split into
        // parallel key/value lists since a native map attribute is not representable here.
        attribute versions_key : String [0..*];
        attribute versions_value : String [0..*];
    }
}
