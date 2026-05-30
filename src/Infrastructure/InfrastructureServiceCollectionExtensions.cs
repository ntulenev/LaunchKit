using Abstractions;

using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

/// <summary>
/// Registers LaunchKit infrastructure services.
/// </summary>
public static class InfrastructureServiceCollectionExtensions
{
    /// <summary>
    /// Adds the infrastructure services used by the launcher.
    /// </summary>
    /// <param name="services">Service collection to configure.</param>
    /// <returns>The configured service collection.</returns>
    public static IServiceCollection AddLaunchKitInfrastructure(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        _ = services.AddSingleton<ILauncherConfiguration, JsonLauncherConfiguration>();
        _ = services.AddSingleton<IProcessStarter, ProcessStarter>();
        _ = services.AddSingleton<ILauncherActions>(serviceProvider =>
            new ProcessLauncherActions(serviceProvider.GetRequiredService<IProcessStarter>()));
        _ = services.AddSingleton<IKeyboardState, WindowsKeyboardState>();
        _ = services.AddSingleton<ILauncherShortcutResolver, LauncherShortcutResolver>();
        _ = services.AddSingleton<ITerminalFacade, TerminalFacade>();
        _ = services.AddSingleton<ILauncherGridStateFactory, LauncherGridStateFactory>();
        _ = services.AddSingleton<ILauncherGridControllerFactory, LauncherGridControllerFactory>();
        _ = services.AddSingleton<ILauncherTileFormatter, LauncherTileFormatter>();
        _ = services.AddSingleton<ILauncherGridViewFactory, LauncherGridViewFactory>();
        _ = services.AddSingleton<IConsoleRenderer>(serviceProvider =>
            new ConsoleRenderer(
                serviceProvider.GetRequiredService<ILauncherConfiguration>(),
                serviceProvider.GetRequiredService<ILauncherGridViewFactory>(),
                serviceProvider.GetRequiredService<ITerminalFacade>()));

        return services;
    }
}
