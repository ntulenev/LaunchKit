using Abstractions;

using Models;

using Terminal.Gui;

namespace Infrastructure;

/// <summary>
/// Displays launcher entries as a paged tile grid.
/// </summary>
internal sealed class LauncherGridView : View
{
    /// <summary>
    /// Initializes a new grid view for launcher entries.
    /// </summary>
    /// <param name="options">Initial launcher options to display.</param>
    /// <param name="launcherActions">Actions available for the selected entry.</param>
    /// <param name="reloadOptions">Delegate that reloads the launcher configuration.</param>
    public LauncherGridView(
        LauncherOptions options,
        ILauncherActions launcherActions,
        Func<LauncherOptions> reloadOptions)
        : this(options, launcherActions, reloadOptions, new LauncherShortcutResolver(new WindowsKeyboardState()))
    {
    }

    internal LauncherGridView(
        LauncherOptions options,
        ILauncherActions launcherActions,
        Func<LauncherOptions> reloadOptions,
        ILauncherShortcutResolver shortcutResolver)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(launcherActions);
        ArgumentNullException.ThrowIfNull(reloadOptions);
        ArgumentNullException.ThrowIfNull(shortcutResolver);

        _state = new LauncherGridState(options);
        _launcherActions = launcherActions;
        _reloadOptions = reloadOptions;
        _shortcutResolver = shortcutResolver;

