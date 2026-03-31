namespace Models;

/// <summary>
/// Represents an optional working directory path.
/// </summary>
public sealed record WorkingDirectoryPath
{
    /// <summary>
    /// Initializes a new working directory path value.
    /// </summary>
    /// <param name="value">Raw working directory path.</param>
    public WorkingDirectoryPath(string? value)
    {
        Value = Normalize(value);
    }

    public string Value { get; }

    /// <summary>
    /// Creates an optional working directory path value.
    /// </summary>
    /// <param name="value">Raw working directory path.</param>
    /// <returns>A working directory value when input is present; otherwise, <see langword="null"/>.</returns>
    public static WorkingDirectoryPath? CreateOptional(string? value)
        => string.IsNullOrWhiteSpace(value)
            ? null
            : new WorkingDirectoryPath(value);

    public string ProcessWorkingDirectory => Value;

    public string? ContainingFolder
        => Directory.Exists(Value)
            ? Value
            : null;

    /// <summary>
    /// Returns the normalized working directory path.
    /// </summary>
    /// <returns>The normalized value.</returns>
    public override string ToString() => Value;

    private static string Normalize(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidDataException("Working directory path cannot be empty.");
        }

        return Environment.ExpandEnvironmentVariables(value.Trim().Trim('"'));
    }
}
