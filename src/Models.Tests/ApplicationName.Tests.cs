using FluentAssertions;

namespace Models.Tests;

public sealed class ApplicationNameTests
{
    [Fact(DisplayName = "The constructor trims the application name.")]
    [Trait("Category", "Unit")]
    public void CtorShouldTrimValue()
    {
        // Arrange

        // Act
        var name = new ApplicationName(" LaunchKit ");

        // Assert
        name.Value.Should().Be("LaunchKit");
    }

    [Theory(DisplayName = "The constructor throws when the application name is white space.")]
    [Trait("Category", "Unit")]
    [InlineData("")]
    [InlineData(" ")]
    public void CtorShouldThrowWhenValueIsWhiteSpace(string value)
    {
        // Arrange

        // Act
        var action = () => new ApplicationName(value);

        // Assert
        action.Should().Throw<InvalidDataException>()
            .WithMessage("Application name is required.");
    }

    [Fact(DisplayName = "The constructor throws when the application name is null.")]
    [Trait("Category", "Unit")]
    public void CtorShouldThrowWhenValueIsNull()
    {
        // Arrange

        // Act
        var action = () => new ApplicationName(null!);

        // Assert
        action.Should().Throw<InvalidDataException>()
            .WithMessage("Application name is required.");
    }

    [Fact(DisplayName = "The string representation returns the normalized application name.")]
    [Trait("Category", "Unit")]
    public void ToStringShouldReturnNormalizedValue()
    {
        // Arrange
        var name = new ApplicationName(" LaunchKit ");

        // Act
        var value = name.ToString();

        // Assert
        value.Should().Be("LaunchKit");
    }
}
