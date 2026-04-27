# Publishing Subsystem

## Overview

The publish subsystem is responsible for generating a human-readable markdown version
report from captured JSON files. It reads the version data produced by the capture
subsystem and consolidates identical versions across jobs, flagging any conflicts.

The publish subsystem consists of a single unit: `MarkdownFormatter`, which converts
a collection of `VersionInfo` records into a markdown string.

## MarkdownFormatter.Format Interface

`MarkdownFormatter.Format` is an `internal static` method with the signature:

```csharp
public static string Format(IEnumerable<VersionInfo> versionInfos, int reportDepth = 2)
```

| Parameter      | Type                       | Description                                             |
|----------------|----------------------------|---------------------------------------------------------|
| `versionInfos` | `IEnumerable<VersionInfo>` | The captured version records to include in the report   |
| `reportDepth`  | `int`                      | Heading depth for the section title (default: 2)        |

**Returns**: A markdown-formatted string ready to be written to the report file.

`reportDepth` must be greater than zero. A value of `0` or less causes
`ArgumentOutOfRangeException` to be thrown.

## Normal-Operation Walkthrough

The `--publish` command follows this pipeline in `Program.RunPublish`:

1. Validate that `--report` was specified; report an error and return if not.
2. Resolve glob patterns (default `versionmark-*.json` when none supplied).
3. Scan the current directory for files matching the glob patterns using
   `Microsoft.Extensions.FileSystemGlobbing.Matcher`.
4. If no files match, write an error message via `context.WriteError` and return.
5. Call `VersionInfo.LoadFromFile` for each matched file to deserialize the JSON.
6. Pass the resulting `IEnumerable<VersionInfo>` to `MarkdownFormatter.Format`
   together with `context.ReportDepth`.
7. Write the returned markdown string to the file specified by `--report`.

## Error Handling

- **`--report` not specified**: `context.WriteError` with mention of `--report`; exit 1
- **No files match the glob patterns**: `context.WriteError` listing the patterns; exit 1
- **`VersionInfo.LoadFromFile` throws**: `context.WriteError` with the exception message;
  exit 1
- **`reportDepth <= 0`**: `ArgumentOutOfRangeException` thrown by
  `MarkdownFormatter.Format`

## reportDepth Configuration and Conflict-Display Logic

`context.ReportDepth` is populated from the `--report-depth` CLI argument
(default: the value of `--depth`, which itself defaults to `1`).

When different jobs report different versions for the same tool, `MarkdownFormatter`
displays one bullet per distinct version, with the contributing job IDs listed in
parentheses after the version string. For example:

```markdown
- **dotnet**: 8.0.0 (job-linux, job-windows)
- **dotnet**: 9.0.0 (job-preview)
```

When all jobs report the same version the job IDs are suppressed:

```markdown
- **dotnet**: 8.0.0
```
