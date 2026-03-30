namespace Models;

/// <summary>
/// Defines configurable layout settings for launcher tiles.
/// </summary>
public sealed class LayoutOptions
{
    public int Columns { get; set; } = 3;

    public int TileWidth { get; set; } = 34;

    public int TileHeight { get; set; } = 6;

    public int TileSpacing { get; set; } = 2;
}
