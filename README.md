# LaunchKit

Console launcher for a predefined list of programs from `appsettings.json`.

## Structure

The repository now follows the same overall layout as `FolderSync`:

- `src/LaunchKit.slnx`: solution entry point
- `src/.editorconfig`: shared style rules
- `src/LaunchKit`: executable project
- `src/Abstractions`: interfaces
- `src/Logic`: orchestration and layout logic
- `src/Infrastructure`: JSON configuration, console UI, process launching
- `src/Models`: shared data models

## What it does

- Reads the list of applications from `appsettings.json`
- Renders them as a tile grid in the console
- Lets you move with the keyboard
- Launches the selected app with `Enter`
- Opens its folder with `O`
- Supports configurable tile layout

## Controls

- `Left/Right/Up/Down`: move selection
- `Enter`: launch selected item
- `O`: open containing folder
- `F5`: reload `appsettings.json`
- `Esc`: exit

## Configuration

Main settings live in `Launcher`:

```json
{
  "Launcher": {
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

`Path` and `WorkingDirectory` support environment variables like `%USERPROFILE%`.

The active config file is:

- `src/LaunchKit/appsettings.json`

## Run

```powershell
dotnet run --project .\src\LaunchKit\LaunchKit.csproj
```

## Publish

```powershell
dotnet build .\src\LaunchKit.slnx
dotnet publish .\src\LaunchKit\LaunchKit.csproj -c Release -r win-x64 --self-contained false
```

After publishing, place a shortcut to the generated executable on your desktop panel/taskbar.
