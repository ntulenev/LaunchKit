using Terminal.Gui;

namespace Infrastructure;

/// <summary>
/// Provides a small abstraction over Terminal.Gui application lifecycle operations.
/// </summary>
internal interface ITerminalFacade
{
    /// <summary>
    /// Gets the top-level container for Terminal.Gui views.
    /// </summary>
    Toplevel Top { get; }

    /// <summary>
    /// Initializes the terminal application.
    /// </summary>
    void Init();

    /// <summary>
    /// Runs the terminal application loop.
    /// </summary>
    void Run();

    /// <summary>
    /// Shuts down the terminal application.
    /// </summary>
    void Shutdown();

    /// <summary>
    /// Requests the terminal application to stop.
    /// </summary>
    void RequestStop();
}
