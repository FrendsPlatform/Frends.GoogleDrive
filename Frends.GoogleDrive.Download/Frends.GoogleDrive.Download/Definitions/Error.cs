using System;

namespace Frends.GoogleDrive.Download.Definitions;

/// <summary>
/// Error that occurred during the task.
/// </summary>
public class Error
{
    /// <summary>
    /// Summary of the error.
    /// </summary>
    /// <example>Unable to download a file.</example>
    public string Message { get; set; }

    /// <summary>
    /// List of messages about files that failed to download.
    /// </summary>
    /// <example>[ "Error downloading file: test.txt. Error: Not found" ]</example>
    public string[] FailedFiles { get; set; } = [];

    /// <summary>
    /// Additional information about the error.
    /// </summary>
    /// <example>object { Exception AdditionalInfo }</example>
    public Exception AdditionalInfo { get; set; }
}
