using FluentAssertions;

namespace Infrastructure.Tests;

public sealed class LayoutConfigurationDocumentTests
{
    [Fact(DisplayName = "The layout document maps all configured layout values.")]
    [Trait("Category", "Unit")]
    public void ToOptionsShouldMapLayoutValues()
    {
        // Arrange
        var document = new LayoutConfigurationDocument
        {
            Columns = 4,
            TileWidth = 40,
            TileHeight = 7,
            TileSpacing = 3
        };

        // Act
        var options = document.ToOptions();

        // Assert
        options.Columns.Should().Be(4);
        options.TileWidth.Should().Be(40);
        options.TileHeight.Should().Be(7);
        options.TileSpacing.Should().Be(3);
    }
}
