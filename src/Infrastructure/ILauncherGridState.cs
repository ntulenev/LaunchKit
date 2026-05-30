using System.Collections.ObjectModel;

using Models;

namespace Infrastructure;

/// <summary>
/// Tracks launcher grid tab and selection state.
/// </summary>
internal interface ILauncherGridState
{
    /// <summary>
    /// Gets the current launcher options.
    /// </summary>
    LauncherOptions Options { get; }

    /// <summary>
    /// Gets the active application tab.
    /// </summary>
    ApplicationTab ActiveTab { get; }

    /// <summary>
    /// Gets the applications visible in the active tab.
    /// </summary>
    ReadOnlyCollection<ApplicationEntry> ActiveApplications { get; }

    /// <summary>
    /// Gets the currently selected application, or <see langword="null"/> when the grid is empty.
    /// </summary>
    ApplicationEntry? SelectedApplication { get; }

    /// <summary>
    /// Gets the selected index within the active tab.
    /// </summary>
    int SelectedIndex { get; }

    /// <summary>
    /// Moves the current selection by the specified delta.
    /// </summary>
    /// <param name="delta">Relative selection offset.</param>
    /// <returns><see langword="true"/> when the selection changed; otherwise, <see langword="false"/>.</returns>
    bool MoveSelection(int delta);

    /// <summary>
    /// Sets the current selection index.
    /// </summary>
    /// <param name="index">Requested selection index.</param>
    /// <returns><see langword="true"/> when the selection changed; otherwise, <see langword="false"/>.</returns>
    bool SetSelection(int index);

    /// <summary>
    /// Switches to the next tab.
    /// </summary>
    /// <returns><see langword="true"/> when the active tab changed; otherwise, <see langword="false"/>.</returns>
    bool NextTab();

    /// <summary>
    /// Replaces launcher options while preserving active tab and selection when possible.
    /// </summary>
    /// <param name="options">Reloaded launcher options.</param>
    void Reload(LauncherOptions options);

    /// <summary>
    /// Creates a layout state for the current active applications.
    /// </summary>
    /// <param name="availableWidth">Available view width.</param>
    /// <param name="availableHeight">Available view height.</param>
    /// <returns>The calculated layout state.</returns>
    LayoutState CreateLayoutState(int availableWidth, int availableHeight);
}
