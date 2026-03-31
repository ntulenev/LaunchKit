using Terminal.Gui;

namespace Infrastructure;

internal interface ITerminalFacade
{
    Toplevel Top { get; }

    void Init();

    void Run();

    void Shutdown();

    void RequestStop();
}
