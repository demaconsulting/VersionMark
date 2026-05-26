## YamlDotNet

### Verification Approach

`YamlDotNet` is an OTS YAML parsing library. The Configuration subsystem uses it to
parse `.versionmark.yaml` files inside `VersionMarkConfig.Load`. All YAML node traversal,
type checking, and source-location extraction performed during configuration loading and
linting depend on this library.

`YamlDotNet` is verified through the Configuration subsystem unit and integration tests.
Every test that invokes `VersionMarkConfig.Load` or `VersionMarkConfig.ReadFromFile` with
a YAML input string exercises the YamlDotNet parsing path. Tests cover valid YAML,
syntactically invalid YAML (confirmed to produce a parse-error `LintIssue`), and edge
cases such as missing required sections. Successful execution of these tests across all
supported OS and .NET version matrix combinations confirms that YamlDotNet is functioning
correctly in each environment.

### Test Scenarios

**YamlDotNet_ParsesValidConfig**: `VersionMarkConfig.Load` with a well-formed YAML file
returns a non-null `Config` and an empty issues list, confirming that YamlDotNet correctly
parses valid YAML input. This scenario is verified by
`VersionMarkConfig_Load_ValidConfig_ReturnsConfig`.

**YamlDotNet_ReportsParseError**: `VersionMarkConfig.Load` with invalid YAML returns a
null `Config` and an error-level `LintIssue` containing source-location information
derived from the `YamlException` thrown by YamlDotNet, confirming that YamlDotNet
surfaces parse errors correctly. This scenario is verified by
`VersionMarkConfig_Load_InvalidYaml_ReturnsNullConfig`.
