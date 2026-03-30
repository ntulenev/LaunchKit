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

    /// <summary>
    /// Determines whether the specified application entry can be launched.
    /// </summary>
    /// <param name="application">Application entry to validate.</param>
    /// <returns><see langword="true"/> when the application is available; otherwise, <see langword="false"/>.</returns>
    bool IsAvailable(ApplicationEntry application);

    /// <summary>
    /// Resolves environment variables and trims quotes in a configured value.
    /// </summary>
    /// <param name="value">Raw configured value.</param>
    /// <returns>The resolved value or an empty string.</returns>
    string ResolveValue(string? value);
}
