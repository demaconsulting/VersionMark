## DemaConsulting.HelpCards

### Purpose

`DemaConsulting.HelpCards` is an OTS library developed by DEMA Consulting that provides the
built-in help of VersionMark: it turns Markdown help cards embedded in the tool assembly into a
validated catalog, serves `versionmark help` from that catalog for people and AI agents, and
supplies the test helpers that prove the cards are valid and that their examples work.

### Classification

Like the other DEMA Consulting tools that VersionMark uses, `DemaConsulting.HelpCards` is produced
by another repository with its own requirements and release lifecycle, and VersionMark does not
change it. It is therefore classified as an OTS software item.

### Features Used

| Feature | Usage in VersionMark |
| --- | --- |
| `HelpSource.FromAssembly` | Exposes the `Help` folder that the package build targets embed in the tool assembly |
| `HelpCli`, `HelpCliOptions` | Answer the `help` command, and supply the usage-text pointer sentence (`Footer`) |
| `HelpCliOptions.GetCatalogConfiguration` | Gives the tests the catalog composition that the tool ships |
| `HelpCatalogBuilder` | `AddSource`, `TagVocabulary`, and `ExpectedRoots` fix the cards, tags, and topics |
| `HelpCardValidator` | Test helper that validates the shipped cards with warnings treated as errors |
| `HelpExampleRunner` | Test helper that runs every command example against the built tool |
| MSBuild targets | Embed `Help/**` as resources of the tool assembly |

### Integration Pattern

`HelpCommand` (Cli subsystem) is the only unit that uses the package in the shipped tool; creating
its `HelpSource` and `HelpCli` performs no I/O and needs no disposal. `Program.Main` calls
`HelpCommand.Default.TryRun` before `Context.Create`, and the package builds the catalog only when
the first argument is `help`.

The tool project references the package directly, because only a project with its own package
reference embeds cards; the test project receives the test helpers through its project reference to
the tool. The package is a pre-release, so its exact version is pinned in the project file;
upgrading it requires re-running the help tests and a review of this design.
