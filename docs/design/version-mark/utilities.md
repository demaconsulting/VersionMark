## Utilities Subsystem

### Overview

The Utilities subsystem provides general-purpose helper classes used by other subsystems
within VersionMark. It currently consists of one unit: `GlobMatcher`, which implements
glob-pattern file matching for the Publish mode.

This subsystem satisfies requirement `VersionMark-Utilities-GlobMatch`.

### Units

#### GlobMatcher

The `GlobMatcher` class (`GlobMatcher.cs`) provides glob-pattern file matching. It exposes
two methods: `FindMatchingFiles`, which accepts an array of glob patterns and returns a
sorted, deduplicated list of matching file paths; and `SplitAbsolutePattern`, which splits
an absolute glob pattern into its root directory and relative pattern components.

See *GlobMatcher Unit Design* for the full unit design.

### Subsystem Interactions

`GlobMatcher.FindMatchingFiles` is called by the Cli Subsystem (`Program.RunPublish`) to
resolve the glob patterns supplied on the command line into a concrete list of JSON capture
files. The Utilities subsystem has no dependencies on other VersionMark subsystems; it
depends only on `Microsoft.Extensions.FileSystemGlobbing` for pattern evaluation.
