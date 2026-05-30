using Models;

namespace Infrastructure;

/// <summary>
/// Creates launcher grid views for a loaded launcher configuration.
/// </summary>
internal interface ILauncherGridViewFactory
{
    /// <summary>
    /// Creates a launcher grid view.
    /// </summary>
    /// <param name="options">Launcher options to display.</param>
    /// <param name="reloadOptions">Delegate used to reload launcher options.</param>
    /// <returns>A launcher grid view instance.</returns>
    LauncherGridView Create(LauncherOptions options, Func<LauncherOptions> reloadOptions);
}
