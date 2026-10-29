namespace Frends.GoogleDrive.List.Definitions;

/// <summary>
/// Result.
/// </summary>
public class Result
{
    /// <summary>
    /// Gets or sets a value indicating whether listing was executed successfully.
    /// </summary>
    /// <example>true</example>
    public bool Success { get; set; }

    /// <summary>
    /// Gets or sets the list of files as JSON string.
    /// </summary>
    /// <example>{ "1bzL3JPJDmN_sLe0hzySuGUzK2mtW1xML": { "Id": "1bzL3JPJDmN_sLe0hzySuGUzK2mtW1xML", "Name": "test-photograph.jpg", "Size": 271191, "Version": 3, "CreatedTime": "2023-11-16T08:01:18.824+02:00" } }</example>
    public string Data { get; set; }

    /// <summary>
    /// Gets or sets the error that occurred during task execution.
    /// </summary>
    /// <example>object { string Message, Exception AdditionalInfo }</example>
    public Error Error { get; set; }
}
