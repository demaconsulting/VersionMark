package VersionMark {
    part def CliSubsystem {
        doc /* Command-line argument parsing, mode dispatch, and output routing. */

        comment testRef /* Test: test/DemaConsulting.VersionMark.Tests/Cli/CliTests.cs */
        comment designRef /* Design: docs/design/version-mark/cli.md */
        comment verificationRef /* Verification: docs/verification/version-mark/cli.md */
        comment reqRef /* Requirements: docs/reqstream/version-mark/cli.yaml */

        part program : Program;
        part context : Context;
    }
}
