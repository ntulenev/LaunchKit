using System.Collections.ObjectModel;

namespace Models;

public sealed class LauncherOptions
{
    public LayoutOptions Layout { get; set; } = new();

    public Collection<ApplicationEntry> Applications { get; } = [];
}
