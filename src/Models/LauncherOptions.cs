using System.Collections.ObjectModel;

namespace Models;

/// <summary>
/// Represents the launcher configuration loaded from application settings.
/// </summary>
public sealed class LauncherOptions
{
    /// <summary>
    /// Initializes a new immutable launcher configuration.
    /// </summary>
    /// <param name="layout">Configured layout options.</param>
    /// <param name="applications">Configured application entries.</param>
    public LauncherOptions(LayoutOptions layout, IEnumerable<ApplicationEntry> applications)
    {
        Layout = layout ?? throw new ArgumentNullException(nameof(layout));
        ArgumentNullException.ThrowIfNull(applications);

        Applications = new ReadOnlyCollection<ApplicationEntry>([.. applications]);
    }

    public LayoutOptions Layout { get; }

    public ReadOnlyCollection<ApplicationEntry> Applications { get; }

    public int Count => Applications.Count;

    public bool HasApplications => Count > 0;

    /// <summary>
    /// Gets the application entry at the specified index.
    /// </summary>
    /// <param name="index">Zero-based application index.</param>
    /// <returns>The requested application entry.</returns>
    public ApplicationEntry GetApplication(int index) => Applications[index];

    /// <summary>
    /// Clamps a selection index to the valid application range.
    /// </summary>
    /// <param name="index">Requested selection index.</param>
    /// <returns>A valid selection index for the current application collection.</returns>
    public int ClampSelection(int index)
        => Count == 0
            ? 0
            : Math.Clamp(index, 0, Count - 1);

    /// <summary>
    /// Creates the calculated layout state for the current console bounds.
    /// </summary>
    /// <param name="availableWidth">Available view width.</param>
    /// <param name="availableHeight">Available view height.</param>
    /// <returns>The calculated layout state.</returns>
    public LayoutState CreateLayoutState(int availableWidth, int availableHeight)
        => Layout.CalculateState(availableWidth, availableHeight, Count);
}
