namespace Models;

/// <summary>
/// Represents the calculated layout used to render launcher tiles.
/// </summary>
/// <param name="Columns">Number of tile columns that fit in the current view.</param>
/// <param name="TileWidth">Calculated tile width.</param>
/// <param name="TileHeight">Calculated tile height.</param>
/// <param name="TileSpacing">Spacing between tiles.</param>
/// <param name="HeaderHeight">Height reserved for header content.</param>
/// <param name="RowsPerPage">Number of tile rows visible per page.</param>
/// <param name="ItemsPerPage">Number of items visible per page.</param>
public sealed record LayoutState(
    int Columns,
    int TileWidth,
    int TileHeight,
    int TileSpacing,
    int HeaderHeight,
    int RowsPerPage,
    int ItemsPerPage);
