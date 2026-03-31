using FluentAssertions;

using IOPath = System.IO.Path;

namespace Models.Tests;

public sealed class WorkingDirectoryPathTests
{
    [Fact(DisplayName = "The optional factory returns null for white-space input.")]
    [Trait("Category", "Unit")]
    public void CreateOptionalShouldReturnNullWhenValueIsWhiteSpace()
    {
        // Arrange

        // Act
        var path = WorkingDirectoryPath.CreateOptional(" ");

        // Assert
        path.Should().BeNull();
    }

    [Fact(DisplayName = "The optional factory returns a value when the working directory is present.")]
    [Trait("Category", "Unit")]
    public void CreateOptionalShouldReturnValueWhenInputIsPresent()
    {
        // Arrange

        // Act
        var path = WorkingDirectoryPath.CreateOptional(" C:\\Work ");

        // Assert
        path.Should().NotBeNull();
        path!.Value.Should().Be("C:\\Work");
    }

    [Fact(DisplayName = "The constructor trims quotes and expands environment variables in the working directory.")]
    [Trait("Category", "Unit")]
    public void CtorShouldTrimQuotesAndExpandEnvironmentVariables()
    {
        // Arrange
        var tempPath = Environment.GetEnvironmentVariable("TEMP");
        tempPath.Should().NotBeNullOrWhiteSpace();

        // Act
        var path = new WorkingDirectoryPath(" \"%TEMP%\\LaunchKit\" ");

        // Assert
        path.Value.Should().Be(IOPath.Combine(tempPath!, "LaunchKit"));
    }

    [Theory(DisplayName = "The constructor throws when the working directory is empty.")]
    [Trait("Category", "Unit")]
    [InlineData("")]
    [InlineData(" ")]
    public void CtorShouldThrowWhenValueIsWhiteSpace(string value)
    {
        // Arrange

        // Act
        var action = () => new WorkingDirectoryPath(value);

        // Assert
        action.Should().Throw<InvalidDataException>()
            .WithMessage("Working directory path cannot be empty.");
    }

    [Fact(DisplayName = "The constructor throws when the working directory is null.")]
    [Trait("Category", "Unit")]
    public void CtorShouldThrowWhenValueIsNull()
    {
        // Arrange

        // Act
        var action = () => new WorkingDirectoryPath(null!);

        // Assert
        action.Should().Throw<InvalidDataException>()
            .WithMessage("Working directory path cannot be empty.");
    }

    [Fact(DisplayName = "The containing folder returns the directory value when it exists.")]
    [Trait("Category", "Unit")]
    public void ContainingFolderShouldReturnValueWhenDirectoryExists()
    {
        // Arrange
        var directoryPath = IOPath.GetDirectoryName(typeof(WorkingDirectoryPathTests).Assembly.Location)!;
        var path = new WorkingDirectoryPath(directoryPath);

        // Act
        var folder = path.ContainingFolder;

        // Assert
        folder.Should().Be(directoryPath);
        path.ProcessWorkingDirectory.Should().Be(directoryPath);
    }

    [Fact(DisplayName = "The containing folder returns null when the directory does not exist.")]
    [Trait("Category", "Unit")]
    public void ContainingFolderShouldReturnNullWhenDirectoryIsMissing()
    {
        // Arrange
        var path = new WorkingDirectoryPath(IOPath.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N")));

        // Act
        var folder = path.ContainingFolder;

        // Assert
        folder.Should().BeNull();
    }

    [Fact(DisplayName = "The string representation returns the normalized working directory.")]
    [Trait("Category", "Unit")]
    public void ToStringShouldReturnNormalizedValue()
    {
        // Arrange
        var path = new WorkingDirectoryPath(" C:\\Work ");

        // Act
        var value = path.ToString();

        // Assert
        value.Should().Be("C:\\Work");
    }
}
