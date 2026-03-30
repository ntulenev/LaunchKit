namespace Abstractions;

/// <summary>
/// Represents the application entry workflow.
/// </summary>
public interface IApplication
{
    /// <summary>
    /// Runs the application workflow.
    /// </summary>
    /// <param name="cancellationToken">Token used to stop the workflow.</param>
    Task RunAsync(CancellationToken cancellationToken);
}
