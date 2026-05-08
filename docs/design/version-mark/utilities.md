## Utilities Subsystem

### Overview

The Utilities subsystem provides general-purpose helper classes used by other subsystems
within VersionMark. It consists of two units: `GlobMatcher`, which implements glob-pattern
file matching for the Publish mode, and `PathHelpers`, which provides safe path combination
to protect against path-traversal attacks.

This subsystem satisfies requirements `VersionMark-Utilities-GlobMatch` and
`VersionMark-Utilities-SafePath`.

### Units

#### GlobMatcher

The `GlobMatcher` class (`GlobMatcher.cs`) provides glob-pattern file matching. It exposes
two methods: `FindMatchingFiles`, which accepts an array of glob patterns and returns a
sorted, deduplicated list of matching file paths; and `SplitAbsolutePattern`, which splits
an absolute glob pattern into its root directory and relative pattern components.

See *GlobMatcher Unit Design* for the full unit design.

#### PathHelpers

The `PathHelpers` class (`PathHelpers.cs`) provides a single static method,
`SafePathCombine`, which safely combines a base path and a relative path while
preventing path-traversal attacks. It is used by `SelfTest.Validation` when
constructing paths inside temporary directories.

See *PathHelpers Unit Design* for the full unit design.

### Subsystem Interactions

`GlobMatcher.FindMatchingFiles` is called by the Cli Subsystem (`Program.RunPublish`) to
resolve the glob patterns supplied on the command line into a concrete list of JSON capture
files. `PathHelpers.SafePathCombine` is called by the SelfTest subsystem (`Validation.Run`)
when constructing paths inside temporary directories. The Utilities subsystem has no
dependencies on other VersionMark subsystems; it depends only on
`Microsoft.Extensions.FileSystemGlobbing` for pattern evaluation.
