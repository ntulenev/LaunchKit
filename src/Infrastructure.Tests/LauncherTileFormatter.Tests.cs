using FluentAssertions;

using Models;

namespace Infrastructure.Tests;

public sealed class LauncherTileFormatterTests
{
    [Fact(DisplayName = "The summary reports empty launcher state.")]
    [Trait("Category", "Unit")]
    public void BuildSummaryShouldReportEmptyState()
    {
        // Arrange
        var state = new LauncherGridState(new LauncherOptions(new LayoutOptions(), []));
        var layout = new LayoutState(1, 34, 6, 2, 1, 1);

        // Act
        var summary = LauncherTileFormatter.BuildSummary(state, layout);

        // Assert
        summary.Should().Be("Items: 0  Page: 1/1  Columns: 1  Selected: none");
    }

    [Fact(DisplayName = "The summary reports active tab, page, columns, and selected application.")]
    [Trait("Category", "Unit")]
    public void BuildSummaryShouldReportActiveSelection()
    {
        // Arrange
        var state = new LauncherGridState(new LauncherOptions(
            new LayoutOptions(),
            [
                CreateApplication("JiraMetrics", "Metrics"),
                CreateApplication("Keyboard US", "Productivity"),
                CreateApplication("Keyboard GB", "Productivity")
            ]));
        _ = state.NextTab();
        _ = state.SetSelection(1);
        var layout = new LayoutState(1, 34, 6, 2, 1, 1);

        // Act
        var summary = LauncherTileFormatter.BuildSummary(state, layout);

        // Assert
        summary.Should().Be("Tab: Productivity  Items: 2/3  Page: 2/2  Columns: 1  Selected: Keyboard GB");
    }

    [Fact(DisplayName = "The tab strip reports no tabs when the launcher is empty.")]
    [Trait("Category", "Unit")]
    public void BuildTabStripShouldReportEmptyTabs()
    {
        // Arrange
        var state = new LauncherGridState(new LauncherOptions(new LayoutOptions(), []));

        // Act
        var tabs = LauncherTileFormatter.BuildTabStrip(state);

        // Assert
        tabs.Should().Be("Tabs: none");
    }

    [Fact(DisplayName = "The tab strip highlights the active tab.")]
    [Trait("Category", "Unit")]
    public void BuildTabStripShouldHighlightActiveTab()
    {
        // Arrange
        var state = new LauncherGridState(new LauncherOptions(
            new LayoutOptions(),
            [
                CreateApplication("JiraMetrics", "Metrics"),
                CreateApplication("Keyboard US", "Productivity")
            ]));
        _ = state.NextTab();

        // Act
        var tabs = LauncherTileFormatter.BuildTabStrip(state);

        // Assert
        tabs.Should().Be("Tabs: Metrics  [Productivity]");
    }

    [Theory(DisplayName = "The formatter clips text to the requested width.")]
    [Trait("Category", "Unit")]
    [InlineData("LaunchKit", 20, "LaunchKit")]
    [InlineData("LaunchKit", 3, "Lau")]
    [InlineData("LaunchKit", 6, "Lau...")]
    public void ClipShouldTrimLongText(string value, int width, string expected)
    {
        // Act
        var clipped = LauncherTileFormatter.Clip(value, width);

        // Assert
        clipped.Should().Be(expected);
    }

    [Theory(DisplayName = "The formatter converts availability to display text.")]
    [Trait("Category", "Unit")]
    [InlineData(ApplicationAvailability.Ready, "Ready")]
    [InlineData(ApplicationAvailability.PathNotFound, "Path not found")]
    public void FormatAvailabilityShouldReturnDisplayText(ApplicationAvailability availability, string expected)
    {
        // Act
        var display = LauncherTileFormatter.FormatAvailability(availability);

        // Assert
        display.Should().Be(expected);
    }

    [Fact(DisplayName = "The formatter returns the full path when configured.")]
    [Trait("Category", "Unit")]
    public void GetPathDisplayTextShouldReturnFullPathWhenConfigured()
    {
        // Arrange
        var application = ApplicationEntry.Create("LaunchKit", @"C:\Tools\LaunchKit.exe");
        var options = new LauncherOptions(new LayoutOptions(), [application], showFullPath: true);

        // Act
        var display = LauncherTileFormatter.GetPathDisplayText(options, application);

        // Assert
        display.Should().Be(application.Path.Value);
    }

    [Fact(DisplayName = "The formatter returns the executable name when full path display is disabled.")]
    [Trait("Category", "Unit")]
    public void GetPathDisplayTextShouldReturnDisplayNameWhenConfigured()
    {
        // Arrange
        var application = ApplicationEntry.Create("LaunchKit", @"C:\Tools\LaunchKit.exe");
        var options = new LauncherOptions(new LayoutOptions(), [application], showFullPath: false);

        // Act
        var display = LauncherTileFormatter.GetPathDisplayText(options, application);

        // Assert
        display.Should().Be("LaunchKit.exe");
    }

    private static ApplicationEntry CreateApplication(string name, string tab)
        => ApplicationEntry.Create(name, "dotnet", tab: tab);
}
