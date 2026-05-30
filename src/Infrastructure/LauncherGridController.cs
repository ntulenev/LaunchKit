using Abstractions;

using Models;

namespace Infrastructure;

/// <summary>
/// Handles launcher grid commands independently from Terminal.Gui input and rendering.
/// </summary>
internal sealed class LauncherGridController : ILauncherGridController
{
    /// <summary>
    /// Initializes a launcher grid controller.
    /// </summary>
    /// <param name="state">State controlled by the controller.</param>
    /// <param name="launcherActions">Actions available for launcher entries.</param>
    /// <param name="reloadOptions">Delegate used to reload launcher options.</param>
    public LauncherGridController(
        ILauncherGridState state,
        ILauncherActions launcherActions,
        Func<LauncherOptions> reloadOptions)
    {
        _state = state ?? throw new ArgumentNullException(nameof(state));
        _launcherActions = launcherActions ?? throw new ArgumentNullException(nameof(launcherActions));
        _reloadOptions = reloadOptions ?? throw new ArgumentNullException(nameof(reloadOptions));
    }

    /// <inheritdoc />
    public LauncherGridUpdate LaunchSelection()
    {
        var application = _state.SelectedApplication;
        return application is null
            ? Status("No applications configured.")
            : Status(_launcherActions.Launch(application));
    }

    /// <inheritdoc />
    public LauncherGridUpdate LaunchSelectionAsAdmin()
    {
        var application = _state.SelectedApplication;
        return application is null
            ? Status("No applications configured.")
            : Status(_launcherActions.LaunchAsAdmin(application));
    }

    /// <inheritdoc />
    public LauncherGridUpdate OpenSelectionFolder()
    {
        var application = _state.SelectedApplication;
        return application is null
            ? Status("No applications configured.")
            : Status(_launcherActions.OpenContainingFolder(application));
    }

    /// <inheritdoc />
    public LauncherGridUpdate NextTab()
    {
        return !_state.NextTab()
            ? LauncherGridUpdate.None
            : new LauncherGridUpdate($"Tab: {_state.ActiveTab.Value}", true, true, true);
    }

    /// <inheritdoc />
    public LauncherGridUpdate ReloadSelection()
    {
        try
        {
            _state.Reload(_reloadOptions());

            return new LauncherGridUpdate(
                "Configuration reloaded.",
                true,
                true,
                true);
        }
        catch (Exception ex) when (
            ex is FileNotFoundException
            or FormatException
            or InvalidDataException
            or InvalidOperationException)
        {
            return Status($"Reload failed: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public LauncherGridUpdate MoveSelection(int delta)
        => SetSelection(_state.SelectedIndex + delta);

    /// <inheritdoc />
    public LauncherGridUpdate SetSelection(int index)
        => !_state.SetSelection(index)
            ? LauncherGridUpdate.None
            : new LauncherGridUpdate(null, true, false, true);

    private static LauncherGridUpdate Status(string status)
        => new(status, false, false, true);

    private readonly ILauncherGridState _state;
    private readonly ILauncherActions _launcherActions;
    private readonly Func<LauncherOptions> _reloadOptions;
}
