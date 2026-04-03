using Abstractions;

using FluentAssertions;

using Moq;

using Models;

namespace Infrastructure.Tests;

public sealed class LauncherGridViewTests
{
    [Fact(DisplayName = "The admin launch action is forwarded for the selected application.")]
    [Trait("Category", "Unit")]
    public void LaunchSelectionAsAdminShouldForwardToLauncherActions()
    {
        // Arrange
        var application = ApplicationEntry.Create("JiraMetrics", "dotnet");
        var launcherActions = new Mock<ILauncherActions>(MockBehavior.Strict);
        launcherActions
            .Setup(actions => actions.LaunchAsAdmin(application))
            .Returns("Launched as admin: JiraMetrics");
        var view = CreateView(launcherActions.Object, true, application);

        // Act
        view.LaunchSelectionAsAdmin();

        // Assert
        launcherActions.Verify(actions => actions.LaunchAsAdmin(application), Times.Once);
    }

    [Fact(DisplayName = "The A key launches the selected application as admin.")]
    [Trait("Category", "Unit")]
    public void ProcessKeyShouldLaunchAsAdminWhenAIsPressed()
    {
        // Arrange
        var application = ApplicationEntry.Create("JiraMetrics", "dotnet");
        var launcherActions = new Mock<ILauncherActions>(MockBehavior.Strict);
        launcherActions
            .Setup(actions => actions.LaunchAsAdmin(application))
            .Returns("Launched as admin: JiraMetrics");
        var view = CreateView(launcherActions.Object, true, application);

        // Act
        var handled = view.ProcessKey(new Terminal.Gui.KeyEvent(Terminal.Gui.Key.A, new Terminal.Gui.KeyModifiers()));

        // Assert
        handled.Should().BeTrue();
        launcherActions.Verify(actions => actions.LaunchAsAdmin(application), Times.Once);
    }

    [Fact(DisplayName = "The tab strip highlights the active tab.")]
    [Trait("Category", "Unit")]
    public void BuildTabStripShouldHighlightActiveTab()
    {
        // Arrange
        var view = CreateView(
            showFullPath: true,
            ApplicationEntry.Create("JiraMetrics", "dotnet", tab: "Metrics"),
            ApplicationEntry.Create("Keyboard US", "dotnet", tab: "Productivity"));

        // Act
        var tabStrip = view.BuildTabStrip();

        // Assert
        tabStrip.Should().Be("Tabs: [Metrics]  Productivity");
    }

    [Fact(DisplayName = "The Tab key switches to the next tab.")]
    [Trait("Category", "Unit")]
    public void ProcessKeyShouldSwitchTabsWhenTabIsPressed()
    {
        // Arrange
        var view = CreateView(
            showFullPath: true,
            ApplicationEntry.Create("JiraMetrics", "dotnet", tab: "Metrics"),
            ApplicationEntry.Create("Keyboard US", "dotnet", tab: "Productivity"));

        // Act
        var handled = view.ProcessKey(new Terminal.Gui.KeyEvent(Terminal.Gui.Key.Tab, new Terminal.Gui.KeyModifiers()));

        // Assert
        handled.Should().BeTrue();
        view.BuildTabStrip().Should().Be("Tabs: Metrics  [Productivity]");
        view.BuildSummary().Should().Contain("Tab: Productivity");
        view.BuildSummary().Should().Contain("Selected: Keyboard US");
    }

    [Fact(DisplayName = "The grid shows the full path when the option is enabled.")]
    [Trait("Category", "Unit")]
    public void GetPathDisplayTextShouldReturnFullPathWhenConfigured()
    {
        // Arrange
        var application = ApplicationEntry.Create(
            "JiraMetrics",
            @"C:\Users\ntyulenev\Desktop\MyApps\JiraFlow\JiraMetrics.exe");
        var view = CreateView(showFullPath: true, application);

        // Act
        var displayText = view.GetPathDisplayText(application);

        // Assert
        displayText.Should().Be(application.Path.Value);
    }

    [Fact(DisplayName = "The grid shows only the executable name when the full path is hidden.")]
    [Trait("Category", "Unit")]
    public void GetPathDisplayTextShouldReturnExecutableNameWhenConfigured()
    {
        // Arrange
        var application = ApplicationEntry.Create(
            "JiraMetrics",
            @"C:\Users\ntyulenev\Desktop\MyApps\JiraFlow\JiraMetrics.exe");
        var view = CreateView(showFullPath: false, application);

        // Act
        var displayText = view.GetPathDisplayText(application);

        // Assert
        displayText.Should().Be("JiraMetrics.exe");
    }

    private static LauncherGridView CreateView(bool showFullPath, params ApplicationEntry[] applications)
        => CreateView(new Mock<ILauncherActions>(MockBehavior.Strict).Object, showFullPath, applications);

    private static LauncherGridView CreateView(
        ILauncherActions launcherActions,
        bool showFullPath,
        params ApplicationEntry[] applications)
        => new(
            new LauncherOptions(new LayoutOptions(), applications, showFullPath),
            launcherActions,
            () => new LauncherOptions(new LayoutOptions(), applications, showFullPath));
}
