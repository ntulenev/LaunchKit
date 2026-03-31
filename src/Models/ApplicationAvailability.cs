namespace Models;

/// <summary>
/// Describes whether an application entry can currently be launched.
/// </summary>
public enum ApplicationAvailability
{
    /// <summary>
    /// The application entry can be launched.
    /// </summary>
    Ready = 0,

    /// <summary>
    /// The configured path could not be resolved to a launchable target.
    /// </summary>
    PathNotFound = 1
}
