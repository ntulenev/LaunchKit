using FluentAssertions;

namespace Infrastructure.Tests;

public sealed class LauncherConfigurationDocumentTests
{
    [Fact(DisplayName = "The launcher document maps layout settings and applications.")]
    [Trait("Category", "Unit")]
    public void ToOptionsShouldMapLayoutAndApplications()
    {
        // Arrange
        var document = new LauncherConfigurationDocument
        {
            ShowFullPath = false,
            Layout = new LayoutConfigurationDocument
            {
                Columns = 2,
                TileWidth = 42,
                TileHeight = 8,
                TileSpacing = 4
            }
        };
        document.Applications.Add(new ApplicationEntryDocument
        {
            Name = "Editor",
            Path = "code",
            Arguments = ".",
            Description = "Workspace"
        });
        document.Applications.Add(new ApplicationEntryDocument
        {
            Name = "Terminal",
            Path = "pwsh"
        });

        // Act
        var options = document.ToOptions();

        // Assert
        options.Layout.Columns.Should().Be(2);
        options.Layout.TileWidth.Should().Be(42);
        options.Layout.TileHeight.Should().Be(8);
        options.Layout.TileSpacing.Should().Be(4);
        options.ShowFullPath.Should().BeFalse();
        options.Applications.Should().HaveCount(2);
        options.Applications[0].Name.Value.Should().Be("Editor");
        options.Applications[0].Arguments.Value.Should().Be(".");
        options.Applications[0].Description.Value.Should().Be("Workspace");
        options.Applications[1].Name.Value.Should().Be("Terminal");
    }
}
