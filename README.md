# LaunchKit

`LaunchKit` is a console launcher for a predefined set of Windows applications loaded from `appsettings.json`.

The application renders a tile grid in the terminal, lets the user navigate with the keyboard, launch the selected entry, open its folder, and reload configuration without restarting the process.

## Features

- Terminal UI built with `Terminal.Gui`
- Launcher entries loaded from `src/LaunchKit/appsettings.json`
- Keyboard navigation with page-aware grid layout
- Launch selected application with `Enter`
- Open selected application folder with `O`
- Reload configuration with `F5`
- Availability status per entry: `Ready` or `Path not found`
- Environment variable expansion for configured paths and arguments
- Immutable domain models for launcher options and application entries
- Domain validation during configuration mapping

## Controls

- `Left` / `Right` / `Up` / `Down`: move selection
- `Home` / `End`: jump to first or last item
- `PageUp` / `PageDown`: move by page
- `Enter`: launch selected item
- `O`: open containing folder
- `F5`: reload `appsettings.json`
- `Esc`: exit

## Project Structure

- `src/LaunchKit`: executable entry point and runtime assets
- `src/Abstractions`: application interfaces
- `src/Models`: domain models and value objects
- `src/Logic`: workflow orchestration
- `src/Infrastructure`: JSON configuration loading, TUI rendering, process launching

## Domain Model

The domain layer is intentionally immutable.

- `LauncherOptions` is created through its constructor and exposes read-only application collection access.
- `ApplicationEntry` is created through constructor or factory method and aggregates typed values instead of raw strings.
- `LayoutOptions` and `LayoutState` are immutable and encapsulate layout calculation logic.

String-like data is represented with dedicated value objects:

- `ApplicationName`
- `ApplicationPath`
- `LaunchArguments`
- `WorkingDirectoryPath`
- `ApplicationDescription`

`ApplicationEntry` returns domain-level availability through `ApplicationAvailability`. Converting that enum into a display string is done in the UI layer, not in the model.

## Configuration

The launcher reads the `Launcher` section from:

- `src/LaunchKit/appsettings.json`

Example:

```json
{
  "Launcher": {
    "ShowFullPath": false,
    "Layout": {
      "Columns": 3,
      "TileWidth": 34,
      "TileHeight": 6,
      "TileSpacing": 2
    },
    "Applications": [
      {
        "Name": "VS Code",
        "Path": "%LOCALAPPDATA%\\Programs\\Microsoft VS Code\\Code.exe",
        "Arguments": "",
        "Description": "Code editor",
        "WorkingDirectory": "%USERPROFILE%\\Documents\\Code"
      }
    ]
  }
}
```

Configuration binding uses mutable infrastructure-only DTOs:

- `LauncherConfigurationDocument`
- `LayoutConfigurationDocument`
- `ApplicationEntryDocument`

Those DTOs are immediately mapped to immutable domain models. Validation errors are raised during this mapping step.

## Path Handling

`ApplicationPath`, `LaunchArguments`, and `WorkingDirectoryPath` normalize input by:

- trimming whitespace
- trimming wrapping quotes
- expanding Windows environment variables such as `%USERPROFILE%`

Launch availability rules:

- `Ready` when the configured target file or directory exists
- `Ready` when the value is a command without explicit path hints, for example `notepad.exe`
- `Path not found` when the value is path-like but the target does not exist

Folder resolution rules:

- explicit `WorkingDirectory` wins
- otherwise the path value is inspected for process working directory or containing folder

## UI Behavior

The terminal UI shows:

- summary line with item count, page, columns, and selected application
- help line with key bindings
- tile grid with entry name, description, path or executable name, and availability
- status bar with actions

The grid layout is calculated from:

- configured column count
- configured tile size and spacing
- current terminal width and height
- number of configured applications

If there are no configured applications, the workflow prints a message and exits instead of opening the TUI.

## Building and Running

Run:

```powershell
dotnet run --project .\src\LaunchKit\LaunchKit.csproj
```

Build:

```powershell
dotnet build .\src\LaunchKit.slnx
```

Publish:

```powershell
dotnet publish .\src\LaunchKit\LaunchKit.csproj -c Release -r win-x64 --self-contained false
```
