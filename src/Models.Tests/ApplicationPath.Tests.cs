using FluentAssertions;

using IOPath = System.IO.Path;

namespace Models.Tests;

public sealed class ApplicationPathTests
{
    [Fact(DisplayName = "The constructor trims quotes and expands environment variables in the path.")]
    [Trait("Category", "Unit")]
    public void CtorShouldTrimQuotesAndExpandEnvironmentVariables()
    {
        // Arrange
        var tempPath = Environment.GetEnvironmentVariable("TEMP");
        tempPath.Should().NotBeNullOrWhiteSpace();

        // Act
        var path = new ApplicationPath(" \"%TEMP%\\LaunchKit.exe\" ");

        // Assert
        path.Value.Should().Be(IOPath.Combine(tempPath!, "LaunchKit.exe"));
    }

    [Theory(DisplayName = "The constructor throws when the path is empty.")]
    [Trait("Category", "Unit")]
    [InlineData("")]
    [InlineData(" ")]
    public void CtorShouldThrowWhenValueIsWhiteSpace(string value)
    {
        // Arrange

        // Act
        var action = () => new ApplicationPath(value);

        // Assert
        action.Should().Throw<InvalidDataException>()
            .WithMessage("Application path is required.");
    }

    [Fact(DisplayName = "The constructor throws when the path is null.")]
    [Trait("Category", "Unit")]
    public void CtorShouldThrowWhenValueIsNull()
    {
        // Arrange

        // Act
        var action = () => new ApplicationPath(null!);

        // Assert
        action.Should().Throw<InvalidDataException>()
            .WithMessage("Application path is required.");
    }

    [Fact(DisplayName = "A command-like path can be launched.")]
    [Trait("Category", "Unit")]
    public void CanLaunchShouldReturnTrueWhenValueIsCommand()
    {
        // Arrange
        var path = new ApplicationPath("dotnet");

        // Act
        var canLaunch = path.CanLaunch();

        // Assert
        canLaunch.Should().BeTrue();
    }

    [Fact(DisplayName = "An existing file path can be launched.")]
    [Trait("Category", "Unit")]
    public void CanLaunchShouldReturnTrueWhenFileExists()
    {
        // Arrange
        var path = new ApplicationPath(typeof(ApplicationPathTests).Assembly.Location);

        // Act
        var canLaunch = path.CanLaunch();

        // Assert
        canLaunch.Should().BeTrue();
    }

    [Fact(DisplayName = "An existing directory path can be launched.")]
    [Trait("Category", "Unit")]
    public void CanLaunchShouldReturnTrueWhenDirectoryExists()
    {
        // Arrange
        var directoryPath = IOPath.GetDirectoryName(typeof(ApplicationPathTests).Assembly.Location)!;
        var path = new ApplicationPath(directoryPath);

        // Act
        var canLaunch = path.CanLaunch();

        // Assert
        canLaunch.Should().BeTrue();
    }

    [Fact(DisplayName = "A missing path with path hints cannot be launched.")]
    [Trait("Category", "Unit")]
    public void CanLaunchShouldReturnFalseWhenPathIsMissing()
    {
        // Arrange
        var path = new ApplicationPath(CreateMissingFilePath());

        // Act
        var canLaunch = path.CanLaunch();

        // Assert
        canLaunch.Should().BeFalse();
    }

    [Fact(DisplayName = "The directory name is null for a command-like path.")]
    [Trait("Category", "Unit")]
    public void GetDirectoryNameShouldReturnNullWhenValueIsCommand()
    {
        // Arrange
        var path = new ApplicationPath("dotnet");

        // Act
        var directory = path.GetDirectoryName();

        // Assert
        directory.Should().BeNull();
    }

    [Fact(DisplayName = "The directory name is returned for a file path.")]
    [Trait("Category", "Unit")]
    public void GetDirectoryNameShouldReturnDirectoryWhenPathContainsDirectoryInformation()
    {
        // Arrange
        var filePath = typeof(ApplicationPathTests).Assembly.Location;
        var path = new ApplicationPath(filePath);

        // Act
        var directory = path.GetDirectoryName();

        // Assert
        directory.Should().Be(IOPath.GetDirectoryName(filePath));
    }

