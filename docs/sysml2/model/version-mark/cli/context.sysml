package VersionMark {
    private import SoftwareItemMetadata::*;

    part def Context {
        metadata info : SoftwareInfo {
            description = "Command-line argument and output container (console, error, log file).";
            sourceFiles = ("src/DemaConsulting.VersionMark/Cli/Context.cs");
            testFiles = ("test/DemaConsulting.VersionMark.Tests/Cli/ContextTests.cs");
            designFiles = ("docs/design/version-mark/cli/context.md");
            verificationFiles = ("docs/verification/version-mark/cli/context.md");
            requirementFiles = ("docs/reqstream/version-mark/cli/context.yaml");
        }
    }
}
