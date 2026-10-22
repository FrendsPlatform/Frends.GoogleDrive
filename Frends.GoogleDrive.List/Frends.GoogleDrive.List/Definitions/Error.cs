using System;

namespace Frends.GoogleDrive.List.Definitions;

/// <summary>
/// Error details.
/// </summary>
public class Error
{
    /// <summary>
    /// Summary of the error.
    /// </summary>
    /// <example>Service account key missing.</example>
    public string Message { get; set; }

    /// <summary>
    /// The exception that caused the error.
    /// </summary>
    /// <example>System.ArgumentException: Service account key missing.</example>
    public Exception AdditionalInfo { get; set; }
}
