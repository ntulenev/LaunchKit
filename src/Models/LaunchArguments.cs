namespace Models;

/// <summary>
/// Represents optional launch arguments.
/// </summary>
public sealed record LaunchArguments
{
    /// <summary>
    /// Initializes a new launch arguments value.
    /// </summary>
    /// <param name="value">Raw launch arguments.</param>
    public LaunchArguments(string? value)
    {
        Value = string.IsNullOrWhiteSpace(value)
            ? string.Empty
            : Environment.ExpandEnvironmentVariables(value.Trim().Trim('"'));
    }

    public string Value { get; }

    public bool HasValue => Value.Length > 0;

    /// <summary>
    /// Returns the normalized launch arguments.
    /// </summary>
    /// <returns>The normalized value.</returns>
    public override string ToString() => Value;
}
