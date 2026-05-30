namespace Models;

/// <summary>
/// Represents the tab/group assigned to an application entry.
/// </summary>
public sealed record ApplicationTab
{
    /// <summary>
    /// Gets the default tab name used when a tab is not configured.
    /// </summary>
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

    /// <summary>
    /// Gets the normalized tab name.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Gets a value indicating whether this tab is the default tab.
    /// </summary>
    public bool IsDefault => string.Equals(Value, DefaultValue, StringComparison.Ordinal);

    /// <summary>
    /// Returns the normalized tab name.
    /// </summary>
    /// <returns>The normalized value.</returns>
    public override string ToString() => Value;
}
