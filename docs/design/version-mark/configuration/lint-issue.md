# LintIssue Unit

## Overview

`LintIssue.cs` contains the types used to surface validation issues found while loading a
`.versionmark.yaml` configuration file. It defines three public types: the `LintSeverity`
enumeration, the `LintIssue` record, and the `VersionMarkLoadResult` record.

## LintSeverity Enumeration

`LintSeverity` classifies the severity of a validation issue:

| Value     | Meaning                                                                    |
|-----------|----------------------------------------------------------------------------|
| `Warning` | Non-fatal advisory message; loading continues.                             |
| `Error`   | Fatal validation failure that prevents the configuration from being built. |

## LintIssue Record

`LintIssue` represents a single issue found during configuration loading. It carries:

| Property      | Type           | Description                                    |
|---------------|----------------|------------------------------------------------|
| `FilePath`    | `string`       | Path to the file containing the issue.         |
| `Line`        | `long`         | One-based line number of the issue.            |
| `Column`      | `long`         | One-based column number of the issue.          |
| `Severity`    | `LintSeverity` | Severity classification.                       |
| `Description` | `string`       | Human-readable description of the issue.       |

`ToString` formats the record as `"{FilePath}({Line},{Column}): {severity}: {Description}"`,
where `{severity}` is emitted in lowercase (`warning` or `error`), producing output in
the familiar `file(line,col): error: message` format understood by CI systems and editors.

## VersionMarkLoadResult Record

`VersionMarkLoadResult` is the return type of `VersionMarkConfig.Load`. It bundles two
properties:

| Property | Type                       | Description                                                                 |
|----------|----------------------------|-----------------------------------------------------------------------------|
| `Config` | `VersionMarkConfig?`       | The loaded configuration, or `null` when errors prevented building it.      |
| `Issues` | `IReadOnlyList<LintIssue>` | All validation issues; may contain warnings even when `Config` is non-null. |

The `ReportIssues` method writes all issues to a `Context`, routing `LintSeverity.Error`
issues to `context.WriteError` and `LintSeverity.Warning` issues to `context.WriteLine`.
