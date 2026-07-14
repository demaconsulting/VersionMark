package VersionMark {
    package Cli {
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
    }
}
