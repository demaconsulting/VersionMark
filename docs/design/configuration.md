# Configuration

## Overview

The configuration layer reads and interprets the `.versionmark.yaml` file that defines
which tools to capture and how to extract their versions. It consists of two classes:
`ToolConfig` (per-tool settings) and `VersionMarkConfig` (the top-level configuration
container).

## ToolConfig Record

The `ToolConfig` record (`VersionMarkConfig.cs`) represents the configuration for a single
tool entry. It holds two dictionaries keyed by OS name:

| Dictionary | Key values                                      | Purpose                            |
|------------|-------------------------------------------------|------------------------------------|
| `Command`  | `""` (default), `"win"`, `"linux"`, `"macos"`   | Shell command to run               |
| `Regex`    | `""` (default), `"win"`, `"linux"`, `"macos"`   | Regex pattern with `version` group |

### OS-Specific Overrides

`GetEffectiveCommand` and `GetEffectiveRegex` resolve the active OS at runtime using
`RuntimeInformation.IsOSPlatform` and then look up the OS-specific key first, falling back
to the default (`""`) key. This satisfies requirements `VersionMark-Cfg-OsCommandOverride`
and `VersionMark-Cfg-OsRegexOverride`.

### YAML Parsing

`FromYamlNode` is an internal factory method that reads a `YamlMappingNode` and populates
the command and regex dictionaries. Known keys are `command`, `command-win`, `command-linux`,
`command-macos`, `regex`, `regex-win`, `regex-linux`, and `regex-macos`. Unknown keys are
silently ignored for forward-compatibility. Both a default `command` and a default `regex`
are required; their absence raises `ArgumentException`. This satisfies
`VersionMark-Cfg-ToolDefinition`.

## VersionMarkConfig Record

The `VersionMarkConfig` record holds a `Dictionary<string, ToolConfig>` mapping tool names
to their configurations.

### ReadFromFile Method

`ReadFromFile` is the public entry point for loading configuration. It:

1. Checks that the file exists; throws `ArgumentException` if not.
2. Parses the YAML stream and validates the root node is a mapping.
3. Locates the `tools` mapping key; throws `ArgumentException` if absent.
4. Iterates tool entries, calling `ToolConfig.FromYamlNode` for each.
5. Validates that at least one tool is present.

YAML parse errors are caught and re-thrown as `ArgumentException` with context. All other
non-`ArgumentException` errors are wrapped similarly. This satisfies requirements
`VersionMark-Cfg-YamlConfig`, `VersionMark-Cfg-ValidateTools`, and
`VersionMark-Cfg-ParseError`.

### FindVersions Method

`FindVersions` accepts a list of tool names and a job ID. For each named tool it:

1. Looks up the `ToolConfig` (throws `ArgumentException` for unknown tools).
2. Calls `GetEffectiveCommand` and `GetEffectiveRegex` for the current OS.
3. Calls the private `RunCommand` helper to execute the command in a shell.
4. Calls the private `ExtractVersion` helper to apply the regex.
5. Stores the result in a `versions` dictionary.

The method returns a `VersionInfo` record. This satisfies requirements
`VersionMark-Cap-Command` and `VersionMark-Cap-MultipleTools`.

### RunCommand Helper

`RunCommand` runs the command through the OS shell (`cmd.exe /c` on Windows, `/bin/sh -c`
on other platforms) using `Process.Start` with redirected stdout and stderr. Output and
error streams are read asynchronously to prevent pipe-deadlock. A non-zero exit code
raises `InvalidOperationException`. This satisfies `VersionMark-Cap-Command`.

### ExtractVersion Helper

`ExtractVersion` compiles the regex with `Multiline | IgnoreCase` and a 1-second timeout,
matches against the command output, and returns the value of the named `version` capture
group. Missing match or missing group raises `InvalidOperationException`. This satisfies
`VersionMark-Cap-Command`.
