### MarkdownFormatter

#### Verification Approach

The `MarkdownFormatter` unit generates consolidated markdown version reports from a
collection of `VersionInfo` records. It sorts tools and job IDs alphabetically, collapses
uniform versions across jobs into a single line, and uses the configured heading depth for
section headers. Tests are in `Publishing/MarkdownFormatterTests.cs` and call
`MarkdownFormatter.Format` directly with constructed lists of `VersionInfo` objects. No
external mocks or file system access is required.

#### Test Environment

N/A - standard test environment. All tests run using `dotnet test` with no additional
environment setup required.

#### Acceptance Criteria

- All unit tests for `MarkdownFormatter` pass with zero failures across all supported OS
  and .NET version matrix combinations (Windows, Linux, macOS x .NET 8, .NET 9, .NET 10).
- Every requirement for the `MarkdownFormatter` unit is covered by at least one named
  test scenario.

#### Test Scenarios

**MarkdownFormatter_Format_SortsToolsAlphabetically**: Tools appear in
alphabetical order in the generated report. This scenario is tested by
`MarkdownFormatter_Format_SortsToolsAlphabetically`.

**MarkdownFormatter_Format_WithUniformVersions_ShowsVersionOnly**: When all jobs
report the same version for a tool, the version is shown without job IDs. This scenario is
tested by `MarkdownFormatter_Format_WithUniformVersions_ShowsVersionOnly`.

**MarkdownFormatter_Format_WithDifferentVersions_ShowsIndividualJobs**: When jobs
report different versions for a tool, each job ID and version is shown individually. This
scenario is tested by
`MarkdownFormatter_Format_WithDifferentVersions_ShowsIndividualJobs`.

**MarkdownFormatter_Format_WithCustomDepth_UsesCorrectHeadingLevel**: A custom
depth value produces the correct markdown heading level. This scenario is tested by
`MarkdownFormatter_Format_WithCustomDepth_UsesCorrectHeadingLevel`.

**MarkdownFormatter_Format_EmptyList_ProducesHeaderOnly**: An empty input list
produces a header with no tool entries. This scenario is tested by
`MarkdownFormatter_Format_EmptyList_ProducesHeaderOnly`.

**MarkdownFormatter_Format_SingleJob_SuppressesJobId**: A single job suppresses
the job ID and shows only the version. This scenario is tested by
`MarkdownFormatter_Format_SingleJob_SuppressesJobId`.

**MarkdownFormatter_Format_MixedVersions_HandlesCorrectly**: A mix of uniform and
differing versions across tools is handled correctly. This scenario is tested by
`MarkdownFormatter_Format_MixedVersions_HandlesCorrectly`.

**MarkdownFormatter_Format_SortsJobIdsAlphabetically**: Job IDs appear in
alphabetical order within each tool entry. This scenario is tested by
`MarkdownFormatter_Format_SortsJobIdsAlphabetically`.

**MarkdownFormatter_Format_WithSpecialCharacters_PreservesVersions**: Version
strings with special characters are preserved in the report. This scenario is tested by
`MarkdownFormatter_Format_WithSpecialCharacters_PreservesVersions`.

**MarkdownFormatter_Format_CaseInsensitiveSorting**: Alphabetical sorting of tools
and job IDs is case-insensitive. This scenario is tested by
`MarkdownFormatter_Format_CaseInsensitiveSorting`.

**MarkdownFormatter_Format_SortsVersionsAlphabetically**: Version strings within a
tool entry are sorted alphabetically. This scenario is tested by
`MarkdownFormatter_Format_SortsVersionsAlphabetically`.

**MarkdownFormatter_Format_WithZeroDepth_ThrowsArgumentOutOfRangeException**: A depth of
zero throws `ArgumentOutOfRangeException`. This scenario is tested by
`MarkdownFormatter_Format_WithZeroDepth_ThrowsArgumentOutOfRangeException`.

**MarkdownFormatter_Format_WithPartialToolCoverage_ShowsAllContributingTools**: Partial
tool coverage across jobs (where not every job reports every tool) shows all contributing
tools. This scenario is tested by
`MarkdownFormatter_Format_WithPartialToolCoverage_ShowsAllContributingTools`.
