# Changelog

## [2.0.0]
### Changed
- The Task now targets .NET 8.
- When `Options.ThrowErrorOnFailure` is `false`, unexpected errors (for example an invalid source directory or service account key) are now returned in the new `Result.Error` property instead of being thrown.

### Added
- Added `Options.ErrorMessageOnFailure` for overriding the error message on failure.
- Added `Result.Error` containing the error message and exception details when the Task fails.

## [1.1.0]
### Added
- Added a flag for shared drive support in the file search

## [1.0.1] - 2023-12-07
### Changed
- Input.ServiceAccountKeyJSON and Result.ErrorMessage documentation update.

## [1.0.0] - 2023-11-15
### Added
- Initial implementation