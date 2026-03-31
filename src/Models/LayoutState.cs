namespace Models;

/// <summary>
/// Represents the calculated layout used to render launcher tiles.
/// </summary>
public sealed class LayoutState
{
    /// <summary>
    /// Initializes a new calculated layout state.
    /// </summary>
    /// <param name="columns">Number of columns that fit in the current view.</param>
    /// <param name="tileWidth">Calculated tile width.</param>
    /// <param name="tileHeight">Calculated tile height.</param>
    /// <param name="tileSpacing">Spacing between tiles.</param>
    /// <param name="rowsPerPage">Number of rows that fit on the current page.</param>
    /// <param name="itemsPerPage">Number of items that fit on the current page.</param>
    public LayoutState(
        int columns,
        int tileWidth,
        int tileHeight,
        int tileSpacing,
        int rowsPerPage,
        int itemsPerPage)
    {
        Columns = columns;
        TileWidth = tileWidth;
        TileHeight = tileHeight;
        TileSpacing = tileSpacing;
        RowsPerPage = rowsPerPage;
        ItemsPerPage = itemsPerPage;
    }

    public int Columns { get; }

    public int TileWidth { get; }

    public int TileHeight { get; }

    public int TileSpacing { get; }

    public int RowsPerPage { get; }

    public int ItemsPerPage { get; }

    /// <summary>
    /// Calculates the number of pages required to display the specified item count.
    /// </summary>
    /// <param name="itemCount">Number of items to display.</param>
    /// <returns>The total page count.</returns>
    public int CalculatePages(int itemCount)
        => Math.Max(1, (int)Math.Ceiling(itemCount / (double)Math.Max(1, ItemsPerPage)));

    /// <summary>
    /// Gets the page index for a selected item.
    /// </summary>
    /// <param name="selectedIndex">Selected item index.</param>
    /// <returns>The zero-based page index.</returns>
    public int GetPage(int selectedIndex)
        => selectedIndex / Math.Max(1, ItemsPerPage);

    /// <summary>
    /// Gets the first item index for the specified page.
    /// </summary>
    /// <param name="page">Zero-based page index.</param>
    /// <returns>The zero-based first item index.</returns>
    public int GetFirstIndexForPage(int page)
        => Math.Max(0, page) * Math.Max(1, ItemsPerPage);

    /// <summary>
    /// Gets the item offset within its page.
    /// </summary>
    /// <param name="index">Absolute item index.</param>
    /// <returns>The zero-based offset within the page.</returns>
    public int GetPageOffset(int index)
        => index - GetFirstIndexForPage(GetPage(index));

    /// <summary>
    /// Gets the column position for an item on the page.
    /// </summary>
    /// <param name="pageOffset">Zero-based item offset within the page.</param>
    /// <returns>The zero-based column index.</returns>
    public int GetColumn(int pageOffset)
        => pageOffset % Math.Max(1, Columns);

    /// <summary>
    /// Gets the row position for an item on the page.
    /// </summary>
    /// <param name="pageOffset">Zero-based item offset within the page.</param>
    /// <returns>The zero-based row index.</returns>
    public int GetRow(int pageOffset)
        => pageOffset / Math.Max(1, Columns);

    /// <summary>
    /// Gets the X coordinate for a tile column.
    /// </summary>
    /// <param name="column">Zero-based column index.</param>
    /// <returns>The X coordinate for the tile.</returns>
    public int GetTileX(int column)
        => column * (TileWidth + TileSpacing);

    /// <summary>
    /// Gets the Y coordinate for a tile row.
    /// </summary>
    /// <param name="row">Zero-based row index.</param>
    /// <returns>The Y coordinate for the tile.</returns>
    public int GetTileY(int row)
        => row * (TileHeight + TileSpacing);
}
