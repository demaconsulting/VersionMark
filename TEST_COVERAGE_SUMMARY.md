# Test Coverage Summary for Publish Feature

This document maps the newly added tests to the requirements they verify.

## Unit Tests Added

### ContextTests.cs (8 new tests)

1. **Context_Create_PublishFlag_SetsPublishTrue**
   - Requirements: PUB-001
   - Tests: --publish flag parsing sets Publish property to true

2. **Context_Create_ReportParameter_SetsReportFile**
   - Requirements: PUB-002
   - Tests: --report parameter parsing captures the output file path

3. **Context_Create_ReportDepthParameter_SetsReportDepth**
   - Requirements: PUB-003
   - Tests: --report-depth parameter parsing captures the depth value

4. **Context_Create_NoReportDepth_DefaultsToTwo**
   - Requirements: PUB-003
   - Tests: Default report-depth value is 2 when not specified

5. **Context_Create_GlobPatternsAfterSeparator_CapturesPatterns**
   - Requirements: PUB-005
   - Tests: Glob patterns after -- are captured in GlobPatterns array

6. **Context_Create_PublishWithoutReport_ParsesSuccessfully**
   - Requirements: PUB-004 (negative test)
   - Tests: --publish flag can be parsed without --report (validation happens in Program.Run)

7. **Context_Create_NoGlobPatterns_EmptyArray**
   - Requirements: PUB-005
   - Tests: GlobPatterns is empty array when not specified (default applied in Program.RunPublish)

8. **Context_Create_GlobPatternsAfterSeparator_CapturesPatterns**
   - Requirements: PUB-005
   - Tests: Multiple glob patterns are correctly parsed and stored

### ProgramTests.cs (6 new tests)

1. **Program_Run_WithPublishCommandWithoutReport_ReturnsError**
   - Requirements: PUB-004
   - Tests: Program exits with error when --report is missing

2. **Program_Run_WithPublishCommandNoMatchingFiles_ReturnsError**
   - Requirements: PUB-007
   - Tests: Program exits with error when no JSON files match glob patterns

3. **Program_Run_WithPublishCommandInvalidJson_ReturnsError**
   - Requirements: PUB-008
   - Tests: Program exits with error when JSON files cannot be parsed

4. **Program_Run_WithPublishCommand_GeneratesMarkdownReport**
   - Requirements: PUB-001, PUB-002, PUB-005, PUB-006, FMT-001
   - Tests: Valid JSON files are processed and markdown report is created

5. **Program_Run_WithPublishCommandCustomDepth_AdjustsHeadingLevels**
   - Requirements: PUB-003, FMT-005
   - Tests: Custom report depth is applied to generated markdown

## Integration Tests Added

### IntegrationTests.cs (6 new tests)

1. **VersionMark_PublishCommand_GeneratesMarkdownReport**
   - Requirements: PUB-001, PUB-002, FMT-001, FMT-002, FMT-003
   - Tests: End-to-end publish workflow with multiple JSON files
   - Verifies: Markdown structure, alphabetical sorting, uniform vs. differing version display

2. **VersionMark_PublishCommand_WithReportDepth_AdjustsHeadingLevels**
   - Requirements: PUB-003, FMT-005
   - Tests: --report-depth parameter controls markdown heading level

3. **VersionMark_PublishCommandWithoutReport_ReturnsError**
   - Requirements: PUB-004
   - Tests: Command fails when --report is missing

4. **VersionMark_PublishCommandWithNoMatchingFiles_ReturnsError**
   - Requirements: PUB-007
   - Tests: Command fails with clear error when no files found

5. **VersionMark_PublishCommandWithInvalidJson_ReturnsError**
   - Requirements: PUB-008
   - Tests: Command fails with clear error for malformed JSON

6. **VersionMark_PublishCommandWithCustomGlobPatterns_FiltersFiles**
   - Requirements: PUB-005
   - Tests: Custom glob patterns correctly filter input files

## Existing Tests (Already Complete)

### MarkdownFormatterTests.cs (11 tests)

All formatter requirements (FMT-001 through FMT-005) were already thoroughly tested:

