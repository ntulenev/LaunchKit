using FluentAssertions;

namespace Models.Tests;

public sealed class LayoutOptionsTests
{
    [Theory(DisplayName = "The constructor throws when a layout value is not positive.")]
    [Trait("Category", "Unit")]
    [InlineData(0, 34, 6, 2, "Launcher:Layout:Columns must be greater than zero.")]
    [InlineData(3, 0, 6, 2, "Launcher:Layout:TileWidth must be greater than zero.")]
    [InlineData(3, 34, 0, 2, "Launcher:Layout:TileHeight must be greater than zero.")]
    [InlineData(3, 34, 6, 0, "Launcher:Layout:TileSpacing must be greater than zero.")]
    public void CtorShouldThrowWhenValueIsNotPositive(
        int columns,
        int tileWidth,
        int tileHeight,
        int tileSpacing,
        string message)
    {
        // Arrange

        // Act
        var action = () => new LayoutOptions(columns, tileWidth, tileHeight, tileSpacing);

        // Assert
        action.Should().Throw<InvalidDataException>()
            .WithMessage(message);
    }

    [Fact(DisplayName = "The calculated state reduces the number of columns when the available width is limited.")]
    [Trait("Category", "Unit")]
    public void CalculateStateShouldReduceColumnsWhenWidthIsLimited()
    {
        // Arrange
        var options = new LayoutOptions(columns: 3, tileWidth: 34, tileHeight: 6, tileSpacing: 2);

        // Act
        var state = options.CalculateState(70, 30, 3);

        // Assert
        state.Columns.Should().Be(2);
        state.ItemsPerPage.Should().Be(6);
    }

    [Fact(DisplayName = "The calculated state clamps the number of columns to the item count.")]
    [Trait("Category", "Unit")]
    public void CalculateStateShouldClampColumnsToItemCount()
    {
        // Arrange
        var options = new LayoutOptions(columns: 5, tileWidth: 34, tileHeight: 6, tileSpacing: 2);

        // Act
        var state = options.CalculateState(300, 30, 2);

        // Assert
        state.Columns.Should().Be(2);
    }

    [Fact(DisplayName = "The calculated state applies minimum dimensions when the available size is too small.")]
    [Trait("Category", "Unit")]
    public void CalculateStateShouldApplyMinimumDimensionsWhenAvailableSizeIsTooSmall()
    {
        // Arrange
        var options = new LayoutOptions(columns: 1, tileWidth: 1, tileHeight: 1, tileSpacing: 1);

        // Act
        var state = options.CalculateState(0, 0, 0);

        // Assert
        state.Columns.Should().Be(1);
        state.TileWidth.Should().Be(28);
        state.TileHeight.Should().Be(6);
        state.TileSpacing.Should().Be(1);
        state.RowsPerPage.Should().Be(1);
        state.ItemsPerPage.Should().Be(1);
    }
}
