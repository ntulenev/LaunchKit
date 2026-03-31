using Abstractions;

using FluentAssertions;

using Moq;

using Models;

using Terminal.Gui;

namespace Infrastructure.Tests;

public sealed class ConsoleRendererTests
{
    [Fact(DisplayName = "The constructor throws when the launcher actions are null.")]
    [Trait("Category", "Unit")]
    public void CtorShouldThrowWhenLauncherActionsAreNull()
    {
        // Arrange
        var configuration = new Mock<ILauncherConfiguration>(MockBehavior.Strict).Object;

        // Act
        var action = () => new ConsoleRenderer(null!, configuration);

        // Assert
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact(DisplayName = "The constructor throws when the launcher configuration is null.")]
    [Trait("Category", "Unit")]
    public void CtorShouldThrowWhenLauncherConfigurationIsNull()
    {
        // Arrange
        var actions = new Mock<ILauncherActions>(MockBehavior.Strict).Object;

        // Act
        var action = () => new ConsoleRenderer(actions, null!);

        // Assert
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact(DisplayName = "The internal constructor throws when the terminal facade is null.")]
    [Trait("Category", "Unit")]
    public void CtorShouldThrowWhenTerminalFacadeIsNull()
    {
        // Arrange
        var actions = new Mock<ILauncherActions>(MockBehavior.Strict).Object;
        var configuration = new Mock<ILauncherConfiguration>(MockBehavior.Strict).Object;

        // Act
        var action = () => new ConsoleRenderer(actions, configuration, null!);

        // Assert
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact(DisplayName = "The renderer throws when the launcher options are null.")]
    [Trait("Category", "Unit")]
    public async Task RunAsyncShouldThrowWhenOptionsAreNull()
    {
        // Arrange
        var renderer = new ConsoleRenderer(
            new Mock<ILauncherActions>(MockBehavior.Strict).Object,
            new Mock<ILauncherConfiguration>(MockBehavior.Strict).Object,
            new FakeTerminalFacade());

        // Act
        Func<Task> action = () => renderer.RunAsync(null!, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact(DisplayName = "The renderer initializes, runs, and shuts down the terminal facade.")]
    [Trait("Category", "Unit")]
    public async Task RunAsyncShouldInitializeRunAndShutdownTerminal()
    {
        // Arrange
        var terminal = new FakeTerminalFacade();
        var renderer = new ConsoleRenderer(
            new Mock<ILauncherActions>(MockBehavior.Strict).Object,
            new Mock<ILauncherConfiguration>(MockBehavior.Strict).Object,
            terminal);
        var options = CreateOptions(CreateApplication());

        // Act
        await renderer.RunAsync(options, CancellationToken.None);

        // Assert
        terminal.InitCalls.Should().Be(1);
        terminal.RunCalls.Should().Be(1);
        terminal.ShutdownCalls.Should().Be(1);
        terminal.Top.Subviews.Should().HaveCount(2);
    }

    [Fact(DisplayName = "The renderer requests stop immediately when the cancellation token is already canceled.")]
    [Trait("Category", "Unit")]
    public async Task RunAsyncShouldRequestStopWhenCancellationIsAlreadyRequested()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        cts.Cancel();
        var terminal = new FakeTerminalFacade();
        var renderer = new ConsoleRenderer(
            new Mock<ILauncherActions>(MockBehavior.Strict).Object,
            new Mock<ILauncherConfiguration>(MockBehavior.Strict).Object,
            terminal);
        var options = CreateOptions(CreateApplication());

        // Act
        await renderer.RunAsync(options, cts.Token);

        // Assert
        terminal.RequestStopCalls.Should().Be(1);
    }

    [Fact(DisplayName = "The renderer still shuts down the terminal facade when the terminal loop fails.")]
    [Trait("Category", "Unit")]
    public async Task RunAsyncShouldShutdownTerminalWhenRunFails()
    {
        // Arrange
        var terminal = new FakeTerminalFacade
        {
            RunException = new InvalidOperationException("Boom")
        };
        var renderer = new ConsoleRenderer(
            new Mock<ILauncherActions>(MockBehavior.Strict).Object,
            new Mock<ILauncherConfiguration>(MockBehavior.Strict).Object,
            terminal);
        var options = CreateOptions(CreateApplication());

        // Act
        Func<Task> action = () => renderer.RunAsync(options, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Boom");
        terminal.ShutdownCalls.Should().Be(1);
    }

    private static LauncherOptions CreateOptions(params ApplicationEntry[] applications)
        => new(new LayoutOptions(), applications);

    private static ApplicationEntry CreateApplication()
        => ApplicationEntry.Create(
            "LaunchKit",
            "dotnet",
            "--version",
            description: "SDK");

    private sealed class FakeTerminalFacade : ITerminalFacade
    {
        public Toplevel Top { get; } = new();

        public int InitCalls { get; private set; }

        public int RunCalls { get; private set; }

        public int ShutdownCalls { get; private set; }

        public int RequestStopCalls { get; private set; }

        public Exception? RunException { get; init; }

        public void Init() => InitCalls++;

        public void Run()
        {
            RunCalls++;
            if (RunException is not null)
            {
                throw RunException;
            }
        }

        public void Shutdown() => ShutdownCalls++;

        public void RequestStop() => RequestStopCalls++;
    }
}
