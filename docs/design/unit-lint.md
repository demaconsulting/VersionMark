# Lint Unit

## Overview

The `Lint` class (`Lint.cs`) is an internal static class with no instance state. All
validation logic is contained in static methods.

## Run Method

`Run` is the public entry point. It accepts a `Context` (for output) and the path to the
configuration file to validate. It returns `true` when no issues are found, `false`
otherwise.

The method proceeds through the following steps:

1. Reports `"Linting '<file>'..."` to indicate the file under examination.
2. If the file does not exist, reports `{file}: error: Configuration file not found` and
   returns `false`.
3. Parses the file as YAML. On `YamlException`, calls `FormatLocation` to include the
   source position in the message and reports the parse error, returning `false`.
4. If the YAML stream contains no documents, reports an error and returns `false`.
5. Validates that the root node is a `YamlMappingNode`; reports an error and returns
   `false` if not.
6. Reports a warning for each top-level key that is not `"tools"`.
7. Checks that the `tools` key is present and its value is a `YamlMappingNode`; reports
   an error and returns `false` for either failure.
8. Checks that at least one tool entry is present.
9. Calls `ValidateTool` for each tool entry, accumulating the returned issue count.
10. If the total issue count is zero, reports `"'<file>': No issues found"`.

Returns `issueCount == 0`.

## ValidateTool Method

`ValidateTool` validates a single tool's `YamlMappingNode`. It returns the number of
issues found for that tool.

For each entry in the tool's mapping:

- Both the key and value must be `YamlScalarNode` instances; non-scalar nodes are reported
  as errors.
- Keys not present in the valid set (`command`, `command-win`, `command-linux`,
  `command-macos`, `regex`, `regex-win`, `regex-linux`, `regex-macos`) are reported as
  warnings.
- For `command`: must be present and non-empty.
- For `regex`: must be present, non-empty, a compilable regular expression (see
  `TryCompileRegex`), and must contain the named capture group `version` (verified via
  `Regex.GetGroupNames().Contains("version")` on the compiled regex).
- For OS-specific `command-*` keys: if present, must not be empty.
- For OS-specific `regex-*` keys: if present and non-empty, must be a compilable regular
  expression and must also contain the `version` named capture group.

After iterating all entries, missing `command` and missing `regex` fields are each reported
as separate errors.

## TryCompileRegex Method

`TryCompileRegex` attempts to compile the pattern using `new Regex(value, RegexOptions.None,
RegexTimeout)`, where `RegexTimeout` is one second. On `ArgumentException`, it reports the
compile error and returns `null`. If compilation succeeds it returns the compiled `Regex`
instance so callers can immediately inspect it (e.g., to check `GetGroupNames()`).

## FormatLocation Method

`FormatLocation` constructs a source-position prefix from a file path and the 0-based line
and column numbers supplied by YamlDotNet. It always converts to 1-based display values by
adding 1 to both, producing:

- `filePath(line+1,column+1)`

The resulting prefix is used in all error and warning messages to produce output in the
familiar `file(line,col): error: <message>` format understood by CI systems and editors.
