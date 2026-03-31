namespace Models;

/// <summary>
/// Represents a validated application name.
/// </summary>
public sealed record ApplicationName
{
    /// <summary>
    /// Initializes a new application name value.
    /// </summary>
    /// <param name="value">Raw application name.</param>
    public ApplicationName(string? value)
    {
        Value = Normalize(value);
    }

    public string Value { get; }

    /// <summary>
    /// Returns the normalized application name.
    /// </summary>
    /// <returns>The normalized value.</returns>
    public override string ToString() => Value;

    private static string Normalize(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidDataException("Application name is required.");
        }

        return value.Trim();
    }
}
