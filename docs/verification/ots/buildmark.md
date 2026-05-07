## BuildMark Verification

### Overview

BuildMark is an OTS tool developed by DEMA Consulting that generates build notes
documenting which versions of tools were used during a build. VersionMark uses BuildMark
in its CI/CD pipeline to capture and publish build notes as part of the compliance
evidence package.

### Verification Approach

BuildMark is verified through its built-in self-validation mechanism. The CI pipeline
runs `dotnet buildmark --validate --results artifacts/buildmark-self-validation.trx`,
which executes BuildMark's internal test suite and writes results to a TRX file. The
presence of a passing TRX file serves as evidence that BuildMark is functioning correctly
in the CI environment.

### Requirements Coverage

The following list maps BuildMark requirements to verification evidence:

- **`VersionMark-OTS-BuildMark`**: `artifacts/buildmark-self-validation.trx`
  (BuildMark self-validation passing in CI)
