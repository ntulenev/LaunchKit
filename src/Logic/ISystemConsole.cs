namespace Logic;

/// <summary>
/// Provides a minimal console abstraction for launcher workflow output.
/// </summary>
internal interface ISystemConsole
{
    /// <summary>
    /// Clears the current console buffer.
    /// </summary>
    void Clear();

    /// <summary>
    /// Writes a line of text to the console.
    /// </summary>
    /// <param name="value">The text to write.</param>
    void WriteLine(string? value);
}
