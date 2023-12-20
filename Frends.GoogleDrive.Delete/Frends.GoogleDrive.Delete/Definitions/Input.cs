using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.GoogleDrive.Delete.Definitions;

/// <summary>
/// Input parameters.
/// </summary>
public class Input
{
    /// <summary>
    /// Query for specific files to be deleted.
    /// See <see href="https://developers.google.com/drive/api/guides/search-files">Google Drive API Guide</see>
    /// </summary>
    /// <example>'42o3j8C7kntMJZeElbZb5B_5u92snSIkb' in parents and name contains 'test'</example>
    [DisplayFormat(DataFormatString = "Text")]
    [DefaultValue("*")]
    public string FileQuery { get; set; }

    /// <summary>
    /// Service account key JSON.
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
    /// </example>
    [DisplayFormat(DataFormatString = "Text")]
    [PasswordPropertyText]
    public string ServiceAccountKeyJSON { get; set; }
}
