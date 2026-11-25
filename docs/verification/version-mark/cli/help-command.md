### HelpCommand

#### Verification Approach

`HelpCommand` is verified with unit tests defined in `HelpCommandTests.cs`. Each test creates a
help command with `HelpCommand.Create`, which writes to in-memory writers instead of the console.
No mocking is applied: the tests use the real help cards embedded in the tool assembly and the
real HelpCards library, a documented dependency of the unit.

#### Test Environment

Standard xUnit v3 test runner with `dotnet test`. No files, external services, or network access
are required; the cards are read from the tool assembly.

#### Acceptance Criteria

All `HelpCommandTests` test methods pass, confirming that the help verb prints the topic index, the
shipped catalog passes validation with warnings treated as errors, and every card prints in lines
of at most 120 characters. No `HelpCommand` unit requirement may remain without at least one named
test scenario (IEC 62304 §5.5.2).

#### Test Scenarios

**HelpCommand_TryRun_HelpVerb_PrintsTopicIndexAndReturnsZero**: Call `TryRun` with `["help"]`;
assert it answers with exit code 0 and the output lists the command of every root topic.
This scenario is tested by `HelpCommand_TryRun_HelpVerb_PrintsTopicIndexAndReturnsZero`.

**HelpCommand_CreateOptions_ShippedCatalog_PassesValidation**: Validate the catalog configuration
from `CreateOptions` with warnings treated as errors; assert no defect is reported, which proves
that the root cards are exactly `RootCardIds`, that every card carries one purpose tag, and that
every command example parses as a command line of the tool.
This scenario is tested by `HelpCommand_CreateOptions_ShippedCatalog_PassesValidation`.

**HelpCommand_TryRun_EveryCard_FitsMaximumLineWidth**: Print every card of the shipped catalog with
its links and tokens expanded and a fixed, long prerelease version in place of the build's version;
assert each exits with code 0 and has no line longer than 120 characters.
This scenario is tested by `HelpCommand_TryRun_EveryCard_FitsMaximumLineWidth`.
