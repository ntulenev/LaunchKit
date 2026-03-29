using Abstractions;

namespace Logic;

public sealed class LauncherWorkflow(
    ILauncherConfiguration configuration,
    IConsoleRenderer consoleRenderer) : ILauncherWorkflow
{
    private const string NO_APPLICATIONS_MESSAGE = "No applications configured.";

    private readonly ILauncherConfiguration _configuration = configuration;
    private readonly IConsoleRenderer _consoleRenderer = consoleRenderer;

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        var options = _configuration.Load();
        if (options.Applications.Count == 0)
        {
            Console.Clear();
            Console.WriteLine(NO_APPLICATIONS_MESSAGE);
            Console.WriteLine(
                $"Edit {Path.Combine(AppContext.BaseDirectory, "appsettings.json")} and restart.");
            return;
        }

        await _consoleRenderer.RunAsync(options, cancellationToken);
    }
}
