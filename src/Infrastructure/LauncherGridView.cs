using Abstractions;

using Models;

using Terminal.Gui;

namespace Infrastructure;

internal sealed class LauncherGridView : View
{
    private const int TILE_HEIGHT = 5;
    private const int TILE_SPACING = 1;
    private const int TILE_MIN_WIDTH = 28;

    private readonly ILauncherActions _launcherActions;
    private readonly Func<LauncherOptions> _reloadOptions;

    private LauncherOptions _options;
    private int _selectedIndex;

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

    public string BuildSummary()
    {
        var columns = CalculateColumns();
        var pages = CalculatePages(columns);
        var currentPage = _selectedIndex / Math.Max(1, ItemsPerPage(columns));
        var selectedName = _options.Applications[_selectedIndex].Name;

        return $"Items: {_options.Applications.Count}  Page: {currentPage + 1}/{pages}  " +
               $"Columns: {columns}  Selected: {selectedName}";
    }

    public void LaunchSelection()
    {
        UpdateStatus(_launcherActions.Launch(_options.Applications[_selectedIndex]));
    }

    public void OpenSelectionFolder()
    {
        UpdateStatus(_launcherActions.OpenContainingFolder(_options.Applications[_selectedIndex]));
    }

    public void ReloadSelection()
    {
        try
        {
            _options = _reloadOptions();
            _selectedIndex = Math.Min(
                _selectedIndex,
                Math.Max(0, _options.Applications.Count - 1));

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
                MoveSelection(-CalculateColumns());
                return true;

            case Key.CursorDown:
                MoveSelection(CalculateColumns());
                return true;

            case Key.Home:
                SetSelection(0);
                return true;

            case Key.End:
                SetSelection(_options.Applications.Count - 1);
                return true;

            case Key.PageUp:
                MoveSelection(-ItemsPerPage(CalculateColumns()));
                return true;

            case Key.PageDown:
                MoveSelection(ItemsPerPage(CalculateColumns()));
                return true;

            case Key.Enter:
                LaunchSelection();
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

    public override void Redraw(Rect bounds)
    {
        Driver.SetAttribute(ColorScheme.Normal);
        Clear();

        var columns = CalculateColumns();
        var itemsPerPage = ItemsPerPage(columns);
        var currentPage = _selectedIndex / Math.Max(1, itemsPerPage);
        var firstIndex = currentPage * itemsPerPage;
        var lastIndexExclusive = Math.Min(firstIndex + itemsPerPage, _options.Applications.Count);
        var tileWidth = CalculateTileWidth(columns);

        for (var index = firstIndex; index < lastIndexExclusive; index++)
        {
            var pageOffset = index - firstIndex;
            var row = pageOffset / columns;
            var column = pageOffset % columns;
            var x = column * (tileWidth + TILE_SPACING);
            var y = row * (TILE_HEIGHT + TILE_SPACING);

            DrawTile(x, y, tileWidth, _options.Applications[index], index == _selectedIndex, index);
        }
    }

    public override void PositionCursor()
    {
        var columns = CalculateColumns();
        var itemsPerPage = ItemsPerPage(columns);
        var tileWidth = CalculateTileWidth(columns);
        var currentPage = _selectedIndex / Math.Max(1, itemsPerPage);
        var pageOffset = _selectedIndex - (currentPage * itemsPerPage);
        var row = pageOffset / columns;
        var column = pageOffset % columns;
        var x = (column * (tileWidth + TILE_SPACING)) + 1;
        var y = (row * (TILE_HEIGHT + TILE_SPACING)) + 1;

        Move(Math.Max(0, x), Math.Max(0, y));
    }

    private void DrawTile(
        int x,
        int y,
        int width,
        ApplicationEntry application,
        bool isSelected,
        int index)
    {
        var innerWidth = Math.Max(1, width - 2);
        var borderAttribute = isSelected ? ColorScheme.Focus : ColorScheme.Normal;
        var textAttribute = isSelected ? ColorScheme.Focus : ColorScheme.Normal;
        var path = _launcherActions.ResolveValue(application.Path);
        var availability = _launcherActions.IsAvailable(application)
            ? "Ready"
            : "Path not found";

        WriteLine(x, y, "+" + new string('-', innerWidth) + "+", borderAttribute);
        WriteLine(
            x,
            y + 1,
            "|" + Clip($"[{index + 1}] {application.Name}", innerWidth).PadRight(innerWidth) + "|",
            textAttribute);
        WriteLine(
            x,
            y + 2,
            "|" + Clip(application.Description ?? string.Empty, innerWidth).PadRight(innerWidth) + "|",
            textAttribute);
        WriteLine(
            x,
            y + 3,
            "|" + Clip(path, innerWidth).PadRight(innerWidth) + "|",
            textAttribute);
        WriteLine(
            x,
            y + 4,
            "|" + Clip(availability, innerWidth).PadRight(innerWidth) + "|",
            textAttribute);
        WriteLine(x, y + 5, "+" + new string('-', innerWidth) + "+", borderAttribute);
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

    private void MoveSelection(int delta)
    {
        SetSelection(_selectedIndex + delta);
    }

    private void SetSelection(int index)
    {
        if (_options.Applications.Count == 0)
        {
            return;
        }

        var clamped = Math.Clamp(index, 0, _options.Applications.Count - 1);
        if (clamped == _selectedIndex)
        {
            return;
        }

        _selectedIndex = clamped;
        NotifySelectionChanged();
        SetNeedsDisplay();
    }

    private void NotifySelectionChanged()
    {
        SelectionChanged?.Invoke(this, BuildSummary());
    }

    private void UpdateStatus(string status)
    {
        StatusChanged?.Invoke(this, status);
        SetNeedsDisplay();
    }

    private int CalculateColumns()
    {
        var availableWidth = Math.Max(1, Bounds.Width);
        var configuredColumns = Math.Max(1, _options.Layout.Columns);
        var tileWidth = Math.Max(TILE_MIN_WIDTH, _options.Layout.TileWidth);
        var columns = Math.Max(1, availableWidth / Math.Max(1, tileWidth + TILE_SPACING));

        return Math.Max(1, Math.Min(configuredColumns, Math.Min(columns, _options.Applications.Count)));
    }

    private int CalculatePages(int columns)
        => Math.Max(
            1,
            (int)Math.Ceiling(_options.Applications.Count / (double)Math.Max(1, ItemsPerPage(columns))));

    private int CalculateTileWidth(int columns)
    {
        var availableWidth = Math.Max(1, Bounds.Width);
        var totalSpacing = Math.Max(0, (columns - 1) * TILE_SPACING);
        var width = (availableWidth - totalSpacing) / Math.Max(1, columns);

        return Math.Max(TILE_MIN_WIDTH, width);
    }

    private int ItemsPerPage(int columns)
    {
        var rows = Math.Max(1, Math.Max(1, Bounds.Height) / (TILE_HEIGHT + TILE_SPACING));
        return Math.Max(1, rows * columns);
    }

    private static string Clip(string value, int width)
    {
        if (string.IsNullOrEmpty(value) || value.Length <= width)
        {
            return value;
        }

        return width <= 3
            ? value[..width]
            : $"{value[..(width - 3)]}...";
    }
}
