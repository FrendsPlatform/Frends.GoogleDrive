namespace Frends.GoogleDrive.Download.Definitions;

/// <summary>
/// Download result.
/// </summary>
public class DownloadResult
{
    /// <summary>
    /// Downloaded file's Id in the Google Drive.
    /// </summary>
    /// <example>1rGpAzFfw2zDSk8lRGYDafwkxxtw_N7fY</example>
    public string DownloadedFileId { get; private set; }

    /// <summary>
    /// Downloaded file's name in the Google Drive.
    /// </summary>
    /// <example>test.txt</example>
    public string DownloadedFilename { get; private set; }

    internal DownloadResult(string downloadedFileId, string downloadedFilename)
    {
        DownloadedFileId = downloadedFileId;
        DownloadedFilename = downloadedFilename;
    }
}
