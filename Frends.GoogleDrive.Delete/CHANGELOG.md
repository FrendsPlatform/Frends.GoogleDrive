# Changelog

## [2.0.0] - 2026-10-22
### Changed
- **[Breaking Change]** The Task now requires an `Options` parameter. Existing Task calls must be updated to provide it.
- Added `ThrowErrorOnFailure` and `ErrorMessageOnFailure` options. When `ThrowErrorOnFailure` is disabled, failures are returned in the result's `Error` property instead of being thrown.
- **[Breaking Change]** Missing service account key now causes a validation error instead of an argument error.
- Upgraded the Task to target .NET 8.

## [1.0.0] - 2023-12-20
### Added
- Initial implementation