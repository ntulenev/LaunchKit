using System.Diagnostics;

namespace Infrastructure;

internal sealed class ProcessStarter : IProcessStarter
{
    public Process? Start(ProcessStartInfo startInfo)
        => Process.Start(startInfo);
}
