# ToolConfig Unit

## Overview

The `ToolConfig` record (`VersionMarkConfig.cs`) represents the configuration for a single
tool entry. It holds two dictionaries keyed by OS name:

| Dictionary | Key values                                      | Purpose                            |
|------------|-------------------------------------------------|------------------------------------|
| `Command`  | `""` (default), `"win"`, `"linux"`, `"macos"`   | Shell command to run               |
| `Regex`    | `""` (default), `"win"`, `"linux"`, `"macos"`   | Regex pattern with `version` group |

## OS-Specific Overrides

`GetEffectiveCommand` and `GetEffectiveRegex` resolve the active OS at runtime using
`RuntimeInformation.IsOSPlatform` and then look up the OS-specific key first, falling back
to the default (`""`) key. This satisfies requirements `VersionMark-Configuration-OsCommandOverride`
and `VersionMark-Configuration-OsRegexOverride`.

## YAML Parsing

`FromYamlNode` is an internal factory method that reads a `YamlMappingNode` and populates
the command and regex dictionaries. Known keys are `command`, `command-win`, `command-linux`,
`command-macos`, `regex`, `regex-win`, `regex-linux`, and `regex-macos`. Unknown keys are
silently ignored for forward-compatibility. Both a default `command` and a default `regex`
are required; their absence raises `ArgumentException`. This satisfies
`VersionMark-Configuration-ToolDefinition`.
