using Abstractions;

using FluentAssertions;

using Moq;

using Models;

namespace Infrastructure.Tests;

public sealed class LauncherGridViewTests
{
    [Fact(DisplayName = "The grid shows the full path when the option is enabled.")]
    [Trait("Category", "Unit")]
    public void GetPathDisplayTextShouldReturnFullPathWhenConfigured()
    {
        // Arrange
        var application = ApplicationEntry.Create(
            "JiraMetrics",
            @"C:\Users\ntyulenev\Desktop\MyApps\JiraFlow\JiraMetrics.exe");
        var view = CreateView(showFullPath: true, application);

        // Act
        var displayText = view.GetPathDisplayText(application);

        // Assert
        displayText.Should().Be(application.Path.Value);
    }

    [Fact(DisplayName = "The grid shows only the executable name when the full path is hidden.")]
    [Trait("Category", "Unit")]
    public void GetPathDisplayTextShouldReturnExecutableNameWhenConfigured()
    {
        // Arrange
        var application = ApplicationEntry.Create(
            "JiraMetrics",
            @"C:\Users\ntyulenev\Desktop\MyApps\JiraFlow\JiraMetrics.exe");
        var view = CreateView(showFullPath: false, application);

        // Act
        var displayText = view.GetPathDisplayText(application);

        // Assert
        displayText.Should().Be("JiraMetrics.exe");
    }

    private static LauncherGridView CreateView(bool showFullPath, params ApplicationEntry[] applications)
        => new(
            new LauncherOptions(new LayoutOptions(), applications, showFullPath),
            new Mock<ILauncherActions>(MockBehavior.Strict).Object,
            () => new LauncherOptions(new LayoutOptions(), applications, showFullPath));
}
