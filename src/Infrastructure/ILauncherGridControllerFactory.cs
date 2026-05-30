using Models;

namespace Infrastructure;

/// <summary>
/// Creates launcher grid controllers for a grid state instance.
/// </summary>
internal interface ILauncherGridControllerFactory
{
    /// <summary>
    /// Creates a controller for the specified state and reload delegate.
    /// </summary>
    /// <param name="state">State controlled by the controller.</param>
    /// <param name="reloadOptions">Delegate used to reload launcher options.</param>
    /// <returns>A launcher grid controller instance.</returns>
    ILauncherGridController Create(ILauncherGridState state, Func<LauncherOptions> reloadOptions);
}
