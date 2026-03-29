namespace Models;

public sealed record LayoutState(
    int Columns,
    int TileWidth,
    int TileHeight,
    int TileSpacing,
    int HeaderHeight,
    int RowsPerPage,
    int ItemsPerPage);
