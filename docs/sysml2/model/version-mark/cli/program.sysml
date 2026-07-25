package VersionMark {
    private import SoftwareItemMetadata::*;

    part def Program {
        metadata info : SoftwareInfo {
            description = "Entry point and mode dispatcher (capture, publish, lint, validate).";
            sourceFiles = ("src/DemaConsulting.VersionMark/Program.cs");
            testFiles = ("test/DemaConsulting.VersionMark.Tests/ProgramTests.cs");
            designFiles = ("docs/design/version-mark/cli/program.md");
            verificationFiles = ("docs/verification/version-mark/cli/program.md");
            requirementFiles = ("docs/reqstream/version-mark/cli/program.yaml");
        }
    }

    // Program dispatches each operational mode to the unit that implements it.
    dependency Program to Context;
    dependency Program to VersionMarkConfig;
    dependency Program to VersionInfo;
    dependency Program to MarkdownFormatter;
    dependency Program to GlobMatcher;
    dependency Program to Validation;
}
