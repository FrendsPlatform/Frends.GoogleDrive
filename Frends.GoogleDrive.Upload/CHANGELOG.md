# Changelog

## [2.0.0]
- **[Breaking Change]** Upgraded the Task to target .NET 8.
- **[Breaking Change]** When `ThrowErrorOnFailure` is false, errors that occur before or during upload (for example missing service account key or source directory) are now returned in the `Error` property of the result instead of being thrown.
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