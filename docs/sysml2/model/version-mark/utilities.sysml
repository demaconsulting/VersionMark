package VersionMark {
    package Utilities {
        private import SoftwareItemMetadata::*;

        part def UtilitiesSubsystem {
            metadata info : SoftwareInfo {
                description = "Glob-pattern matching and safe path combination helpers.";
                designFiles = ("docs/design/version-mark/utilities.md");
                verificationFiles = ("docs/verification/version-mark/utilities.md");
                requirementFiles = ("docs/reqstream/version-mark/utilities.yaml");
            }

            part globMatcher : GlobMatcher;
            part pathHelpers : PathHelpers;
        }
    }
}
