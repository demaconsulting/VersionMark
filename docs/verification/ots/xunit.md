## xUnit Verification

### Overview

xUnit is the OTS unit testing framework used by the VersionMark test project
`DemaConsulting.VersionMark.Tests`. It provides the test runner, assertion library, and
TRX results output used throughout this verification design document.

### Verification Approach

xUnit is verified through execution of the VersionMark test suite. The CI pipeline runs:

```text
dotnet test --no-build --configuration Release
  --collect "XPlat Code Coverage;Format=opencover"
  --logger "trx;LogFilePrefix=<os>"
  --results-directory artifacts
```

This command is executed across multiple operating system and .NET version combinations
in the CI matrix (Windows, Linux, macOS against .NET 8, .NET 9, and .NET 10). A passing
test run on each combination provides evidence that xUnit is functioning correctly in
each environment. The resulting TRX files are collected as CI artifacts.

### Requirements Coverage

The following list maps xUnit requirements to verification evidence:

- **`VersionMark-OTS-xUnit`**: TRX result files from `dotnet test` across all CI matrix
  OS/dotnet combinations
