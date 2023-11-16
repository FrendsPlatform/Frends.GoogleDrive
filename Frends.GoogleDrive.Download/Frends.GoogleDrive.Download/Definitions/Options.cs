using System.ComponentModel;

namespace Frends.GoogleDrive.Download.Definitions;

/// <summary>
/// Options class usually contains parameters that are optional.
/// </summary>
public class Options
{
    /// <summary>
    /// Value indicating whether download error should stop the Task and throw an exception (true) or try to add the error message to Result.ErrorMessages and continue to download next file.</summary>
    /// <example>true</example>
    [DefaultValue(true)]
    public bool ThrowErrorOnFailure { get; set; }

    /// <summary>
    /// Determines whether to create the destination folder if it does not already exist.
    /// </summary>
    /// <example>false</example>
    public bool CreateDestinationFolder { get; set; }
}