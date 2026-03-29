using Abstractions;

using Models;

using Terminal.Gui;

namespace Infrastructure;

public sealed class ConsoleRenderer(
    ILauncherActions launcherActions,
    ILauncherConfiguration launcherConfiguration) : IConsoleRenderer
{
    private readonly ILauncherActions _launcherActions = launcherActions;
    private readonly ILauncherConfiguration _launcherConfiguration = launcherConfiguration;

    public Task RunAsync(LauncherOptions options, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(options);

        Application.Init();

        try
        {
            var top = Application.Top;
            using var window = new Window("LaunchKit")
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(1)
            };

            var summaryLabel = new Label(string.Empty)
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill()
            };

            var statusLabel = new Label("Ready")
            {
                X = 0,
                Y = 1,
                Width = Dim.Fill()
            };

            var gridView = new LauncherGridView(
                options,
                _launcherActions,
                ReloadOptions)
            {
                X = 0,
                Y = 3,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                CanFocus = true
            };

            gridView.SelectionChanged += (_, summary) => summaryLabel.Text = summary;
            gridView.StatusChanged += (_, status) => statusLabel.Text = status;

            summaryLabel.Text = gridView.BuildSummary();
            statusLabel.Text = "Arrows move  Enter launch  O open folder  F5 reload  Esc exit";

            window.Add(summaryLabel, statusLabel, gridView);
            top.Add(window);

            using var statusBar = new StatusBar([
                new StatusItem(Key.Enter, "~Enter~ Launch", () => gridView.LaunchSelection()),
                new StatusItem(Key.O, "~O~ Open Folder", () => gridView.OpenSelectionFolder()),
                new StatusItem(Key.F5, "~F5~ Reload", () => gridView.ReloadSelection()),
                new StatusItem(Key.Esc, "~Esc~ Exit", () => Application.RequestStop())
            ]);

            top.Add(statusBar);

            using var registration = cancellationToken.Register(() => Application.RequestStop());
            Application.Run();
        }
        finally
        {
            Application.Shutdown();
        }

        return Task.CompletedTask;
    }

    private LauncherOptions ReloadOptions()
        => _launcherConfiguration.Load();
}
