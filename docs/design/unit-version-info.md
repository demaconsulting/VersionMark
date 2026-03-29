# VersionInfo Unit

## Overview

The `VersionInfo` record (`VersionInfo.cs`) is a positional record with two properties:

| Property   | Type                          | Description                                           |
|------------|-------------------------------|-------------------------------------------------------|
| `JobId`    | `string`                      | Identifies the CI/CD job that captured these versions |
| `Versions` | `Dictionary<string, string>`  | Maps tool names to their version strings              |

`VersionInfo` is the interface between the capture mode and the publish mode: capture
produces it by executing commands and saving to JSON; publish reads it back and passes it
to `MarkdownFormatter`.

## SaveToFile Method

`SaveToFile` serializes the record to indented JSON using `JsonSerializer.Serialize` with
`WriteIndented = true` and writes it to the specified path using UTF-8 encoding.
Non-`InvalidOperationException` errors are wrapped and re-thrown as
`InvalidOperationException` with context. This satisfies requirement
`VersionMark-Capture-JsonOutput`. The default output filename (`versionmark-<job-id>.json`)
is determined by the CLI layer and contributes to satisfying `VersionMark-Capture-DefaultOutput`.

## LoadFromFile Method

`LoadFromFile` is the symmetric counterpart to `SaveToFile`. It:

1. Checks that the file exists; throws `ArgumentException` if not.
2. Reads the file content as UTF-8.
3. Deserializes using `JsonSerializer.Deserialize<VersionInfo>`.
4. Validates the result is not null.

`JsonException` is caught and re-thrown as `ArgumentException`. Other
non-`ArgumentException` errors are wrapped similarly. This satisfies
`VersionMark-Publish-Consolidate` and `VersionMark-Publish-MultipleFiles`.

## JSON Schema

The JSON file produced by `SaveToFile` has this structure:

```json
{
  "JobId": "build-linux",
  "Versions": {
    "dotnet": "9.0.1",
    "node": "22.12.0"
  }
}
```

The property names in JSON match the C# property names exactly because no custom
`JsonPropertyName` attributes are applied. The file is UTF-8 encoded with indentation
for human readability and diff-friendliness in version control.
