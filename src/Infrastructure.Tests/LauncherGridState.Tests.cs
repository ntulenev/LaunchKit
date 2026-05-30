using FluentAssertions;

using Models;

namespace Infrastructure.Tests;

public sealed class LauncherGridStateTests
{
    [Fact(DisplayName = "The state exposes no active applications when no options are configured.")]
    [Trait("Category", "Unit")]
    public void ActiveApplicationsShouldBeEmptyWhenOptionsHaveNoApplications()
    {
        // Arrange
        var state = new LauncherGridState(new LauncherOptions(new LayoutOptions(), []));

        // Act
        var applications = state.ActiveApplications;

        // Assert
        applications.Should().BeEmpty();
        state.SelectedApplication.Should().BeNull();
        state.SelectedIndex.Should().Be(0);
        state.ActiveTab.Value.Should().Be(ApplicationTab.DefaultValue);
    }

    [Fact(DisplayName = "The state switches active tabs and exposes applications for the active tab.")]
    [Trait("Category", "Unit")]
    public void NextTabShouldSwitchActiveApplications()
    {
        // Arrange
        var first = CreateApplication("JiraMetrics", "Metrics");
        var second = CreateApplication("Keyboard US", "Productivity");
        var state = new LauncherGridState(CreateOptions(first, second));

        // Act
        var switched = state.NextTab();

        // Assert
        switched.Should().BeTrue();
        state.ActiveTab.Value.Should().Be("Productivity");
        state.ActiveApplications.Should().ContainSingle().Which.Should().BeSameAs(second);
    }

    [Fact(DisplayName = "The state does not switch tabs when only one tab is configured.")]
    [Trait("Category", "Unit")]
    public void NextTabShouldReturnFalseWhenOnlyOneTabExists()
    {
        // Arrange
        var state = new LauncherGridState(CreateOptions(CreateApplication("JiraMetrics", "Metrics")));

        // Act
        var switched = state.NextTab();

        // Assert
        switched.Should().BeFalse();
        state.ActiveTab.Value.Should().Be("Metrics");
    }

    [Fact(DisplayName = "The state clamps selection to the active tab application range.")]
    [Trait("Category", "Unit")]
    public void SetSelectionShouldClampSelection()
    {
        // Arrange
        var first = CreateApplication("One", "Metrics");
        var second = CreateApplication("Two", "Metrics");
        var state = new LauncherGridState(CreateOptions(first, second));

        // Act
        var changed = state.SetSelection(10);

        // Assert
        changed.Should().BeTrue();
        state.SelectedIndex.Should().Be(1);
        state.SelectedApplication.Should().BeSameAs(second);
    }

    [Fact(DisplayName = "The state preserves selection independently per tab.")]
    [Trait("Category", "Unit")]
    public void SelectionShouldBePreservedPerTab()
    {
        // Arrange
        var firstMetrics = CreateApplication("Metrics One", "Metrics");
        var secondMetrics = CreateApplication("Metrics Two", "Metrics");
        var productivity = CreateApplication("Keyboard US", "Productivity");
        var state = new LauncherGridState(CreateOptions(firstMetrics, secondMetrics, productivity));

        // Act
        _ = state.SetSelection(1);
        _ = state.NextTab();
        _ = state.SetSelection(0);
        _ = state.NextTab();

        // Assert
        state.ActiveTab.Value.Should().Be("Metrics");
        state.SelectedApplication.Should().BeSameAs(secondMetrics);
    }

    [Fact(DisplayName = "The state preserves active tab across reload when it still exists.")]
    [Trait("Category", "Unit")]
    public void ReloadShouldPreserveActiveTabWhenItStillExists()
    {
        // Arrange
        var state = new LauncherGridState(CreateOptions(
            CreateApplication("JiraMetrics", "Metrics"),
            CreateApplication("Keyboard US", "Productivity")));
        _ = state.NextTab();
        var reloadedProductivity = CreateApplication("Keyboard GB", "Productivity");

        // Act
        state.Reload(CreateOptions(
            CreateApplication("JiraReport", "Metrics"),
            reloadedProductivity));

        // Assert
        state.ActiveTab.Value.Should().Be("Productivity");
        state.SelectedApplication.Should().BeSameAs(reloadedProductivity);
    }

    [Fact(DisplayName = "The state falls back to the first tab across reload when the active tab is removed.")]
    [Trait("Category", "Unit")]
    public void ReloadShouldFallBackToFirstTabWhenActiveTabIsRemoved()
    {
        // Arrange
        var state = new LauncherGridState(CreateOptions(
            CreateApplication("JiraMetrics", "Metrics"),
            CreateApplication("Keyboard US", "Productivity")));
        _ = state.NextTab();

        // Act
        state.Reload(CreateOptions(CreateApplication("JiraReport", "Metrics")));

        // Assert
        state.ActiveTab.Value.Should().Be("Metrics");
        state.SelectedApplication!.Name.Value.Should().Be("JiraReport");
    }

    private static LauncherOptions CreateOptions(params ApplicationEntry[] applications)
        => new(new LayoutOptions(), applications);

    private static ApplicationEntry CreateApplication(string name, string tab)
        => ApplicationEntry.Create(name, "dotnet", tab: tab);
}
