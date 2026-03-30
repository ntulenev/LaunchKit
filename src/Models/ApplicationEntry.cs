namespace Models;

/// <summary>
/// Describes a single application entry that can be shown in the launcher.
/// </summary>
public sealed class ApplicationEntry
{
    public string Name { get; set; } = string.Empty;

    public string Path { get; set; } = string.Empty;

    public string? Arguments { get; set; }

    public string? WorkingDirectory { get; set; }

    public string? Description { get; set; }
}
