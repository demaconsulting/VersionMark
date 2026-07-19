package VersionMark {
    private import SoftwareItemMetadata::*;
    import OtsDependencies::*;

    part def GlobMatcher {
        metadata info : SoftwareInfo {
            description = "Resolves glob patterns into concrete file path lists for publish mode.";
            sourceFiles = ("src/DemaConsulting.VersionMark/Utilities/GlobMatcher.cs");
            testFiles = ("test/DemaConsulting.VersionMark.Tests/Utilities/GlobMatcherTests.cs");
            designFiles = ("docs/design/version-mark/utilities/glob-matcher.md");
            verificationFiles = ("docs/verification/version-mark/utilities/glob-matcher.md");
            requirementFiles = ("docs/reqstream/version-mark/utilities/glob-matcher.yaml");
        }
    }

    // FindMatchingFiles delegates pattern evaluation to Microsoft.Extensions.FileSystemGlobbing.
    dependency GlobMatcher to FileSystemGlobbing;
}
