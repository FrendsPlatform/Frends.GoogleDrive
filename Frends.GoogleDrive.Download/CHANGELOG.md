# Changelog

## [2.0.0]
### Changed
- [Breaking] Removed `Result.ErrorMessage`. Download failures are now reported in `Result.Error.FailedFiles`, with a summary in `Result.Error.Message`.
- [Breaking] Added `ContinueOnFailure` option (default `false`). Per-file download failures now stop the Task based on this option. When `true`, the Task continues with the next file, and `Result.Success` is `true` while failed files are listed in `Result.Error.FailedFiles`.
- Upgraded the Task to target .NET 8.
- Added `ErrorMessageOnFailure` option to override the error message when the Task fails.
- Added `Error` property to the result, containing the error message and additional information when the Task fails and `ThrowErrorOnFailure` is false.

## [1.1.0]
### Added
- Added a flag to include shared drives in the file search

## [1.0.0] - 2023-11-16
### Added
- Initial implementation
