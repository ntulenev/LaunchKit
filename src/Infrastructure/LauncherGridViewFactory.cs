using Models;

namespace Infrastructure;

/// <summary>
/// Default factory for launcher grid views.
/// </summary>
internal sealed class LauncherGridViewFactory(
    ILauncherGridStateFactory stateFactory,
    ILauncherGridControllerFactory controllerFactory,
    ILauncherShortcutResolver shortcutResolver,
    ILauncherTileFormatter tileFormatter) : ILauncherGridViewFactory
{
    /// <inheritdoc />
    public LauncherGridView Create(LauncherOptions options, Func<LauncherOptions> reloadOptions)
    {
        var state = _stateFactory.Create(options);
        var controller = _controllerFactory.Create(state, reloadOptions);

        return new LauncherGridView(
            state,
            controller,
            _shortcutResolver,
            _tileFormatter);
    }

    private readonly ILauncherGridStateFactory _stateFactory = stateFactory
        ?? throw new ArgumentNullException(nameof(stateFactory));
    private readonly ILauncherGridControllerFactory _controllerFactory = controllerFactory
        ?? throw new ArgumentNullException(nameof(controllerFactory));
    private readonly ILauncherShortcutResolver _shortcutResolver = shortcutResolver
        ?? throw new ArgumentNullException(nameof(shortcutResolver));
    private readonly ILauncherTileFormatter _tileFormatter = tileFormatter
        ?? throw new ArgumentNullException(nameof(tileFormatter));
}
