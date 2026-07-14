package VersionMark {
    package Capture {
        private import SoftwareItemMetadata::*;

        part def VersionInfo {
            metadata info : SoftwareInfo {
                description = "Captured version record; JSON serialization for capture/publish handoff.";
                sourceFiles = ("src/DemaConsulting.VersionMark/Capture/VersionInfo.cs");
                testFiles = ("test/DemaConsulting.VersionMark.Tests/Capture/VersionInfoTests.cs");
                designFiles = ("docs/design/version-mark/capture/version-info.md");
                verificationFiles = ("docs/verification/version-mark/capture/version-info.md");
                requirementFiles = ("docs/reqstream/version-mark/capture/version-info.yaml");
            }
        }
    }
}
