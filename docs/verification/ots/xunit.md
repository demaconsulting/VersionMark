## xUnit

### Verification Approach

xUnit is the OTS unit testing framework used by the VersionMark test project
`DemaConsulting.VersionMark.Tests`. It provides the test runner, assertion library, and
TRX results output used throughout this verification design document.

xUnit is verified through execution of the VersionMark test suite. The CI pipeline runs:

```text
dotnet test --no-build --configuration Release
  --collect "XPlat Code Coverage;Format=opencover"
  --logger "trx;LogFilePrefix=<os>"
  --results-directory artifacts
```

This command is executed across multiple operating system and .NET version combinations
in the CI matrix (Windows, Linux, macOS against .NET 8, .NET 9, and .NET 10). A passing
test run on each combination provides evidence that xUnit is functioning correctly in each
environment. The resulting TRX files are collected as CI artifacts.

### Test Scenarios

**xUnitRunsTestSuiteOnWindows**: The CI pipeline runs `dotnet test` on Windows across
.NET 8, .NET 9, and .NET 10. TRX result files must be produced with no failed tests. This
scenario is verified by `artifacts/validation-windows-*.trx`.

**xUnitRunsTestSuiteOnLinux**: The CI pipeline runs `dotnet test` on Linux across
.NET 8, .NET 9, and .NET 10. TRX result files must be produced with no failed tests. This
scenario is verified by `artifacts/validation-linux-*.trx`.

**xUnitRunsTestSuiteOnMacOs**: The CI pipeline runs `dotnet test` on macOS across
.NET 8, .NET 9, and .NET 10. TRX result files must be produced with no failed tests. This
scenario is verified by `artifacts/validation-macos-*.trx`.
