## SarifMark

### Verification Approach

SarifMark is an OTS SARIF report tool developed by DEMA Consulting. It converts SARIF
(Static Analysis Results Interchange Format) files produced by code analysis tools into
human-readable markdown reports. VersionMark uses SarifMark in its CI/CD pipeline to
generate code quality reports from static analysis output.

SarifMark is verified through two mechanisms. First, the CI pipeline runs
`dotnet sarifmark --validate --results artifacts/sarifmark-self-validation.trx`, which
executes SarifMark's internal test suite and writes results to a TRX file. Second, the CI
pipeline passes the SARIF file produced by the CodeQL code scanning step to SarifMark,
which generates `docs/code_quality/generated/quality.md`. A passing CI run producing this
report confirms SarifMark is reading SARIF correctly and generating markdown output.

### Test Environment

N/A — SarifMark is an OTS tool verified through the GitHub Actions CI pipeline. No
additional test environment configuration is required beyond a successful CI workflow run
with the SarifMark tool installed.

### Acceptance Criteria

The self-validation TRX (`artifacts/sarifmark-self-validation.trx`) must be produced and
contain zero failed tests. The generated quality report
(`docs/code_quality/generated/quality.md`) must be produced and incorporated into the
code quality document collection.

### Test Scenarios

**SarifMarkSelfValidation**: The CI pipeline runs
`dotnet sarifmark --validate --results artifacts/sarifmark-self-validation.trx`, which
executes SarifMark's internal test suite. The TRX file must be produced and contain no
failed tests. This scenario is verified by `artifacts/sarifmark-self-validation.trx`.

**SarifMark_SarifReading**: The CI pipeline runs SarifMark against the CodeQL SARIF output
file. The tool must read the SARIF without error and extract all findings. This scenario
is verified by the successful generation of `docs/code_quality/generated/quality.md`.

**SarifMark_MarkdownReportGeneration**: The CI pipeline runs SarifMark to generate
`docs/code_quality/generated/quality.md` from the CodeQL SARIF file. The markdown quality
report must be produced and incorporated into the code quality document collection. This
scenario is verified by `docs/code_quality/generated/quality.md`.
