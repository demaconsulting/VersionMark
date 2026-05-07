### MarkdownFormatter Unit Verification

#### Overview

The `MarkdownFormatter` unit generates consolidated markdown version reports from a
collection of `VersionInfo` records. It sorts tools and job IDs alphabetically, collapses
uniform versions across jobs into a single line, and uses the configured heading depth for
section headers. Tests are in `Publishing/MarkdownFormatterTests.cs`.

#### Test Scenarios

The following test scenarios verify `MarkdownFormatter`:

- **`MarkdownFormatter_FormatVersions_SortsToolsAlphabetically`**: Tools appear in alphabetical order in the report.
- **`MarkdownFormatter_FormatVersions_WithUniformVersions_ShowsVersionOnly`**:
  Same version across all jobs shows version without job IDs.
- **`MarkdownFormatter_FormatVersions_WithDifferentVersions_ShowsIndividualJobs`**:
  Different versions across jobs show each job ID and version.
- **`MarkdownFormatter_FormatVersions_WithCustomDepth_UsesCorrectHeadingLevel`**:
  Custom depth produces the correct Markdown heading level.
- **`MarkdownFormatter_FormatVersions_EmptyList_ProducesHeaderOnly`**: Empty input list produces a header with no tool entries.
- **`MarkdownFormatter_FormatVersions_SingleJob_ShowsAllJobs`**: Single job shows all version entries.
- **`MarkdownFormatter_FormatVersions_MixedVersions_HandlesCorrectly`**:
  Mix of uniform and differing versions is handled correctly.
- **`MarkdownFormatter_FormatVersions_SortsJobIdsAlphabetically`**: Job IDs appear in alphabetical order within a tool entry.
- **`MarkdownFormatter_FormatVersions_WithSpecialCharacters_PreservesVersions`**:
  Version strings with special characters are preserved.
- **`MarkdownFormatter_FormatVersions_CaseInsensitiveSorting`**: Alphabetical sorting is case-insensitive.
- **`MarkdownFormatter_FormatVersions_SortsVersionsAlphabetically`**:
  Version strings are sorted alphabetically within a tool entry.
- **`MarkdownFormatter_Format_WithZeroDepth_ThrowsArgumentOutOfRangeException`**:
  A depth of zero throws ArgumentOutOfRangeException.
- **`MarkdownFormatter_Format_WithPartialToolCoverage_ShowsAllContributingTools`**:
  Partial tool coverage across jobs shows all contributing tools.

#### Dependencies

No external mocks or file system access is required. Tests call `MarkdownFormatter.Format`
directly with constructed lists of `VersionInfo` objects.

#### Requirements Coverage

The following list maps `MarkdownFormatter` unit requirements to test scenarios:

- **`VersionMark-Formatter-Structure`**: `MarkdownFormatter_FormatVersions_SortsToolsAlphabetically`,
  `MarkdownFormatter_FormatVersions_EmptyList_ProducesHeaderOnly`,
  `MarkdownFormatter_FormatVersions_SortsJobIdsAlphabetically`,
  `MarkdownFormatter_FormatVersions_CaseInsensitiveSorting`,
  `MarkdownFormatter_FormatVersions_SortsVersionsAlphabetically`,
  `MarkdownFormatter_Format_WithPartialToolCoverage_ShowsAllContributingTools`
- **`VersionMark-Formatter-JobId`**: `MarkdownFormatter_FormatVersions_WithUniformVersions_ShowsVersionOnly`,
  `MarkdownFormatter_FormatVersions_MixedVersions_HandlesCorrectly`
- **`VersionMark-Formatter-Versions`**: `MarkdownFormatter_FormatVersions_WithDifferentVersions_ShowsIndividualJobs`,
  `MarkdownFormatter_FormatVersions_SingleJob_ShowsAllJobs`,
  `MarkdownFormatter_FormatVersions_MixedVersions_HandlesCorrectly`,
  `MarkdownFormatter_FormatVersions_WithSpecialCharacters_PreservesVersions`
- **`VersionMark-Formatter-MarkdownList`**: `MarkdownFormatter_FormatVersions_WithDifferentVersions_ShowsIndividualJobs`
- **`VersionMark-Formatter-MarkdownConsolidation`**:
  `MarkdownFormatter_FormatVersions_WithCustomDepth_UsesCorrectHeadingLevel`,
  `MarkdownFormatter_Format_WithZeroDepth_ThrowsArgumentOutOfRangeException`
