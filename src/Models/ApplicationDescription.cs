namespace Models;

/// <summary>
/// Represents application description text.
/// </summary>
public sealed record ApplicationDescription
{
    /// <summary>
    /// Initializes a new description value.
    /// </summary>
    /// <param name="value">Raw description text.</param>
    public ApplicationDescription(string? value)
    {
        Value = string.IsNullOrWhiteSpace(value)
            ? string.Empty
            : value.Trim();
    }

    public string Value { get; }

    public bool HasValue => Value.Length > 0;

    /// <summary>
    /// Returns the normalized description text.
    /// </summary>
    /// <returns>The normalized value.</returns>
    public override string ToString() => Value;
}
