using Abstractions;

namespace LaunchKit.Utility;

public sealed class Application(ILauncherWorkflow launcherWorkflow) : IApplication
{
    private readonly ILauncherWorkflow _launcherWorkflow = launcherWorkflow;

    public Task RunAsync(CancellationToken cancellationToken)
        => _launcherWorkflow.RunAsync(cancellationToken);
}
