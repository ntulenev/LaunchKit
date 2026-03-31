using FluentAssertions;

namespace Infrastructure.Tests;

public sealed class JsonLauncherConfigurationTests
{
    [Fact(DisplayName = "The configuration loader maps launcher options from appsettings.json.")]
    [Trait("Category", "Unit")]
    public void LoadShouldMapLauncherOptionsFromConfigurationFile()
    {
        // Arrange
        const string appSettings = """
        {
          "Launcher": {
            "Layout": {
              "Columns": 2,
              "TileWidth": 40,
              "TileHeight": 7,
              "TileSpacing": 3
            },
            "Applications": [
              {
                "Name": "LaunchKit",
                "Path": "dotnet",
                "Arguments": "--info",
                "Description": "SDK"
              }
            ]
          }
        }
        """;

        // Act
        var options = ExecuteWithAppSettings(appSettings, static configuration => configuration.Load());

        // Assert
        options.Layout.Columns.Should().Be(2);
        options.Layout.TileWidth.Should().Be(40);
        options.Layout.TileHeight.Should().Be(7);
        options.Layout.TileSpacing.Should().Be(3);
        options.Applications.Should().HaveCount(1);
        options.Applications[0].Name.Value.Should().Be("LaunchKit");
        options.Applications[0].Arguments.Value.Should().Be("--info");
        options.Applications[0].Description.Value.Should().Be("SDK");
    }

    [Fact(DisplayName = "The configuration loader returns default options when the launcher section is missing.")]
    [Trait("Category", "Unit")]
    public void LoadShouldReturnDefaultOptionsWhenLauncherSectionIsMissing()
    {
        // Arrange
        const string appSettings = """
        {
          "Logging": {
            "LogLevel": {
              "Default": "Information"
            }
          }
        }
        """;

        // Act
        var options = ExecuteWithAppSettings(appSettings, static configuration => configuration.Load());

        // Assert
        options.Layout.Columns.Should().Be(3);
        options.Layout.TileWidth.Should().Be(34);
        options.Layout.TileHeight.Should().Be(6);
        options.Layout.TileSpacing.Should().Be(2);
        options.Applications.Should().BeEmpty();
    }

    private static T ExecuteWithAppSettings<T>(string content, Func<JsonLauncherConfiguration, T> action)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
        var backup = File.Exists(path) ? File.ReadAllText(path) : null;
        File.WriteAllText(path, content);

        try
        {
            return action(new JsonLauncherConfiguration());
        }
        finally
        {
            if (backup is null)
            {
                File.Delete(path);
            }
            else
            {
                File.WriteAllText(path, backup);
            }
        }
    }
}
