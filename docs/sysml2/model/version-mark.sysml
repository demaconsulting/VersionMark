package VersionMark {
    import OtsDependencies::*;
    private import SoftwareItemMetadata::*;

    doc
    /*
     * VersionMark is a .NET global tool that captures tool/dependency versions,
     * publishes markdown version reports, and self-validates its own operation.
     */

    part def VersionMarkSystem {
        doc /* The VersionMark system as a whole. */

        metadata info : SoftwareInfo {
            testFiles = ("test/DemaConsulting.VersionMark.Tests/IntegrationTests.cs");
            designFiles = ("docs/design/version-mark.md");
            verificationFiles = ("docs/verification/version-mark.md");
            requirementFiles = ("docs/reqstream/version-mark.yaml");
        }

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
