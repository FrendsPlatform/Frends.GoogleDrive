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
    public bool Success { get; private set; }

    /// <summary>
    /// List of upload result(s).
    /// </summary>
    /// <example>{ {"id", "filename"}, {"id", filename"} }</example>
    public List<UploadResult> Data { get; private set; }

    /// <summary>
    /// Gets error message.
    /// </summary>
    /// <example>{ {"An error occured..."}, {"An error occured..."} }</example>
    public List<dynamic> ErrorMessage { get; private set; }

    internal Result(bool success, List<UploadResult> data, List<dynamic> errorMessage)
    {
        Success = success;
        Data = data;
        ErrorMessage = errorMessage;
    }
}
