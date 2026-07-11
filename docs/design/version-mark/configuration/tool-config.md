### ToolConfig

![Configuration Structure](ConfigurationView.svg)

#### Purpose

`ToolConfig` (`VersionMarkConfig.cs`) represents the configuration for a single tool
entry in a `.versionmark.yaml` file. It holds per-OS command and regex dictionaries,
and provides helper methods to resolve the effective command and regex for a given
operating system, applying OS-specific overrides where present and falling back to the
default entry.

#### Data Model

| Dictionary | Key values                                     | Purpose                             |
|------------|------------------------------------------------|-------------------------------------|
| `Command`  | `""` (default), `"win"`, `"linux"`, `"macos"` | Shell command to execute            |
| `Regex`    | `""` (default), `"win"`, `"linux"`, `"macos"` | Regex pattern with `version` group  |

Both dictionaries are populated by `VersionMarkConfig.ValidateTool` during YAML parsing.
The empty-string key `""` holds the default (OS-independent) value.

#### Key Methods

**`GetEffectiveCommand(string os)`** — Returns the command for the specified OS string.
Looks up the OS-specific key first; if absent, falls back to the default `""` key. Throws
`InvalidOperationException` if neither key is present.

**`GetEffectiveRegex(string os)`** — Identical lookup logic to `GetEffectiveCommand`, but
applied to the `Regex` dictionary.

**`GetCurrentOs()` (static)** — Returns the lowercase OS identifier string: `"win"` on
Windows, `"linux"` on Linux, `"macos"` on macOS. Used by callers that need to resolve the
OS once before calling `GetEffectiveCommand` / `GetEffectiveRegex`.

#### Error Handling

| Condition                                  | Behavior                                    |
|--------------------------------------------|---------------------------------------------|
| No matching OS key and no default `""` key | `InvalidOperationException` thrown          |
| No `command` field (`command`, `command-win`, `command-linux`, or `command-macos`) is defined | Error `LintIssue` added by `ValidateTool`   |
| No `regex` field (`regex`, `regex-win`, `regex-linux`, or `regex-macos`) is defined           | Error `LintIssue` added by `ValidateTool`   |
| Unknown YAML key for a tool entry          | Warning `LintIssue` added by `ValidateTool` |
| Empty value for a known YAML key           | Error `LintIssue` added by `ValidateTool`   |

#### Dependencies

- `System.Runtime.InteropServices.RuntimeInformation` (BCL) — OS detection in
  `GetCurrentOs`.

#### Callers

- `VersionMarkConfig.ValidateTool` — constructs `ToolConfig` instances from parsed YAML.
- `VersionMarkConfig.FindVersions` — calls `GetEffectiveCommand` and `GetEffectiveRegex`
  with the already-resolved OS string.
