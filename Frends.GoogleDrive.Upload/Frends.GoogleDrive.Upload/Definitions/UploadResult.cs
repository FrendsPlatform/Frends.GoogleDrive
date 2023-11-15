namespace Frends.GoogleDrive.Upload.Definitions;

/// <summary>
/// Upload result.
/// </summary>
public class UploadResult
{
    /// <summary>
    /// Uploaded file's Id in the Google Drive.
    /// </summary>
    /// <example>1rGpAzFfw2zDSk8lRGYDafwkxxtw_N7fY</example>
    public string UploadedFileId { get; private set; }

    /// <summary>
    /// Uploaded file's name in the Google Drive.
    /// </summary>
    /// <example>test.txt</example>
    public string UploadedFileName { get; private set; }

    internal UploadResult(string uploadedFileId, string uploadedFileName)
    {
        UploadedFileId = uploadedFileId;
        UploadedFileName = uploadedFileName;
    }
}
