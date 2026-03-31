using Abstractions;

namespace LaunchKit.Utility;

/// <summary>
/// Default application entry point implementation.
/// </summary>
/// <param name="launcherWorkflow">Workflow that coordinates application execution.</param>
public sealed class Application(ILauncherWorkflow launcherWorkflow) : IApplication
{
    /// <summary>
    /// Runs the application workflow.
    /// </summary>
    /// <param name="cancellationToken">Token used to stop the workflow.</param>
    public Task RunAsync(CancellationToken cancellationToken)
        => _launcherWorkflow.RunAsync(cancellationToken);

    private readonly ILauncherWorkflow _launcherWorkflow = launcherWorkflow;
}
