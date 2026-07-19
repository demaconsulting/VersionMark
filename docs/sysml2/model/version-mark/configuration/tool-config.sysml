package VersionMark {
    private import SoftwareItemMetadata::*;
    private import ScalarValues::String;

    part def ToolConfig {
        metadata info : SoftwareInfo {
            description = "Per-tool capture configuration (command, regex, OS-specific overrides).";
            sourceFiles = ("src/DemaConsulting.VersionMark/Configuration/VersionMarkConfig.cs");
            testFiles = ("test/DemaConsulting.VersionMark.Tests/Configuration/VersionMarkConfigTests.cs");
            designFiles = ("docs/design/version-mark/configuration/tool-config.md");
            verificationFiles = ("docs/verification/version-mark/configuration/tool-config.md");
            requirementFiles = ("docs/reqstream/version-mark/configuration/tool-config.yaml");
        }

        // Command dictionary, keyed by OS name ("" for default, "win", "linux", "macos"),
        // split into parallel key/value lists since a native map attribute is not
        // representable here.
        attribute command_key : String [0..*];
        attribute command_value : String [0..*];

        // Regex dictionary, keyed by OS name ("" for default, "win", "linux", "macos"),
        // split into parallel key/value lists since a native map attribute is not
        // representable here.
        attribute regex_key : String [0..*];
        attribute regex_value : String [0..*];
    }
}
