using System.Collections.Generic;

namespace Frends.GoogleDrive.Upload.Definitions;

/// <summary>
/// Result.
/// </summary>
public class Result
{
    /// <summary>
    /// Gets a value indicating whether upload was executed successfully.
    /// </summary>
    /// <example>true</example>
    public bool Success { get; set; }

    /// <summary>
    /// List of upload result(s).
    /// </summary>
    /// <example>{ {"id", "filename"}, {"id", filename"} }</example>
    public List<UploadResult> Data { get; set; }

    /// <summary>
    /// Gets error message.
    /// </summary>
    /// <example>{ {"An error occured..."}, {"An error occured..."} }</example>
    public List<dynamic> ErrorMessage { get; set; }

    /// <summary>
    /// Error that occurred during task execution.
    /// </summary>
    /// <example>object { string Message, Exception AdditionalInfo }</example>
    public Error Error { get; set; }
}
