package VersionMark {
    part def ConfigurationSubsystem {
        doc /* YAML configuration loading, validation, and version capture execution. */

        comment testRef /* Test: test/DemaConsulting.VersionMark.Tests/Configuration/ConfigurationTests.cs */
        comment designRef /* Design: docs/design/version-mark/configuration.md */
        comment verificationRef /* Verification: docs/verification/version-mark/configuration.md */
        comment reqRef /* Requirements: docs/reqstream/version-mark/configuration.yaml */

        part versionMarkConfig : VersionMarkConfig;
        part toolConfig : ToolConfig;
        part lintIssue : LintIssue;
    }
}