        CanFocus = true;
    }

    public event EventHandler<string>? SelectionChanged;

    public event EventHandler<string>? StatusChanged;

    public event EventHandler<string>? TabChanged;

    /// <summary>
    /// Builds a text summary for the current selection and page.
    /// </summary>
    /// <returns>A summary line describing the grid state.</returns>
    public string BuildSummary()
        => LauncherTileFormatter.BuildSummary(_state, BuildLayoutState());

    public string BuildTabStrip()
        => LauncherTileFormatter.BuildTabStrip(_state);

    /// <summary>
    /// Launches the currently selected application entry.
    /// </summary>
    public void LaunchSelection()
    {
        var application = _state.SelectedApplication;
        if (application is null)
        {
            UpdateStatus("No applications configured.");
            return;
        }

        UpdateStatus(_launcherActions.Launch(application));
    }

    /// <summary>
    /// Launches the currently selected application entry with administrator privileges.
    /// </summary>
    public void LaunchSelectionAsAdmin()
    {
        var application = _state.SelectedApplication;
        if (application is null)
        {
            UpdateStatus("No applications configured.");
            return;
        }

        UpdateStatus(_launcherActions.LaunchAsAdmin(application));
    }

    /// <summary>
    /// Opens the containing folder for the currently selected application entry.
    /// </summary>
    public void OpenSelectionFolder()
    {
        var application = _state.SelectedApplication;
        if (application is null)
        {
            UpdateStatus("No applications configured.");
            return;
        }

        UpdateStatus(_launcherActions.OpenContainingFolder(application));
    }

    /// <summary>
    /// Switches to the next application tab.
    /// </summary>
    public void NextTab()
    {
        if (!_state.NextTab())
        {
            return;
        }

        NotifyTabChanged();
        NotifySelectionChanged();
        UpdateStatus($"Tab: {_state.ActiveTab.Value}");
    }

    /// <summary>
    /// Reloads launcher options while preserving the current selection when possible.
    /// </summary>
    public void ReloadSelection()
    {
        try
        {
            _state.Reload(_reloadOptions());

            UpdateStatus("Configuration reloaded.");
            NotifyTabChanged();
            NotifySelectionChanged();
            SetNeedsDisplay();
        }
        catch (Exception ex) when (
            ex is FileNotFoundException
            or FormatException
            or InvalidDataException
            or InvalidOperationException)
        {
            UpdateStatus($"Reload failed: {ex.Message}");
        }
    }

    /// <summary>
    /// Processes keyboard input for navigation and actions.
    /// </summary>
    /// <param name="keyEvent">The key event to process.</param>
    /// <returns><see langword="true"/> when the key is handled; otherwise, <see langword="false"/>.</returns>
    public override bool ProcessKey(KeyEvent keyEvent)
    {
        switch (keyEvent.Key)
        {
            case Key.Tab:
                NextTab();
                return true;

            case Key.CursorLeft:
                MoveSelection(-1);
                return true;

            case Key.CursorRight:
                MoveSelection(1);
                return true;

            case Key.CursorUp:
                MoveSelection(-BuildLayoutState().Columns);
                return true;

            case Key.CursorDown:
                MoveSelection(BuildLayoutState().Columns);
                return true;

            case Key.Home:
                SetSelection(0);
                return true;

            case Key.End:
                SetSelection(_state.ActiveApplications.Count - 1);
                return true;

            case Key.PageUp:
                MoveSelection(-BuildLayoutState().ItemsPerPage);
                return true;

            case Key.PageDown:
                MoveSelection(BuildLayoutState().ItemsPerPage);
                return true;

            case Key.Enter:
                LaunchSelection();
                return true;

            case Key.A:
            case Key.a:
                LaunchSelectionAsAdmin();
                return true;

            case Key.O:
            case Key.o:
                OpenSelectionFolder();
                return true;

            case var key when _shortcutResolver.IsAdminLaunchShortcut(key):
                LaunchSelectionAsAdmin();
                return true;

            case var key when _shortcutResolver.IsOpenFolderShortcut(key):
                OpenSelectionFolder();
                return true;

            case Key.F5:
                ReloadSelection();
                return true;

            case Key.Esc:
                Application.RequestStop();
                return true;

            default:
                return base.ProcessKey(keyEvent);
        }
    }

    /// <summary>
    /// Redraws the grid contents within the provided bounds.
    /// </summary>
    /// <param name="bounds">Bounds that should be redrawn.</param>
    public override void Redraw(Rect bounds)
    {
        Driver.SetAttribute(ColorScheme.Normal);
        Clear();

        var applications = _state.ActiveApplications;
        if (applications.Count == 0)
        {
            return;
        }

        var layout = BuildLayoutState();
        var selectedIndex = _state.SelectedIndex;
        var currentPage = layout.GetPage(selectedIndex);
        var firstIndex = layout.GetFirstIndexForPage(currentPage);
        var lastIndexExclusive = Math.Min(firstIndex + layout.ItemsPerPage, applications.Count);

        for (var index = firstIndex; index < lastIndexExclusive; index++)
        {
            var pageOffset = layout.GetPageOffset(index);
            var row = layout.GetRow(pageOffset);
            var column = layout.GetColumn(pageOffset);
            var x = layout.GetTileX(column);
            var y = layout.GetTileY(row);

            DrawTile(x, y, layout, applications[index], index == selectedIndex, index);
        }
    }

    /// <summary>
    /// Moves the cursor to the currently selected tile.
    /// </summary>
    public override void PositionCursor()
    {
        var applications = _state.ActiveApplications;
        if (applications.Count == 0)
        {
            return;
        }

        var layout = BuildLayoutState();
        var pageOffset = layout.GetPageOffset(_state.SelectedIndex);
        var row = layout.GetRow(pageOffset);
        var column = layout.GetColumn(pageOffset);
        var x = layout.GetTileX(column) + 1;
        var y = layout.GetTileY(row) + 1;

        Move(Math.Max(0, x), Math.Max(0, y));
    }

    private void DrawTile(
        int x,
        int y,
        LayoutState layout,
        ApplicationEntry application,
        bool isSelected,
        int index)
    {
        var innerWidth = Math.Max(1, layout.TileWidth - 2);
        var borderAttribute = isSelected ? ColorScheme.Focus : ColorScheme.Normal;
        var textAttribute = isSelected ? ColorScheme.Focus : ColorScheme.Normal;
        var contentLines = new[]
        {
            $"[{index + 1}] {application.Name.Value}",
            application.Description.Value,
            GetPathDisplayText(application),
            LauncherTileFormatter.FormatAvailability(application.GetAvailability())
        };

        WriteLine(x, y, "+" + new string('-', innerWidth) + "+", borderAttribute);

        var bodyHeight = Math.Max(1, layout.TileHeight - 2);
        for (var lineIndex = 0; lineIndex < bodyHeight; lineIndex++)
        {
            var content = lineIndex < contentLines.Length
                ? contentLines[lineIndex]
                : string.Empty;

            WriteLine(
                x,
                y + 1 + lineIndex,
                "|" + LauncherTileFormatter.Clip(content, innerWidth).PadRight(innerWidth) + "|",
                textAttribute);
        }

        WriteLine(x, y + layout.TileHeight - 1, "+" + new string('-', innerWidth) + "+", borderAttribute);
    }

    private void WriteLine(int x, int y, string text, Terminal.Gui.Attribute attribute)
    {
        if (y < 0 || y >= Bounds.Height)
        {
            return;
        }

        Move(x, y);
        Driver.SetAttribute(attribute);
        Driver.AddStr(text);
    }

    private void MoveSelection(int delta) => SetSelection(_state.SelectedIndex + delta);

    private void SetSelection(int index)
    {
        if (!_state.SetSelection(index))
        {
            return;
        }

        NotifySelectionChanged();
        SetNeedsDisplay();
    }

    private void NotifySelectionChanged() => SelectionChanged?.Invoke(this, BuildSummary());

    private void NotifyTabChanged() => TabChanged?.Invoke(this, BuildTabStrip());

    private void UpdateStatus(string status)
    {
        StatusChanged?.Invoke(this, status);
        SetNeedsDisplay();
    }

    internal string GetPathDisplayText(ApplicationEntry application)
        => LauncherTileFormatter.GetPathDisplayText(_state.Options, application);

    private LayoutState BuildLayoutState()
        => _state.CreateLayoutState(Bounds.Width, Bounds.Height);

    private readonly ILauncherActions _launcherActions;
    private readonly Func<LauncherOptions> _reloadOptions;
    private readonly ILauncherShortcutResolver _shortcutResolver;
    private readonly LauncherGridState _state;

}
