using Abstractions;

using Models;

namespace Infrastructure;

/// <summary>
/// Default factory for launcher grid controllers.
/// </summary>
internal sealed class LauncherGridControllerFactory(ILauncherActions launcherActions) : ILauncherGridControllerFactory
{
    /// <inheritdoc />
    public ILauncherGridController Create(ILauncherGridState state, Func<LauncherOptions> reloadOptions)
        => new LauncherGridController(state, _launcherActions, reloadOptions);

    private readonly ILauncherActions _launcherActions = launcherActions
        ?? throw new ArgumentNullException(nameof(launcherActions));
}
