package VersionMark {
    package SelfTest {
        private import SoftwareItemMetadata::*;

        part def Validation {
            metadata info : SoftwareInfo {
                description = "Exercises capture, publish, and lint modes end-to-end for self-validation.";
                sourceFiles = ("src/DemaConsulting.VersionMark/SelfTest/Validation.cs");
                testFiles = ("test/DemaConsulting.VersionMark.Tests/SelfTest/SelfTestTests.cs");
                designFiles = ("docs/design/version-mark/self-test/validation.md");
                verificationFiles = ("docs/verification/version-mark/self-test/validation.md");
                requirementFiles = ("docs/reqstream/version-mark/self-test/validation.yaml");
            }
        }
    }
}
