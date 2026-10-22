# Changelog

## [2.0.0] - 2026-10-22
### Changed
- **[Breaking Change]** Added the required `Options` parameter to the Task, so existing processes must be updated to supply it.
- **[Breaking Change]** The Task result now contains an `Error` property.
- Added `ThrowErrorOnFailure` and `ErrorMessageOnFailure` options to control how failures are reported. With `ThrowErrorOnFailure` set to false, errors are returned in the result instead of being thrown.
- Upgraded the Task to target .NET 8.

## [1.0.0] - 2023-12-20
### Added
- Initial implementation