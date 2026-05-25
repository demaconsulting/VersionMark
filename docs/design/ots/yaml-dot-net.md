## YamlDotNet

### Purpose

YamlDotNet is a YAML parsing and serialization library for .NET. VersionMark
uses it to deserialize `.versionmark.yaml` configuration files into C# object
graphs inside `VersionMarkConfig.Load`. It is chosen because it exposes
source-location information (`Mark`) on every YAML node, which VersionMark uses
to report lint issues with precise file, line, and column coordinates.

### Features Used

| Feature                    | Usage in VersionMark                                          |
|----------------------------|---------------------------------------------------------------|
| `YamlStream` / `YamlDocument` | Load the raw YAML document from a `TextReader`            |
| `MappingNode`              | Traverse the top-level mapping and each tool's mapping        |
| `ScalarNode`               | Read string values (command, regex, OS override keys/values)  |
| `YamlNode.Start` (`Mark`)  | Extract line and column for lint issue location reporting     |
| `YamlException`            | Catch YAML parse errors and convert to error-level LintIssue  |

YamlDotNet's object-model API (not the serializer API) is used so that unknown
keys can be detected and the source location of each node can be preserved.

### Integration Pattern

YamlDotNet is initialized and consumed entirely within `VersionMarkConfig.Load`.
No YamlDotNet types are exposed in public method signatures or return types.

1. A `YamlStream` is created and `Load` is called with a `StreamReader` opened
   on the configuration file path.
2. If the file is missing, the `FileNotFoundException` is caught before YamlDotNet
   is invoked; a `LintIssue` of severity Error is returned.
3. If the YAML is syntactically invalid, `YamlException` is caught and converted
   to a `LintIssue` of severity Error using `YamlException.Start` for location.
4. The document root is expected to be a `MappingNode`. Each child key is
   extracted as a `ScalarNode` and its `Value` property is compared against the
   known key set (`tools`, plus OS-override suffixes). Unknown keys produce a
   `LintIssue` of severity Warning.
5. No `Dispose` is required; the `YamlStream` does not hold unmanaged resources.
