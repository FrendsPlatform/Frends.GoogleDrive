# Changelog

## [2.0.0] - 2026-10-25
### Changed
- **[Breaking Change]** Added the `Options` parameter with `ThrowErrorOnFailure` and `ErrorMessageOnFailure`. The Task can now return a failed result with `Error` details instead of throwing.
- **[Breaking Change]** A missing service account key now produces a validation error instead of an `ArgumentException`.
- **[Breaking Change]** Added the `Error` property to the Task result.
- Updated the Task to target .NET 8.

## [1.1.0]
### Added
- Added a flag to include shared drives in the file search

## [1.0.0] - 2023-11-17
### Added
- Initial implementation