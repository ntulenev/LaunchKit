using Models;

namespace Infrastructure;

/// <summary>
/// Formats launcher grid text independently from Terminal.Gui rendering.
/// </summary>
internal sealed class LauncherTileFormatter : ILauncherTileFormatter
{
    /// <inheritdoc />
    public string BuildSummary(ILauncherGridState state, LayoutState layout)
    {
        ArgumentNullException.ThrowIfNull(state);
        ArgumentNullException.ThrowIfNull(layout);

        if (!state.Options.HasApplications)
        {
            return "Items: 0  Page: 1/1  Columns: 1  Selected: none";
        }

        var applications = state.ActiveApplications;
        var selectedIndex = state.SelectedIndex;
        var pages = layout.CalculatePages(applications.Count);
        var currentPage = layout.GetPage(selectedIndex);
        var selectedName = applications[selectedIndex].Name.Value;
        var activeTab = state.ActiveTab.Value;

        return $"Tab: {activeTab}  Items: {applications.Count}/{state.Options.Count}  Page: {currentPage + 1}/{pages}  " +
               $"Columns: {layout.Columns}  Selected: {selectedName}";
    }

    /// <inheritdoc />
    public string BuildTabStrip(ILauncherGridState state)
    {
        ArgumentNullException.ThrowIfNull(state);

        if (state.Options.Tabs.Count == 0)
        {
            return "Tabs: none";
        }

        return "Tabs: " + string.Join("  ", state.Options.Tabs.Select(tab =>
            tab == state.ActiveTab
                ? $"[{tab.Value}]"
                : tab.Value));
    }

    /// <inheritdoc />
    public string GetPathDisplayText(LauncherOptions options, ApplicationEntry application)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(application);

        return options.ShowFullPath
            ? application.Path.Value
            : application.Path.GetDisplayName();
    }

    /// <inheritdoc />
    public string Clip(string value, int width)
    {
        return string.IsNullOrEmpty(value) || value.Length <= width
            ? value
            : width <= 3
            ? value[..width]
            : $"{value[..(width - 3)]}...";
    }

    /// <inheritdoc />
    public string FormatAvailability(ApplicationAvailability availability)
        => availability switch
        {
            ApplicationAvailability.Ready => "Ready",
            ApplicationAvailability.PathNotFound => "Path not found",
            _ => availability.ToString()
        };
}
