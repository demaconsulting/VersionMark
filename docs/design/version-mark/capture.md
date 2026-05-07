## Capture Subsystem

### Overview

The capture subsystem is responsible for persisting tool version information for the
current CI/CD job environment. It receives structured version results produced by the
configuration subsystem (for example, by `VersionMarkConfig.FindVersions`) and saves
them to a JSON file. The captured data is later consumed by the publish subsystem to
generate the version report.

The capture subsystem consists of a single unit: `VersionInfo`, which is the data transfer
record for captured version data.

The `VersionInfo` unit is described in full in the *VersionInfo Unit Design* section of this document.

### Cross-Subsystem Dependencies

- **`Cli.Context`** (Caller → Capture): Provides `JobId`, `OutputFile`, `ToolNames` to
  `RunCapture`
- **`Configuration.VersionMarkConfig`** (Caller → Capture): `FindVersions` produces the
  `VersionInfo` record saved by `RunCapture`
- **`Program.RunCapture`** (Caller → Capture): Orchestrates load-config → find-versions →
  save pipeline
- **`Program.RunPublish`** (Caller → Capture): Calls `VersionInfo.LoadFromFile` for each
  matched JSON file
