### LintIssue

#### Purpose

`LintIssue.cs` defines the types used to surface validation issues found while loading a
`.versionmark.yaml` configuration file. It provides `LintSeverity` (severity
classification), `LintIssue` (a single located issue), and `VersionMarkLoadResult` (the
return value of `VersionMarkConfig.Load` bundling the loaded configuration with all issues
found).

#### Data Model

**`LintSeverity` enumeration**

| Value     | Meaning                                                                    |
|-----------|----------------------------------------------------------------------------|
| `Warning` | Non-fatal advisory message; loading continues.                             |
| `Error`   | Fatal validation failure that prevents the configuration from being used.  |

**`LintIssue` record**

| Property      | Type           | Description                                    |
|---------------|----------------|------------------------------------------------|
| `FilePath`    | `string`       | Path to the file containing the issue.         |
| `Line`        | `long`         | One-based line number of the issue.            |
| `Column`      | `long`         | One-based column number of the issue.          |
| `Severity`    | `LintSeverity` | Severity classification.                       |
| `Description` | `string`       | Human-readable description of the issue.       |

`ToString` formats the record as `"{FilePath}({Line},{Column}): {severity}: {Description}"`
where `{severity}` is lowercase (`warning` or `error`), producing output in the familiar
`file(line,col): level: message` format understood by CI systems and editors.

**`VersionMarkLoadResult` record**

| Property | Type                       | Description                                                                |
|----------|----------------------------|----------------------------------------------------------------------------|
| `Config` | `VersionMarkConfig?`       | Loaded configuration; `null` when any error-level issues were found.       |
| `Issues` | `IReadOnlyList<LintIssue>` | All validation issues; may contain warnings even when `Config` is non-null. |

#### Key Methods

**`VersionMarkLoadResult.ReportIssues(Context context)` (internal)** — Iterates all issues
and routes each to `context.WriteError` (for `Error` severity) or `context.WriteLine` (for
`Warning` severity).

#### Error Handling

These types are primarily value-carrying data records with minimal internal error handling.
All error accumulation is performed by `VersionMarkConfig.Load`, which populates the `Issues`
list. Callers inspect the returned `VersionMarkLoadResult` to determine whether the
configuration is usable.

`VersionMarkLoadResult.ReportIssues` guards against a null `context` argument by calling
`ArgumentNullException.ThrowIfNull(context)` before iterating issues.

#### Dependencies

- `Context` (Cli subsystem) — used by `ReportIssues` to route issue output.

#### Callers

- `VersionMarkConfig.Load` — creates `LintIssue` records and returns them in a
  `VersionMarkLoadResult`.
- `Program.RunCapture` and `Program.RunLint` — call `result.ReportIssues` to write
  discovered issues to the context output.
