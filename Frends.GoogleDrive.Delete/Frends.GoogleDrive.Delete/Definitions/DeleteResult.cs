namespace Frends.GoogleDrive.Delete.Definitions;

/// <summary>
/// Delete result.
/// </summary>
public class DeleteResult
{
    /// <summary>
    /// File's Id in the Google Drive.
    /// </summary>
    /// <example>1rGpAzFfw2zDSk8lRGYDafwkxxtw_N7fY</example>
    public string FileId { get; private set; }

    /// <summary>
    /// File's name in the Google Drive.
    /// </summary>
    /// <example>test.txt</example>
    public string Filename { get; private set; }

    internal DeleteResult(string fileId, string filename)
    {
        FileId = fileId;
        Filename = filename;
    }
}
