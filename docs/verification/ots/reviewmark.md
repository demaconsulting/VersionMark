## ReviewMark

### Verification Approach

ReviewMark is an OTS code review enforcement tool developed by DEMA Consulting. It tracks
the review status of files using a `.reviewmark.yaml` configuration and generates code
review reports. VersionMark uses ReviewMark to enforce formal review coverage across all
source, documentation, and requirements files.

ReviewMark is verified through two mechanisms. First, the CI pipeline runs
`dotnet reviewmark --validate --results artifacts/reviewmark-self-validation.trx`, which
executes ReviewMark's internal test suite and writes results to a TRX file. Second, the CI
pipeline runs ReviewMark against the VersionMark `.reviewmark.yaml` configuration to
generate a review plan markdown document and a review report markdown document. A passing
CI run producing both documents confirms ReviewMark is reading the review configuration
and generating correct output.

### Test Scenarios

**ReviewMarkSelfValidation**: The CI pipeline runs
`dotnet reviewmark --validate --results artifacts/reviewmark-self-validation.trx`, which
executes ReviewMark's internal test suite. The TRX file must be produced and contain no
failed tests. This scenario is verified by `artifacts/reviewmark-self-validation.trx`.

**ReviewMarkReviewPlanGeneration**: The CI pipeline runs ReviewMark to generate
`docs/code_review_plan/generated/plan.md` from the `.reviewmark.yaml` configuration. The
review plan document must be produced listing all files with their review status. This
scenario is verified by `docs/code_review_plan/generated/plan.md`.

**ReviewMarkReviewReportGeneration**: The CI pipeline runs ReviewMark to generate
`docs/code_review_report/generated/report.md`. The review report document must be produced
summarizing review coverage across the repository. This scenario is verified by
`docs/code_review_report/generated/report.md`.
