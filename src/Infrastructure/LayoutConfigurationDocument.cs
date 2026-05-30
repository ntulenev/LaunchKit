using Models;

namespace Infrastructure;

/// <summary>
/// Mutable layout document used only for configuration binding.
/// </summary>
public sealed class LayoutConfigurationDocument
{
    /// <summary>
    /// Gets or sets the requested number of tile columns.
    /// </summary>
    public int Columns { get; set; } = 3;

    /// <summary>
    /// Gets or sets the requested tile width.
    /// </summary>
    public int TileWidth { get; set; } = 34;

    /// <summary>
    /// Gets or sets the requested tile height.
    /// </summary>
    public int TileHeight { get; set; } = 6;

    /// <summary>
    /// Gets or sets the requested spacing between tiles.
    /// </summary>
    public int TileSpacing { get; set; } = 2;

    /// <summary>
    /// Converts the mutable layout document into immutable layout options.
    /// </summary>
    /// <returns>The mapped layout options.</returns>
    public LayoutOptions ToOptions()
        => new(Columns, TileWidth, TileHeight, TileSpacing);
}
