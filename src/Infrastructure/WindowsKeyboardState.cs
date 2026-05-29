using System.Runtime.InteropServices;

namespace Infrastructure;

/// <summary>
/// Reads the Windows keyboard state by using the native user32 API.
/// </summary>
internal sealed class WindowsKeyboardState : IKeyboardState
{
    /// <inheritdoc />
    public bool IsKeyDown(KeyboardKey key)
        => OperatingSystem.IsWindows()
           && (GetAsyncKeyState(GetVirtualKey(key)) & KEY_PRESSED_MASK) != 0;

    private static int GetVirtualKey(KeyboardKey key)
        => key switch
        {
            KeyboardKey.A => VIRTUAL_KEY_A,
            KeyboardKey.O => VIRTUAL_KEY_O,
            _ => throw new ArgumentOutOfRangeException(nameof(key), key, null)
        };

#pragma warning disable SYSLIB1054
    [DllImport("user32.dll")]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    private static extern short GetAsyncKeyState(int virtualKey);
#pragma warning restore SYSLIB1054

    private const int KEY_PRESSED_MASK = 0x8000;
    private const int VIRTUAL_KEY_A = 0x41;
    private const int VIRTUAL_KEY_O = 0x4F;
}
