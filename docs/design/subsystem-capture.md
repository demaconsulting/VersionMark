# Capture Subsystem

## Overview

The capture subsystem is responsible for recording tool version information from the
current CI/CD job environment. It reads the tool configuration, executes the configured
commands, extracts version strings using regular expressions, and saves the results to a
JSON file. The captured data is later consumed by the publish subsystem to generate the
version report.

The capture subsystem consists of a single unit: `VersionInfo`, which is the data transfer
record for captured version data.
