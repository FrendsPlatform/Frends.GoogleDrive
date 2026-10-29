using System;

namespace Frends.GoogleDrive.Upload.Definitions;

/// <summary>
/// Error that occurred during the task.
/// </summary>
public class Error
{
    /// <summary>
    /// Summary of the error.
    /// </summary>
    /// <example>Unable to upload a file.</example>
    public string Message { get; set; }

    /// <summary>
    /// List of messages about files that failed to upload.
    /// </summary>
    /// <example>[ "Error uploading file: test.txt. Error: Not found" ]</example>
    public string[] FailedFiles { get; set; } = [];

    /// <summary>
    /// Additional information about the error.
    /// </summary>
    /// <example>object { Exception AdditionalInfo }</example>
    public Exception AdditionalInfo { get; set; }
}
