package VersionMark_Validation {
    private import SoftwareItemMetadata::*;
    private import OtsDependencies::*;

    part def Validation {
        doc /* Exercises capture, publish, and lint modes end-to-end for self-validation. */

        metadata info : SoftwareInfo {
            sourceFiles = ("src/DemaConsulting.VersionMark/SelfTest/Validation.cs");
            testFiles = ("test/DemaConsulting.VersionMark.Tests/SelfTest/SelfTestTests.cs");
            designFiles = ("docs/design/version-mark/self-test/validation.md");
            verificationFiles = ("docs/verification/version-mark/self-test/validation.md");
            requirementFiles = ("docs/reqstream/version-mark/self-test/validation.yaml");
        }
    }

    // Validation drives each functional test re-entrantly through Program, and records
    // results via TestResults.
    dependency Validation to VersionMark_Context::Context;
    dependency Validation to VersionMark_Program::Program;
    dependency Validation to VersionMark_VersionInfo::VersionInfo;
    dependency Validation to VersionMark_PathHelpers::PathHelpers;
    dependency Validation to OtsDependencies::TestResults;
}
