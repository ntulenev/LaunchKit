using FluentAssertions;

namespace Infrastructure.Tests;

public sealed class ApplicationEntryDocumentTests
{
    [Fact(DisplayName = "The document maps all configured values to an application entry.")]
    [Trait("Category", "Unit")]
    public void ToModelShouldMapDocumentToApplicationEntry()
    {
        // Arrange
        var document = new ApplicationEntryDocument
        {
            Name = " LaunchKit ",
            Path = " dotnet ",
            Arguments = " --info ",
            WorkingDirectory = " C:\\Temp ",
            Description = " Launcher "
        };

        // Act
        var model = document.ToModel(1);

        // Assert
        model.Name.Value.Should().Be("LaunchKit");
        model.Path.Value.Should().Be("dotnet");
        model.Arguments.Value.Should().Be("--info");
        model.WorkingDirectory!.Value.Should().Be("C:\\Temp");
        model.Description.Value.Should().Be("Launcher");
    }

    [Fact(DisplayName = "The document includes the configuration path when validation fails.")]
    [Trait("Category", "Unit")]
    public void ToModelShouldIncludeConfigurationPathWhenValidationFails()
    {
        // Arrange
        var document = new ApplicationEntryDocument
        {
            Path = "dotnet"
        };

        // Act
        var action = () => document.ToModel(2);

        // Assert
        action.Should().Throw<InvalidDataException>()
            .WithMessage("Launcher:Applications:2: Application name is required.");
    }
}
