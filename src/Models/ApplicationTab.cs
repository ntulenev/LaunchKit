namespace Models;

/// <summary>
/// Represents the tab/group assigned to an application entry.
/// </summary>
public sealed record ApplicationTab
{
    public static string DefaultValue { get; } = "Apps";

    /// <summary>
    /// Initializes a new tab value.
    /// </summary>
    /// <param name="value">Raw tab name.</param>
    public ApplicationTab(string? value)
    {
        Value = string.IsNullOrWhiteSpace(value)
            ? DefaultValue
            : value.Trim();
    }

    public string Value { get; }

    public bool IsDefault => string.Equals(Value, DefaultValue, StringComparison.Ordinal);

    /// <summary>
    /// Returns the normalized tab name.
    /// </summary>
    /// <returns>The normalized value.</returns>
    public override string ToString() => Value;
}
