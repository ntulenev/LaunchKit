using FluentAssertions;

namespace Models.Tests;

public sealed class ApplicationEntryTests
{
    [Fact(DisplayName = "The constructor throws when the application name is null.")]
    [Trait("Category", "Unit")]
    public void CtorShouldThrowWhenNameIsNull()
    {
        // Arrange
        var path = new ApplicationPath("dotnet");

        // Act
        var action = () => new ApplicationEntry(null!, path);

        // Assert
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact(DisplayName = "The constructor throws when the application path is null.")]
    [Trait("Category", "Unit")]
    public void CtorShouldThrowWhenPathIsNull()
    {
        // Arrange
        var name = new ApplicationName("LaunchKit");

        // Act
        var action = () => new ApplicationEntry(name, null!);

        // Assert
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact(DisplayName = "The constructor creates default arguments and description values when optional values are missing.")]
    [Trait("Category", "Unit")]
    public void CtorShouldCreateDefaultOptionalValuesWhenTheyAreMissing()
    {
        // Arrange

        // Act
        var entry = new ApplicationEntry(
            new ApplicationName("LaunchKit"),
            new ApplicationPath("dotnet"));

        // Assert
        entry.Arguments.Value.Should().BeEmpty();
        entry.WorkingDirectory.Should().BeNull();
        entry.Description.Value.Should().BeEmpty();
        entry.Tab.Value.Should().Be(ApplicationTab.DefaultValue);
    }

    [Fact(DisplayName = "The factory maps the configured tab.")]
    [Trait("Category", "Unit")]
    public void CreateShouldMapConfiguredTab()
    {
        // Arrange

        // Act
        var entry = ApplicationEntry.Create(
            name: "LaunchKit",
            path: "dotnet",
            tab: "Metrics");

        // Assert
        entry.Tab.Value.Should().Be("Metrics");
    }

    [Fact(DisplayName = "The factory includes the configuration source when validation fails.")]
    [Trait("Category", "Unit")]
    public void CreateShouldIncludeSourceWhenValidationFails()
    {
        // Arrange

        // Act
        var action = () => ApplicationEntry.Create(
            name: null,
            path: "dotnet",
            source: "Launcher:Applications:0");

        // Assert
        action.Should().Throw<InvalidDataException>()
            .WithMessage("Launcher:Applications:0: Application name is required.");
    }

    [Fact(DisplayName = "The factory preserves the original validation message when the configuration source is missing.")]
    [Trait("Category", "Unit")]
    public void CreateShouldPreserveOriginalMessageWhenSourceIsMissing()
    {
        // Arrange

        // Act
        var action = () => ApplicationEntry.Create(
            name: null,
            path: "dotnet");

        // Assert
        action.Should().Throw<InvalidDataException>()
            .WithMessage("Application name is required.");
    }

    [Fact(DisplayName = "The availability is ready when the path represents a command.")]
    [Trait("Category", "Unit")]
    public void GetAvailabilityShouldReturnReadyWhenPathIsCommand()
    {
        // Arrange
        var application = ApplicationEntry.Create("LaunchKit", "dotnet");

        // Act
        var availability = application.GetAvailability();

        // Assert
        availability.Should().Be(ApplicationAvailability.Ready);
        application.CanLaunch().Should().BeTrue();
    }

    [Fact(DisplayName = "The availability is not ready when the path points to a missing file.")]
    [Trait("Category", "Unit")]
    public void GetAvailabilityShouldReturnPathNotFoundWhenPathIsMissing()
    {
        // Arrange
        var application = ApplicationEntry.Create("LaunchKit", CreateMissingFilePath());

        // Act
        var availability = application.GetAvailability();

        // Assert
        availability.Should().Be(ApplicationAvailability.PathNotFound);
        application.CanLaunch().Should().BeFalse();
    }

    private static string CreateMissingFilePath()
        => Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"), "LaunchKit.exe");
}
