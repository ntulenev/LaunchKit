namespace Models;

/// <summary>
/// Describes a single application entry that can be shown in the launcher.
/// </summary>
public sealed class ApplicationEntry
{
    /// <summary>
    /// Initializes a new immutable application entry.
    /// </summary>
    /// <param name="name">Validated application name.</param>
    /// <param name="path">Validated launch target path.</param>
    /// <param name="arguments">Optional launch arguments.</param>
    /// <param name="workingDirectory">Optional working directory.</param>
    /// <param name="description">Optional application description.</param>
    public ApplicationEntry(
        ApplicationName name,
        ApplicationPath path,
        LaunchArguments? arguments = null,
        WorkingDirectoryPath? workingDirectory = null,
        ApplicationDescription? description = null)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Path = path ?? throw new ArgumentNullException(nameof(path));
        Arguments = arguments ?? new LaunchArguments(null);
        WorkingDirectory = workingDirectory;
        Description = description ?? new ApplicationDescription(null);
    }

    public ApplicationName Name { get; }

    public ApplicationPath Path { get; }

    public LaunchArguments Arguments { get; }

    public WorkingDirectoryPath? WorkingDirectory { get; }

    public ApplicationDescription Description { get; }

    /// <summary>
    /// Creates an application entry from raw configuration values.
    /// </summary>
    /// <param name="name">Raw application name.</param>
    /// <param name="path">Raw launch target path.</param>
    /// <param name="arguments">Raw launch arguments.</param>
    /// <param name="workingDirectory">Raw working directory.</param>
    /// <param name="description">Raw description.</param>
    /// <param name="source">Optional configuration path used in error messages.</param>
    /// <returns>A validated immutable application entry.</returns>
    public static ApplicationEntry Create(
        string? name,
        string? path,
        string? arguments = null,
        string? workingDirectory = null,
        string? description = null,
        string? source = null)
    {
        try
        {
            return new ApplicationEntry(
                new ApplicationName(name),
                new ApplicationPath(path),
                new LaunchArguments(arguments),
                WorkingDirectoryPath.CreateOptional(workingDirectory),
                new ApplicationDescription(description));
        }
        catch (InvalidDataException exception)
        {
            throw new InvalidDataException(BuildError(source, exception.Message), exception);
        }
    }

    /// <summary>
    /// Gets the current launch availability for the application entry.
    /// </summary>
    /// <returns>The calculated availability value.</returns>
    public ApplicationAvailability GetAvailability()
        => Path.CanLaunch()
            ? ApplicationAvailability.Ready
            : ApplicationAvailability.PathNotFound;

    /// <summary>
    /// Determines whether the application entry can currently be launched.
    /// </summary>
    /// <returns><see langword="true"/> when the application can be launched; otherwise, <see langword="false"/>.</returns>
    public bool CanLaunch() => GetAvailability() is ApplicationAvailability.Ready;

    private static string BuildError(string? source, string message)
        => string.IsNullOrWhiteSpace(source)
            ? message
            : $"{source}: {message}";
}
