package VersionMark {
    private import SoftwareItemMetadata::*;
    import OtsDependencies::*;

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

    // Validation drives each functional test re-entrantly through Program, and records
    // results via TestResults.
    dependency Validation to Context;
    dependency Validation to Program;
    dependency Validation to VersionInfo;
    dependency Validation to PathHelpers;
    dependency Validation to TestResults;
}
