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

    /// <summary>
    /// Gets the normalized description text.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Gets a value indicating whether description text is present.
    /// </summary>
    public bool HasValue => Value.Length > 0;

    /// <summary>
    /// Returns the normalized description text.
    /// </summary>
    /// <returns>The normalized value.</returns>
    public override string ToString() => Value;
}
