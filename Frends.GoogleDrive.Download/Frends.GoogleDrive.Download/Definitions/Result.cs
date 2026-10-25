using System.Collections.Generic;

namespace Frends.GoogleDrive.Download.Definitions;

/// <summary>
/// Result.
/// </summary>
public class Result
{
    /// <summary>
    /// Gets a value indicating whether download was executed successfully.
    /// </summary>
    /// <example>true</example>
    public bool Success { get; set; }

    /// <summary>
    /// List of download result(s).
    /// </summary>
    /// <example>{ { "1rGpAzFfw2zDSk8lRGYDafwkxxtw_N7fY", "test.txt" } }</example>
    public List<DownloadResult> Data { get; set; }

    /// <summary>
    /// List of error messages.
    /// </summary>
    /// <example>{ "Error downloading file: test.txt. Error: Not found" }</example>
    public List<dynamic> ErrorMessage { get; set; }

    /// <summary>
    /// Error that occurred during task execution.
    /// </summary>
    /// <example>object { string Message, Exception AdditionalInfo }</example>
    public Error Error { get; set; }
}
