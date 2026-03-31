using FluentAssertions;

namespace Models.Tests;

public sealed class ApplicationDescriptionTests
{
    [Fact(DisplayName = "The constructor trims the description value.")]
    [Trait("Category", "Unit")]
    public void CtorShouldTrimValue()
    {
        // Arrange

        // Act
        var description = new ApplicationDescription(" launcher ");

        // Assert
        description.Value.Should().Be("launcher");
    }

    [Fact(DisplayName = "The constructor returns an empty value for white-space input.")]
    [Trait("Category", "Unit")]
    public void CtorShouldReturnEmptyWhenValueIsWhiteSpace()
    {
        // Arrange

        // Act
        var description = new ApplicationDescription(" ");

        // Assert
        description.Value.Should().BeEmpty();
        description.HasValue.Should().BeFalse();
    }

    [Fact(DisplayName = "The string representation returns the normalized description.")]
    [Trait("Category", "Unit")]
    public void ToStringShouldReturnNormalizedValue()
    {
        // Arrange
        var description = new ApplicationDescription(" launcher ");

        // Act
        var value = description.ToString();

        // Assert
        value.Should().Be("launcher");
    }
}
