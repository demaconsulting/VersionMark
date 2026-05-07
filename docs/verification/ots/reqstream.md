## ReqStream Verification

### Overview

ReqStream is an OTS requirements traceability tool developed by DEMA Consulting. It
processes requirements YAML files and generates traceability reports showing which
requirements are covered by which tests. VersionMark uses ReqStream to generate the
requirements traceability report as part of the compliance evidence package.

### Verification Approach

ReqStream is verified through its built-in self-validation mechanism. The CI pipeline
runs `dotnet reqstream --validate --results artifacts/reqstream-self-validation.trx`,
which executes ReqStream's internal test suite and writes results to a TRX file. A
passing TRX file serves as evidence that ReqStream is functioning correctly in the CI
environment.

### Requirements Coverage

The following list maps ReqStream requirements to verification evidence:

- **`VersionMark-OTS-ReqStream`**: `artifacts/reqstream-self-validation.trx`
  (ReqStream self-validation passing in CI)
