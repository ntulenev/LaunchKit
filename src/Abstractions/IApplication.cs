namespace Abstractions;

public interface IApplication
{
    Task RunAsync(CancellationToken cancellationToken);
}
