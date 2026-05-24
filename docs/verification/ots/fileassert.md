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
document types (requirements report, design document, verification document, user guide,
build notes, code quality report, and code review report). A passing CI run with all
FileAssert assertions provides functional evidence that FileAssert is operating correctly.

### Test Scenarios

**FileAssertSelfValidation**: The CI pipeline runs
`dotnet fileassert --validate --results artifacts/fileassert-self-validation.trx`, which
executes FileAssert's internal test suite. The TRX file must be produced and contain no
failed tests. This scenario is verified by `artifacts/fileassert-self-validation.trx`.

**FileAssertFunctionalEvidence**: The CI pipeline runs FileAssert assertions on each of
the seven generated document types (HTML and PDF). All assertions must pass, confirming
that FileAssert is evaluating file content correctly in the CI environment. This scenario
is verified by the FileAssert TRX results covering all seven document types.
