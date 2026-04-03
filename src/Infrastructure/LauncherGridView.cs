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
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(launcherActions);
        ArgumentNullException.ThrowIfNull(reloadOptions);

        _options = options;
        _launcherActions = launcherActions;
        _reloadOptions = reloadOptions;

        CanFocus = true;
    }

    public event EventHandler<string>? SelectionChanged;

    public event EventHandler<string>? StatusChanged;

    /// <summary>
    /// Builds a text summary for the current selection and page.
    /// </summary>
    /// <returns>A summary line describing the grid state.</returns>
    public string BuildSummary()
    {
        if (!_options.HasApplications)
        {
            return "Items: 0  Page: 1/1  Columns: 1  Selected: none";
        }

        var layout = BuildLayoutState();
        var pages = layout.CalculatePages(_options.Count);
        var currentPage = layout.GetPage(_selectedIndex);
        var selectedName = _options.GetApplication(_selectedIndex).Name.Value;

        return $"Items: {_options.Count}  Page: {currentPage + 1}/{pages}  " +
               $"Columns: {layout.Columns}  Selected: {selectedName}";
    }

    /// <summary>
    /// Launches the currently selected application entry.
    /// </summary>
    public void LaunchSelection()
    {
        if (!_options.HasApplications)
        {
            UpdateStatus("No applications configured.");
            return;
        }

        UpdateStatus(_launcherActions.Launch(_options.GetApplication(_selectedIndex)));
    }

    /// <summary>
    /// Launches the currently selected application entry with administrator privileges.
    /// </summary>
    public void LaunchSelectionAsAdmin()
    {
        if (!_options.HasApplications)
        {
            UpdateStatus("No applications configured.");
            return;
        }

        UpdateStatus(_launcherActions.LaunchAsAdmin(_options.GetApplication(_selectedIndex)));
    }

    /// <summary>
    /// Opens the containing folder for the currently selected application entry.
    /// </summary>
    public void OpenSelectionFolder()
    {
        if (!_options.HasApplications)
        {
            UpdateStatus("No applications configured.");
            return;
        }

        UpdateStatus(_launcherActions.OpenContainingFolder(_options.GetApplication(_selectedIndex)));
    }

    /// <summary>
    /// Reloads launcher options while preserving the current selection when possible.
    /// </summary>
    public void ReloadSelection()
    {
        try
        {
            _options = _reloadOptions();
            _selectedIndex = _options.ClampSelection(_selectedIndex);

            UpdateStatus("Configuration reloaded.");
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
                SetSelection(_options.Count - 1);
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

        if (!_options.HasApplications)
        {
            return;
        }

        var layout = BuildLayoutState();
        var currentPage = layout.GetPage(_selectedIndex);
        var firstIndex = layout.GetFirstIndexForPage(currentPage);
        var lastIndexExclusive = Math.Min(firstIndex + layout.ItemsPerPage, _options.Count);

        for (var index = firstIndex; index < lastIndexExclusive; index++)
        {
            var pageOffset = layout.GetPageOffset(index);
            var row = layout.GetRow(pageOffset);
            var column = layout.GetColumn(pageOffset);
            var x = layout.GetTileX(column);
            var y = layout.GetTileY(row);

            DrawTile(x, y, layout, _options.GetApplication(index), index == _selectedIndex, index);
        }
    }

    /// <summary>
    /// Moves the cursor to the currently selected tile.
    /// </summary>
    public override void PositionCursor()
    {
        if (!_options.HasApplications)
        {
            return;
        }

        var layout = BuildLayoutState();
        var pageOffset = layout.GetPageOffset(_selectedIndex);
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
            FormatAvailability(application.GetAvailability())
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
                "|" + Clip(content, innerWidth).PadRight(innerWidth) + "|",
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

    private void MoveSelection(int delta) => SetSelection(_selectedIndex + delta);

    private void SetSelection(int index)
    {
        if (!_options.HasApplications)
        {
            return;
        }

        var clamped = _options.ClampSelection(index);
        if (clamped == _selectedIndex)
        {
            return;
        }

        _selectedIndex = clamped;
        NotifySelectionChanged();
        SetNeedsDisplay();
    }

    private void NotifySelectionChanged() => SelectionChanged?.Invoke(this, BuildSummary());

    private void UpdateStatus(string status)
    {
        StatusChanged?.Invoke(this, status);
        SetNeedsDisplay();
    }

    private static string Clip(string value, int width)
    {
        return string.IsNullOrEmpty(value) || value.Length <= width
            ? value
            : width <= 3
            ? value[..width]
            : $"{value[..(width - 3)]}...";
    }

    private static string FormatAvailability(ApplicationAvailability availability)
        => availability switch
        {
            ApplicationAvailability.Ready => "Ready",
            ApplicationAvailability.PathNotFound => "Path not found",
            _ => availability.ToString()
        };

    internal string GetPathDisplayText(ApplicationEntry application)
    {
        ArgumentNullException.ThrowIfNull(application);

        return _options.ShowFullPath
            ? application.Path.Value
            : application.Path.GetDisplayName();
    }

    private LayoutState BuildLayoutState()
        => _options.CreateLayoutState(Bounds.Width, Bounds.Height);

    private readonly ILauncherActions _launcherActions;
    private readonly Func<LauncherOptions> _reloadOptions;

    private LauncherOptions _options;
    private int _selectedIndex;
}
