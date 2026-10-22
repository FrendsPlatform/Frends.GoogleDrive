# Changelog

## [2.0.0]
### Changed
- Upgraded the Task to target .NET 8.
- Breaking: the Task now requires an `Options` parameter and the Task class is now static.
- Breaking: the Result now contains an `Error` property describing the failure when the Task does not throw.

### Added
- Added `ThrowErrorOnFailure` and `ErrorMessageOnFailure` options to control error handling.

## [1.1.0]
### Added
- Added a flag to include shared drives in the file search

## [1.0.0] - 2023-11-17
### Added
- Initial implementation