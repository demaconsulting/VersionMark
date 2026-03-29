# Lint Subsystem

## Overview

The lint subsystem validates `.versionmark.yaml` configuration files without stopping at
the first error. It is invoked by the `--lint [<config-file>]` command and reports all
issues found before returning. This satisfies requirement `VersionMark-Cmd-Lint`.

The lint subsystem consists of a single unit: `Lint`, which performs all validation logic.
