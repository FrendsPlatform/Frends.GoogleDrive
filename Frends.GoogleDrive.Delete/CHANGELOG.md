# Changelog

## [2.0.0] - 2026-10-22
### Changed
- Upgraded the Task to target .NET 8.
- Added an Options parameter to the Task. Existing Task calls must be updated to provide it.
- Added error handling options: `ThrowErrorOnFailure` and `ErrorMessageOnFailure`. When `ThrowErrorOnFailure` is false, failures are returned in the result's `Error` property instead of being thrown.

## [1.0.0] - 2023-12-20
### Added
- Initial implementation