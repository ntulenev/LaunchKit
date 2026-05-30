using System.Collections.ObjectModel;

using Models;

namespace Infrastructure;

/// <summary>
/// Mutable configuration document used only for binding appsettings.json.
/// </summary>
public sealed class LauncherConfigurationDocument
{
    /// <summary>
    /// Gets or sets the mutable layout configuration document.
    /// </summary>
    public LayoutConfigurationDocument Layout { get; set; } = new();

    /// <summary>
    /// Gets or sets a value indicating whether tiles should show full paths.
    /// </summary>
    public bool ShowFullPath { get; set; } = true;

    /// <summary>
    /// Gets the mutable configured application documents.
    /// </summary>
    public Collection<ApplicationEntryDocument> Applications { get; } = [];

    /// <summary>
    /// Converts the mutable configuration document into immutable launcher options.
    /// </summary>
    /// <returns>The mapped launcher options.</returns>
    public LauncherOptions ToOptions()
        => new(
            Layout.ToOptions(),
            Applications.Select((application, index) => application.ToModel(index)),
            ShowFullPath);
}
