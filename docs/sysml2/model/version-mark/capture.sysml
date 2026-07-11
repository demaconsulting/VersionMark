package VersionMark {
    part def CaptureSubsystem {
        doc /* JSON serialization of captured version data. */

        comment testRef /* Test: test/DemaConsulting.VersionMark.Tests/Capture/CaptureTests.cs */
        comment designRef /* Design: docs/design/version-mark/capture.md */
        comment verificationRef /* Verification: docs/verification/version-mark/capture.md */
        comment reqRef /* Requirements: docs/reqstream/version-mark/capture.yaml */

        part versionInfo : VersionInfo;
    }
}
