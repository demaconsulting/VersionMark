# SelfTest Subsystem

## Overview

The validation subsystem provides built-in verification of the tool's core functionality
and safe path construction for use within that verification. It consists of two units:
`Validation` (the self-validation test runner) and `PathHelpers` (a safe path combination
utility used internally by `Validation`).

The validation subsystem is invoked when the `--validate` flag is passed and can write
results to a TRX or JUnit XML file when `--results` is also provided. This satisfies
requirements `VersionMark-CommandLine-Validate` and `VersionMark-CommandLine-Results`.
