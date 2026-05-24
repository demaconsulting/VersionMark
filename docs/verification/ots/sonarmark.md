## SonarMark

### Verification Approach

SonarMark is an OTS SonarCloud report tool developed by DEMA Consulting. It retrieves
code quality metrics from SonarCloud and generates summary reports. VersionMark uses
SonarMark in its CI/CD pipeline to include SonarCloud quality gate results in the
compliance evidence package.

SonarMark is verified through two mechanisms. First, the CI pipeline runs
`dotnet sonarmark --validate --results artifacts/sonarmark-self-validation.trx`, which
executes SonarMark's internal test suite and writes results to a TRX file. Second, the CI
pipeline runs SonarMark to retrieve quality gate status, open issues, and security
hotspot data from the SonarCloud project for VersionMark, then generates
`docs/code_quality/generated/sonarmark.md`. A passing CI run producing this report
confirms SonarMark is communicating with SonarCloud and generating correct markdown
output.

### Test Scenarios

**SonarMarkSelfValidation**: The CI pipeline runs
`dotnet sonarmark --validate --results artifacts/sonarmark-self-validation.trx`, which
executes SonarMark's internal test suite. The TRX file must be produced and contain no
failed tests. This scenario is verified by `artifacts/sonarmark-self-validation.trx`.

**SonarMarkQualityGateRetrieval**: The CI pipeline runs SonarMark to retrieve the
SonarCloud quality gate status for the VersionMark project. The tool must return the gate
result without error. This scenario is verified by the successful generation of
`docs/code_quality/generated/sonarmark.md`.

**SonarMarkIssuesRetrieval**: The CI pipeline runs SonarMark to retrieve open issues from
SonarCloud. The tool must return issue count and severity data for inclusion in the quality
report. This scenario is verified by the open issues section of
`docs/code_quality/generated/sonarmark.md`.

**SonarMarkHotSpotsRetrieval**: The CI pipeline runs SonarMark to retrieve security
hotspots from SonarCloud. The tool must return hotspot data for inclusion in the quality
report. This scenario is verified by the security hotspots section of
`docs/code_quality/generated/sonarmark.md`.

**SonarMarkMarkdownReportGeneration**: The CI pipeline runs SonarMark to generate
`docs/code_quality/generated/sonarmark.md` from the SonarCloud data. The markdown quality
report must be produced and incorporated into the code quality document collection. This
scenario is verified by `docs/code_quality/generated/sonarmark.md`.
