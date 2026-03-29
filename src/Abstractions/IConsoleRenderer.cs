using Models;

namespace Abstractions;

public interface IConsoleRenderer
{
    Task RunAsync(LauncherOptions options, CancellationToken cancellationToken);
}