    [Fact(DisplayName = "The display name returns the command for a command-like path.")]
    [Trait("Category", "Unit")]
    public void GetDisplayNameShouldReturnCommandWhenValueIsCommand()
    {
        // Arrange
        var path = new ApplicationPath("dotnet");

        // Act
        var displayName = path.GetDisplayName();

        // Assert
        displayName.Should().Be("dotnet");
    }

    [Fact(DisplayName = "The display name returns the file name for a file path.")]
    [Trait("Category", "Unit")]
    public void GetDisplayNameShouldReturnFileNameWhenValueContainsDirectoryInformation()
    {
        // Arrange
        var filePath = IOPath.Combine(Path.GetTempPath(), "LaunchKit", "JiraMetrics.exe");
        var path = new ApplicationPath(filePath);

        // Act
        var displayName = path.GetDisplayName();

        // Assert
        displayName.Should().Be("JiraMetrics.exe");
    }

    [Fact(DisplayName = "The process working directory is returned for an existing file path.")]
    [Trait("Category", "Unit")]
    public void ProcessWorkingDirectoryShouldReturnParentWhenFileExists()
    {
        // Arrange
        var filePath = typeof(ApplicationPathTests).Assembly.Location;
        var path = new ApplicationPath(filePath);

        // Act
        var directory = path.ProcessWorkingDirectory;

        // Assert
        directory.Should().Be(IOPath.GetDirectoryName(filePath));
    }

    [Fact(DisplayName = "The process working directory is null when the file does not exist.")]
    [Trait("Category", "Unit")]
    public void ProcessWorkingDirectoryShouldReturnNullWhenFileDoesNotExist()
    {
        // Arrange
        var path = new ApplicationPath(CreateMissingFilePath());

        // Act
        var directory = path.ProcessWorkingDirectory;

        // Assert
        directory.Should().BeNull();
    }

    [Fact(DisplayName = "The containing folder is returned when the path points to an existing directory.")]
    [Trait("Category", "Unit")]
    public void ContainingFolderShouldReturnDirectoryWhenDirectoryExists()
    {
        // Arrange
        var directoryPath = IOPath.GetDirectoryName(typeof(ApplicationPathTests).Assembly.Location)!;
        var path = new ApplicationPath(directoryPath);

        // Act
        var folder = path.ContainingFolder;

        // Assert
        folder.Should().Be(directoryPath);
    }

    [Fact(DisplayName = "The containing folder falls back to the file directory for a file path.")]
    [Trait("Category", "Unit")]
    public void ContainingFolderShouldReturnParentDirectoryWhenValuePointsToAFile()
    {
        // Arrange
        var filePath = typeof(ApplicationPathTests).Assembly.Location;
        var path = new ApplicationPath(filePath);

        // Act
        var folder = path.ContainingFolder;

        // Assert
        folder.Should().Be(IOPath.GetDirectoryName(filePath));
    }

    [Fact(DisplayName = "The containing folder is null for a command-like path.")]
    [Trait("Category", "Unit")]
    public void ContainingFolderShouldReturnNullWhenValueIsCommand()
    {
        // Arrange
        var path = new ApplicationPath("dotnet");

        // Act
        var folder = path.ContainingFolder;

        // Assert
        folder.Should().BeNull();
    }

    [Fact(DisplayName = "The string representation returns the normalized path value.")]
    [Trait("Category", "Unit")]
    public void ToStringShouldReturnNormalizedValue()
    {
        // Arrange
        var path = new ApplicationPath(" dotnet ");

        // Act
        var value = path.ToString();

        // Assert
        value.Should().Be("dotnet");
    }

    private static string CreateMissingFilePath()
        => IOPath.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"), "LaunchKit.exe");
}
