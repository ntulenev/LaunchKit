namespace Infrastructure;

/// <summary>
/// Reads the current physical keyboard state.
/// </summary>
internal interface IKeyboardState
{
    /// <summary>
    /// Determines whether the specified physical key is currently pressed.
    /// </summary>
    /// <param name="key">The physical key to inspect.</param>
    /// <returns><see langword="true"/> when the key is pressed; otherwise, <see langword="false"/>.</returns>
    bool IsKeyDown(KeyboardKey key);
}
