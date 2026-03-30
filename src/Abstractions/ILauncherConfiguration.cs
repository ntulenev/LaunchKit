using Models;

namespace Abstractions;

/// <summary>
/// Loads launcher configuration data.
/// </summary>
public interface ILauncherConfiguration
{
    /// <summary>
    /// Loads launcher options from the configured source.
    /// </summary>
    /// <returns>The loaded launcher options.</returns>
    LauncherOptions Load();
}
