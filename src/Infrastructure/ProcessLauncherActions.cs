using System.ComponentModel;
using System.Diagnostics;

using Abstractions;

using Models;

namespace Infrastructure;

public sealed class ProcessLauncherActions : ILauncherActions
{
    public string Launch(ApplicationEntry application)
    {
        ArgumentNullException.ThrowIfNull(application);

        var resolvedPath = ResolveValue(application.Path);
        var resolvedArguments = ResolveValue(application.Arguments);
        var workingDirectory = ResolveWorkingDirectory(application);

        if (!CanLaunch(resolvedPath))
        {
            return $"Cannot launch: {application.Name}. Path not found.";
        }

        var startInfo = new ProcessStartInfo
        {
            FileName = resolvedPath,
            Arguments = resolvedArguments,
            WorkingDirectory = workingDirectory ?? string.Empty,
            UseShellExecute = true
        };

        try
        {
            _ = Process.Start(startInfo);
            return $"Launched: {application.Name}";
        }
        catch (Win32Exception ex)
        {
            return $"Launch failed: {application.Name}. {ex.Message}";
        }
        catch (InvalidOperationException ex)
        {
            return $"Launch failed: {application.Name}. {ex.Message}";
        }
        catch (FileNotFoundException ex)
        {
            return $"Launch failed: {application.Name}. {ex.Message}";
        }
        catch (PlatformNotSupportedException ex)
        {
            return $"Launch failed: {application.Name}. {ex.Message}";
        }
    }

    public string OpenContainingFolder(ApplicationEntry application)
    {
        ArgumentNullException.ThrowIfNull(application);

        var folder = ResolveFolder(application);
        if (string.IsNullOrWhiteSpace(folder) || !Directory.Exists(folder))
        {
            return $"Cannot open folder for: {application.Name}";
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
            return $"Open folder failed: {application.Name}. {ex.Message}";
        }
        catch (InvalidOperationException ex)
        {
            return $"Open folder failed: {application.Name}. {ex.Message}";
        }
        catch (FileNotFoundException ex)
        {
            return $"Open folder failed: {application.Name}. {ex.Message}";
        }
        catch (PlatformNotSupportedException ex)
        {
            return $"Open folder failed: {application.Name}. {ex.Message}";
        }
    }

    public bool IsAvailable(ApplicationEntry application)
    {
        ArgumentNullException.ThrowIfNull(application);
        return CanLaunch(ResolveValue(application.Path));
    }

    public string ResolveValue(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        return Environment.ExpandEnvironmentVariables(value.Trim().Trim('"'));
    }

    private string? ResolveFolder(ApplicationEntry application)
    {
        var workingDirectory = ResolveWorkingDirectory(application);
        if (!string.IsNullOrWhiteSpace(workingDirectory) && Directory.Exists(workingDirectory))
        {
            return workingDirectory;
        }

        var resolvedPath = ResolveValue(application.Path);
        if (string.IsNullOrWhiteSpace(resolvedPath))
        {
            return null;
        }

        if (Directory.Exists(resolvedPath))
        {
            return resolvedPath;
        }

        return HasPathHints(resolvedPath)
            ? Path.GetDirectoryName(resolvedPath)
            : null;
    }

    private string? ResolveWorkingDirectory(ApplicationEntry application)
    {
        var workingDirectory = ResolveValue(application.WorkingDirectory);
        if (!string.IsNullOrWhiteSpace(workingDirectory))
        {
            return workingDirectory;
        }

        var resolvedPath = ResolveValue(application.Path);
        return File.Exists(resolvedPath)
            ? Path.GetDirectoryName(resolvedPath)
            : null;
    }

    private static bool CanLaunch(string resolvedPath)
    {
        if (string.IsNullOrWhiteSpace(resolvedPath))
        {
            return false;
        }

        if (File.Exists(resolvedPath) || Directory.Exists(resolvedPath))
        {
            return true;
        }

        return !HasPathHints(resolvedPath);
    }

    private static bool HasPathHints(string value)
        => value.Contains(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal)
            || value.Contains(Path.AltDirectorySeparatorChar.ToString(), StringComparison.Ordinal)
            || Path.IsPathRooted(value);
}
