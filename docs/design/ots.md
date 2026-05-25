# OTS Integration Design

VersionMark consumes three runtime Off-The-Shelf (OTS) libraries:
**YamlDotNet**, **Microsoft.Extensions.FileSystemGlobbing**, and
**DemaConsulting.TestResults**. This document describes the overall integration
strategy and provides a cross-reference to the per-item design files.

## Integration Strategy

Each OTS library is consumed only through a single local unit that acts as an
adapter boundary. No OTS type leaks beyond the unit that owns it:

| OTS Item                                  | Consuming Unit      | Subsystem     |
|-------------------------------------------|---------------------|---------------|
| YamlDotNet                                | VersionMarkConfig   | Configuration |
| Microsoft.Extensions.FileSystemGlobbing   | GlobMatcher         | Utilities     |
| DemaConsulting.TestResults                | Validation          | SelfTest      |

Restricting each OTS item to a single consuming unit minimises the blast radius
of an API change during an upgrade: only the adapter unit needs to change. See
the individual per-item design files for details of features used and
initialization requirements:

- See _YamlDotNet Integration Design_
- See _Microsoft.Extensions.FileSystemGlobbing Integration Design_
- See _DemaConsulting.TestResults Integration Design_
