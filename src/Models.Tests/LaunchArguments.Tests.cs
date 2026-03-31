using FluentAssertions;

namespace Models.Tests;

public sealed class LaunchArgumentsTests
{
    [Fact(DisplayName = "The constructor trims quotes and expands environment variables in the arguments.")]
    [Trait("Category", "Unit")]
    public void CtorShouldTrimQuotesAndExpandEnvironmentVariables()
    {
        // Arrange
        var tempPath = Environment.GetEnvironmentVariable("TEMP");
        tempPath.Should().NotBeNullOrWhiteSpace();

        // Act
        var arguments = new LaunchArguments(" \"--path=%TEMP%\" ");

        // Assert
        arguments.Value.Should().Be($"--path={tempPath}");
        arguments.HasValue.Should().BeTrue();
    }

    [Fact(DisplayName = "The constructor returns an empty argument list for white-space input.")]
    [Trait("Category", "Unit")]
    public void CtorShouldReturnEmptyWhenValueIsWhiteSpace()
    {
        // Arrange

        // Act
        var arguments = new LaunchArguments(" ");

        // Assert
        arguments.Value.Should().BeEmpty();
        arguments.HasValue.Should().BeFalse();
    }

    [Fact(DisplayName = "The string representation returns the normalized arguments.")]
    [Trait("Category", "Unit")]
    public void ToStringShouldReturnNormalizedValue()
    {
        // Arrange
        var arguments = new LaunchArguments(" --help ");

        // Act
        var value = arguments.ToString();

        // Assert
        value.Should().Be("--help");
    }
}
