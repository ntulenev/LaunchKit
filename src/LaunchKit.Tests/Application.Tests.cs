using Abstractions;

using FluentAssertions;

using Moq;

using LaunchKit.Utility;

namespace LaunchKit.Tests;

public sealed class ApplicationTests
{
    [Fact(DisplayName = "The constructor throws when the workflow is null.")]
    [Trait("Category", "Unit")]
    public void CtorShouldThrowWhenWorkflowIsNull()
    {
        // Arrange

        // Act
        var action = () => new Application(null!);

        // Assert
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact(DisplayName = "The application delegates execution to the workflow.")]
    [Trait("Category", "Unit")]
    public async Task RunAsyncShouldDelegateToWorkflow()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        var workflow = new Mock<ILauncherWorkflow>(MockBehavior.Strict);
        workflow.Setup(w => w.RunAsync(It.Is<CancellationToken>(token => token == cts.Token)))
            .Returns(Task.CompletedTask);
        var application = new Application(workflow.Object);

        // Act
        await application.RunAsync(cts.Token);

        // Assert
        workflow.VerifyAll();
    }
}
