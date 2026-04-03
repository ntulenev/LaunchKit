using FluentAssertions;

namespace Models.Tests;

public sealed class LauncherOptionsTests
{
    [Fact(DisplayName = "The constructor throws when the layout is null.")]
    [Trait("Category", "Unit")]
    public void CtorShouldThrowWhenLayoutIsNull()
    {
        // Arrange

        // Act
        var action = () => new LauncherOptions(null!, []);

        // Assert
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact(DisplayName = "The constructor throws when the applications collection is null.")]
    [Trait("Category", "Unit")]
    public void CtorShouldThrowWhenApplicationsIsNull()
    {
        // Arrange

        // Act
        var action = () => new LauncherOptions(new LayoutOptions(), null!);

        // Assert
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact(DisplayName = "The options report that no applications are configured when the collection is empty.")]
    [Trait("Category", "Unit")]
    public void HasApplicationsShouldReturnFalseWhenCollectionIsEmpty()
    {
        // Arrange
        var options = new LauncherOptions(new LayoutOptions(), []);

        // Act
        var hasApplications = options.HasApplications;

        // Assert
        hasApplications.Should().BeFalse();
        options.Count.Should().Be(0);
    }

    [Fact(DisplayName = "The requested application is returned by index.")]
    [Trait("Category", "Unit")]
    public void GetApplicationShouldReturnApplicationAtRequestedIndex()
    {
        // Arrange
        var first = CreateApplication("LaunchKit");
        var second = CreateApplication("Terminal");
        var options = new LauncherOptions(new LayoutOptions(), [first, second]);

        // Act
        var application = options.GetApplication(1);

        // Assert
        application.Should().BeSameAs(second);
    }

    [Fact(DisplayName = "The selection index is zero when the collection is empty.")]
    [Trait("Category", "Unit")]
    public void ClampSelectionShouldReturnZeroWhenCollectionIsEmpty()
    {
        // Arrange
        var options = new LauncherOptions(new LayoutOptions(), []);

        // Act
        var selection = options.ClampSelection(10);

        // Assert
        selection.Should().Be(0);
    }

    [Fact(DisplayName = "The selection index is clamped to the available range.")]
    [Trait("Category", "Unit")]
    public void ClampSelectionShouldClampToValidRange()
    {
        // Arrange
        var options = new LauncherOptions(new LayoutOptions(), [CreateApplication("LaunchKit")]);

        // Act
        var lowSelection = options.ClampSelection(-1);
        var highSelection = options.ClampSelection(10);

        // Assert
        lowSelection.Should().Be(0);
        highSelection.Should().Be(0);
    }

    [Fact(DisplayName = "The layout state is created from the configured layout options.")]
    [Trait("Category", "Unit")]
    public void CreateLayoutStateShouldUseConfiguredLayout()
    {
        // Arrange
        var options = new LauncherOptions(
            new LayoutOptions(columns: 3, tileWidth: 34, tileHeight: 6, tileSpacing: 2),
            [CreateApplication("One"), CreateApplication("Two"), CreateApplication("Three")]);

        // Act
        var state = options.CreateLayoutState(120, 24);

        // Assert
        state.Columns.Should().Be(3);
        state.TileWidth.Should().Be(34);
        state.TileHeight.Should().Be(6);
        state.TileSpacing.Should().Be(2);
    }

    [Fact(DisplayName = "The show full path flag defaults to true.")]
    [Trait("Category", "Unit")]
    public void ShowFullPathShouldDefaultToTrue()
    {
        // Arrange
        var options = new LauncherOptions(new LayoutOptions(), [CreateApplication("LaunchKit")]);

        // Act
        var showFullPath = options.ShowFullPath;

        // Assert
        showFullPath.Should().BeTrue();
    }

    [Fact(DisplayName = "The show full path flag can be disabled.")]
    [Trait("Category", "Unit")]
    public void ShowFullPathShouldUseConfiguredValue()
    {
        // Arrange
        var options = new LauncherOptions(
            new LayoutOptions(),
            [CreateApplication("LaunchKit")],
            showFullPath: false);

        // Act
        var showFullPath = options.ShowFullPath;

        // Assert
        showFullPath.Should().BeFalse();
    }

    private static ApplicationEntry CreateApplication(string name)
        => ApplicationEntry.Create(name, "dotnet");
}
