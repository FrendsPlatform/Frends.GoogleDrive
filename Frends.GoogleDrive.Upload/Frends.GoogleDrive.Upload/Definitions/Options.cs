using System.ComponentModel;

namespace Frends.GoogleDrive.Upload.Definitions;

/// <summary>
/// Options class usually contains parameters that are optional.
/// </summary>
public class Options
{
    /// <summary>
    /// Value indicating whether upload error should stop the Task and throw an exception (true) or try to add the error message to Result.ErrorMessages and continue to upload next file.</summary>
    /// <example>true</example>
    [DefaultValue(true)]
    public bool ThrowErrorOnFailure { get; set; }

    /// <summary>
    /// Throw error if there are no file(s) in the path matching the filemask.
    /// </summary>
    /// <example>false</example>
    public bool ThrowErrorIfNoMatch { get; set; }
}