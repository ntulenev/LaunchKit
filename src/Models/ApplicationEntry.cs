namespace Models;

public sealed class ApplicationEntry
{
    public string Name { get; set; } = string.Empty;

    public string Path { get; set; } = string.Empty;

    public string? Arguments { get; set; }

    public string? WorkingDirectory { get; set; }

    public string? Description { get; set; }
}
