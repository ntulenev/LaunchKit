using System.Collections.ObjectModel;

using Models;

namespace Infrastructure;

/// <summary>
/// Mutable configuration document used only for binding appsettings.json.
/// </summary>
public sealed class LauncherConfigurationDocument
{
    public LayoutConfigurationDocument Layout { get; set; } = new();

    public Collection<ApplicationEntryDocument> Applications { get; } = [];

    /// <summary>
    /// Converts the mutable configuration document into immutable launcher options.
    /// </summary>
    /// <returns>The mapped launcher options.</returns>
    public LauncherOptions ToOptions()
        => new(
            Layout.ToOptions(),
            Applications.Select((application, index) => application.ToModel(index)));
}
