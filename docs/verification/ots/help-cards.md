## DemaConsulting.HelpCards

### Verification Approach

`DemaConsulting.HelpCards` is a runtime library with no self-validation mode, so it is verified by
tests in the VersionMark test suite, which use the real package and the cards embedded in the tool
assembly. No mocking is applied.

### Test Environment

Tests require:

- No network access; the cards are read from the tool assembly.
- A writable temporary directory, in which the example runner creates a scratch folder per card.

### Acceptance Criteria

The OTS integration is accepted when all linked tests pass with zero failures.

### Test Scenarios

**HelpCommand**: The help verb is answered from the embedded cards, in-process and by the built
tool. Tested by `HelpCommand_TryRun_HelpVerb_PrintsTopicIndexAndReturnsZero` and
`VersionMark_HelpTopics_HelpVerb_ListsTopics`.

**TestHelpers**: The validator accepts the shipped cards, and the example runner runs every command
example against the built tool. Tested by `HelpCommand_CreateOptions_ShippedCatalog_PassesValidation`
and `VersionMark_HelpExamples_AllCommandExamples_GiveStatedResults`.
