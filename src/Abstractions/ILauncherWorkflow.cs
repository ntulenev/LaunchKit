namespace Abstractions;

/// <summary>
/// Coordinates the launcher execution flow.
/// </summary>
public interface ILauncherWorkflow
{
    /// <summary>
    /// Runs the launcher workflow.
    /// </summary>
    /// <param name="cancellationToken">Token used to stop the workflow.</param>
    Task RunAsync(CancellationToken cancellationToken);
}
