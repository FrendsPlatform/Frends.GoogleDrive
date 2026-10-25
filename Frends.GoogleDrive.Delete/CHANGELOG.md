# Changelog

## [2.0.0] - 2026-10-25
### Changed
- **[Breaking Change]** The Task now requires an `Options` parameter with `ThrowErrorOnFailure` and `ErrorMessageOnFailure` settings.
- **[Breaking Change]** The result now contains an `Error` property. If `ThrowErrorOnFailure` is false, failures are returned as a result with `Success` set to false instead of an exception being thrown.
- Updated the Task to target .NET 8.

## [1.0.0] - 2023-12-20
### Added
- Initial implementation