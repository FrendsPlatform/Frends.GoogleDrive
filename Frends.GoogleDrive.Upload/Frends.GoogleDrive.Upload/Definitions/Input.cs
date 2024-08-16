using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.GoogleDrive.Upload.Definitions;

/// <summary>
/// Input parameters.
/// </summary>
public class Input
{
    /// <summary>
    /// Source directory.
    /// </summary>
    /// <example>c:\temp, \\network\folder</example>
    [DisplayFormat(DataFormatString = "Text")]
    public string SourceDirectory { get; set; }

    /// <summary>
    /// Windows-style filemask. Empty field = all objects (*).
    /// If you want to upload only one file, define exact filename as filemask.
    /// Consider using .zip (for example) when uploading multiple objects at the same time.
    /// </summary>
    /// <example>*.* , ?_file.*, foo_*.txt, singlefile.txt</example>
    [DisplayFormat(DataFormatString = "Text")]
    [DefaultValue("*")]
    public string FileMask { get; set; }

    /// <summary>
    /// Id of the target Google Drive folder.
    /// </summary>
    /// <example>42o3j8D7cmhMJFAAlbZb5B_8u9OqnAIkn</example>
    [DisplayFormat(DataFormatString = "Text")]
    public string TargetFolderId { get; set; }

    /// <summary>
    /// The JSON service account key generated in the Google Cloud portal.
    /// Value can be either JSON string or full filepath to JSON file.
    /// </summary>
    /// <example>
    /// {
    ///      "type": "service_account",
    ///      "project_id": "something",
    ///      "private_key_id": "fdsafdsafdsalmnop12345678909876543212344",
    ///      "private_key": "-----BEGIN PRIVATE KEY-----\nMIIE.......Hw==\n-----END PRIVATE KEY-----\n",
    ///      "client_email": "someone@something.iam.gserviceaccount.com",
    ///      "client_id": "123456789012345678900",
    ///      "auth_uri": "https://accounts.google.com/o/oauth2/auth",
    ///      "token_uri": "https://oauth2.googleapis.com/token",
    ///      "auth_provider_x509_cert_url": "https://www.googleapis.com/oauth2/v1/certs",
    ///      "client_x509_cert_url": "https://www....nt.com"
    /// }
    /// ,
    /// C:\temp\ServiceAccountKeyJSON.json
    /// </example>
    [DisplayFormat(DataFormatString = "Text")]
    [PasswordPropertyText]
    public string ServiceAccountKeyJSON { get; set; }

    /// <summary>
    /// Whether to enable support for shared drives.
    /// </summary>
    /// <example>false</example>
    [DefaultValue(false)]
    public bool IncludeSharedDrives { get; set; }
}
