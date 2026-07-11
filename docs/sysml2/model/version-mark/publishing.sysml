package VersionMark {
    part def PublishingSubsystem {
        doc /* Markdown report generation from captured version data. */

        comment testRef /* Test: test/DemaConsulting.VersionMark.Tests/Publishing/PublishingTests.cs */
        comment designRef /* Design: docs/design/version-mark/publishing.md */
        comment verificationRef /* Verification: docs/verification/version-mark/publishing.md */
        comment reqRef /* Requirements: docs/reqstream/version-mark/publishing.yaml */

        part markdownFormatter : MarkdownFormatter;
    }
}
