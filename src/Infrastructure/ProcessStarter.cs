using System.Diagnostics;

namespace Infrastructure;

/// <summary>
/// Default implementation that forwards process starts to <see cref="Process"/>.
/// </summary>
internal sealed class ProcessStarter : IProcessStarter
{
    /// <summary>
    /// Starts a process with the specified start information.
    /// </summary>
    /// <param name="startInfo">The process start information.</param>
    /// <returns>The started process, or <see langword="null"/> when no process instance is returned.</returns>
    public Process? Start(ProcessStartInfo startInfo)
        => Process.Start(startInfo);
}
