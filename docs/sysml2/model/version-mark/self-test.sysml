package VersionMark {
    private import SoftwareItemMetadata::*;

    part def SelfTestSubsystem {
        doc /* Built-in self-verification of all operational modes. */

        metadata info : SoftwareInfo {
            testFiles = ("test/DemaConsulting.VersionMark.Tests/SelfTest/SelfTestTests.cs");
            designFiles = ("docs/design/version-mark/self-test.md");
            verificationFiles = ("docs/verification/version-mark/self-test.md");
            requirementFiles = ("docs/reqstream/version-mark/self-test.yaml");
        }

        part validation : Validation;
    }
}
