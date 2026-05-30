namespace Infrastructure;

/// <summary>
/// Handles launcher grid commands.
/// </summary>
internal interface ILauncherGridController
{
    /// <summary>
    /// Launches the selected application.
    /// </summary>
    /// <returns>The resulting grid update.</returns>
    LauncherGridUpdate LaunchSelection();

    /// <summary>
    /// Launches the selected application with administrator privileges.
    /// </summary>
    /// <returns>The resulting grid update.</returns>
    LauncherGridUpdate LaunchSelectionAsAdmin();

    /// <summary>
    /// Opens the containing folder for the selected application.
    /// </summary>
    /// <returns>The resulting grid update.</returns>
    LauncherGridUpdate OpenSelectionFolder();

    /// <summary>
    /// Switches to the next application tab.
    /// </summary>
    /// <returns>The resulting grid update.</returns>
    LauncherGridUpdate NextTab();

    /// <summary>
    /// Reloads launcher options.
    /// </summary>
    /// <returns>The resulting grid update.</returns>
    LauncherGridUpdate ReloadSelection();

    /// <summary>
    /// Moves the selected application by the specified offset.
    /// </summary>
    /// <param name="delta">Relative selection offset.</param>
    /// <returns>The resulting grid update.</returns>
    LauncherGridUpdate MoveSelection(int delta);

    /// <summary>
    /// Sets the selected application index.
    /// </summary>
    /// <param name="index">Requested selection index.</param>
    /// <returns>The resulting grid update.</returns>
    LauncherGridUpdate SetSelection(int index);
}
