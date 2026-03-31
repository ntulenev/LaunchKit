using Terminal.Gui;

namespace Infrastructure;

/// <summary>
/// Adapts static <see cref="Application"/> calls to <see cref="ITerminalFacade"/>.
/// </summary>
internal sealed class TerminalFacade : ITerminalFacade
{
    /// <summary>
    /// Gets the top-level container for Terminal.Gui views.
    /// </summary>
    public Toplevel Top => Application.Top;

    /// <summary>
    /// Initializes the terminal application.
    /// </summary>
    public void Init() => Application.Init();

    /// <summary>
    /// Runs the terminal application loop.
    /// </summary>
    public void Run() => Application.Run();

    /// <summary>
    /// Shuts down the terminal application.
    /// </summary>
    public void Shutdown() => Application.Shutdown();

    /// <summary>
    /// Requests the terminal application to stop.
    /// </summary>
    public void RequestStop() => Application.RequestStop();
}
