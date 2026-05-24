## BuildMark

### Verification Approach

BuildMark is an OTS tool developed by DEMA Consulting that generates build notes
documenting which versions of tools were used during a build. VersionMark uses BuildMark
in its CI/CD pipeline to capture and publish build notes as part of the compliance
evidence package.

BuildMark is verified through two mechanisms. First, the CI pipeline runs
`dotnet buildmark --validate --results artifacts/buildmark-self-validation.trx`, which
executes BuildMark's internal test suite and writes results to a TRX file. Second, the CI
pipeline runs BuildMark to generate `docs/build_notes/generated/build_notes.md` from
GitHub Actions workflow metadata. A passing CI run producing this file confirms BuildMark
is operating correctly in the CI environment.

### Test Scenarios

**BuildMarkSelfValidation**: The CI pipeline runs
`dotnet buildmark --validate --results artifacts/buildmark-self-validation.trx`, which
executes BuildMark's internal test suite. The TRX file must be produced and contain no
failed tests. This scenario is verified by `artifacts/buildmark-self-validation.trx`.

**BuildMarkMarkdownReportGeneration**: The CI pipeline runs BuildMark to generate
`docs/build_notes/generated/build_notes.md` from GitHub Actions workflow metadata. The
markdown build notes document must be produced and incorporated into the build notes
document collection. This scenario is verified by
`docs/build_notes/generated/build_notes.md`.
