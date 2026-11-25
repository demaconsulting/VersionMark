## Cli

![Cli Structure](CliView.svg)

### Overview

The Cli subsystem is responsible for parsing command-line arguments, routing program flow
to the appropriate subsystem, and managing all output (console, error, and log file). It
also answers the built-in `help` command. It consists of three units: `Program` (the entry
point and mode dispatcher), `Context` (the command-line argument and output container), and
`HelpCommand` (the built-in `help` command, answered from embedded help cards).

### Interfaces

**`Context.Create(string[] args)`**: Factory method that constructs a fully-parsed Context
instance.

- *Type*: In-process .NET public API.
- *Role*: Provider.
- *Contract*: Parses the command-line argument array; returns a Context with all properties
  populated. Opens the log file if `--log` was specified. Caller must dispose the returned
  Context.
- *Constraints*: Throws `ArgumentException` for unknown arguments.

**`Context.WriteLine(string message)`**: Writes a message to stdout and the log file.

- *Type*: In-process .NET public API.
- *Role*: Provider.
- *Contract*: Writes `message` to `Console.Out` unless `Silent` is set; also writes to the
  log file if one is open.
- *Constraints*: No output is produced when `Context.Silent` is `true`.

**`Context.WriteError(string message)`**: Writes an error message and sets the exit code to 1.

- *Type*: In-process .NET public API.
- *Role*: Provider.
- *Contract*: Writes `message` to `Console.Error` in red; sets `ExitCode` to `1`; writes
  to the log file if one is open.
- *Constraints*: stderr is suppressed when `Silent` is `true`; `_hasErrors` is always set and `ExitCode` always reflects
  the failure. The exit code cannot be reset once set to `1`.

**`Context.ExitCode`**: Returns the process exit code for the current invocation.

- *Type*: In-process .NET public API.
- *Role*: Provider.
- *Contract*: Returns `0` when no errors have been reported; `1` after any call to
  `WriteError`.
- *Constraints*: Read-only computed property.

**`Context.Dispose()`**: Releases the log file writer.

- *Type*: In-process .NET public API.
- *Role*: Provider.
- *Contract*: Closes and disposes the log file writer if one was opened.
- *Constraints*: Must be called to avoid resource leaks when `--log` was specified.

**`Program.Run(Context context)`**: Executes the appropriate operational mode.

- *Type*: In-process .NET public API.
- *Role*: Provider.
- *Contract*: Implements priority-ordered dispatch: Version → (banner) → Help → Validate → Lint →
  Capture → Publish → default. The application banner is printed after the version check and before
  all other modes; it is suppressed when lint is the dispatched action (i.e. `--lint` is set and
  neither `--help` nor `--validate` is set) to produce clean, issue-only output. Called re-entrantly
  by `Validation.Run` for self-tests.
- *Constraints*: Does not throw; all errors are routed through `context.WriteError`.

**`Program.Main(string[] args)`**: Process entry point.

- *Type*: .NET process entry point.
- *Role*: Provider.
- *Contract*: First offers the arguments to `HelpCommand.TryRun` and, when the help command
  answers, returns its exit code. Otherwise creates `Context`, calls `Run`, returns
  `context.ExitCode`. Catches `ArgumentException` and `InvalidOperationException`; writes to
  `Console.Error`; returns exit code `1`. Re-throws other exceptions.
- *Constraints*: Called once by the .NET runtime.

**`HelpCommand.TryRun(string[] args, out int exitCode)`**: Answers the `help` command from the
built-in help cards.

- *Type*: In-process .NET instance method (on the shared `HelpCommand.Default` instance).
- *Role*: Provider.
- *Contract*: Returns `false`, writing nothing, when the first argument is not `help`. Otherwise
  writes the answer to the console, returns `true`, and sets the exit code (see *Help Command* in
  the system design).
- *Constraints*: Does not throw for user input; does not use `Context`.

**`HelpCommand.Footer`**: The sentence that points a reader of the usage text to the help command.

- *Type*: In-process .NET instance property.
- *Role*: Provider.
- *Contract*: Returns the one-line pointer that HelpCards supplies.
- *Constraints*: Reading it never builds the help catalog.

The accepted CLI flags recognized by `Context.Create` are:

- **`--version`** (`-v`): Print version string and exit.
- **`--help`** (`-?`, `-h`): Print usage and exit.
- **`--silent`**: Suppress stdout output.
- **`--validate`**: Run built-in self-verification.
- **`--log <file>`**: Open log file for all output.
- **`--result`** / **`--results <file>`**: Write TRX (`.trx`) or JUnit (`.xml`) results file.
- **`--lint [<file>]`**: Validate config; defaults to `.versionmark.yaml`.
- **`--capture`**: Activate capture mode.
- **`--job-id <id>`**: Set job identifier for capture output filename.
- **`--output <file>`**: Override default capture output filename.
- **`--publish`**: Activate publish mode.
- **`--report <file>`**: Set the markdown report output file.
- **`--report-depth <1-6>`**: Markdown heading depth for the report section.
- **`--depth <1-6>`**: Default heading depth; used for validate and as fallback for
  `--report-depth`.
- **`--`**: Separator; subsequent tokens are tool names (capture mode) or glob patterns
  (publish mode).

### Design

The Cli subsystem contains three units:

1. **`Program`** — the entry point. It first offers the arguments to `HelpCommand.TryRun`;
   otherwise it constructs `Context` via `Context.Create`, dispatches
   to the appropriate mode helper (`RunCapture`, `RunPublish`, `RunLint`, or
   `Validation.Run`), catches top-level `ArgumentException` and `InvalidOperationException`,
   and returns `context.ExitCode` to the OS.

2. **`Context`** — the state container. It encapsulates all parsed command-line arguments
   as strongly-typed properties and owns all output channels (stdout writer, stderr, log
   file). Output routing (`WriteLine`, `WriteError`) is centralized here so that all
   subsystems receiving a `Context` have a uniform interface for messaging and error
   recording.

3. **`HelpCommand`** — the built-in help. It answers the `help` command from the help cards
   embedded in the assembly and supplies the footer that ends the `--help` usage text.

`Program` and `Context` are coupled through `Program` constructing `Context` and passing it to
all mode helpers. `Context` has no dependency on `Program`. `Context` and `HelpCommand` do not
call each other: when the help command answers, the invocation ends with its exit code and no
`Context` is created. `HelpCommand` writes its answers to the console itself; it is the one
exception to `Context` owning all output, because the `help` command is answered before a
`Context` exists.
