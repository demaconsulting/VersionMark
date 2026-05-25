## FileAssert

### Verification Approach

FileAssert is an OTS tool developed by DEMA Consulting that asserts the content of
generated files against expected patterns. VersionMark uses FileAssert in its CI/CD
pipeline to verify that generated documents (HTML, PDF, and other output types) are
produced correctly and contain the expected content.

FileAssert is verified through two mechanisms. First, the CI pipeline runs
`dotnet fileassert --validate --results artifacts/fileassert-self-validation.trx`, which
executes FileAssert's internal test suite and writes results to a TRX file. Second,
FileAssert is exercised in CI by asserting the content of each of the seven generated
document types (build notes, code quality report, code review plan, code review report,
design document, verification document, and user guide). A passing CI run with all
FileAssert assertions provides functional evidence that FileAssert is operating correctly.

### Test Environment

N/A — FileAssert is an OTS tool verified through the GitHub Actions CI pipeline. No
additional test environment configuration is required beyond a successful CI workflow run
with the FileAssert tool installed and the document artifacts produced by Pandoc and
WeasyPrint.

### Acceptance Criteria

The self-validation TRX (`artifacts/fileassert-self-validation.trx`) must be produced and
contain zero failed tests. All seven document-assertion TRX files
(`artifacts/fileassert-build-notes.trx`, `artifacts/fileassert-code-quality.trx`,
`artifacts/fileassert-code-review.trx`, `artifacts/fileassert-design.trx`,
`artifacts/fileassert-verification.trx`, `artifacts/fileassert-user-guide.trx`, and
`artifacts/fileassert-requirements.trx`) must be produced and contain zero failed tests.

### Test Scenarios

**FileAssertSelfValidation**: The CI pipeline runs
`dotnet fileassert --validate --results artifacts/fileassert-self-validation.trx`, which
executes FileAssert's internal test suite. The TRX file must be produced and contain no
failed tests. This scenario is verified by `artifacts/fileassert-self-validation.trx`.
This scenario corresponds to test IDs `FileAssert_VersionDisplay` and
`FileAssert_HelpDisplay` in the requirements traceability matrix.

**FileAssertFunctionalEvidence**: The CI pipeline runs FileAssert assertions on each of
the seven generated document types (HTML and PDF). All assertions must pass, confirming
that FileAssert is evaluating file content correctly in the CI environment. This scenario
is verified by `artifacts/fileassert-build-notes.trx` (tests `Pandoc_BuildNotesHtml` and
`WeasyPrint_BuildNotesPdf`), `artifacts/fileassert-code-quality.trx` (tests
`Pandoc_CodeQualityHtml` and `WeasyPrint_CodeQualityPdf`),
`artifacts/fileassert-code-review.trx` (tests `Pandoc_ReviewPlanHtml`,
`WeasyPrint_ReviewPlanPdf`, `Pandoc_ReviewReportHtml`, and `WeasyPrint_ReviewReportPdf`),
`artifacts/fileassert-design.trx` (tests `Pandoc_DesignHtml` and `WeasyPrint_DesignPdf`),
`artifacts/fileassert-verification.trx` (tests `Pandoc_VerificationHtml` and
`WeasyPrint_VerificationPdf`), and `artifacts/fileassert-user-guide.trx` (tests
`Pandoc_UserGuideHtml` and `WeasyPrint_UserGuidePdf`).
