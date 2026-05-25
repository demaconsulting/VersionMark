## BuildMark

### Verification Approach

BuildMark is an OTS tool developed by DEMA Consulting that captures GitHub Actions
workflow run metadata (run number, commit SHA, workflow name, and related details) and
renders it as a markdown build-notes document. VersionMark uses BuildMark in its CI/CD
pipeline to capture and publish build notes as part of the compliance evidence package.

BuildMark is verified through two mechanisms. First, the CI pipeline runs
`dotnet buildmark --validate --results artifacts/buildmark-self-validation.trx`, which
executes BuildMark's internal test suite and writes results to a TRX file. Second, the CI
pipeline runs BuildMark to generate `docs/build_notes/generated/build_notes.md` from
GitHub Actions workflow metadata. A passing CI run producing this file confirms BuildMark
is operating correctly in the CI environment.

### Test Environment

N/A — BuildMark is an OTS tool verified through the GitHub Actions CI pipeline. No
additional test environment configuration is required beyond a successful CI workflow run
with the BuildMark tool installed.

### Acceptance Criteria

The self-validation TRX (`artifacts/buildmark-self-validation.trx`) must be produced and
contain zero failed tests. The generated build notes document
(`docs/build_notes/generated/build_notes.md`) must be produced and incorporated into the
build notes document collection.

### Test Scenarios

**BuildMarkSelfValidation**: The CI pipeline runs
`dotnet buildmark --validate --results artifacts/buildmark-self-validation.trx`, which
executes BuildMark's internal test suite. The TRX file must be produced and contain no
failed tests. This scenario is verified by `artifacts/buildmark-self-validation.trx`.

**BuildMark_MarkdownReportGeneration**: The CI pipeline runs BuildMark to generate
`docs/build_notes/generated/build_notes.md` from GitHub Actions workflow metadata. The
markdown build notes document must be produced and incorporated into the build notes
document collection. This scenario is verified by
`docs/build_notes/generated/build_notes.md`.
