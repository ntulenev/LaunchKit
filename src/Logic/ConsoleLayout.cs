using Models;

namespace Logic;

/// <summary>
/// Calculates grid layout values for the console launcher.
/// </summary>
public static class ConsoleLayout
{
    /// <summary>
    /// Calculates the effective layout for the current console size and item count.
    /// </summary>
    /// <param name="options">Launcher options that define the preferred layout.</param>
    /// <param name="itemCount">Number of items that need to be displayed.</param>
    /// <returns>The computed layout state.</returns>
    public static LayoutState Calculate(LauncherOptions options, int itemCount)
    {
        ArgumentNullException.ThrowIfNull(options);

        var layout = options.Layout;
        var tileWidth = Math.Max(24, layout.TileWidth);
        var tileHeight = Math.Max(5, layout.TileHeight);
        var tileSpacing = Math.Max(1, layout.TileSpacing);
        var consoleWidth = Math.Max(40, Console.WindowWidth);
        var consoleHeight = Math.Max(18, Console.WindowHeight);

        var requestedColumns = Math.Max(1, layout.Columns);
        var columns = Math.Max(1, Math.Min(requestedColumns, Math.Max(1, itemCount)));

        while (columns > 1 && RequiredWidth(columns, tileWidth, tileSpacing) > consoleWidth)
        {
            columns--;
        }

        const int HEADER_HEIGHT = 5;
        var rowsPerPage = Math.Max(1, (consoleHeight - HEADER_HEIGHT - 1) / (tileHeight + 1));
        var itemsPerPage = Math.Max(1, rowsPerPage * columns);

        return new LayoutState(
            columns,
            tileWidth,
            tileHeight,
            tileSpacing,
            HEADER_HEIGHT,
            rowsPerPage,
            itemsPerPage);
    }

    private static int RequiredWidth(int columns, int tileWidth, int tileSpacing)
        => (columns * tileWidth) + ((columns - 1) * tileSpacing);
}
