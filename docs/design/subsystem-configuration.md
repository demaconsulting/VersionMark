# Configuration Subsystem

## Overview

The configuration subsystem reads and interprets the `.versionmark.yaml` file that defines
which tools to capture and how to extract their versions. It consists of two units:
`ToolConfig` (per-tool settings) and `VersionMarkConfig` (the top-level configuration
container).
