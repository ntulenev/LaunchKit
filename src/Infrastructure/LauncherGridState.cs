using System.Collections.ObjectModel;

using Models;

namespace Infrastructure;

/// <summary>
/// Tracks launcher grid tab and selection state independently from Terminal.Gui.
/// </summary>
internal sealed class LauncherGridState : ILauncherGridState
{
    /// <summary>
    /// Initializes a new grid state for launcher options.
    /// </summary>
    /// <param name="options">Launcher options to track.</param>
    public LauncherGridState(LauncherOptions options)
    {
        Options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <inheritdoc />
    public LauncherOptions Options { get; private set; }

    /// <inheritdoc />
    public ApplicationTab ActiveTab {
        get
        {
            NormalizeActiveTabIndex();

            return Options.Tabs.Count == 0
                ? new ApplicationTab(null)
                : Options.Tabs[_activeTabIndex];
        }
    }

    /// <inheritdoc />
    public ReadOnlyCollection<ApplicationEntry> ActiveApplications
        => Options.Tabs.Count == 0
            ? _emptyApplications
            : Options.GetApplicationsForTab(ActiveTab);

    /// <inheritdoc />
    public ApplicationEntry? SelectedApplication
        => ActiveApplications.Count == 0
            ? null
            : ActiveApplications[SelectedIndex];

    /// <inheritdoc />
    public int SelectedIndex {
        get
        {
            var applications = ActiveApplications;
            if (applications.Count == 0)
            {
                return 0;
            }

            var tab = ActiveTab.Value;
            _ = _selectedIndicesByTab.TryGetValue(tab, out var index);
            var clamped = Math.Clamp(index, 0, applications.Count - 1);
            _selectedIndicesByTab[tab] = clamped;

            return clamped;
        }
    }

    /// <inheritdoc />
    public bool MoveSelection(int delta) => SetSelection(SelectedIndex + delta);

    /// <inheritdoc />
    public bool SetSelection(int index)
    {
        var applications = ActiveApplications;
        if (applications.Count == 0)
        {
            return false;
        }

        var tab = ActiveTab.Value;
        var clamped = Math.Clamp(index, 0, applications.Count - 1);
        if (_selectedIndicesByTab.TryGetValue(tab, out var existing) && existing == clamped)
        {
            return false;
        }

        _selectedIndicesByTab[tab] = clamped;
        return true;
    }

    /// <inheritdoc />
    public bool NextTab()
    {
        if (Options.Tabs.Count <= 1)
        {
            return false;
        }

        _activeTabIndex = (_activeTabIndex + 1) % Options.Tabs.Count;
        return true;
    }

    /// <inheritdoc />
    public void Reload(LauncherOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var activeTabName = Options.Tabs.Count > 0
            ? ActiveTab.Value
            : null;

        Options = options;
        PruneSelectionCache();
        RestoreActiveTab(activeTabName);
        _ = SelectedIndex;
    }

    /// <inheritdoc />
    public LayoutState CreateLayoutState(int availableWidth, int availableHeight)
        => Options.Layout.CalculateState(availableWidth, availableHeight, ActiveApplications.Count);

    private void NormalizeActiveTabIndex()
    {
        _activeTabIndex = Options.Tabs.Count == 0
            ? 0
            : Math.Clamp(_activeTabIndex, 0, Options.Tabs.Count - 1);
    }

    private void RestoreActiveTab(string? activeTabName)
    {
        if (Options.Tabs.Count == 0)
        {
            _activeTabIndex = 0;
            return;
        }

        if (string.IsNullOrWhiteSpace(activeTabName))
        {
            _activeTabIndex = 0;
            return;
        }

        var matchingIndex = Options.Tabs
            .Select((tab, index) => new { tab, index })
            .FirstOrDefault(item => string.Equals(item.tab.Value, activeTabName, StringComparison.Ordinal));

        _activeTabIndex = matchingIndex?.index ?? 0;
    }

    private void PruneSelectionCache()
    {
        var validTabs = Options.Tabs
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

    private readonly Dictionary<string, int> _selectedIndicesByTab = [];
    private static readonly ReadOnlyCollection<ApplicationEntry> _emptyApplications = new([]);

    private int _activeTabIndex;
}
