# Capture Subsystem

## Overview

The capture subsystem is responsible for persisting tool version information for the
current CI/CD job environment. It receives structured version results produced by the
configuration subsystem (for example, by `VersionMarkConfig.FindVersions`) and saves
them to a JSON file. The captured data is later consumed by the publish subsystem to
generate the version report.

The capture subsystem consists of a single unit: `VersionInfo`, which is the data transfer
record for captured version data.

## VersionInfo Unit Design

`VersionInfo` is a positional record with two properties:

| Property   | Type                         | Description                                           |
|------------|------------------------------|-------------------------------------------------------|
| `JobId`    | `string`                     | Identifies the CI/CD job that captured these versions |
| `Versions` | `Dictionary<string, string>` | Maps tool names to their captured version strings     |

### Constructor

The record constructor accepts `JobId` and `Versions` as positional parameters.
These values are immutable once set. No validation is performed in the constructor
because `VersionInfo` is a pure data-transfer record; validation of the source data
occurs in the configuration and CLI layers.

### SaveToFile Method

`SaveToFile(string filePath)` serializes the record to indented JSON using
`JsonSerializer.Serialize` with `WriteIndented = true` and writes the result to the
specified path using UTF-8 encoding.

- **Success**: Creates or overwrites the file at `filePath` with JSON content.
- **Error contract**: Any exception other than `InvalidOperationException` is caught and
  re-thrown as `InvalidOperationException` with an explanatory message and the original
  exception as inner cause. This lets callers rely on a single exception type for
  file-write failures.

### LoadFromFile Method

`LoadFromFile(string filePath)` is the symmetric counterpart to `SaveToFile`. It:

1. Checks that the file exists; throws `ArgumentException` if not
   (message contains "not found").
2. Reads the file content as UTF-8.
3. Deserializes using `JsonSerializer.Deserialize<VersionInfo>`.
4. Validates the result is not null; throws `ArgumentException` if it is
   (message contains "deserialize").

`JsonException` from step 3 is caught and re-thrown as `ArgumentException` (message
contains "parse"). Any other non-`ArgumentException` error is wrapped similarly.

## JSON File Schema

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

Property names in JSON match the C# property names exactly because no
`JsonPropertyName` attributes are applied. The file is UTF-8 encoded with
indentation for human readability and version-control diff-friendliness.

## Error Handling Contract

| Situation                             | Exception type              | Message contains  |
|---------------------------------------|-----------------------------|-------------------|
| File not found (`LoadFromFile`)       | `ArgumentException`         | "not found"       |
| JSON deserialization returns null     | `ArgumentException`         | "deserialize"     |
| JSON is malformed (`LoadFromFile`)    | `ArgumentException`         | "parse"           |
| Other read failure (`LoadFromFile`)   | `ArgumentException`         | "Failed to read"  |
| Write failure (`SaveToFile`)          | `InvalidOperationException` | "Failed to save"  |

## Cross-Subsystem Dependencies

- **`Cli.Context`** (Caller → Capture): Provides `JobId`, `OutputFile`, `ToolNames` to
  `RunCapture`
- **`Configuration.VersionMarkConfig`** (Caller → Capture): `FindVersions` produces the
  `VersionInfo` record saved by `RunCapture`
- **`Program.RunCapture`** (Caller → Capture): Orchestrates load-config → find-versions →
  save pipeline
- **`Program.RunPublish`** (Caller → Capture): Calls `VersionInfo.LoadFromFile` for each
  matched JSON file
