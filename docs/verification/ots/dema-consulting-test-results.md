## DemaConsulting.TestResults

### Verification Approach

`DemaConsulting.TestResults` is an OTS library developed by DEMA Consulting that provides
`TestResults`, `TrxSerializer`, and `JUnitSerializer` types. The SelfTest subsystem uses
this library to collect in-process test results and serialize them to TRX (`.trx`) or
JUnit XML (`.xml`) format for downstream traceability tooling.

`DemaConsulting.TestResults` is verified through the SelfTest subsystem integration tests
and the built-in `--validate` mode. Tests that exercise the `--results <file>` flag cause
`Validation.WriteResultsFile` to call `TrxSerializer.Serialize`, producing a real TRX
output file. Tests that exercise the `--results-xml <file>` path call
`JUnitSerializer.Serialize`, producing a real JUnit XML file. Both files are asserted for
existence and correct content by the integration tests. Correct serialization of a
`TestResults` collection into a well-formed TRX or JUnit XML file confirms the library is
functioning correctly.

### Test Scenarios

**TestResults_TrxSerialization**: The SelfTest integration test invokes
`--validate --results <tmp>.trx`, which serializes the internal test results to a TRX
file. The resulting TRX file must be produced and its content must assert correct
structure. This scenario is verified by `SelfTest_Run_WithResultsFlag_WritesResultsFile`.

**TestResults_JUnitSerialization**: The SelfTest integration test invokes
`--validate --results-xml <tmp>.xml`, which serializes the internal test results to a
JUnit XML file. The resulting JUnit XML file must be produced and asserted for correct
structure. This scenario is verified by
`SelfTest_Run_WithResultsXmlFlag_WritesJUnitResultsFile`.
