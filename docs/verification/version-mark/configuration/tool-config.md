### ToolConfig

#### Verification Approach

The `ToolConfig` unit holds the per-tool configuration record, including default command,
default regex, and OS-specific overrides. It provides `GetEffectiveCommand` and
`GetEffectiveRegex` methods that select the appropriate value for the current (or a
specified) operating system. Tests are in `Configuration/VersionMarkConfigTests.cs` and
call methods directly on `ToolConfig` instances. No external mocks or file system access
is required.

#### Test Environment

N/A - standard test environment. All tests run using `dotnet test` with no additional
environment setup required.

#### Acceptance Criteria

- All unit tests for `ToolConfig` pass with zero failures across all supported OS and .NET
  version matrix combinations (Windows, Linux, macOS x .NET 8, .NET 9, .NET 10).
- Every requirement for the `ToolConfig` unit is covered by at least one named test
  scenario.

#### Test Scenarios

**ToolConfig_GetEffectiveCommand_NoOverride_ReturnsDefaultCommand**: No OS override returns
the default command. This scenario is tested by
`ToolConfig_GetEffectiveCommand_NoOverride_ReturnsDefaultCommand`.

**ToolConfig_GetEffectiveCommand_WithExplicitOs_ReturnsCorrectCommand**: An explicit OS
argument returns the matching override command. This scenario is tested by
`ToolConfig_GetEffectiveCommand_WithExplicitOs_ReturnsCorrectCommand`.

**ToolConfig_GetEffectiveCommand_WindowsOverride_ReturnsWindowsCommand**: The Windows
override is selected when running on Windows. This scenario is tested by
`ToolConfig_GetEffectiveCommand_WindowsOverride_ReturnsWindowsCommand`.

**ToolConfig_GetEffectiveCommand_LinuxOverride_ReturnsLinuxCommand**: The Linux override
is selected when running on Linux. This scenario is tested by
`ToolConfig_GetEffectiveCommand_LinuxOverride_ReturnsLinuxCommand`.

**ToolConfig_GetEffectiveCommand_MacOsOverride_ReturnsMacOsCommand**: The macOS override
is selected when running on macOS. This scenario is tested by
`ToolConfig_GetEffectiveCommand_MacOsOverride_ReturnsMacOsCommand`.

**ToolConfig_GetEffectiveCommand_NoDefaultKey_ThrowsInvalidOperationException**: No default
key and no matching OS override throws `InvalidOperationException`. This scenario is
tested by `ToolConfig_GetEffectiveCommand_NoDefaultKey_ThrowsInvalidOperationException`.

**ToolConfig_GetEffectiveRegex_NoOverride_ReturnsDefaultRegex**: No OS override returns the
default regex. This scenario is tested by
`ToolConfig_GetEffectiveRegex_NoOverride_ReturnsDefaultRegex`.

**ToolConfig_GetEffectiveRegex_WithExplicitOs_ReturnsCorrectRegex**: An explicit OS
argument returns the matching override regex. This scenario is tested by
`ToolConfig_GetEffectiveRegex_WithExplicitOs_ReturnsCorrectRegex`.

**ToolConfig_GetEffectiveRegex_WindowsOverride_ReturnsWindowsRegex**: The Windows regex
override is selected when running on Windows. This scenario is tested by
`ToolConfig_GetEffectiveRegex_WindowsOverride_ReturnsWindowsRegex`.

**ToolConfig_GetEffectiveRegex_LinuxOverride_ReturnsLinuxRegex**: The Linux regex override
is selected when running on Linux. This scenario is tested by
`ToolConfig_GetEffectiveRegex_LinuxOverride_ReturnsLinuxRegex`.

**ToolConfig_GetEffectiveRegex_MacOsOverride_ReturnsMacOsRegex**: The macOS regex override
is selected when running on macOS. This scenario is tested by
`ToolConfig_GetEffectiveRegex_MacOsOverride_ReturnsMacOsRegex`.

**ToolConfig_GetEffectiveRegex_NoDefaultKey_ThrowsInvalidOperationException**: No default
key and no matching OS override throws `InvalidOperationException`. This scenario is
tested by `ToolConfig_GetEffectiveRegex_NoDefaultKey_ThrowsInvalidOperationException`.
