using System.Collections.ObjectModel;

namespace Models;

/// <summary>
/// Represents the launcher configuration loaded from application settings.
/// </summary>
public sealed class LauncherOptions
{
    public LayoutOptions Layout { get; set; } = new();

    public Collection<ApplicationEntry> Applications { get; } = [];
}
