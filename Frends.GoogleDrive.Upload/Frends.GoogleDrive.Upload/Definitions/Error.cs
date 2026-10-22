using System;

namespace Frends.GoogleDrive.Upload.Definitions;

/// <summary>
/// Error details.
/// </summary>
public class Error
{
    /// <summary>
    /// Summary of the error.
    /// </summary>
    /// <example>Source path not found.</example>
    public string Message { get; set; }

    /// <summary>
    /// Additional information about the error.
    /// </summary>
    /// <example>System.ArgumentException: Source path not found.</example>
    public Exception AdditionalInfo { get; set; }
}
