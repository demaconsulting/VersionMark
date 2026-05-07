## SonarMark Verification

### Overview

SonarMark is an OTS SonarCloud report tool developed by DEMA Consulting. It retrieves
code quality metrics from SonarCloud and generates summary reports. VersionMark uses
SonarMark in its CI/CD pipeline to include SonarCloud quality gate results in the
compliance evidence package.

### Verification Approach

SonarMark is verified through its built-in self-validation mechanism. The CI pipeline
runs `dotnet sonarmark --validate --results artifacts/sonarmark-self-validation.trx`,
which executes SonarMark's internal test suite and writes results to a TRX file. A
passing TRX file serves as evidence that SonarMark is functioning correctly in the CI
environment.

### Requirements Coverage

The following list maps SonarMark requirements to verification evidence:

- **`VersionMark-OTS-SonarMark`**: `artifacts/sonarmark-self-validation.trx`
  (SonarMark self-validation passing in CI)
