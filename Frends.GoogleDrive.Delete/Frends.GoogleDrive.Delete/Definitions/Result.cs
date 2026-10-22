using System.Collections.Generic;

namespace Frends.GoogleDrive.Delete.Definitions;

/// <summary>
/// Result.
/// </summary>
public class Result
{
    /// <summary>
    /// Gets a value indicating whether process was executed successfully.
    /// </summary>
    /// <example>true</example>
    public bool Success { get; set; }

    /// <summary>
    /// List of deleted items.
    /// </summary>
    /// <example>[ { FileId = "1rGpAzFfw2zDSk8lRGYDafwkxxtw_N7fY", Filename = "test.txt" } ]</example>
    public List<DeleteResult> Data { get; set; }

    /// <summary>
    /// Error that occurred during task execution.
    /// </summary>
    /// <example>object { string Message, Exception AdditionalInfo }</example>
    public Error Error { get; set; }
}
