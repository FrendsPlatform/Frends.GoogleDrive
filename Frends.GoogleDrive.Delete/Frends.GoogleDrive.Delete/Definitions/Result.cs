using System.Collections.Generic;

namespace Frends.GoogleDrive.Delete.Definitions;

/// <summary>
/// Result.
/// </summary>
public class Result
{
    /// <summary>
    /// Gets or sets a value indicating whether process was executed successfully.
    /// </summary>
    /// <example>true</example>
    public bool Success { get; set; }

    /// <summary>
    /// List of deleted items.
    /// </summary>
    /// <example>{ {"id", "filename"}, {"id", filename"} }</example>
    public List<DeleteResult> Data { get; set; }

    /// <summary>
    /// Error that occurred during task execution.
    /// </summary>
    /// <example>object { string Message, Exception AdditionalInfo }</example>
    public Error Error { get; set; }
}
