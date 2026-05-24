## ReqStream

### Verification Approach

ReqStream is an OTS requirements traceability tool developed by DEMA Consulting. It
processes requirements YAML files and generates traceability reports showing which
requirements are covered by which tests. VersionMark uses ReqStream to generate the
requirements traceability report as part of the compliance evidence package.

ReqStream is verified through two mechanisms. First, the CI pipeline runs
`dotnet reqstream --validate --results artifacts/reqstream-self-validation.trx`, which
executes ReqStream's internal test suite and writes results to a TRX file. Second, the CI
pipeline runs ReqStream with the `--enforce` flag against the VersionMark requirements
YAML files and the TRX test results produced by `dotnet test`. A non-zero exit code from
ReqStream causes the pipeline to fail, meaning a passing CI run proves every requirement
in the project is linked to at least one passing test — confirming ReqStream is enforcing
requirements coverage correctly.

### Test Scenarios

**ReqStreamSelfValidation**: The CI pipeline runs
`dotnet reqstream --validate --results artifacts/reqstream-self-validation.trx`, which
executes ReqStream's internal test suite. The TRX file must be produced and contain no
failed tests. This scenario is verified by `artifacts/reqstream-self-validation.trx`.

**ReqStreamEnforcementMode**: The CI pipeline runs `dotnet reqstream --enforce` with all
VersionMark requirements YAML files and test TRX results. The tool must exit zero,
confirming every requirement has passing test evidence. This scenario is verified by the
passing `--enforce` run in CI.
