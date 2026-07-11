## Capture

![Capture Structure](CaptureView.svg)

### Overview

The Capture subsystem is responsible for persisting tool version information for the
current CI/CD job environment. It receives structured version results produced by the
Configuration subsystem and saves them to a JSON file. The captured data is later consumed
by the Publishing subsystem to generate the version report. It consists of a single unit:
`VersionInfo`, which is the data transfer record for captured version data.

### Interfaces

**`VersionInfo.SaveToFile(string path)`**: Serializes the record to JSON and writes it to
disk.

- *Type*: In-process .NET public API (instance method).
- *Role*: Provider.
- *Contract*: Serializes the record to indented JSON using UTF-8 encoding and writes it to
  `path`. Non-`InvalidOperationException` errors are caught and re-thrown as
  `InvalidOperationException` with context.
- *Constraints*: Overwrites the file if it already exists.

**`VersionInfo.LoadFromFile(string path)`**: Reads and deserializes a JSON file into a
`VersionInfo` record.

- *Type*: In-process .NET public API (static method).
- *Role*: Provider.
- *Contract*: Checks file existence, reads the file as UTF-8, deserializes via
  `JsonSerializer.Deserialize<VersionInfo>`, and validates the result is not null.
- *Constraints*: Throws `ArgumentException` if the file does not exist, the JSON is
  invalid, or deserialization produces a null result.

**`VersionInfo.JobId`**: The CI/CD job identifier that produced this record.

- *Type*: In-process .NET public API (record property).
- *Role*: Provider.
- *Contract*: String value identifying the CI/CD job; set during capture mode.
- *Constraints*: Immutable after construction.

**`VersionInfo.Versions`**: Maps tool names to their captured version strings.

- *Type*: In-process .NET public API (record property).
- *Role*: Provider.
- *Contract*: `Dictionary<string, string>` mapping tool names to version strings as
  extracted by the Configuration subsystem.
- *Constraints*: Immutable after construction.

### Design

The Capture subsystem consists of the single `VersionInfo` record, which serves as the
data transfer object between the capture and publish operational modes:

- In **capture mode**, `VersionMarkConfig.FindVersions` produces a `VersionInfo` record.
  `Program.RunCapture` calls `VersionInfo.SaveToFile` to persist it as a JSON artifact.
- In **publish mode**, `Program.RunPublish` calls `VersionInfo.LoadFromFile` for each JSON
  artifact discovered by `GlobMatcher`. The resulting records are passed to
  `MarkdownFormatter.Format` for report generation.

The `VersionInfo` record has no dependencies on other VersionMark subsystems; it depends
only on `System.Text.Json` from the .NET BCL.
