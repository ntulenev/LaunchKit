using Models;

namespace Infrastructure;

/// <summary>
/// Mutable application entry document used only for configuration binding.
/// </summary>
public sealed class ApplicationEntryDocument
{
    /// <summary>
    /// Gets or sets the raw application name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the raw launch target path or command.
    /// </summary>
    public string? Path { get; set; }

    /// <summary>
    /// Gets or sets the raw launch arguments.
    /// </summary>
    public string? Arguments { get; set; }

    /// <summary>
    /// Gets or sets the raw working directory path.
    /// </summary>
    public string? WorkingDirectory { get; set; }

    /// <summary>
    /// Gets or sets the raw application description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the raw tab/group name.
    /// </summary>
    public string? Tab { get; set; }

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
            $"Launcher:Applications:{index}",
            Tab);
}
