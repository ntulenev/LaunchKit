using Models;

namespace Infrastructure;

/// <summary>
/// Mutable layout document used only for configuration binding.
/// </summary>
public sealed class LayoutConfigurationDocument
{
    public int Columns { get; set; } = 3;

    public int TileWidth { get; set; } = 34;

    public int TileHeight { get; set; } = 6;

    public int TileSpacing { get; set; } = 2;

    /// <summary>
    /// Converts the mutable layout document into immutable layout options.
    /// </summary>
    /// <returns>The mapped layout options.</returns>
    public LayoutOptions ToOptions()
        => new(Columns, TileWidth, TileHeight, TileSpacing);
}
