// Views rendered into docs/design/generated/ for the Design document.
// Per SysML2Tools syntax, 'expose' is only valid
// inside a named 'view' usage (not a 'view def' definition), and it - not
// 'render' - is what scopes the rendered diagram content.
package VersionMarkDesignViews {
    private import StandardViewDefinitions::*;
    private import Views::*;
    private import VersionMark::*;

    view SoftwareStructureView : InterconnectionView {
        expose VersionMark::VersionMarkSystem;
    }

    view VersionMarkSystemView {
        expose VersionMarkSystem;
        expose VersionMarkSystem::*;
        render asInterconnectionDiagram;
    }

    view CliView {
        expose CliSubsystem;
        expose CliSubsystem::*;
        render asInterconnectionDiagram;
    }

    view ConfigurationView {
        expose ConfigurationSubsystem;
        expose ConfigurationSubsystem::*;
        render asInterconnectionDiagram;
    }

    view CaptureView {
        expose CaptureSubsystem;
        expose CaptureSubsystem::*;
        render asInterconnectionDiagram;
    }

    view PublishingView {
        expose PublishingSubsystem;
        expose PublishingSubsystem::*;
        render asInterconnectionDiagram;
    }

    view SelfTestView {
        expose SelfTestSubsystem;
        expose SelfTestSubsystem::*;
        render asInterconnectionDiagram;
    }

    view UtilitiesView {
        expose UtilitiesSubsystem;
        expose UtilitiesSubsystem::*;
        render asInterconnectionDiagram;
    }
}
