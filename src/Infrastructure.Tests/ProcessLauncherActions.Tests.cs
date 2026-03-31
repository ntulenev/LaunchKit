using System.ComponentModel;
using System.Diagnostics;

using FluentAssertions;

using Models;

namespace Infrastructure.Tests;

public sealed class ProcessLauncherActionsTests
{
    [Fact(DisplayName = "The launch action throws when the application entry is null.")]
    [Trait("Category", "Unit")]
    public void LaunchShouldThrowWhenApplicationIsNull()
    {
        // Arrange
        var actions = new ProcessLauncherActions();

        // Act
        var action = () => actions.Launch(null!);

        // Assert
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact(DisplayName = "The launch action reports that the application path cannot be launched.")]
    [Trait("Category", "Unit")]
    public void LaunchShouldReturnMessageWhenPathCannotBeLaunched()
    {
        // Arrange
        var actions = new ProcessLauncherActions();
        var application = CreateApplication(CreateMissingFilePath());

        // Act
        var result = actions.Launch(application);

        // Assert
        result.Should().Be($"Cannot launch: {application.Name.Value}. Path not found.");
    }

    [Fact(DisplayName = "The launch action starts the process and returns a success message.")]
    [Trait("Category", "Unit")]
    public void LaunchShouldReturnSuccessMessageWhenProcessStarts()
    {
        // Arrange
        var starter = new FakeProcessStarter();
        var application = CreateApplication("dotnet", "--info", "C:\\Work");
        var actions = new ProcessLauncherActions(starter);

        // Act
        var result = actions.Launch(application);

        // Assert
        result.Should().Be("Launched: LaunchKit");
        starter.StartCalls.Should().HaveCount(1);
        starter.StartCalls[0].FileName.Should().Be("dotnet");
        starter.StartCalls[0].Arguments.Should().Be("--info");
        starter.StartCalls[0].WorkingDirectory.Should().Be("C:\\Work");
        starter.StartCalls[0].UseShellExecute.Should().BeTrue();
    }

    [Theory(DisplayName = "The launch action returns the exception message when process start fails.")]
    [Trait("Category", "Unit")]
    [InlineData("Win32")]
    [InlineData("InvalidOperation")]
    [InlineData("FileNotFound")]
    [InlineData("PlatformNotSupported")]
    public void LaunchShouldReturnFailureMessageWhenProcessStartFails(string exceptionKind)
    {
        // Arrange
        var exception = CreateException(exceptionKind);
        var starter = new FakeProcessStarter { ExceptionToThrow = exception };
        var application = CreateApplication("dotnet");
        var actions = new ProcessLauncherActions(starter);

        // Act
        var result = actions.Launch(application);

        // Assert
        result.Should().Be($"Launch failed: LaunchKit. {exception.Message}");
    }

    [Fact(DisplayName = "The open-folder action throws when the application entry is null.")]
    [Trait("Category", "Unit")]
    public void OpenContainingFolderShouldThrowWhenApplicationIsNull()
    {
        // Arrange
        var actions = new ProcessLauncherActions();

        // Act
        var action = () => actions.OpenContainingFolder(null!);

        // Assert
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact(DisplayName = "The open-folder action reports that the folder cannot be opened when it is missing.")]
    [Trait("Category", "Unit")]
    public void OpenContainingFolderShouldReturnMessageWhenFolderIsMissing()
    {
        // Arrange
        var actions = new ProcessLauncherActions();
        var application = CreateApplication(CreateMissingFilePath());

        // Act
        var result = actions.OpenContainingFolder(application);

        // Assert
        result.Should().Be($"Cannot open folder for: {application.Name.Value}");
    }

    [Fact(DisplayName = "The open-folder action starts the process and returns a success message.")]
    [Trait("Category", "Unit")]
    public void OpenContainingFolderShouldReturnSuccessMessageWhenFolderOpens()
    {
        // Arrange
        var directoryPath = Path.GetDirectoryName(typeof(ProcessLauncherActionsTests).Assembly.Location)!;
        var starter = new FakeProcessStarter();
        var application = CreateApplication("dotnet", workingDirectory: directoryPath);
        var actions = new ProcessLauncherActions(starter);

        // Act
        var result = actions.OpenContainingFolder(application);

        // Assert
        result.Should().Be($"Opened folder: {directoryPath}");
        starter.StartCalls.Should().HaveCount(1);
        starter.StartCalls[0].FileName.Should().Be(directoryPath);
        starter.StartCalls[0].UseShellExecute.Should().BeTrue();
    }

    [Theory(DisplayName = "The open-folder action returns the exception message when opening the folder fails.")]
    [Trait("Category", "Unit")]
    [InlineData("Win32")]
    [InlineData("InvalidOperation")]
    [InlineData("FileNotFound")]
    [InlineData("PlatformNotSupported")]
    public void OpenContainingFolderShouldReturnFailureMessageWhenProcessStartFails(string exceptionKind)
    {
        // Arrange
        var exception = CreateException(exceptionKind);
        var directoryPath = Path.GetDirectoryName(typeof(ProcessLauncherActionsTests).Assembly.Location)!;
        var starter = new FakeProcessStarter { ExceptionToThrow = exception };
        var application = CreateApplication("dotnet", workingDirectory: directoryPath);
        var actions = new ProcessLauncherActions(starter);

        // Act
        var result = actions.OpenContainingFolder(application);

        // Assert
        result.Should().Be($"Open folder failed: LaunchKit. {exception.Message}");
    }

    private static ApplicationEntry CreateApplication(
        string path,
        string? arguments = null,
        string? workingDirectory = null)
        => ApplicationEntry.Create(
            "LaunchKit",
            path,
            arguments,
            workingDirectory,
            "Launcher");

    private static string CreateMissingFilePath()
        => Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"), "LaunchKit.exe");

    private static Win32Exception CreateWin32Exception(string message)
        => (Win32Exception)Activator.CreateInstance(typeof(Win32Exception), 5, message)!;

    private static Exception CreateException(string exceptionKind)
        => exceptionKind switch
        {
            "Win32" => CreateWin32Exception("Win32 failure"),
            "InvalidOperation" => new InvalidOperationException("Invalid operation"),
            "FileNotFound" => new FileNotFoundException("Missing file"),
            "PlatformNotSupported" => new PlatformNotSupportedException("Platform is not supported"),
            _ => throw new InvalidOperationException($"Unknown exception kind: {exceptionKind}")
        };

    private sealed class FakeProcessStarter : IProcessStarter
    {
        public List<ProcessStartInfo> StartCalls { get; } = [];

        public Exception? ExceptionToThrow { get; init; }

        public Process? Start(ProcessStartInfo startInfo)
        {
            StartCalls.Add(startInfo);

            if (ExceptionToThrow is not null)
            {
                throw ExceptionToThrow;
            }

            return null;
        }
    }
}
