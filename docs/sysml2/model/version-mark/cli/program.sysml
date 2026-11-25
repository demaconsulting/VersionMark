package VersionMark_Program {
    private import SoftwareItemMetadata::*;

    part def Program {
        doc /* Entry point and mode dispatcher (capture, publish, lint, validate). */

        metadata info : SoftwareInfo {
            sourceFiles = ("src/DemaConsulting.VersionMark/Program.cs");
            testFiles = ("test/DemaConsulting.VersionMark.Tests/ProgramTests.cs");
            designFiles = ("docs/design/version-mark/cli/program.md");
            verificationFiles = ("docs/verification/version-mark/cli/program.md");
            requirementFiles = ("docs/reqstream/version-mark/cli/program.yaml");
        }
    }

    // Program dispatches each operational mode to the unit that implements it.
    dependency Program to VersionMark_Context::Context;
    dependency Program to VersionMark_HelpCommand::HelpCommand;
    dependency Program to VersionMark_VersionMarkConfig::VersionMarkConfig;
    dependency Program to VersionMark_VersionInfo::VersionInfo;
    dependency Program to VersionMark_MarkdownFormatter::MarkdownFormatter;
    dependency Program to VersionMark_GlobMatcher::GlobMatcher;
    dependency Program to VersionMark_Validation::Validation;
}
