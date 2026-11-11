package VersionMark {
    private import SoftwareItemMetadata::*;

    part def PublishingSubsystem {
        doc /* Markdown report generation from captured version data. */

        metadata info : SoftwareInfo {
            testFiles = ("test/DemaConsulting.VersionMark.Tests/Publishing/PublishingTests.cs");
            designFiles = ("docs/design/version-mark/publishing.md");
            verificationFiles = ("docs/verification/version-mark/publishing.md");
            requirementFiles = ("docs/reqstream/version-mark/publishing.yaml");
        }

        part markdownFormatter : MarkdownFormatter;
    }
}
