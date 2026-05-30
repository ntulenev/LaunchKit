using Models;

namespace Infrastructure;

/// <summary>
/// Creates launcher grid state instances for a loaded launcher configuration.
/// </summary>
internal interface ILauncherGridStateFactory
{
    /// <summary>
    /// Creates a state object for the specified launcher options.
    /// </summary>
    /// <param name="options">Launcher options to track.</param>
    /// <returns>A launcher grid state instance.</returns>
    ILauncherGridState Create(LauncherOptions options);
}
