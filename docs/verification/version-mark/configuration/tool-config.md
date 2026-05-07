### ToolConfig Unit Verification

#### Overview

The `ToolConfig` unit holds the per-tool configuration record, including default command,
default regex, and OS-specific overrides. It provides `GetEffectiveCommand` and
`GetEffectiveRegex` methods that select the appropriate value for the current (or a
specified) operating system. Tests are in `Configuration/VersionMarkConfigTests.cs`.

#### Test Scenarios — GetEffectiveCommand

The following test scenarios verify `ToolConfig.GetEffectiveCommand`:

- **`ToolConfig_GetEffectiveCommand_NoOverride_ReturnsDefaultCommand`**: No OS override returns the default command.
- **`ToolConfig_GetEffectiveCommand_WithExplicitOs_ReturnsCorrectCommand`**: Explicit OS argument returns the matching override.
- **`ToolConfig_GetEffectiveCommand_WindowsOverride_ReturnsWindowsCommand`**: Windows override selected on Windows.
- **`ToolConfig_GetEffectiveCommand_LinuxOverride_ReturnsLinuxCommand`**: Linux override selected on Linux.
- **`ToolConfig_GetEffectiveCommand_MacOsOverride_ReturnsMacOsCommand`**: macOS override selected on macOS.
- **`ToolConfig_GetEffectiveCommand_NoDefaultKey_ThrowsInvalidOperationException`**:
  No default key and no matching override throws InvalidOperationException.

#### Test Scenarios — GetEffectiveRegex

The following test scenarios verify `ToolConfig.GetEffectiveRegex`:

- **`ToolConfig_GetEffectiveRegex_NoOverride_ReturnsDefaultRegex`**: No OS override returns the default regex.
- **`ToolConfig_GetEffectiveRegex_WithExplicitOs_ReturnsCorrectRegex`**: Explicit OS argument returns the matching override.
- **`ToolConfig_GetEffectiveRegex_WindowsOverride_ReturnsWindowsRegex`**: Windows override selected on Windows.
- **`ToolConfig_GetEffectiveRegex_LinuxOverride_ReturnsLinuxRegex`**: Linux override selected on Linux.
- **`ToolConfig_GetEffectiveRegex_MacOsOverride_ReturnsMacOsRegex`**: macOS override selected on macOS.
- **`ToolConfig_GetEffectiveRegex_NoDefaultKey_ThrowsInvalidOperationException`**:
  No default key and no matching override throws InvalidOperationException.

#### Dependencies

No external mocks or file system access is required. Tests call methods directly on
`ToolConfig` instances.

#### Requirements Coverage

The following list maps `ToolConfig` unit requirements to test scenarios:

- **`VersionMark-ToolConfig-EffectiveCommand`**: `ToolConfig_GetEffectiveCommand_NoOverride_ReturnsDefaultCommand`,
  `ToolConfig_GetEffectiveCommand_WithExplicitOs_ReturnsCorrectCommand`,
  `ToolConfig_GetEffectiveCommand_WindowsOverride_ReturnsWindowsCommand`,
  `ToolConfig_GetEffectiveCommand_LinuxOverride_ReturnsLinuxCommand`,
  `ToolConfig_GetEffectiveCommand_MacOsOverride_ReturnsMacOsCommand`,
  `ToolConfig_GetEffectiveCommand_NoDefaultKey_ThrowsInvalidOperationException`
- **`VersionMark-ToolConfig-EffectiveRegex`**: `ToolConfig_GetEffectiveRegex_NoOverride_ReturnsDefaultRegex`,
  `ToolConfig_GetEffectiveRegex_WithExplicitOs_ReturnsCorrectRegex`,
  `ToolConfig_GetEffectiveRegex_WindowsOverride_ReturnsWindowsRegex`,
  `ToolConfig_GetEffectiveRegex_LinuxOverride_ReturnsLinuxRegex`,
  `ToolConfig_GetEffectiveRegex_MacOsOverride_ReturnsMacOsRegex`,
  `ToolConfig_GetEffectiveRegex_NoDefaultKey_ThrowsInvalidOperationException`
