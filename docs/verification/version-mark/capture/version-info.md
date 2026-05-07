### VersionInfo Unit Verification

#### Overview

The `VersionInfo` unit is a JSON-serializable record that holds captured version data for
a single CI/CD job. It provides `SaveToFile` to write the record to a JSON file and
`LoadFromFile` to read it back. Tests are in `Capture/VersionInfoTests.cs`.

#### Test Scenarios

The following test scenarios verify `VersionInfo`:

- **`VersionInfo_SaveToFile_CreatesJsonFile`**: `SaveToFile` creates a JSON file at the specified path.
- **`VersionInfo_SaveAndLoad_RoundTripPreservesData`**: Save followed by load preserves all version entries.
- **`VersionInfo_EmptyVersions_SavesAndLoadsCorrectly`**: An empty versions dictionary saves and loads correctly.
- **`VersionInfo_SpecialCharacters_SavesAndLoadsCorrectly`**: Version strings with special characters are preserved.
- **`VersionInfo_LoadFromFile_ReadsJsonFile`**: `LoadFromFile` reads a pre-existing JSON file correctly.
- **`VersionInfo_LoadFromFile_NonExistentFile_ThrowsArgumentException`**: Non-existent file throws ArgumentException.
- **`VersionInfo_LoadFromFile_InvalidJson_ThrowsArgumentException`**: Invalid JSON content throws ArgumentException.
- **`VersionInfo_LoadFromFile_EmptyJson_ThrowsArgumentException`**: Empty JSON file throws ArgumentException.
- **`VersionInfo_LoadFromFile_NullJson_ThrowsArgumentException`**: JSON containing only null throws ArgumentException.
- **`VersionInfo_SaveToFile_InvalidPath_ThrowsInvalidOperationException`**: Invalid file path throws InvalidOperationException.

#### Dependencies

No external mocks are required. Tests read and write temporary JSON files.

#### Requirements Coverage

The following list maps `VersionInfo` unit requirements to test scenarios:

- **`VersionMark-VersionInfo-Save`**: `VersionInfo_SaveToFile_CreatesJsonFile`,
  `VersionInfo_SaveAndLoad_RoundTripPreservesData`,
  `VersionInfo_EmptyVersions_SavesAndLoadsCorrectly`,
  `VersionInfo_SpecialCharacters_SavesAndLoadsCorrectly`
- **`VersionMark-VersionInfo-Load`**: `VersionInfo_LoadFromFile_ReadsJsonFile`,
  `VersionInfo_SaveAndLoad_RoundTripPreservesData`,
  `VersionInfo_EmptyVersions_SavesAndLoadsCorrectly`,
  `VersionInfo_SpecialCharacters_SavesAndLoadsCorrectly`
- **`VersionMark-VersionInfo-Error`**: `VersionInfo_LoadFromFile_NonExistentFile_ThrowsArgumentException`,
  `VersionInfo_LoadFromFile_InvalidJson_ThrowsArgumentException`,
  `VersionInfo_LoadFromFile_EmptyJson_ThrowsArgumentException`,
  `VersionInfo_LoadFromFile_NullJson_ThrowsArgumentException`,
  `VersionInfo_SaveToFile_InvalidPath_ThrowsInvalidOperationException`
