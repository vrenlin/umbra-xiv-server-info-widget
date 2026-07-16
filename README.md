# umbra-xiv-server-info-widget

An [Umbra](https://github.com/una-xiv/umbra) plugin that adds a **Server Info Menu** toolbar widget.

Instead of laying out every Server Info Bar (DTR bar) entry across the toolbar like the built-in
"Server Info Bar" widget does, this widget collapses them behind a single button. Clicking it opens
a dynamic popup menu listing every currently active Server Info Bar entry (world name, queue status,
FPS counters, third-party plugin entries, etc.), each of which stays fully interactable — clicking an
entry in the menu invokes the same click action it would if it were on the native bar.

## Features

- Single toolbar button that expands into a dynamic menu of all active Server Info Bar entries.
- Menu entries stay live — text, tooltips, visibility and sort order update every frame, same as the
  native bar.
- Entries that have a click action (queue windows, duty finder, third-party plugin bars, etc.) remain
  clickable from within the menu.
- Optionally hides the game's native Server Info Bar while the widget is active, since its contents are
  now reachable from the menu.
- Standard Umbra widget customization (icon, text, colors, sizing) via the widget's settings panel.

## Installation (for users)

1. Install [Umbra](https://umbra.una.gg/) through Dalamud's plugin installer.
2. Add this plugin's `latest.zip` as a custom plugin repository/DLL in Umbra's **Settings → Plugins** section
   (see the [Umbra plugin docs](https://github.com/una-xiv/umbra) for how third-party Umbra plugins are loaded).
3. Add the **Server Info Menu** widget to your toolbar from the widget picker.

## Building from source

This project follows the same structure as [Umbra.SamplePlugin](https://github.com/una-xiv/Umbra.SamplePlugin).

Prerequisites:

- .NET SDK 10.x or higher
- Umbra installed through XIVLauncher/Dalamud (used to locate `Umbra.dll`, `Umbra.Common.dll`, `Umbra.Game.dll`)
- Dalamud's dev hooks (installed automatically alongside FFXIV via XIVLauncher)

```
dotnet build --configuration Release ServerInfoMenu.sln
```

The built plugin will be placed in `out/Release`.

## Project layout

```
ServerInfoMenu.sln
ServerInfoMenu/
  ServerInfoMenu.csproj
  Widgets/
    ServerInfoMenuWidget.cs           # Core widget: menu population, entry sync
    ServerInfoMenuWidget.Config.cs    # User-configurable options
    ServerInfoMenuWidget.NativeBar.cs # Hides/restores the native Server Info Bar
```
