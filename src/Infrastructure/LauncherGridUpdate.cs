namespace Infrastructure;

/// <summary>
/// Describes view notifications caused by a launcher grid command.
/// </summary>
internal readonly record struct LauncherGridUpdate(
    string? Status,
    bool SelectionChanged,
    bool TabChanged,
    bool NeedsDisplay)
{
    public static LauncherGridUpdate None { get; } = new(null, false, false, false);

    public bool HasStatus => !string.IsNullOrWhiteSpace(Status);
}
