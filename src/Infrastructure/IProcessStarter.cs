using System.Diagnostics;

namespace Infrastructure;

/// <summary>
/// Starts operating system processes for launcher actions.
/// </summary>
internal interface IProcessStarter
{
    /// <summary>
    /// Starts a process with the specified start information.
    /// </summary>
    /// <param name="startInfo">The process start information.</param>
    /// <returns>The started process, or <see langword="null"/> when no process instance is returned.</returns>
    Process? Start(ProcessStartInfo startInfo);
}
