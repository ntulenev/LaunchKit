using Abstractions;

using Models;

namespace Infrastructure;

/// <summary>
/// Handles launcher grid commands independently from Terminal.Gui input and rendering.
/// </summary>
internal sealed class LauncherGridController
{
    public LauncherGridController(
        LauncherGridState state,
        ILauncherActions launcherActions,
        Func<LauncherOptions> reloadOptions)
    {
        _state = state ?? throw new ArgumentNullException(nameof(state));
        _launcherActions = launcherActions ?? throw new ArgumentNullException(nameof(launcherActions));
        _reloadOptions = reloadOptions ?? throw new ArgumentNullException(nameof(reloadOptions));
    }

    public LauncherGridUpdate LaunchSelection()
    {
        var application = _state.SelectedApplication;
        return application is null
            ? Status("No applications configured.")
            : Status(_launcherActions.Launch(application));
    }

    public LauncherGridUpdate LaunchSelectionAsAdmin()
    {
        var application = _state.SelectedApplication;
        return application is null
            ? Status("No applications configured.")
            : Status(_launcherActions.LaunchAsAdmin(application));
    }

    public LauncherGridUpdate OpenSelectionFolder()
    {
        var application = _state.SelectedApplication;
        return application is null
            ? Status("No applications configured.")
            : Status(_launcherActions.OpenContainingFolder(application));
    }

    public LauncherGridUpdate NextTab()
    {
        return !_state.NextTab()
            ? LauncherGridUpdate.None
            : new LauncherGridUpdate($"Tab: {_state.ActiveTab.Value}", SelectionChanged: true, TabChanged: true, NeedsDisplay: true);
    }

    public LauncherGridUpdate ReloadSelection()
    {
        try
        {
            _state.Reload(_reloadOptions());

            return new LauncherGridUpdate(
                "Configuration reloaded.",
                SelectionChanged: true,
                TabChanged: true,
                NeedsDisplay: true);
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

    public LauncherGridUpdate MoveSelection(int delta)
        => SetSelection(_state.SelectedIndex + delta);

    public LauncherGridUpdate SetSelection(int index)
        => !_state.SetSelection(index)
            ? LauncherGridUpdate.None
            : new LauncherGridUpdate(null, SelectionChanged: true, TabChanged: false, NeedsDisplay: true);

    private static LauncherGridUpdate Status(string status)
        => new(status, SelectionChanged: false, TabChanged: false, NeedsDisplay: true);

    private readonly LauncherGridState _state;
    private readonly ILauncherActions _launcherActions;
    private readonly Func<LauncherOptions> _reloadOptions;
}
