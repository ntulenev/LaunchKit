using Abstractions;

using Microsoft.Extensions.Configuration;

using Models;

namespace Infrastructure;

public sealed class JsonLauncherConfiguration : ILauncherConfiguration
{
    public LauncherOptions Load()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .Build();

        var options = configuration.GetSection("Launcher").Get<LauncherOptions>()
            ?? new LauncherOptions();

        return options;
    }
}
