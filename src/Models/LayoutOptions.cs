namespace Models;

/// <summary>
/// Defines configurable layout settings for launcher tiles.
/// </summary>
public sealed class LayoutOptions
{
    /// <summary>
    /// Initializes a new set of layout options.
    /// </summary>
    /// <param name="columns">Requested number of columns.</param>
    /// <param name="tileWidth">Requested tile width.</param>
    /// <param name="tileHeight">Requested tile height.</param>
    /// <param name="tileSpacing">Requested spacing between tiles.</param>
    public LayoutOptions(
        int columns = DEFAULT_COLUMNS,
        int tileWidth = DEFAULT_TILE_WIDTH,
        int tileHeight = DEFAULT_TILE_HEIGHT,
        int tileSpacing = DEFAULT_TILE_SPACING)
    {
        Columns = ValidatePositive(columns, "Launcher:Layout:Columns");
        TileWidth = ValidatePositive(tileWidth, "Launcher:Layout:TileWidth");
        TileHeight = ValidatePositive(tileHeight, "Launcher:Layout:TileHeight");
        TileSpacing = ValidatePositive(tileSpacing, "Launcher:Layout:TileSpacing");
    }

    public int Columns { get; }

    public int TileWidth { get; }

    public int TileHeight { get; }

    public int TileSpacing { get; }

    /// <summary>
    /// Calculates the effective layout for the provided bounds and item count.
    /// </summary>
    /// <param name="availableWidth">Available view width.</param>
    /// <param name="availableHeight">Available view height.</param>
    /// <param name="itemCount">Number of items to display.</param>
    /// <returns>The calculated layout state.</returns>
    public LayoutState CalculateState(int availableWidth, int availableHeight, int itemCount)
    {
        var normalizedWidth = Math.Max(MIN_AVAILABLE_WIDTH, availableWidth);
        var normalizedHeight = Math.Max(MIN_AVAILABLE_HEIGHT, availableHeight);
        var normalizedItemCount = Math.Max(1, itemCount);
        var tileWidth = Math.Max(MIN_TILE_WIDTH, TileWidth);
        var tileHeight = Math.Max(MIN_TILE_HEIGHT, TileHeight);
        var tileSpacing = Math.Max(MIN_TILE_SPACING, TileSpacing);
        var columns = Math.Min(Math.Max(1, Columns), normalizedItemCount);

        while (columns > 1 && RequiredWidth(columns, tileWidth, tileSpacing) > normalizedWidth)
        {
            columns--;
        }

        var rowsPerPage = Math.Max(1, (normalizedHeight + tileSpacing) / (tileHeight + tileSpacing));
        var itemsPerPage = Math.Max(1, rowsPerPage * columns);

        return new LayoutState(
            columns,
            tileWidth,
            tileHeight,
            tileSpacing,
            rowsPerPage,
            itemsPerPage);
    }

    private static int RequiredWidth(int columns, int tileWidth, int tileSpacing)
        => (columns * tileWidth) + ((columns - 1) * tileSpacing);

    private static int ValidatePositive(int value, string propertyPath)
        => value > 0
            ? value
            : throw new InvalidDataException($"{propertyPath} must be greater than zero.");

    private const int DEFAULT_COLUMNS = 3;
    private const int DEFAULT_TILE_WIDTH = 34;
    private const int DEFAULT_TILE_HEIGHT = 6;
    private const int DEFAULT_TILE_SPACING = 2;

    private const int MIN_AVAILABLE_WIDTH = 1;
    private const int MIN_AVAILABLE_HEIGHT = 1;
    private const int MIN_TILE_WIDTH = 28;
    private const int MIN_TILE_HEIGHT = 6;
    private const int MIN_TILE_SPACING = 1;
}
