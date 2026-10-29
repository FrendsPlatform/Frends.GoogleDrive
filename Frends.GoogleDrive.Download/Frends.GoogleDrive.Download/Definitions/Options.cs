using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.GoogleDrive.Download.Definitions;

/// <summary>
/// Options class usually contains parameters that are optional.
/// </summary>
public class Options
{
    /// <summary>
    /// Determines whether to create the destination folder if it does not already exist.
    /// </summary>
    /// <example>false</example>
    public bool CreateDestinationFolder { get; set; }

    /// <summary>
    /// Value indicating if we should continue downloading the next file when a download error occurs. Rollback in case of partial success is not supported.
    /// </summary>
    /// <example>false</example>
    [DefaultValue(false)]
    public bool ContinueOnFailure { get; set; } = false;

    /// <summary>
    /// Value indicating whether download error should stop the Task and throw an exception (true) or try to add the error message to Result.ErrorMessages and continue to download next file.</summary>
    /// <example>true</example>
    [DefaultValue(true)]
    public bool ThrowErrorOnFailure { get; set; } = true;

    /// <summary>
    /// Overrides the error message on failure.
    /// </summary>
    /// <example>Custom error message</example>
    [DisplayFormat(DataFormatString = "Text")]
    [DefaultValue("")]
    public string ErrorMessageOnFailure { get; set; } = string.Empty;
}
