using Models;

using Terminal.Gui;

namespace Infrastructure;

/// <summary>
/// Displays launcher entries as a paged tile grid.
/// </summary>
internal sealed class LauncherGridView : View
{
    /// <summary>
    /// Initializes a launcher grid view.
    /// </summary>
    /// <param name="state">Grid state used by the view.</param>
    /// <param name="controller">Controller used to execute grid commands.</param>
    /// <param name="shortcutResolver">Shortcut resolver used for alternate keyboard layouts.</param>
    /// <param name="tileFormatter">Formatter used to create grid display text.</param>
    internal LauncherGridView(
        ILauncherGridState state,
        ILauncherGridController controller,
        ILauncherShortcutResolver shortcutResolver,
        ILauncherTileFormatter tileFormatter)
    {
        _state = state ?? throw new ArgumentNullException(nameof(state));
        _controller = controller ?? throw new ArgumentNullException(nameof(controller));
        _shortcutResolver = shortcutResolver ?? throw new ArgumentNullException(nameof(shortcutResolver));
        _tileFormatter = tileFormatter ?? throw new ArgumentNullException(nameof(tileFormatter));

        CanFocus = true;
    }

    /// <summary>
    /// Raised when the selected application changes.
    /// </summary>
    public event EventHandler<string>? SelectionChanged;

    /// <summary>
    /// Raised when a launcher status message changes.
    /// </summary>
    public event EventHandler<string>? StatusChanged;

    /// <summary>
    /// Raised when the active tab changes.
    /// </summary>
    public event EventHandler<string>? TabChanged;

    /// <summary>
    /// Builds a text summary for the current selection and page.
    /// </summary>
    /// <returns>A summary line describing the grid state.</returns>
    public string BuildSummary()
        => _tileFormatter.BuildSummary(_state, BuildLayoutState());

    /// <summary>
    /// Builds the text representation of available tabs.
    /// </summary>
    /// <returns>A tab strip line.</returns>
    public string BuildTabStrip()
        => _tileFormatter.BuildTabStrip(_state);

    /// <summary>
    /// Launches the currently selected application entry.
    /// </summary>
    public void LaunchSelection()
        => Apply(_controller.LaunchSelection());

    /// <summary>
    /// Launches the currently selected application entry with administrator privileges.
    /// </summary>
    public void LaunchSelectionAsAdmin()
        => Apply(_controller.LaunchSelectionAsAdmin());

    /// <summary>
    /// Opens the containing folder for the currently selected application entry.
    /// </summary>
    public void OpenSelectionFolder()
        => Apply(_controller.OpenSelectionFolder());

    /// <summary>
    /// Switches to the next application tab.
    /// </summary>
    public void NextTab()
        => Apply(_controller.NextTab());

    /// <summary>
    /// Reloads launcher options while preserving the current selection when possible.
    /// </summary>
    public void ReloadSelection()
        => Apply(_controller.ReloadSelection());

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
            _tileFormatter.FormatAvailability(application.GetAvailability())
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
                "|" + _tileFormatter.Clip(content, innerWidth).PadRight(innerWidth) + "|",
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

    private void MoveSelection(int delta) => Apply(_controller.MoveSelection(delta));

    private void SetSelection(int index)
        => Apply(_controller.SetSelection(index));

    private void Apply(LauncherGridUpdate update)
    {
        if (update.TabChanged)
        {
            NotifyTabChanged();
        }

        if (update.SelectionChanged)
        {
            NotifySelectionChanged();
        }

        if (update.HasStatus)
        {
            StatusChanged?.Invoke(this, update.Status!);
        }

        if (update.NeedsDisplay)
        {
            SetNeedsDisplay();
        }
    }

    private void NotifySelectionChanged() => SelectionChanged?.Invoke(this, BuildSummary());

    private void NotifyTabChanged() => TabChanged?.Invoke(this, BuildTabStrip());

    internal string GetPathDisplayText(ApplicationEntry application)
        => _tileFormatter.GetPathDisplayText(_state.Options, application);

    private LayoutState BuildLayoutState()
        => _state.CreateLayoutState(Bounds.Width, Bounds.Height);

    private readonly ILauncherGridController _controller;
    private readonly ILauncherShortcutResolver _shortcutResolver;
    private readonly ILauncherGridState _state;
    private readonly ILauncherTileFormatter _tileFormatter;

}
