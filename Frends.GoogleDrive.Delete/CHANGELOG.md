# Changelog

## [2.0.0] - 2026-10-22
### Changed
- Upgraded the Task to target .NET 8.
- Breaking: the Task now has an `Options` parameter. Existing processes must be reopened to set it.
- Breaking: the Task result now contains an `Error` property. Errors can be returned in the result instead of thrown by disabling `ThrowErrorOnFailure`; an optional custom message can be set with `ErrorMessageOnFailure`.
- A missing service account key is now reported as a validation error.

## [1.0.0] - 2023-12-20
### Added
- Initial implementation