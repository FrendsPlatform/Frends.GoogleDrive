# Changelog

## [2.0.0]
### Changed
- Task now targets .NET 8.
- Task class is now static.
### Added
- Added `ErrorMessageOnFailure` option for overriding the error message on failure.
- Added `Error` property to the Result with the error message and exception details.
- When `ThrowErrorOnFailure` is false, failures are now returned as a failed Result instead of being thrown.

## [1.1.0]
### Added
- Added a flag to include shared drives in the file search

## [1.0.0] - 2023-11-16
### Added
- Initial implementation