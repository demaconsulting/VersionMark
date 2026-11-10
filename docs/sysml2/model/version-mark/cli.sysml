package VersionMark {
    private import SoftwareItemMetadata::*;

    part def CliSubsystem {
        doc /* Command-line argument parsing, mode dispatch, and output routing. */

        metadata info : SoftwareInfo {
            description = "Command-line argument parsing, mode dispatch, and output routing.";
            testFiles = ("test/DemaConsulting.VersionMark.Tests/Cli/CliTests.cs");
            designFiles = ("docs/design/version-mark/cli.md");
            verificationFiles = ("docs/verification/version-mark/cli.md");
            requirementFiles = ("docs/reqstream/version-mark/cli.yaml");
        }

        part program : Program;
        part context : Context;
    }
}
