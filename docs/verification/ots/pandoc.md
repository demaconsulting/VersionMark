## Pandoc

### Verification Approach

Pandoc is an OTS document conversion tool used in the VersionMark CI/CD pipeline to
compile the multiple markdown input files for each document collection into a single HTML
output file. It processes `definition.yaml` files to determine input ordering, template,
and table-of-contents settings.

Pandoc is verified through functional evidence. The CI pipeline generates seven document
types (requirements report, design document, verification document, user guide, build
notes, code quality report, and code review report) using Pandoc. FileAssert then asserts
the content of each generated document. A passing CI run with all FileAssert assertions
provides evidence that Pandoc is converting documents correctly.

### Test Scenarios

**PandocGeneratesRequirementsReport**: The CI pipeline runs Pandoc on
`docs/requirements_doc/definition.yaml` to generate the requirements HTML document.
FileAssert verifies the resulting HTML contains the expected content. This scenario is
verified by the FileAssert TRX results for the requirements report.

**PandocGeneratesDesignDocument**: The CI pipeline runs Pandoc on
`docs/design/definition.yaml` to generate the design HTML document. FileAssert verifies
the resulting HTML contains the expected content. This scenario is verified by the
FileAssert TRX results for the design document.

**PandocGeneratesVerificationDocument**: The CI pipeline runs Pandoc on
`docs/verification/definition.yaml` to generate the verification HTML document. FileAssert
verifies the resulting HTML contains the expected content. This scenario is verified by
the FileAssert TRX results for the verification document.

**PandocGeneratesUserGuide**: The CI pipeline runs Pandoc on `docs/user_guide/definition.yaml`
to generate the user guide HTML document. FileAssert verifies the resulting HTML contains
the expected content. This scenario is verified by the FileAssert TRX results for the user
guide.

**PandocGeneratesBuildNotes**: The CI pipeline runs Pandoc on
`docs/build_notes/definition.yaml` to generate the build notes HTML document. FileAssert
verifies the resulting HTML. This scenario is verified by the FileAssert TRX results for
the build notes.

**PandocGeneratesCodeQualityReport**: The CI pipeline runs Pandoc on
`docs/code_quality/definition.yaml` to generate the code quality HTML document. FileAssert
verifies the resulting HTML. This scenario is verified by the FileAssert TRX results for
the code quality report.

**PandocGeneratesCodeReviewReport**: The CI pipeline runs Pandoc on
`docs/code_review_report/definition.yaml` to generate the code review HTML document.
FileAssert verifies the resulting HTML. This scenario is verified by the FileAssert TRX
results for the code review report.
