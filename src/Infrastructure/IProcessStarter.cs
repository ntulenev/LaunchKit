using System.Diagnostics;

namespace Infrastructure;

internal interface IProcessStarter
{
    Process? Start(ProcessStartInfo startInfo);
}
