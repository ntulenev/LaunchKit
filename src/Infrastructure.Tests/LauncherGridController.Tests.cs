using Abstractions;

using FluentAssertions;

using Moq;

using Models;

namespace Infrastructure.Tests;

public sealed class LauncherGridControllerTests
{
    [Fact(DisplayName = "The launch command forwards the selected application to launcher actions.")]
    [Trait("Category", "Unit")]
    public void LaunchSelectionShouldForwardSelectedApplication()
    {
        // Arrange
        var application = CreateApplication("LaunchKit", "Apps");
        var actions = new Mock<ILauncherActions>(MockBehavior.Strict);
        actions.Setup(a => a.Launch(application))
            .Returns("Launched: LaunchKit");
        var controller = CreateController(actions.Object, CreateOptions(application));

        // Act
        var update = controller.LaunchSelection();

        // Assert
        update.Status.Should().Be("Launched: LaunchKit");
        update.NeedsDisplay.Should().BeTrue();
        actions.VerifyAll();
    }

    [Fact(DisplayName = "The admin launch command forwards the selected application to launcher actions.")]
    [Trait("Category", "Unit")]
    public void LaunchSelectionAsAdminShouldForwardSelectedApplication()
    {
        // Arrange
        var application = CreateApplication("LaunchKit", "Apps");
        var actions = new Mock<ILauncherActions>(MockBehavior.Strict);
        actions.Setup(a => a.LaunchAsAdmin(application))
            .Returns("Launched as admin: LaunchKit");
        var controller = CreateController(actions.Object, CreateOptions(application));

        // Act
        var update = controller.LaunchSelectionAsAdmin();

        // Assert
        update.Status.Should().Be("Launched as admin: LaunchKit");
        update.NeedsDisplay.Should().BeTrue();
        actions.VerifyAll();
    }

    [Fact(DisplayName = "The open-folder command forwards the selected application to launcher actions.")]
    [Trait("Category", "Unit")]
    public void OpenSelectionFolderShouldForwardSelectedApplication()
    {
        // Arrange
        var application = CreateApplication("LaunchKit", "Apps");
        var actions = new Mock<ILauncherActions>(MockBehavior.Strict);
        actions.Setup(a => a.OpenContainingFolder(application))
            .Returns("Opened folder: C:\\Tools");
        var controller = CreateController(actions.Object, CreateOptions(application));

        // Act
        var update = controller.OpenSelectionFolder();

        // Assert
        update.Status.Should().Be("Opened folder: C:\\Tools");
        update.NeedsDisplay.Should().BeTrue();
        actions.VerifyAll();
    }

    [Fact(DisplayName = "The launch command reports empty configuration when no application is selected.")]
    [Trait("Category", "Unit")]
    public void LaunchSelectionShouldReportEmptyConfiguration()
    {
        // Arrange
        var controller = CreateController(
            new Mock<ILauncherActions>(MockBehavior.Strict).Object,
            CreateOptions());

        // Act
        var update = controller.LaunchSelection();

        // Assert
        update.Status.Should().Be("No applications configured.");
        update.NeedsDisplay.Should().BeTrue();
    }

    [Fact(DisplayName = "The next-tab command reports tab and selection changes.")]
    [Trait("Category", "Unit")]
    public void NextTabShouldReportChangedTabAndSelection()
    {
        // Arrange
        var controller = CreateController(
            new Mock<ILauncherActions>(MockBehavior.Strict).Object,
            CreateOptions(
                CreateApplication("JiraMetrics", "Metrics"),
                CreateApplication("Keyboard US", "Productivity")));

        // Act
        var update = controller.NextTab();

        // Assert
        update.Status.Should().Be("Tab: Productivity");
        update.TabChanged.Should().BeTrue();
        update.SelectionChanged.Should().BeTrue();
        update.NeedsDisplay.Should().BeTrue();
    }

    [Fact(DisplayName = "The next-tab command is a no-op when only one tab is configured.")]
    [Trait("Category", "Unit")]
    public void NextTabShouldReturnNoUpdateWhenOnlyOneTabExists()
    {
        // Arrange
        var controller = CreateController(
            new Mock<ILauncherActions>(MockBehavior.Strict).Object,
            CreateOptions(CreateApplication("JiraMetrics", "Metrics")));

        // Act
        var update = controller.NextTab();

        // Assert
        update.Should().Be(LauncherGridUpdate.None);
    }

    [Fact(DisplayName = "The move-selection command reports selection changes.")]
    [Trait("Category", "Unit")]
    public void MoveSelectionShouldReportSelectionChange()
    {
        // Arrange
        var controller = CreateController(
            new Mock<ILauncherActions>(MockBehavior.Strict).Object,
            CreateOptions(
                CreateApplication("One", "Apps"),
                CreateApplication("Two", "Apps")));

        // Act
        var update = controller.MoveSelection(1);

        // Assert
        update.SelectionChanged.Should().BeTrue();
        update.NeedsDisplay.Should().BeTrue();
        update.TabChanged.Should().BeFalse();
        update.Status.Should().BeNull();
    }

    [Fact(DisplayName = "The reload command reports tab and selection changes when configuration reloads.")]
    [Trait("Category", "Unit")]
    public void ReloadSelectionShouldReportReloadSuccess()
    {
        // Arrange
        var state = new LauncherGridState(CreateOptions(CreateApplication("One", "Apps")));
        var controller = new LauncherGridController(
            state,
            new Mock<ILauncherActions>(MockBehavior.Strict).Object,
            () => CreateOptions(CreateApplication("Two", "Apps")));

        // Act
        var update = controller.ReloadSelection();

        // Assert
        update.Status.Should().Be("Configuration reloaded.");
        update.TabChanged.Should().BeTrue();
        update.SelectionChanged.Should().BeTrue();
        update.NeedsDisplay.Should().BeTrue();
        state.SelectedApplication!.Name.Value.Should().Be("Two");
    }

    [Fact(DisplayName = "The reload command reports handled reload errors.")]
    [Trait("Category", "Unit")]
    public void ReloadSelectionShouldReportReloadFailure()
    {
        // Arrange
        var controller = new LauncherGridController(
            new LauncherGridState(CreateOptions(CreateApplication("One", "Apps"))),
            new Mock<ILauncherActions>(MockBehavior.Strict).Object,
            static () => throw new InvalidDataException("Bad config"));

        // Act
        var update = controller.ReloadSelection();

        // Assert
        update.Status.Should().Be("Reload failed: Bad config");
        update.NeedsDisplay.Should().BeTrue();
        update.TabChanged.Should().BeFalse();
        update.SelectionChanged.Should().BeFalse();
    }

    private static LauncherGridController CreateController(
        ILauncherActions actions,
        LauncherOptions options)
        => new(new LauncherGridState(options), actions, () => options);

    private static LauncherOptions CreateOptions(params ApplicationEntry[] applications)
        => new(new LayoutOptions(), applications);

    private static ApplicationEntry CreateApplication(string name, string tab)
        => ApplicationEntry.Create(name, "dotnet", tab: tab);
}
