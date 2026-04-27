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
to the default (`""`) key. When no default (`""`) key is present either, an
`InvalidOperationException` is thrown. This satisfies requirements `VersionMark-ToolConfig-EffectiveCommand`
and `VersionMark-ToolConfig-EffectiveRegex`.

## YAML Parsing

Tool YAML parsing is performed by the private `VersionMarkConfig.ValidateTool` method.
It reads a `YamlMappingNode` and populates the command and regex dictionaries. Known keys
are `command`, `command-win`, `command-linux`, `command-macos`, `regex`, `regex-win`,
`regex-linux`, and `regex-macos`. Unknown keys produce a warning lint issue but do not
prevent loading. Both a default `command` and a default `regex` are required; their absence
produces an error lint issue. This satisfies `VersionMark-Configuration-ToolDefinition`.