1. **MarkdownFormatter_FormatVersions_SortsToolsAlphabetically** - FMT-001
2. **MarkdownFormatter_FormatVersions_WithUniformVersions_ShowsAllJobs** - FMT-002
3. **MarkdownFormatter_FormatVersions_WithDifferentVersions_ShowsIndividualJobs** - FMT-003, FMT-004
4. **MarkdownFormatter_FormatVersions_WithCustomDepth_UsesCorrectHeadingLevel** - FMT-005
5. **MarkdownFormatter_FormatVersions_EmptyList_ProducesHeaderOnly** - Edge case
6. **MarkdownFormatter_FormatVersions_SingleJob_ShowsAllJobs** - Edge case
7. **MarkdownFormatter_FormatVersions_MixedVersions_HandlesCorrectly** - FMT-002, FMT-003
8. **MarkdownFormatter_FormatVersions_SortsJobIdsAlphabetically** - FMT-004
9. **MarkdownFormatter_FormatVersions_WithSpecialCharacters_PreservesVersions** - Edge case
10. **MarkdownFormatter_FormatVersions_CaseInsensitiveSorting** - FMT-001
11. **MarkdownFormatter_FormatVersions_SortsVersionsAlphabetically** - FMT-003

## Requirements Coverage

All publish and formatter requirements are now covered by tests:

- **PUB-001**: ✅ Context_Create_PublishFlag_SetsPublishTrue,
  Program_Run_WithPublishCommand_GeneratesMarkdownReport,
  VersionMark_PublishCommand_GeneratesMarkdownReport
- **PUB-002**: ✅ Context_Create_ReportParameter_SetsReportFile,
  Program_Run_WithPublishCommand_GeneratesMarkdownReport,
  VersionMark_PublishCommand_GeneratesMarkdownReport
- **PUB-003**: ✅ Context_Create_ReportDepthParameter_SetsReportDepth,
  Context_Create_NoReportDepth_DefaultsToTwo,
  Program_Run_WithPublishCommandCustomDepth_AdjustsHeadingLevels,
  VersionMark_PublishCommand_WithReportDepth_AdjustsHeadingLevels
- **PUB-004**: ✅ Context_Create_PublishWithoutReport_ParsesSuccessfully,
  Program_Run_WithPublishCommandWithoutReport_ReturnsError,
  VersionMark_PublishCommandWithoutReport_ReturnsError
- **PUB-005**: ✅ Context_Create_GlobPatternsAfterSeparator_CapturesPatterns,
  Context_Create_NoGlobPatterns_EmptyArray,
  Program_Run_WithPublishCommand_GeneratesMarkdownReport,
  VersionMark_PublishCommandWithCustomGlobPatterns_FiltersFiles
- **PUB-006**: ✅ Program_Run_WithPublishCommand_GeneratesMarkdownReport,
  VersionMark_PublishCommand_GeneratesMarkdownReport
- **PUB-007**: ✅ Program_Run_WithPublishCommandNoMatchingFiles_ReturnsError,
  VersionMark_PublishCommandWithNoMatchingFiles_ReturnsError
- **PUB-008**: ✅ Program_Run_WithPublishCommandInvalidJson_ReturnsError,
  VersionMark_PublishCommandWithInvalidJson_ReturnsError
- **FMT-001**: ✅ MarkdownFormatter_FormatVersions_SortsToolsAlphabetically,
  MarkdownFormatter_FormatVersions_CaseInsensitiveSorting,
  Program_Run_WithPublishCommand_GeneratesMarkdownReport,
  VersionMark_PublishCommand_GeneratesMarkdownReport
- **FMT-002**: ✅ MarkdownFormatter_FormatVersions_WithUniformVersions_ShowsAllJobs,
  MarkdownFormatter_FormatVersions_MixedVersions_HandlesCorrectly,
  VersionMark_PublishCommand_GeneratesMarkdownReport
- **FMT-003**: ✅ MarkdownFormatter_FormatVersions_WithDifferentVersions_ShowsIndividualJobs,
  MarkdownFormatter_FormatVersions_MixedVersions_HandlesCorrectly,
  MarkdownFormatter_FormatVersions_SortsVersionsAlphabetically,
  VersionMark_PublishCommand_GeneratesMarkdownReport
- **FMT-004**: ✅ MarkdownFormatter_FormatVersions_WithDifferentVersions_ShowsIndividualJobs,
  MarkdownFormatter_FormatVersions_SortsJobIdsAlphabetically
- **FMT-005**: ✅ MarkdownFormatter_FormatVersions_WithCustomDepth_UsesCorrectHeadingLevel,
  Program_Run_WithPublishCommandCustomDepth_AdjustsHeadingLevels,
  VersionMark_PublishCommand_WithReportDepth_AdjustsHeadingLevels

## Test Organization

Tests follow the AAA (Arrange-Act-Assert) pattern with clear documentation:

- **Arrange**: Set up test conditions and data
- **Act**: Execute the behavior being tested
- **Assert**: Verify the results with clear comments documenting what is proved

Each test includes:

- Clear test name indicating what is being tested
- XML documentation comments explaining the test purpose
- Inline comments documenting what the assertions prove
- Links to requirements being tested
