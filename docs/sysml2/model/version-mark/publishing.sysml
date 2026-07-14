package VersionMark {
    package Publishing {
        private import SoftwareItemMetadata::*;

        part def PublishingSubsystem {
            metadata info : SoftwareInfo {
                description = "Markdown report generation from captured version data.";
                testFiles = ("test/DemaConsulting.VersionMark.Tests/Publishing/PublishingTests.cs");
                designFiles = ("docs/design/version-mark/publishing.md");
                verificationFiles = ("docs/verification/version-mark/publishing.md");
                requirementFiles = ("docs/reqstream/version-mark/publishing.yaml");
            }

            part markdownFormatter : MarkdownFormatter;
        }
    }
}
