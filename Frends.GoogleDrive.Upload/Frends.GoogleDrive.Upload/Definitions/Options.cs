using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.GoogleDrive.Upload.Definitions;

/// <summary>
/// Options class usually contains parameters that are optional.
/// </summary>
public class Options
{
    /// <summary>
    /// Value indicating if we should continue uploading the next file when an upload error occurs. Rollback in case of partial success is not supported.
    /// </summary>
    /// <example>false</example>
    [DefaultValue(false)]
    public bool ContinueOnFailure { get; set; } = false;

    /// <summary>
    /// Value indicating whether an error should stop the Task and throw an exception (true) or return the error in Result.Error (false).</summary>
    /// <example>true</example>
    [DefaultValue(true)]
    public bool ThrowErrorOnFailure { get; set; } = true;

    /// <summary>
    /// Throw error if there are no file(s) in the path matching the filemask.
    /// </summary>
    /// <example>false</example>
    public bool ThrowErrorIfNoMatch { get; set; }

    /// <summary>
    /// Overrides the error message on failure.
    /// </summary>
    /// <example>Custom error message</example>
    [DisplayFormat(DataFormatString = "Text")]
    [DefaultValue("")]
    public string ErrorMessageOnFailure { get; set; } = string.Empty;
}
