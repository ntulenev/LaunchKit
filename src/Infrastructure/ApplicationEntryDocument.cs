using Models;

namespace Infrastructure;

/// <summary>
/// Mutable application entry document used only for configuration binding.
/// </summary>
public sealed class ApplicationEntryDocument
{
    public string? Name { get; set; }

    public string? Path { get; set; }

    public string? Arguments { get; set; }

    public string? WorkingDirectory { get; set; }

    public string? Description { get; set; }

    /// <summary>
    /// Converts the mutable configuration document into an immutable application entry.
    /// </summary>
    /// <param name="index">Zero-based entry index used for configuration error paths.</param>
    /// <returns>The mapped application entry.</returns>
    public ApplicationEntry ToModel(int index)
        => ApplicationEntry.Create(
            Name,
            Path,
            Arguments,
            WorkingDirectory,
            Description,
            $"Launcher:Applications:{index}");
}
