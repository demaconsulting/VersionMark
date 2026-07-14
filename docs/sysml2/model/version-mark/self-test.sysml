package VersionMark {
    package SelfTest {
        private import SoftwareItemMetadata::*;

        part def SelfTestSubsystem {
            metadata info : SoftwareInfo {
                description = "Built-in self-verification of all operational modes.";
                testFiles = ("test/DemaConsulting.VersionMark.Tests/SelfTest/SelfTestTests.cs");
                designFiles = ("docs/design/version-mark/self-test.md");
                verificationFiles = ("docs/verification/version-mark/self-test.md");
                requirementFiles = ("docs/reqstream/version-mark/self-test.yaml");
            }

            part validation : Validation;
        }
    }
}
