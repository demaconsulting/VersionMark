package VersionMark {
    package Publishing {
        private import SoftwareItemMetadata::*;

        part def MarkdownFormatter {
            metadata info : SoftwareInfo {
                description = "Converts loaded version records into a consolidated markdown report.";
                sourceFiles = ("src/DemaConsulting.VersionMark/Publishing/MarkdownFormatter.cs");
                testFiles = ("test/DemaConsulting.VersionMark.Tests/Publishing/MarkdownFormatterTests.cs");
                designFiles = ("docs/design/version-mark/publishing/markdown-formatter.md");
                verificationFiles = ("docs/verification/version-mark/publishing/markdown-formatter.md");
                requirementFiles = ("docs/reqstream/version-mark/publishing/markdown-formatter.yaml");
            }
        }
    }
}
