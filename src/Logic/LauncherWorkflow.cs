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
    internal LauncherWorkflow(
        ILauncherConfiguration configuration,
        IConsoleRenderer consoleRenderer,
        ISystemConsole systemConsole)
        : this(configuration, consoleRenderer)
    {
        _systemConsole = systemConsole ?? throw new ArgumentNullException(nameof(systemConsole));
    }

    /// <summary>
    /// Runs the launcher workflow.
    /// </summary>
    /// <param name="cancellationToken">Token used to stop the workflow.</param>
    public async Task RunAsync(CancellationToken cancellationToken)
    {
        var options = _configuration.Load();
        if (!options.HasApplications)
        {
            _systemConsole.Clear();
            _systemConsole.WriteLine(NO_APPLICATIONS_MESSAGE);
            _systemConsole.WriteLine(
                $"Edit {Path.Combine(AppContext.BaseDirectory, "appsettings.json")} and restart.");
            return;
        }

        await _consoleRenderer.RunAsync(options, cancellationToken).ConfigureAwait(false);
    }

    private const string NO_APPLICATIONS_MESSAGE = "No applications configured.";

    private readonly ILauncherConfiguration _configuration = configuration
        ?? throw new ArgumentNullException(nameof(configuration));
    private readonly IConsoleRenderer _consoleRenderer = consoleRenderer
        ?? throw new ArgumentNullException(nameof(consoleRenderer));
    private readonly ISystemConsole _systemConsole = new SystemConsole();
}
