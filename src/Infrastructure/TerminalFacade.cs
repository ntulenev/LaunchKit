using Terminal.Gui;

namespace Infrastructure;

internal sealed class TerminalFacade : ITerminalFacade
{
    public Toplevel Top => Application.Top;

    public void Init() => Application.Init();

    public void Run() => Application.Run();

    public void Shutdown() => Application.Shutdown();

    public void RequestStop() => Application.RequestStop();
}
