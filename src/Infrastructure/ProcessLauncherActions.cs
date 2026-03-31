using System.ComponentModel;
using System.Diagnostics;

using Abstractions;

using Models;

namespace Infrastructure;

/// <summary>
/// Launches applications and opens their folders by using operating system processes.
/// </summary>
public sealed class ProcessLauncherActions : ILauncherActions
{
    /// <summary>
    /// Launches the specified application entry.
    /// </summary>
    /// <param name="application">Application entry to launch.</param>
    /// <returns>A status message describing the result.</returns>
    public string Launch(ApplicationEntry application)
    {
        ArgumentNullException.ThrowIfNull(application);

        if (!application.CanLaunch())
        {
            return $"Cannot launch: {application.Name.Value}. Path not found.";
        }

        var startInfo = new ProcessStartInfo
        {
            FileName = application.Path.Value,
            Arguments = application.Arguments.Value,
            WorkingDirectory = ResolveWorkingDirectory(application) ?? string.Empty,
            UseShellExecute = true
        };

        try
        {
            _ = Process.Start(startInfo);
            return $"Launched: {application.Name.Value}";
        }
        catch (Win32Exception ex)
        {
            return $"Launch failed: {application.Name.Value}. {ex.Message}";
        }
        catch (InvalidOperationException ex)
        {
            return $"Launch failed: {application.Name.Value}. {ex.Message}";
        }
        catch (FileNotFoundException ex)
        {
            return $"Launch failed: {application.Name.Value}. {ex.Message}";
        }
        catch (PlatformNotSupportedException ex)
        {
            return $"Launch failed: {application.Name.Value}. {ex.Message}";
        }
    }

    /// <summary>
    /// Opens the containing folder for the specified application entry.
    /// </summary>
    /// <param name="application">Application entry whose folder should be opened.</param>
    /// <returns>A status message describing the result.</returns>
    public string OpenContainingFolder(ApplicationEntry application)
    {
        ArgumentNullException.ThrowIfNull(application);

        var folder = ResolveContainingFolder(application);
        if (string.IsNullOrWhiteSpace(folder) || !Directory.Exists(folder))
        {
            return $"Cannot open folder for: {application.Name.Value}";
        }

        try
        {
            _ = Process.Start(new ProcessStartInfo
            {
                FileName = folder,
                UseShellExecute = true
            });

            return $"Opened folder: {folder}";
        }
        catch (Win32Exception ex)
        {
            return $"Open folder failed: {application.Name.Value}. {ex.Message}";
        }
        catch (InvalidOperationException ex)
        {
            return $"Open folder failed: {application.Name.Value}. {ex.Message}";
        }
        catch (FileNotFoundException ex)
        {
            return $"Open folder failed: {application.Name.Value}. {ex.Message}";
        }
        catch (PlatformNotSupportedException ex)
        {
            return $"Open folder failed: {application.Name.Value}. {ex.Message}";
        }
    }

    private static string? ResolveWorkingDirectory(ApplicationEntry application)
        => application.WorkingDirectory?.ProcessWorkingDirectory
            ?? application.Path.ProcessWorkingDirectory;

    private static string? ResolveContainingFolder(ApplicationEntry application)
        => application.WorkingDirectory?.ContainingFolder
            ?? application.Path.ContainingFolder;
}
