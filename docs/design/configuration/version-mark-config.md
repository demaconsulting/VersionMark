# VersionMarkConfig Unit

## Overview

The `VersionMarkConfig` record holds a `Dictionary<string, ToolConfig>` mapping tool names
to their configurations. It is the top-level entry point for loading configuration from
the `.versionmark.yaml` file.

## ReadFromFile Method

`ReadFromFile` is the public entry point for loading configuration. It:

1. Checks that the file exists; throws `ArgumentException` if not.
2. Parses the YAML stream and validates the root node is a mapping.
3. Locates the `tools` mapping key; throws `ArgumentException` if absent.
4. Iterates tool entries, calling `ToolConfig.FromYamlNode` for each.
5. Validates that at least one tool is present.

YAML parse errors are caught and re-thrown as `ArgumentException` with context. All other
non-`ArgumentException` errors are wrapped similarly. This satisfies requirements
`VersionMark-Configuration-YamlConfig`, `VersionMark-Configuration-ValidateTools`, and
`VersionMark-Configuration-ParseError`.

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
