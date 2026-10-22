# Changelog

## [2.0.0] - 2026-10-22
### Changed
- **[Breaking Change]** Added the required `Options` parameter to the Task, with `ThrowErrorOnFailure` and `ErrorMessageOnFailure` settings.
- **[Breaking Change]** The Task result now contains an `Error` property. When `ThrowErrorOnFailure` is false, failures are returned in the result instead of thrown.
- **[Breaking Change]** A missing service account key now causes a validation error instead of an argument exception.
- Updated the Task to target .NET 8.

## [1.0.0] - 2023-12-20
### Added
- Initial implementation