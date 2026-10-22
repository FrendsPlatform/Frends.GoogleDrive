using System;

namespace Frends.GoogleDrive.Download.Definitions;

/// <summary>
/// Error details.
/// </summary>
public class Error
{
    /// <summary>
    /// Summary of the error.
    /// </summary>
    /// <example>Destination directory doesn't exist.</example>
    public string Message { get; set; }

    /// <summary>
    /// The exception that caused the error.
    /// </summary>
    /// <example>System.ArgumentException: Destination directory doesn't exist.</example>
    public Exception AdditionalInfo { get; set; }
}
