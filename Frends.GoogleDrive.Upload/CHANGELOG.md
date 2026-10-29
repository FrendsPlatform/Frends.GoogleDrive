# Changelog

## [2.0.0]
- Upgraded the Task to target .NET 8.
- **[Breaking Change]** When `ThrowErrorOnFailure` is false, errors that occur before or during upload (for example, missing service account key or source directory) are now returned in the `Error` property of the result instead of being thrown.
- **[Breaking Change]** Removed `Result.ErrorMessage`. Upload failures are now reported in `Result.Error.FailedFiles`, with a summary in `Result.Error.Message`.
- **[Breaking Change]** Added `ContinueOnFailure` option (default `false`). Per-file upload failures now stop the Task based on this option. When `true`, the Task continues with the next file, and `Result.Success` is `true` while failed files are listed in `Result.Error.FailedFiles`.
- Added `Error` property to the result, containing the error message and exception details on failure.
- Added `ErrorMessageOnFailure` option for overriding the error message on failure.

## [1.1.0]
### Added
- Added a flag for shared drive support in the file search

## [1.0.1] - 2023-12-07
### Changed
- Input.ServiceAccountKeyJSON and Result.ErrorMessage documentation update.

## [1.0.0] - 2023-11-15
### Added
- Initial implementation
