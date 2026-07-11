package VersionMark {
    import OtsDependencies::*;

    doc
    /*
     * VersionMark is a .NET global tool that captures tool/dependency versions,
     * publishes markdown version reports, and self-validates its own operation.
     */

    part def VersionMarkSystem {
        doc /* The VersionMark system as a whole. */

        comment testRef /* Test: test/DemaConsulting.VersionMark.Tests/IntegrationTests.cs */
        comment designRef /* Design: docs/design/version-mark.md */
        comment verificationRef /* Verification: docs/verification/version-mark.md */
        comment reqRef /* Requirements: docs/reqstream/version-mark.yaml */

        part cli : CliSubsystem;
        part configuration : ConfigurationSubsystem;
        part capture : CaptureSubsystem;
        part publishing : PublishingSubsystem;
        part selfTest : SelfTestSubsystem;
        part utilities : UtilitiesSubsystem;

        part yamlDotNet : YamlDotNet;
        part fileSystemGlobbing : FileSystemGlobbing;
        part testResults : TestResults;
    }
}
