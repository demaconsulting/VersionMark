package VersionMark {
    part def UtilitiesSubsystem {
        doc /* Glob-pattern matching and safe path combination helpers. */

        comment designRef /* Design: docs/design/version-mark/utilities.md */
        comment verificationRef /* Verification: docs/verification/version-mark/utilities.md */
        comment reqRef /* Requirements: docs/reqstream/version-mark/utilities.yaml */

        part globMatcher : GlobMatcher;
        part pathHelpers : PathHelpers;
    }
}
