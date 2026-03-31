using Abstractions;

using FluentAssertions;

using Moq;

using Models;

namespace Logic.Tests;

public sealed class LauncherWorkflowTests
{
    [Fact(DisplayName = "The constructor throws when the configuration is null.")]
    [Trait("Category", "Unit")]
    public void CtorShouldThrowWhenConfigurationIsNull()
    {
        // Arrange
        var renderer = new Mock<IConsoleRenderer>(MockBehavior.Strict).Object;

        // Act
        var action = () => new LauncherWorkflow(null!, renderer);

        // Assert
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact(DisplayName = "The constructor throws when the console renderer is null.")]
    [Trait("Category", "Unit")]
    public void CtorShouldThrowWhenConsoleRendererIsNull()
    {
        // Arrange
        var configuration = new Mock<ILauncherConfiguration>(MockBehavior.Strict).Object;

        // Act
        var action = () => new LauncherWorkflow(configuration, null!);

        // Assert
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact(DisplayName = "The workflow delegates rendering when applications are available.")]
    [Trait("Category", "Unit")]
    public async Task RunAsyncShouldDelegateToRendererWhenApplicationsExist()
    {
        // Arrange
        var options = CreateOptions(CreateApplication());
        var configuration = new Mock<ILauncherConfiguration>(MockBehavior.Strict);
        configuration.Setup(c => c.Load())
            .Returns(options);
        using var cts = new CancellationTokenSource();
        var renderer = new Mock<IConsoleRenderer>(MockBehavior.Strict);
        renderer.Setup(r => r.RunAsync(
                options,
                It.Is<CancellationToken>(token => token == cts.Token)))
            .Returns(Task.CompletedTask);
        var workflow = new LauncherWorkflow(configuration.Object, renderer.Object);

        // Act
        await workflow.RunAsync(cts.Token);

        // Assert
        configuration.VerifyAll();
        renderer.VerifyAll();
    }

    [Fact(DisplayName = "The workflow prints a guidance message when no applications are configured.")]
    [Trait("Category", "Unit")]
    public async Task RunAsyncShouldPrintGuidanceWhenNoApplicationsAreConfigured()
    {
        // Arrange
        var options = CreateOptions();
        var configuration = new Mock<ILauncherConfiguration>(MockBehavior.Strict);
        configuration.Setup(c => c.Load())
            .Returns(options);
        var renderer = new Mock<IConsoleRenderer>(MockBehavior.Strict);
        var systemConsole = new RecordingSystemConsole();
        var workflow = new LauncherWorkflow(configuration.Object, renderer.Object, systemConsole);

        // Act
        await workflow.RunAsync(CancellationToken.None);

        // Assert
        renderer.Verify(r => r.RunAsync(It.IsAny<LauncherOptions>(), It.IsAny<CancellationToken>()), Times.Never);
        configuration.VerifyAll();
        systemConsole.ClearCalls.Should().Be(1);
        systemConsole.Messages.Should().Contain("No applications configured.");
        systemConsole.Messages.Should().Contain(message => message.Contains("appsettings.json"));
    }

    private static LauncherOptions CreateOptions(params ApplicationEntry[] applications)
        => new(new LayoutOptions(), applications);

    private static ApplicationEntry CreateApplication()
        => ApplicationEntry.Create(
            "LaunchKit",
            "dotnet",
            "--version",
            description: "SDK");

    private sealed class RecordingSystemConsole : ISystemConsole
    {
        public int ClearCalls { get; private set; }

        public List<string?> Messages { get; } = [];

        public void Clear() => ClearCalls++;

        public void WriteLine(string? value) => Messages.Add(value);
    }
}
