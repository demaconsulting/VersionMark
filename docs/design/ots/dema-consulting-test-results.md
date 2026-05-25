## DemaConsulting.TestResults

### Purpose

`DemaConsulting.TestResults` is an OTS library developed by DEMA Consulting that
provides in-process test result collection and serialization to TRX (MSTest) and
JUnit XML formats. VersionMark uses it inside the `Validation` class (SelfTest
subsystem) to accumulate self-validation test outcomes and write them to a
results file when `--results` or `--results-xml` is specified. It is chosen
because it produces the same TRX format consumed by the CI/CD traceability
pipeline, enabling self-validation results to be included in the compliance
evidence set alongside the xUnit results.

### Features Used

| Feature                    | Usage in VersionMark                                          |
|----------------------------|---------------------------------------------------------------|
| `TestResults` (class)      | Collection of test case results named `"VersionMark Self-Validation"` |
| `TestResults.AddResult`    | Record the pass/fail outcome of each self-validation scenario |
| `TrxSerializer.Serialize`  | Write the collection to a `.trx` file                        |
| `JUnitSerializer.Serialize`| Write the collection to a JUnit XML `.xml` file              |

### Integration Pattern

`DemaConsulting.TestResults` is consumed entirely within `Validation` and its
private helpers. No library types are exposed in the `Validation` public
signature (`Run` takes and returns only `Context`).

1. `Validation.Run` creates a `TestResults` instance named
   `"VersionMark Self-Validation"` before executing any test helper.
2. Each private test helper (`RunCaptureTest`, `RunPublishTest`,
   `RunLintValidTest`, `RunLintInvalidTest`) calls `testResults.AddResult` with
   the test name, pass/fail status, and an optional failure message.
3. After all helpers complete, `Validation.WriteResultsFile` inspects
   `context.ResultsFile`:
   - `.trx` extension → `TrxSerializer.Serialize(results, path)`.
   - `.xml` extension → `JUnitSerializer.Serialize(results, path)`.
   - Any other extension → `context.WriteError` is called; no file is written.
4. Both serializers write directly to disk; no intermediate stream management is
   required by the caller. No `Dispose` is required on `TestResults`.
