using Abstractions;

using Models;

using System.Collections.ObjectModel;

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

    public event EventHandler<string>? TabChanged;

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

        var applications = GetActiveApplications();
        var layout = BuildLayoutState();
        var selectedIndex = GetSelectedIndex();
        var pages = layout.CalculatePages(applications.Count);
        var currentPage = layout.GetPage(selectedIndex);
        var selectedName = applications[selectedIndex].Name.Value;
        var activeTab = GetActiveTab().Value;

        return $"Tab: {activeTab}  Items: {applications.Count}/{_options.Count}  Page: {currentPage + 1}/{pages}  " +
               $"Columns: {layout.Columns}  Selected: {selectedName}";
    }

    public string BuildTabStrip()
    {
        if (_options.Tabs.Count == 0)
        {
            return "Tabs: none";
        }

        return "Tabs: " + string.Join("  ", _options.Tabs.Select((tab, index) =>
            index == _activeTabIndex
                ? $"[{tab.Value}]"
                : tab.Value));
    }

    /// <summary>
    /// Launches the currently selected application entry.
    /// </summary>
    public void LaunchSelection()
    {
        var application = GetSelectedApplication();
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
        var application = GetSelectedApplication();
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
        var application = GetSelectedApplication();
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
        if (_options.Tabs.Count <= 1)
        {
            return;
        }

        _activeTabIndex = (_activeTabIndex + 1) % _options.Tabs.Count;
        NotifyTabChanged();
        NotifySelectionChanged();
        UpdateStatus($"Tab: {GetActiveTab().Value}");
    }

    /// <summary>
    /// Reloads launcher options while preserving the current selection when possible.
    /// </summary>
    public void ReloadSelection()
    {
        try
        {
            var activeTabName = _options.Tabs.Count > 0
                ? GetActiveTab().Value
                : null;

            _options = _reloadOptions();
            PruneSelectionCache();
            RestoreActiveTab(activeTabName);
            _ = GetSelectedIndex();

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
                SetSelection(GetActiveApplications().Count - 1);
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

        var applications = GetActiveApplications();
        if (applications.Count == 0)
        {
            return;
        }

        var layout = BuildLayoutState();
        var selectedIndex = GetSelectedIndex();
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
        var applications = GetActiveApplications();
        if (applications.Count == 0)
        {
            return;
        }

        var layout = BuildLayoutState();
        var pageOffset = layout.GetPageOffset(GetSelectedIndex());
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

    private void MoveSelection(int delta) => SetSelection(GetSelectedIndex() + delta);

    private void SetSelection(int index)
    {
        var applications = GetActiveApplications();
        if (applications.Count == 0)
        {
            return;
        }

        var tab = GetActiveTab().Value;
        var clamped = Math.Clamp(index, 0, applications.Count - 1);
        if (_selectedIndicesByTab.TryGetValue(tab, out var existing) && existing == clamped)
        {
            return;
        }

        _selectedIndicesByTab[tab] = clamped;
        NotifySelectionChanged();
        SetNeedsDisplay();
    }

    private ApplicationEntry? GetSelectedApplication()
    {
        var applications = GetActiveApplications();
        return applications.Count == 0
            ? null
            : applications[GetSelectedIndex()];
    }

    private int GetSelectedIndex()
    {
        var applications = GetActiveApplications();
        if (applications.Count == 0)
        {
            return 0;
        }

        var tab = GetActiveTab().Value;
        _ = _selectedIndicesByTab.TryGetValue(tab, out var index);
        var clamped = Math.Clamp(index, 0, applications.Count - 1);
        _selectedIndicesByTab[tab] = clamped;

        return clamped;
    }

    private ApplicationTab GetActiveTab()
    {
        _activeTabIndex = _options.Tabs.Count == 0
            ? 0
            : Math.Clamp(_activeTabIndex, 0, _options.Tabs.Count - 1);

        return _options.Tabs.Count == 0
            ? new ApplicationTab(null)
            : _options.Tabs[_activeTabIndex];
    }

    private ReadOnlyCollection<ApplicationEntry> GetActiveApplications()
        => _options.Tabs.Count == 0
            ? _emptyApplications
            : _options.GetApplicationsForTab(GetActiveTab());

    private void RestoreActiveTab(string? activeTabName)
    {
        if (_options.Tabs.Count == 0)
        {
            _activeTabIndex = 0;
            return;
        }

        if (string.IsNullOrWhiteSpace(activeTabName))
        {
            _activeTabIndex = 0;
            return;
        }

        var matchingIndex = _options.Tabs
            .Select((tab, index) => new { tab, index })
            .FirstOrDefault(item => string.Equals(item.tab.Value, activeTabName, StringComparison.Ordinal));

        _activeTabIndex = matchingIndex?.index ?? 0;
    }

    private void PruneSelectionCache()
    {
        var validTabs = _options.Tabs
            .Select(tab => tab.Value)
            .ToHashSet(StringComparer.Ordinal);
        var staleTabs = _selectedIndicesByTab.Keys
            .Where(key => !validTabs.Contains(key))
            .ToArray();

        foreach (var staleTab in staleTabs)
        {
            _ = _selectedIndicesByTab.Remove(staleTab);
        }
    }

    private void NotifySelectionChanged() => SelectionChanged?.Invoke(this, BuildSummary());

    private void NotifyTabChanged() => TabChanged?.Invoke(this, BuildTabStrip());

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
        => _options.Layout.CalculateState(Bounds.Width, Bounds.Height, GetActiveApplications().Count);

    private readonly ILauncherActions _launcherActions;
    private readonly Func<LauncherOptions> _reloadOptions;
    private readonly Dictionary<string, int> _selectedIndicesByTab = [];
    private static readonly ReadOnlyCollection<ApplicationEntry> _emptyApplications = new([]);

    private LauncherOptions _options;
    private int _activeTabIndex;
}
