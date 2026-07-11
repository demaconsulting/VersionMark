# OTS Integration Design

VersionMark consumes three runtime Off-The-Shelf (OTS) libraries:
**YamlDotNet**, **Microsoft.Extensions.FileSystemGlobbing**, and
**DemaConsulting.TestResults**. It also relies on **SysML2Tools**, a build-time
dotnet global tool that validates the architecture model and renders its
declared views to SVG diagrams consumed by this design documentation. This
document describes the overall integration strategy and provides a
cross-reference to the per-item design files.

## Integration Strategy

The OTS items used by this project fall into two functional groups:

- **Production code libraries** — YamlDotNet, Microsoft.Extensions.FileSystemGlobbing, and
  DemaConsulting.TestResults are each consumed only through a single local unit that acts as an
  adapter boundary. No OTS type leaks beyond the unit that owns it:

  | OTS Item                                  | Consuming Unit      | Subsystem     |
  |-------------------------------------------|---------------------|---------------|
  | YamlDotNet                                | VersionMarkConfig   | Configuration |
  | Microsoft.Extensions.FileSystemGlobbing   | GlobMatcher         | Utilities     |
  | DemaConsulting.TestResults                | Validation          | SelfTest      |

  Restricting each OTS item to a single consuming unit minimizes the blast radius of an API
  change during an upgrade: only the adapter unit needs to change.

- **Build pipeline tool** — SysML2Tools is a dotnet global tool installed via
  `.config/dotnet-tools.json` and invoked directly from `lint.ps1` and the GitHub Actions
  workflow. It operates on the `docs/sysml2/` architecture model rather than on any in-house
  unit, so it has no consuming unit of its own.

See the individual per-item design files for details of features used and initialization
requirements:

- See _YamlDotNet Integration Design_
- See _Microsoft.Extensions.FileSystemGlobbing Integration Design_
- See _DemaConsulting.TestResults Integration Design_
- See _SysML2Tools OTS Design_
