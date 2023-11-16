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
    public bool Success { get; private set; }

    /// <summary>
    /// List of download result(s).
    /// </summary>
    /// <example>{ {"id", "filename"}, {"id", filename"} }</example>
    public List<DownloadResult> Data { get; private set; }

    /// <summary>
    /// List of error messages.
    /// </summary>
    /// <example>{ "An error occured...", "Another error occured" }</example>
    public List<dynamic> ErrorMessage { get; private set; }

    internal Result(bool success, List<DownloadResult> data, List<dynamic> errorMessage)
    {
        Success = success;
        Data = data;
        ErrorMessage = errorMessage;
    }
}
