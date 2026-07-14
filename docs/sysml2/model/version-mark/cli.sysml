package VersionMark {
    package Cli {
        private import SoftwareItemMetadata::*;

        part def CliSubsystem {
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
}
