### VersionInfo

#### Purpose

`VersionInfo` is the data transfer record between the capture and publish operational modes.
In capture mode, it is produced by `VersionMarkConfig.FindVersions` and saved as a JSON
artifact by `VersionInfo.SaveToFile`. In publish mode, it is loaded from those artifacts
by `VersionInfo.LoadFromFile` and passed to `MarkdownFormatter.Format` for report
generation.

#### Data Model

| Property   | Type                         | Description                                           |
|------------|------------------------------|-------------------------------------------------------|
| `JobId`    | `string`                     | Identifies the CI/CD job that captured these versions |
| `Versions` | `Dictionary<string, string>` | Maps tool names to their captured version strings     |

JSON representation produced by `SaveToFile`:

```json
{
  "JobId": "build-linux",
  "Versions": {
    "dotnet": "9.0.1",
    "node": "22.12.0"
  }
}
```

Property names in JSON match C# property names exactly because no `JsonPropertyName`
attributes are applied. Files are UTF-8 encoded with indentation for human readability
and diff-friendliness.

#### Key Methods

**`SaveToFile(string path)`** — Serializes the record to indented JSON using
`JsonSerializer.Serialize` with `WriteIndented = true` and writes it to `path` using UTF-8
encoding. Overwrites the file if it already exists.

**`LoadFromFile(string path)` (static)** — Symmetric counterpart to `SaveToFile`. Checks
file existence, reads the file as UTF-8, deserializes via
`JsonSerializer.Deserialize<VersionInfo>`, and validates the result is not null. Returns
the deserialized `VersionInfo` record.

#### Error Handling

| Condition                         | Method          | Behavior                                         |
|-----------------------------------|-----------------|--------------------------------------------------|
| File does not exist               | `LoadFromFile`  | `ArgumentException` thrown                       |
| JSON is invalid                   | `LoadFromFile`  | `JsonException` caught, re-thrown as `ArgumentException` |
| Deserialization returns null      | `LoadFromFile`  | `ArgumentException` thrown                       |
| Other non-`ArgumentException` error | `LoadFromFile` | Wrapped and re-thrown as `ArgumentException` with context |
| Other non-`InvalidOperationException` error | `SaveToFile` | Wrapped and re-thrown as `InvalidOperationException` with context |

#### Dependencies

- `System.Text.Json` (BCL) — JSON serialization and deserialization.

#### Callers

- `VersionMarkConfig.FindVersions` — constructs a `VersionInfo` record and returns it.
- `Program.RunCapture` — calls `VersionInfo.SaveToFile` to persist the capture artifact.
- `Program.RunPublish` — calls `VersionInfo.LoadFromFile` for each JSON artifact.
- `Validation.RunPublishTest` — constructs `VersionInfo` records and writes them as JSON
  for the self-validation publish test.
