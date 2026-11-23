package VersionMark_UtilitiesSubsystem {
    public import VersionMark_GlobMatcher::*;
    public import VersionMark_PathHelpers::*;
    private import SoftwareItemMetadata::*;

    part def UtilitiesSubsystem {
        doc /* Glob-pattern matching and safe path combination helpers. */

        metadata info : SoftwareInfo {
            designFiles = ("docs/design/version-mark/utilities.md");
            verificationFiles = ("docs/verification/version-mark/utilities.md");
            requirementFiles = ("docs/reqstream/version-mark/utilities.yaml");
        }

        part globMatcher : GlobMatcher;
        part pathHelpers : PathHelpers;
    }
}
