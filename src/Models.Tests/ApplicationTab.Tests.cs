using FluentAssertions;

namespace Models.Tests;

public sealed class ApplicationTabTests
{
    [Fact(DisplayName = "The constructor uses the default tab when the value is empty.")]
    [Trait("Category", "Unit")]
    public void CtorShouldUseDefaultValueWhenValueIsEmpty()
    {
        // Arrange

        // Act
        var tab = new ApplicationTab(" ");

        // Assert
        tab.Value.Should().Be(ApplicationTab.DefaultValue);
        tab.IsDefault.Should().BeTrue();
    }

    [Fact(DisplayName = "The constructor trims the tab name.")]
    [Trait("Category", "Unit")]
    public void CtorShouldTrimValue()
    {
        // Arrange

        // Act
        var tab = new ApplicationTab(" Metrics ");

        // Assert
        tab.Value.Should().Be("Metrics");
        tab.IsDefault.Should().BeFalse();
    }

    [Fact(DisplayName = "The string representation returns the normalized tab name.")]
    [Trait("Category", "Unit")]
    public void ToStringShouldReturnNormalizedValue()
    {
        // Arrange
        var tab = new ApplicationTab(" Productivity ");

        // Act
        var value = tab.ToString();

        // Assert
        value.Should().Be("Productivity");
    }
}
