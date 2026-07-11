// Views rendered into docs/design/generated/ for the Design document.
// Reopens the VersionMark package so each 'expose' statement can reference
// its target part def directly. Per SysML2Tools syntax, 'expose' is only valid
// inside a named 'view' usage (not a 'view def' definition), and it - not
// 'render' - is what scopes the rendered diagram content.
package VersionMark {
    view SoftwareStructureView {
        expose VersionMark;
    }

    view VersionMarkView {
        expose VersionMarkSystem;
    }

    view CliView {
        expose CliSubsystem;
    }

    view ConfigurationView {
        expose ConfigurationSubsystem;
    }

    view CaptureView {
        expose CaptureSubsystem;
    }

    view PublishingView {
        expose PublishingSubsystem;
    }

    view SelfTestView {
        expose SelfTestSubsystem;
    }

    view UtilitiesView {
        expose UtilitiesSubsystem;
    }
}
