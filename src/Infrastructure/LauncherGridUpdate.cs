namespace Infrastructure;

/// <summary>
/// Describes view notifications caused by a launcher grid command.
/// </summary>
internal readonly record struct LauncherGridUpdate
{
    /// <summary>
    /// Initializes a launcher grid update.
    /// </summary>
    /// <param name="status">Optional status message to display.</param>
    /// <param name="selectionChanged">Whether selection subscribers should be notified.</param>
    /// <param name="tabChanged">Whether tab subscribers should be notified.</param>
    /// <param name="needsDisplay">Whether the grid should be redrawn.</param>
    public LauncherGridUpdate(
        string? status,
        bool selectionChanged,
        bool tabChanged,
        bool needsDisplay)
    {
        Status = status;
        SelectionChanged = selectionChanged;
        TabChanged = tabChanged;
        NeedsDisplay = needsDisplay;
    }

    /// <summary>
    /// Gets an update that does not require any view notification.
    /// </summary>
    public static LauncherGridUpdate None { get; } = new(null, false, false, false);

    /// <summary>
    /// Gets the optional status message to display.
    /// </summary>
    public string? Status { get; }

    /// <summary>
    /// Gets a value indicating whether selection subscribers should be notified.
    /// </summary>
    public bool SelectionChanged { get; }

    /// <summary>
    /// Gets a value indicating whether tab subscribers should be notified.
    /// </summary>
    public bool TabChanged { get; }

    /// <summary>
    /// Gets a value indicating whether the grid should be redrawn.
    /// </summary>
    public bool NeedsDisplay { get; }

    /// <summary>
    /// Gets a value indicating whether the update includes a status message.
    /// </summary>
    public bool HasStatus => !string.IsNullOrWhiteSpace(Status);
}
