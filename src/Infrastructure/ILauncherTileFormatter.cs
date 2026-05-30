using Models;

namespace Infrastructure;

/// <summary>
/// Formats launcher grid text.
/// </summary>
internal interface ILauncherTileFormatter
{
    /// <summary>
    /// Builds a summary line for the current grid state and layout.
    /// </summary>
    /// <param name="state">Current grid state.</param>
    /// <param name="layout">Current layout state.</param>
    /// <returns>A summary line.</returns>
    string BuildSummary(ILauncherGridState state, LayoutState layout);

    /// <summary>
    /// Builds a tab strip for the current grid state.
    /// </summary>
    /// <param name="state">Current grid state.</param>
    /// <returns>A tab strip line.</returns>
    string BuildTabStrip(ILauncherGridState state);

    /// <summary>
    /// Gets the path text displayed for an application tile.
    /// </summary>
    /// <param name="options">Current launcher options.</param>
    /// <param name="application">Application entry to format.</param>
    /// <returns>The path display text.</returns>
    string GetPathDisplayText(LauncherOptions options, ApplicationEntry application);

    /// <summary>
    /// Clips text to the specified width.
    /// </summary>
    /// <param name="value">Text to clip.</param>
    /// <param name="width">Maximum display width.</param>
    /// <returns>The clipped text.</returns>
    string Clip(string value, int width);

    /// <summary>
    /// Formats application availability for display.
    /// </summary>
    /// <param name="availability">Availability value to format.</param>
    /// <returns>The availability display text.</returns>
    string FormatAvailability(ApplicationAvailability availability);
}
