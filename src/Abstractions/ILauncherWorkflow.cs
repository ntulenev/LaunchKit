namespace Abstractions;

public interface ILauncherWorkflow
{
    Task RunAsync(CancellationToken cancellationToken);
}
