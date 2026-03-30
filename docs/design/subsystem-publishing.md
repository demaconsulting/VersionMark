# Publishing Subsystem

## Overview

The publish subsystem is responsible for generating a human-readable markdown version
report from captured JSON files. It reads the version data produced by the capture
subsystem and consolidates identical versions across jobs, flagging any conflicts.

The publish subsystem consists of a single unit: `MarkdownFormatter`, which converts
a collection of `VersionInfo` records into a markdown string.
