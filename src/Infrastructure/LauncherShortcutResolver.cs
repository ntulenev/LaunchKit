using Terminal.Gui;

namespace Infrastructure;

/// <summary>
/// Resolves launcher shortcuts across terminal input and alternate keyboard layouts.
/// </summary>
/// <param name="keyboardState">Keyboard state used to inspect physical key presses.</param>
internal sealed class LauncherShortcutResolver(IKeyboardState keyboardState) : ILauncherShortcutResolver
{
    /// <inheritdoc />
    public bool IsAdminLaunchShortcut(Key key)
        => IsShortcutKey(key, '\u0444', '\u0424', KeyboardKey.A);

    /// <inheritdoc />
    public bool IsOpenFolderShortcut(Key key)
        => IsShortcutKey(key, '\u0449', '\u0429', KeyboardKey.O);

    private bool IsShortcutKey(Key key, char russianLowerKey, char russianUpperKey, KeyboardKey physicalKey)
        => key == (Key)russianLowerKey
           || key == (Key)russianUpperKey
           || _keyboardState.IsKeyDown(physicalKey);

    private readonly IKeyboardState _keyboardState = keyboardState
        ?? throw new ArgumentNullException(nameof(keyboardState));
}
