# Command-Line Interface Subsystem

## Overview

The command-line interface subsystem is responsible for parsing command-line arguments,
routing program flow to the appropriate subsystem, and managing all output (console,
error, and log file). It consists of two units: `Program` (the entry point) and `Context`
(the argument and output container).
