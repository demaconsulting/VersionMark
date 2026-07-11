# OTS Verification

VersionMark uses several Off-The-Shelf (OTS) components. This section documents the
verification evidence for each OTS component. OTS components are third-party tools and
libraries not developed in-house. Their verification relies on the vendor's own quality
assurance and, where available, self-validation mechanisms provided by the tool itself.

For OTS tools that include a `--validate` flag, VersionMark's CI pipeline runs
self-validation and captures the results as TRX artifacts. For tools without a built-in
self-validation mode, verification is based on functional evidence produced during the
build and document generation pipeline.

## Verification Strategy

OTS components are divided into two verification categories based on whether the tool
provides a built-in self-validation mode.

Tools that expose a `--validate` flag are verified by invoking that flag directly in the
CI pipeline. The following tools fall into this category: BuildMark, FileAssert, ReqStream,
ReviewMark, SarifMark, SonarMark, and SysML2Tools. Each tool's self-validation exercises its own
internal logic and reports results in TRX format. xUnit is treated similarly: the
`dotnet test` run that executes the VersionMark test suite simultaneously validates that
xUnit itself is operating correctly.

Tools without a built-in self-validation mode are verified through functional evidence
produced during the CI document generation pipeline. Pandoc and WeasyPrint fall into this
category. The CI pipeline generates seven document types — requirements report, design
document, verification document, user guide, build notes, code quality report, and code
review report — and FileAssert asserts the content of each generated document. Successful
FileAssert assertions confirm that Pandoc and WeasyPrint performed their respective
conversion steps correctly.

DemaConsulting.TestResults is verified through the SelfTest subsystem integration tests
that exercise TRX and JUnit serialization. Microsoft.Extensions.FileSystemGlobbing is
verified through the GlobMatcher unit tests that exercise glob-pattern file matching.
YamlDotNet is verified through the Configuration subsystem unit and integration tests that
exercise YAML parsing.

## Qualification Evidence

The following evidence artifacts are collected during each CI run:

For tools with built-in self-validation (BuildMark, FileAssert, ReqStream, ReviewMark,
SarifMark, SonarMark, SysML2Tools), the CI pipeline captures TRX result files stored as pipeline
artifacts under the path `artifacts/{tool}-self-validation.trx`, where `{tool}` is the
lowercase tool name (for example, `artifacts/buildmark-self-validation.trx`).

For xUnit, evidence is provided by the TRX files produced by `dotnet test` across the
supported platform matrix (Windows, Linux, macOS × .NET 8, .NET 9, .NET 10), stored at
`artifacts/validation-{os}-{dotnet}.trx`.

For Pandoc and WeasyPrint, evidence is provided by the FileAssert TRX results that assert
the content of each of the seven generated document types. These TRX files are captured
alongside the other self-validation artifacts and confirm that document conversion
completed successfully and produced output matching the expected structure and content.

For DemaConsulting.TestResults, evidence is provided by the SelfTest integration tests
that produce and assert TRX and JUnit XML files. For Microsoft.Extensions.FileSystemGlobbing,
evidence is provided by the GlobMatcher unit tests. For YamlDotNet, evidence is provided
by the Configuration subsystem unit and integration tests.

## Regression Approach

When an OTS component is upgraded to a new version, the full CI pipeline is re-run against
that version before the upgrade is accepted.

For tools with built-in self-validation (BuildMark, FileAssert, ReqStream, ReviewMark,
SarifMark, SonarMark, SysML2Tools, xUnit), the corresponding self-validation TRX must complete
with zero failures. Any failure in the TRX output blocks the upgrade until the root cause is
resolved.

For Pandoc and WeasyPrint, all FileAssert assertions on the seven generated document types
must pass. A failure in any FileAssert assertion indicates that a conversion step produced
unexpected output and must be investigated before the upgrade proceeds.

For DemaConsulting.TestResults, Microsoft.Extensions.FileSystemGlobbing, and YamlDotNet,
the relevant subsystem and unit tests must pass with zero failures across all supported
platform matrix combinations.

In addition to executing the pipeline, the release notes for the new version are reviewed
to identify any changes in behavior that may affect the features used by VersionMark or
its document generation workflow. Behavioral changes that affect used features are assessed
and, where necessary, addressed before the upgrade is approved.
