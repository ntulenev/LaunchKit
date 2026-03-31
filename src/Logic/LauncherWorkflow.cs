using Abstractions;

namespace Logic;

/// <summary>
/// Coordinates configuration loading and UI execution for the launcher.
/// </summary>
/// <param name="configuration">Configuration source for launcher options.</param>
/// <param name="consoleRenderer">Renderer used to display the launcher UI.</param>
public sealed class LauncherWorkflow(
    ILauncherConfiguration configuration,
    IConsoleRenderer consoleRenderer) : ILauncherWorkflow
{
    /// <summary>
    /// Runs the launcher workflow.
    /// </summary>
    /// <param name="cancellationToken">Token used to stop the workflow.</param>
    public async Task RunAsync(CancellationToken cancellationToken)
    {
        var options = _configuration.Load();
        if (!options.HasApplications)
        {
            Console.Clear();
            Console.WriteLine(NO_APPLICATIONS_MESSAGE);
            Console.WriteLine(
                $"Edit {Path.Combine(AppContext.BaseDirectory, "appsettings.json")} and restart.");
            return;
        }

        await _consoleRenderer.RunAsync(options, cancellationToken).ConfigureAwait(false);
    }

    private const string NO_APPLICATIONS_MESSAGE = "No applications configured.";

    private readonly ILauncherConfiguration _configuration = configuration;
    private readonly IConsoleRenderer _consoleRenderer = consoleRenderer;
}
