# VersionMarkConfig Unit

## Overview

The `VersionMarkConfig` record holds a `Dictionary<string, ToolConfig>` mapping tool names
to their configurations. It is the top-level entry point for loading configuration from
the `.versionmark.yaml` file.

## Load Method

`Load` is the primary entry point for loading configuration with integrated linting. It:

1. Checks that the file exists; adds an error issue if not.
2. Parses the YAML stream and validates the root node is a mapping.
3. Locates the `tools` mapping key; adds an error issue if absent.
4. Iterates tool entries, calling the private `ValidateTool` method for each.
5. Validates that at least one tool is present.

The method returns a `VersionMarkLoadResult` containing both the parsed configuration and
a list of `LintIssue` records. YAML parse errors are captured as error-level issues with
source location. This satisfies requirements `VersionMark-Configuration-YamlConfig`,
`VersionMark-Configuration-ValidateTools`, and `VersionMark-Configuration-ParseError`.

## ReadFromFile Method

`ReadFromFile` is a backward-compatibility wrapper that delegates to `Load`. It throws
`ArgumentException` if any error-level lint issues are present. Use `Load` directly when
you need access to lint issues.

## FindVersions Method

`FindVersions` accepts a list of tool names and a job ID. For each named tool it:

1. Looks up the `ToolConfig` (throws `ArgumentException` for unknown tools).
2. Calls `GetEffectiveCommand` and `GetEffectiveRegex` for the current OS.
3. Calls the private `RunCommand` helper to execute the command in a shell.
4. Calls the private `ExtractVersion` helper to apply the regex.
5. Stores the result in a `versions` dictionary.

The method returns a `VersionInfo` record. This satisfies requirements
`VersionMark-Capture-Command` and `VersionMark-Capture-MultipleTools`.

## RunCommand Helper

`RunCommand` runs the command through the OS shell (`cmd.exe /c` on Windows, `/bin/sh -c`
on other platforms) using `Process.Start` with redirected stdout and stderr. Output and
error streams are read asynchronously to prevent pipe-deadlock. A non-zero exit code
raises `InvalidOperationException`. This satisfies `VersionMark-Capture-Command`.

## ExtractVersion Helper

`ExtractVersion` compiles the regex with `Multiline | IgnoreCase` and a 1-second timeout,
matches against the command output, and returns the value of the named `version` capture
group. Missing match or missing group raises `InvalidOperationException`. This satisfies
`VersionMark-Capture-Command`.
