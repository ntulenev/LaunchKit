using Abstractions;

using Models;

using Terminal.Gui;

namespace Infrastructure;

/// <summary>
/// Renders the launcher UI by using Terminal.Gui.
/// </summary>
public sealed class ConsoleRenderer : IConsoleRenderer
{
    /// <summary>
    /// Initializes a renderer with custom collaborators for testing.
    /// </summary>
    /// <param name="launcherConfiguration">Configuration source used for reload operations.</param>
    /// <param name="gridViewFactory">Factory used to create the launcher grid view.</param>
    /// <param name="terminalFacade">Facade used to control Terminal.Gui lifecycle operations.</param>
    internal ConsoleRenderer(
        ILauncherConfiguration launcherConfiguration,
        ILauncherGridViewFactory gridViewFactory,
        ITerminalFacade terminalFacade)
    {
        _launcherConfiguration = launcherConfiguration ?? throw new ArgumentNullException(nameof(launcherConfiguration));
        _gridViewFactory = gridViewFactory ?? throw new ArgumentNullException(nameof(gridViewFactory));
        _terminalFacade = terminalFacade ?? throw new ArgumentNullException(nameof(terminalFacade));
    }

    /// <summary>
    /// Starts the interactive launcher window.
    /// </summary>
    /// <param name="options">Launcher options to display.</param>
    /// <param name="cancellationToken">Token used to stop the UI loop.</param>
    public Task RunAsync(LauncherOptions options, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(options);

        _terminalFacade.Init();

        try
        {
            var top = _terminalFacade.Top;
            using var window = new Window("LaunchKit")
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(1)
            };

            var tabsLabel = new Label(string.Empty)
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill()
            };

            var summaryLabel = new Label(string.Empty)
            {
                X = 0,
                Y = 1,
                Width = Dim.Fill()
            };

            var statusLabel = new Label("Ready")
            {
                X = 0,
                Y = 2,
                Width = Dim.Fill()
            };

            var gridView = _gridViewFactory.Create(options, ReloadOptions);
            gridView.X = 0;
            gridView.Y = 4;
            gridView.Width = Dim.Fill();
            gridView.Height = Dim.Fill();
            gridView.CanFocus = true;

            gridView.TabChanged += (_, tabs) => tabsLabel.Text = tabs;
            gridView.SelectionChanged += (_, summary) => summaryLabel.Text = summary;
            gridView.StatusChanged += (_, status) => statusLabel.Text = status;

            tabsLabel.Text = gridView.BuildTabStrip();
            summaryLabel.Text = gridView.BuildSummary();
            statusLabel.Text = "Tab switch  Arrows move  Enter launch  A admin launch  O open folder  F5 reload  Esc exit";

            window.Add(tabsLabel, summaryLabel, statusLabel, gridView);
            top.Add(window);

            using var statusBar = new StatusBar([
                new StatusItem(Key.Tab, "~Tab~ Next Tab", gridView.NextTab),
                new StatusItem(Key.Enter, "~Enter~ Launch", gridView.LaunchSelection),
                new StatusItem(Key.A, "~A~ Admin Launch", gridView.LaunchSelectionAsAdmin),
                new StatusItem(Key.O, "~O~ Open Folder", gridView.OpenSelectionFolder),
                new StatusItem(Key.F5, "~F5~ Reload", gridView.ReloadSelection),
                new StatusItem(Key.Esc, "~Esc~ Exit", _terminalFacade.RequestStop)
            ]);

            top.Add(statusBar);

            using var registration = cancellationToken.Register(_terminalFacade.RequestStop);
            _terminalFacade.Run();
        }
        finally
        {
            _terminalFacade.Shutdown();
        }

        return Task.CompletedTask;
    }

    private LauncherOptions ReloadOptions()
        => _launcherConfiguration.Load();

    private readonly ILauncherConfiguration _launcherConfiguration;
    private readonly ILauncherGridViewFactory _gridViewFactory;
    private readonly ITerminalFacade _terminalFacade;
}
