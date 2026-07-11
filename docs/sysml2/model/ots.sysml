package OtsDependencies {
    doc /* Off-the-shelf (OTS) runtime dependencies used by VersionMark. */

    part def YamlDotNet {
        doc /* OTS: YamlDotNet. */

        comment designRef /* Design: docs/design/ots/yaml-dot-net.md */
        comment verificationRef /* Verification: docs/verification/ots/yaml-dot-net.md */
        comment reqRef /* Requirements: docs/reqstream/ots/yaml-dot-net.yaml */
    }

    part def FileSystemGlobbing {
        doc /* OTS: Microsoft.Extensions.FileSystemGlobbing. */

        comment designRef /* Design: docs/design/ots/microsoft-extensions-file-system-globbing.md */
        comment verificationRef /* Verification: docs/verification/ots/microsoft-extensions-file-system-globbing.md */
        comment reqRef /* Requirements: docs/reqstream/ots/microsoft-extensions-file-system-globbing.yaml */
    }

    part def TestResults {
        doc /* OTS: DemaConsulting.TestResults. */

        comment designRef /* Design: docs/design/ots/dema-consulting-test-results.md */
        comment verificationRef /* Verification: docs/verification/ots/dema-consulting-test-results.md */
        comment reqRef /* Requirements: docs/reqstream/ots/dema-consulting-test-results.yaml */
    }
}
