using IOPath = System.IO.Path;

namespace Models;

/// <summary>
/// Represents a validated launch target path or shell command.
/// </summary>
public sealed record ApplicationPath
{
    /// <summary>
    /// Initializes a new application path value.
    /// </summary>
    /// <param name="value">Raw launch target path or shell command.</param>
    public ApplicationPath(string? value)
    {
        Value = Normalize(value);
    }

    public string Value { get; }

    /// <summary>
    /// Determines whether the path points to a launchable target.
    /// </summary>
    /// <returns><see langword="true"/> when the value can be launched; otherwise, <see langword="false"/>.</returns>
    public bool CanLaunch()
    {
        if (File.Exists(Value) || Directory.Exists(Value))
        {
            return true;
        }

        return !HasPathHints(Value);
    }

    /// <summary>
    /// Gets the directory name when the value contains path information.
    /// </summary>
    /// <returns>The directory name, or <see langword="null"/> when the value is not a path.</returns>
    public string? GetDirectoryName()
        => HasPathHints(Value)
            ? IOPath.GetDirectoryName(Value)
            : null;

    /// <summary>
    /// Gets a compact display name for the configured path.
    /// </summary>
    /// <returns>
    /// The executable or folder name when the value looks like a path; otherwise, the command text itself.
    /// </returns>
    public string GetDisplayName()
    {
        if (!HasPathHints(Value))
        {
            return Value;
        }

        var trimmedValue = Value.TrimEnd(IOPath.DirectorySeparatorChar, IOPath.AltDirectorySeparatorChar);
        var fileName = IOPath.GetFileName(trimmedValue);

        return string.IsNullOrWhiteSpace(fileName)
            ? Value
            : fileName;
    }

    public string? ProcessWorkingDirectory
        => File.Exists(Value)
            ? IOPath.GetDirectoryName(Value)
            : null;

    public string? ContainingFolder
        => Directory.Exists(Value)
            ? Value
            : GetDirectoryName();

    /// <summary>
    /// Returns the normalized launch path value.
    /// </summary>
    /// <returns>The normalized value.</returns>
    public override string ToString() => Value;

    private static bool HasPathHints(string value)
        => value.Contains(IOPath.DirectorySeparatorChar.ToString(), StringComparison.Ordinal)
            || value.Contains(IOPath.AltDirectorySeparatorChar.ToString(), StringComparison.Ordinal)
            || IOPath.IsPathRooted(value);

    private static string Normalize(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidDataException("Application path is required.");
        }

        return Environment.ExpandEnvironmentVariables(value.Trim().Trim('"'));
    }
}
