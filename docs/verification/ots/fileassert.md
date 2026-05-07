## FileAssert Verification

### Overview

FileAssert is an OTS tool developed by DEMA Consulting that asserts the content of
generated files against expected patterns. VersionMark uses FileAssert in its CI/CD
pipeline to verify that generated documents (HTML, PDF, and other output types) are
produced correctly and contain the expected content.

### Verification Approach

FileAssert is verified through two mechanisms:

1. **Self-validation**: The CI pipeline runs
   `dotnet fileassert --validate --results artifacts/fileassert-self-validation.trx`,
   which executes FileAssert's internal test suite and writes results to a TRX file.

2. **Functional evidence**: FileAssert is exercised in CI by asserting the content
   of each of the seven generated document types (requirements report, design document,
   verification document, and others). A passing CI run with FileAssert assertions
   provides functional evidence that FileAssert is operating correctly.

### Requirements Coverage

The following list maps FileAssert requirements to verification evidence:

- **`VersionMark-OTS-FileAssert`**: `artifacts/fileassert-self-validation.trx`
  (FileAssert self-validation passing in CI) and FileAssert assertions on generated documents
