package VersionMark {
    part def SelfTestSubsystem {
        doc /* Built-in self-verification of all operational modes. */

        comment testRef /* Test: test/DemaConsulting.VersionMark.Tests/SelfTest/SelfTestTests.cs */
        comment designRef /* Design: docs/design/version-mark/self-test.md */
        comment verificationRef /* Verification: docs/verification/version-mark/self-test.md */
        comment reqRef /* Requirements: docs/reqstream/version-mark/self-test.yaml */

        part validation : Validation;
    }
}
