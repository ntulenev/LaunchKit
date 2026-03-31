using Abstractions;

using Microsoft.Extensions.Configuration;

using Models;

namespace Infrastructure;

/// <summary>
/// Loads launcher options from <c>appsettings.json</c>.
/// </summary>
public sealed class JsonLauncherConfiguration : ILauncherConfiguration
{
    /// <summary>
    /// Loads launcher options from the application configuration file.
    /// </summary>
    /// <returns>The loaded launcher options, or a default instance when the section is missing.</returns>
    public LauncherOptions Load()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .Build();

        var document = configuration.GetSection("Launcher").Get<LauncherConfigurationDocument>()
            ?? new LauncherConfigurationDocument();

        return document.ToOptions();
    }
}
