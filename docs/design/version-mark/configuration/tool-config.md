### ToolConfig Unit

#### Overview

The `ToolConfig` record (`VersionMarkConfig.cs`) represents the configuration for a single
tool entry. It holds two dictionaries keyed by OS name:

| Dictionary | Key values                                      | Purpose                            |
|------------|-------------------------------------------------|------------------------------------|
| `Command`  | `""` (default), `"win"`, `"linux"`, `"macos"`   | Shell command to run               |
| `Regex`    | `""` (default), `"win"`, `"linux"`, `"macos"`   | Regex pattern with `version` group |

#### OS-Specific Overrides

`GetEffectiveCommand(string os)` and `GetEffectiveRegex(string os)` both take a concrete,
already-resolved OS string — they do not accept `null` and do not call `GetCurrentOs`
internally. The caller is responsible for resolving the OS once (e.g. `os ?? ToolConfig.GetCurrentOs()`
in `FindVersions`) and passing the resulting string. Each method looks up the OS-specific key
first, falling back to the default (`""`) key. When no default (`""`) key is present either, an
`InvalidOperationException` is thrown. This satisfies requirements `VersionMark-ToolConfig-EffectiveCommand`
and `VersionMark-ToolConfig-EffectiveRegex`.

#### YAML Parsing

Tool YAML parsing is performed by the private `VersionMarkConfig.ValidateTool` method.
It reads a `YamlMappingNode` and populates the command and regex dictionaries. Known keys
are `command`, `command-win`, `command-linux`, `command-macos`, `regex`, `regex-win`,
`regex-linux`, and `regex-macos`. Unknown keys produce a warning lint issue but do not
prevent loading. At least one `command` (either the default `command` or an OS-specific
`command-win`/`command-linux`/`command-macos`) and at least one `regex` (either the default
`regex` or an OS-specific `regex-win`/`regex-linux`/`regex-macos`) are required; their
complete absence produces an error lint issue. This satisfies `VersionMark-Configuration-ToolDefinition`.
