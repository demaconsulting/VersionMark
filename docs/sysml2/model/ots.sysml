package OtsDependencies {
    private import SoftwareItemMetadata::*;

    doc /* Off-the-shelf (OTS) runtime dependencies used by VersionMark. */

    part def YamlDotNet {
        metadata info : SoftwareInfo {
            description = "OTS: YamlDotNet.";
            designFiles = ("docs/design/ots/yaml-dot-net.md");
            verificationFiles = ("docs/verification/ots/yaml-dot-net.md");
            requirementFiles = ("docs/reqstream/ots/yaml-dot-net.yaml");
        }
    }

    part def FileSystemGlobbing {
        metadata info : SoftwareInfo {
            description = "OTS: Microsoft.Extensions.FileSystemGlobbing.";
            designFiles = ("docs/design/ots/microsoft-extensions-file-system-globbing.md");
            verificationFiles = ("docs/verification/ots/microsoft-extensions-file-system-globbing.md");
            requirementFiles = ("docs/reqstream/ots/microsoft-extensions-file-system-globbing.yaml");
        }
    }

    part def TestResults {
        metadata info : SoftwareInfo {
            description = "OTS: DemaConsulting.TestResults.";
            designFiles = ("docs/design/ots/dema-consulting-test-results.md");
            verificationFiles = ("docs/verification/ots/dema-consulting-test-results.md");
            requirementFiles = ("docs/reqstream/ots/dema-consulting-test-results.yaml");
        }
    }
}
