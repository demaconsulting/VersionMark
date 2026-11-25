package VersionMark_CliSubsystem {
    public import VersionMark_Program::*;
    public import VersionMark_Context::*;
    public import VersionMark_HelpCommand::*;
    private import SoftwareItemMetadata::*;

    part def CliSubsystem {
        doc /* Command-line argument parsing, mode dispatch, output routing, and the built-in help command. */

        metadata info : SoftwareInfo {
            testFiles = ("test/DemaConsulting.VersionMark.Tests/Cli/CliTests.cs");
            designFiles = ("docs/design/version-mark/cli.md");
            verificationFiles = ("docs/verification/version-mark/cli.md");
            requirementFiles = ("docs/reqstream/version-mark/cli.yaml");
        }

        part program : Program;
        part context : Context;
        part helpCommand : HelpCommand;
    }
}
