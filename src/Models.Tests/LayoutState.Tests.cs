using FluentAssertions;

namespace Models.Tests;

public sealed class LayoutStateTests
{
    [Fact(DisplayName = "The page count is at least one even when there are no items.")]
    [Trait("Category", "Unit")]
    public void CalculatePagesShouldReturnAtLeastOne()
    {
        // Arrange
        var state = CreateState();

        // Act
        var pages = state.CalculatePages(0);

        // Assert
        pages.Should().Be(1);
    }

    [Fact(DisplayName = "The page index is calculated from the selected item.")]
    [Trait("Category", "Unit")]
    public void GetPageShouldReturnPageIndex()
    {
        // Arrange
        var state = CreateState();

        // Act
        var page = state.GetPage(5);

        // Assert
        page.Should().Be(1);
    }

    [Fact(DisplayName = "The first index for a page is calculated from the page number.")]
    [Trait("Category", "Unit")]
    public void GetFirstIndexForPageShouldReturnPageStart()
    {
        // Arrange
        var state = CreateState();

        // Act
        var firstIndex = state.GetFirstIndexForPage(2);

        // Assert
        firstIndex.Should().Be(8);
    }

    [Fact(DisplayName = "The page offset is calculated relative to the beginning of the page.")]
    [Trait("Category", "Unit")]
    public void GetPageOffsetShouldReturnOffsetWithinPage()
    {
        // Arrange
        var state = CreateState();

        // Act
        var offset = state.GetPageOffset(5);

        // Assert
        offset.Should().Be(1);
    }

    [Fact(DisplayName = "The column is calculated from the page offset.")]
    [Trait("Category", "Unit")]
    public void GetColumnShouldReturnColumnIndex()
    {
        // Arrange
        var state = CreateState();

        // Act
        var column = state.GetColumn(3);

        // Assert
        column.Should().Be(1);
    }

    [Fact(DisplayName = "The row is calculated from the page offset.")]
    [Trait("Category", "Unit")]
    public void GetRowShouldReturnRowIndex()
    {
        // Arrange
        var state = CreateState();

        // Act
        var row = state.GetRow(3);

        // Assert
        row.Should().Be(1);
    }

    [Fact(DisplayName = "The horizontal tile coordinate is calculated from the column index.")]
    [Trait("Category", "Unit")]
    public void GetTileXShouldReturnHorizontalOffset()
    {
        // Arrange
        var state = CreateState();

        // Act
        var x = state.GetTileX(2);

        // Assert
        x.Should().Be(72);
    }

    [Fact(DisplayName = "The vertical tile coordinate is calculated from the row index.")]
    [Trait("Category", "Unit")]
    public void GetTileYShouldReturnVerticalOffset()
    {
        // Arrange
        var state = CreateState();

        // Act
        var y = state.GetTileY(2);

        // Assert
        y.Should().Be(16);
    }

    private static LayoutState CreateState()
        => new(columns: 2, tileWidth: 34, tileHeight: 6, tileSpacing: 2, rowsPerPage: 2, itemsPerPage: 4);
}
