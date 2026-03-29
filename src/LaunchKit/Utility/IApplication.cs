namespace LaunchKit.Utility;

public interface IApplication
{
    Task RunAsync(CancellationToken cancellationToken);
}
