### PathHelpers

#### Verification Approach

The `PathHelpers` unit provides a `SafePathCombine` method that combines a base path and
a relative path while preventing path traversal attacks. It rejects relative paths that
contain `..` components that would escape the base directory, as well as absolute paths.
Tests are in `Utilities/PathHelpersTests.cs` and call `PathHelpers.SafePathCombine`
directly. No external mocks or file system access is required.

#### Test Environment

N/A - standard test environment. All tests run using `dotnet test` with no additional
environment setup required.

#### Acceptance Criteria

- All unit tests for `PathHelpers` pass with zero failures across all supported OS and
  .NET version matrix combinations (Windows, Linux, macOS x .NET 8, .NET 9, .NET 10).
- Every requirement for the `PathHelpers` unit is covered by at least one named test
  scenario.

#### Test Scenarios

**PathHelpers_SafePathCombine_ValidPaths_CombinesCorrectly**: A simple relative path is
combined with the base path. This scenario is tested by
`PathHelpers_SafePathCombine_ValidPaths_CombinesCorrectly`.

**PathHelpers_SafePathCombine_PathTraversalWithDoubleDots_ThrowsArgumentException**: A path
beginning with `../` throws `ArgumentException`. This scenario is tested by
`PathHelpers_SafePathCombine_PathTraversalWithDoubleDots_ThrowsArgumentException`.

**PathHelpers_SafePathCombine_DoubleDotsInMiddle_ThrowsArgumentException**: A path
containing `..` in the middle throws `ArgumentException`. This scenario is tested by
`PathHelpers_SafePathCombine_DoubleDotsInMiddle_ThrowsArgumentException`.

**PathHelpers_SafePathCombine_AbsolutePath_ThrowsArgumentException**: A rooted absolute
path throws `ArgumentException`. This scenario is tested by
`PathHelpers_SafePathCombine_AbsolutePath_ThrowsArgumentException`.

**PathHelpers_SafePathCombine_CurrentDirectoryReference_CombinesCorrectly**: A path
containing `.` (current directory) combines correctly. This scenario is tested by
`PathHelpers_SafePathCombine_CurrentDirectoryReference_CombinesCorrectly`.

**PathHelpers_SafePathCombine_NestedPaths_CombinesCorrectly**: A nested relative path
combines correctly. This scenario is tested by
`PathHelpers_SafePathCombine_NestedPaths_CombinesCorrectly`.

**PathHelpers_SafePathCombine_EmptyRelativePath_ReturnsBasePath**: An empty relative path
returns the base path unchanged. This scenario is tested by
`PathHelpers_SafePathCombine_EmptyRelativePath_ReturnsBasePath`.

**PathHelpers_SafePathCombine_DotDotAsNamePrefix_CombinesCorrectly**: A filename that
starts with `..` but is not a traversal combines correctly. This scenario is tested by
`PathHelpers_SafePathCombine_DotDotAsNamePrefix_CombinesCorrectly`.

**PathHelpers_SafePathCombine_NullBasePath_ThrowsArgumentNullException**: A null base path
throws `ArgumentNullException`. This scenario is tested by
`PathHelpers_SafePathCombine_NullBasePath_ThrowsArgumentNullException`.

**PathHelpers_SafePathCombine_NullRelativePath_ThrowsArgumentNullException**: A null
relative path throws `ArgumentNullException`. This scenario is tested by
`PathHelpers_SafePathCombine_NullRelativePath_ThrowsArgumentNullException`.

**PathHelpers_SafePathCombine_DeepPathTraversal_ThrowsArgumentException**: A deep path
traversal attempt (e.g. `../../../etc/passwd`) throws `ArgumentException`. This scenario is
tested by `PathHelpers_SafePathCombine_DeepPathTraversal_ThrowsArgumentException`.

**PathHelpers_SafePathCombine_MultiSegmentRelativePath_ProducesExpectedPath**: A valid
multi-segment relative path (e.g. `test-results/output.trx`) is combined with a base
directory correctly. This scenario is tested by
`PathHelpers_SafePathCombine_MultiSegmentRelativePath_ProducesExpectedPath`.

**PathHelpers_SafePathCombine_DllInBaseDirectory_FileExists**: A path pointing to
`DemaConsulting.VersionMark.dll` in the base directory resolves to an existing file. This
scenario is tested by `PathHelpers_SafePathCombine_DllInBaseDirectory_FileExists`.
