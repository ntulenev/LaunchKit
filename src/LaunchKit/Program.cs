using Abstractions;

using Infrastructure;

using LaunchKit.Utility;

using Logic;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using var cancellationTokenSource = new CancellationTokenSource();

var builder = Host.CreateDefaultBuilder()
    .ConfigureServices((hostContext, services) =>
    {
        _ = hostContext;
        _ = services.AddSingleton<IApplication, Application>();
        _ = services.AddSingleton<ILauncherConfiguration, JsonLauncherConfiguration>();
        _ = services.AddSingleton<ILauncherActions, ProcessLauncherActions>();
        _ = services.AddSingleton<IConsoleRenderer, ConsoleRenderer>();
        _ = services.AddSingleton<ILauncherWorkflow, LauncherWorkflow>();
    });

var host = builder.Build();
using var scope = host.Services.CreateScope();
var application = scope.ServiceProvider.GetRequiredService<IApplication>();
await application.RunAsync(cancellationTokenSource.Token).ConfigureAwait(false);
