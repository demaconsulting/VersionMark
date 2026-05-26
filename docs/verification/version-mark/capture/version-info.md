### VersionInfo

#### Verification Approach

The `VersionInfo` unit is a JSON-serializable record that holds captured version data for
a single CI/CD job. It provides `SaveToFile` to write the record to a JSON file and
`LoadFromFile` to read it back. Tests are in `Capture/VersionInfoTests.cs` and read and
write temporary JSON files. No external mocks are required.

#### Test Environment

N/A - standard test environment. Tests create temporary JSON files during setup and clean
them up afterwards.

#### Acceptance Criteria

- All unit tests for `VersionInfo` pass with zero failures across all supported OS and
  .NET version matrix combinations (Windows, Linux, macOS x .NET 8, .NET 9, .NET 10).
- Every requirement for the `VersionInfo` unit is covered by at least one named test
  scenario.

#### Test Scenarios

**VersionInfo_SaveToFile_CreatesJsonFile**: `SaveToFile` creates a JSON file at the
specified path. This scenario is tested by `VersionInfo_SaveToFile_CreatesJsonFile`.

**VersionInfo_SaveAndLoad_RoundTripPreservesData**: A save followed by a load preserves all
version entries. This scenario is tested by
`VersionInfo_SaveAndLoad_RoundTripPreservesData`.

**VersionInfo_EmptyVersions_SavesAndLoadsCorrectly**: An empty versions dictionary saves
and loads correctly. This scenario is tested by
`VersionInfo_EmptyVersions_SavesAndLoadsCorrectly`.

**VersionInfo_SpecialCharacters_SavesAndLoadsCorrectly**: Version strings with special
characters are preserved through a save and load cycle. This scenario is tested by
`VersionInfo_SpecialCharacters_SavesAndLoadsCorrectly`.

**VersionInfo_LoadFromFile_ReadsJsonFile**: `LoadFromFile` reads a pre-existing JSON file
correctly. This scenario is tested by `VersionInfo_LoadFromFile_ReadsJsonFile`.

**VersionInfo_LoadFromFile_NonExistentFile_ThrowsArgumentException**: A non-existent file
throws `ArgumentException`. This scenario is tested by
`VersionInfo_LoadFromFile_NonExistentFile_ThrowsArgumentException`.

**VersionInfo_LoadFromFile_InvalidJson_ThrowsArgumentException**: Invalid JSON content
throws `ArgumentException`. This scenario is tested by
`VersionInfo_LoadFromFile_InvalidJson_ThrowsArgumentException`.

**VersionInfo_LoadFromFile_EmptyJson_ThrowsArgumentException**: An empty JSON file throws
`ArgumentException`. This scenario is tested by
`VersionInfo_LoadFromFile_EmptyJson_ThrowsArgumentException`.

**VersionInfo_LoadFromFile_NullJson_ThrowsArgumentException**: A JSON file containing only
null throws `ArgumentException`. This scenario is tested by
`VersionInfo_LoadFromFile_NullJson_ThrowsArgumentException`.

**VersionInfo_SaveToFile_InvalidPath_ThrowsInvalidOperationException**: An invalid file
path throws `InvalidOperationException`. This scenario is tested by
`VersionInfo_SaveToFile_InvalidPath_ThrowsInvalidOperationException`.
