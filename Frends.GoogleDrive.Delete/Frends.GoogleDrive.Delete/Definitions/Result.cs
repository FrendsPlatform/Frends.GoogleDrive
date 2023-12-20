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
    public bool Success { get; private set; }

    /// <summary>
    /// List of deleted items.
    /// </summary>
    /// <example>{ {"id", "filename"}, {"id", filename"} }</example>
    public List<DeleteResult> Data { get; private set; }

    internal Result(bool success, List<DeleteResult> data)
    {
        Success = success;
        Data = data;
    }
}
