using FluentAssertions;

using Terminal.Gui;

namespace Infrastructure.Tests;

public sealed class LauncherShortcutResolverTests
{
    [Fact(DisplayName = "The Russian A-layout key is recognized as the admin launch shortcut.")]
    [Trait("Category", "Unit")]
    public void IsAdminLaunchShortcutShouldRecognizeRussianAKeyPosition()
    {
        // Arrange
        var resolver = CreateResolver();

        // Act
        var isShortcut = resolver.IsAdminLaunchShortcut((Key)'\u0444');

        // Assert
        isShortcut.Should().BeTrue();
    }

    [Fact(DisplayName = "The Russian O-layout key is recognized as the open folder shortcut.")]
    [Trait("Category", "Unit")]
    public void IsOpenFolderShortcutShouldRecognizeRussianOKeyPosition()
    {
        // Arrange
        var resolver = CreateResolver();

        // Act
        var isShortcut = resolver.IsOpenFolderShortcut((Key)'\u0449');

        // Assert
        isShortcut.Should().BeTrue();
    }

    [Fact(DisplayName = "The physical A key is recognized as the admin launch shortcut.")]
    [Trait("Category", "Unit")]
    public void IsAdminLaunchShortcutShouldRecognizePhysicalAKey()
    {
        // Arrange
        var resolver = CreateResolver(KeyboardKey.A);

        // Act
        var isShortcut = resolver.IsAdminLaunchShortcut(Key.F1);

        // Assert
        isShortcut.Should().BeTrue();
    }

    [Fact(DisplayName = "The physical O key is recognized as the open folder shortcut.")]
    [Trait("Category", "Unit")]
    public void IsOpenFolderShortcutShouldRecognizePhysicalOKey()
    {
        // Arrange
        var resolver = CreateResolver(KeyboardKey.O);

        // Act
        var isShortcut = resolver.IsOpenFolderShortcut(Key.F1);

        // Assert
        isShortcut.Should().BeTrue();
    }

    private static LauncherShortcutResolver CreateResolver(params KeyboardKey[] pressedKeys)
        => new(new FakeKeyboardState(pressedKeys.ToHashSet()));

    private sealed class FakeKeyboardState(IReadOnlySet<KeyboardKey> pressedKeys) : IKeyboardState
    {
        public bool IsKeyDown(KeyboardKey key) => pressedKeys.Contains(key);
    }
}
