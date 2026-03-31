namespace Logic;

/// <summary>
/// Adapts <see cref="Console"/> to <see cref="ISystemConsole"/>.
/// </summary>
internal sealed class SystemConsole : ISystemConsole
{
    /// <summary>
    /// Clears the current console buffer.
    /// </summary>
    public void Clear() => Console.Clear();

    /// <summary>
    /// Writes a line of text to the console.
    /// </summary>
    /// <param name="value">The text to write.</param>
    public void WriteLine(string? value) => Console.WriteLine(value);
}
