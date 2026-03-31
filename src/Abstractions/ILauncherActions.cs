using Models;

namespace Abstractions;

/// <summary>
/// Provides actions that can be performed for configured launcher entries.
/// </summary>
public interface ILauncherActions
{
    /// <summary>
    /// Launches the specified application entry.
    /// </summary>
    /// <param name="application">Application entry to launch.</param>
    /// <returns>A status message describing the result.</returns>
    string Launch(ApplicationEntry application);

    /// <summary>
    /// Opens the folder that contains the specified application entry.
    /// </summary>
    /// <param name="application">Application entry whose folder should be opened.</param>
    /// <returns>A status message describing the result.</returns>
    string OpenContainingFolder(ApplicationEntry application);
}
