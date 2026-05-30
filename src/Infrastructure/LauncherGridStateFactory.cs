using Models;

namespace Infrastructure;

/// <summary>
/// Default factory for launcher grid state instances.
/// </summary>
internal sealed class LauncherGridStateFactory : ILauncherGridStateFactory
{
    /// <inheritdoc />
    public ILauncherGridState Create(LauncherOptions options)
        => new LauncherGridState(options);
}
