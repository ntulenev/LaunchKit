using Models;

namespace Abstractions;

/// <summary>
/// Renders the launcher user interface.
/// </summary>
public interface IConsoleRenderer
{
    /// <summary>
    /// Starts the console UI for the provided launcher options.
    /// </summary>
    /// <param name="options">Launcher options to present to the user.</param>
    /// <param name="cancellationToken">Token used to stop the UI loop.</param>
    Task RunAsync(LauncherOptions options, CancellationToken cancellationToken);
}
