# Configuration Subsystem

## Overview

The configuration subsystem reads and interprets the `.versionmark.yaml` file that defines
which tools to capture and how to extract their versions, and reports any validation issues
found during loading. It consists of three units: `ToolConfig` (per-tool settings),
`VersionMarkConfig` (the top-level configuration container and validation entry point), and
`LintIssue` (the types used to surface validation issues to callers).
