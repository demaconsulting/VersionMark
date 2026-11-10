package VersionMark {
    private import SoftwareItemMetadata::*;

    part def CaptureSubsystem {
        doc /* JSON serialization of captured version data. */

        metadata info : SoftwareInfo {
            description = "JSON serialization of captured version data.";
            testFiles = ("test/DemaConsulting.VersionMark.Tests/Capture/CaptureTests.cs");
            designFiles = ("docs/design/version-mark/capture.md");
            verificationFiles = ("docs/verification/version-mark/capture.md");
            requirementFiles = ("docs/reqstream/version-mark/capture.yaml");
        }

        part versionInfo : VersionInfo;
    }
}
