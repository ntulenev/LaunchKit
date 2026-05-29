using Terminal.Gui;

namespace Infrastructure;

/// <summary>
/// Resolves launcher keyboard shortcuts from terminal key events and physical key state.
/// </summary>
internal interface ILauncherShortcutResolver
{
    /// <summary>
    /// Determines whether the key input should launch the selected application as administrator.
    /// </summary>
    /// <param name="key">The terminal key input to inspect.</param>
    /// <returns><see langword="true"/> when the admin launch shortcut is active; otherwise, <see langword="false"/>.</returns>
    bool IsAdminLaunchShortcut(Key key);

    /// <summary>
    /// Determines whether the key input should open the selected application's containing folder.
    /// </summary>
    /// <param name="key">The terminal key input to inspect.</param>
    /// <returns><see langword="true"/> when the open folder shortcut is active; otherwise, <see langword="false"/>.</returns>
    bool IsOpenFolderShortcut(Key key);
}
