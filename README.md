# LightSwitch Monitor

LightSwitch Monitor is a lightweight Windows 11 tray application that automatically dims every monitor except the one currently under the mouse cursor. It is built for multi-monitor setups where the active screen should stay visually prominent without changing physical monitor brightness.

## Features

- Automatically detects the monitor currently containing the mouse cursor.
- Dims inactive displays using transparent click-through overlays.
- Runs quietly in the Windows notification area.
- Opens a compact settings window from the tray icon.
- Provides an adjustable dimming slider from 5% to 85%.
- Saves user preferences between launches.
- Keeps running in the tray when the settings window is closed.

## How It Works

The application creates borderless WPF overlay windows on inactive monitors. These overlays are black, semi-transparent, topmost, and configured with native Windows extended styles so they do not steal focus or block mouse interaction.

The cursor position is checked continuously. When the cursor moves to another monitor, the previous active monitor is dimmed and the new active monitor is restored.

## Requirements

- Windows 11
- .NET SDK compatible with `net10.0-windows`
- At least two monitors to see the dimming behavior

## Build

Run the following command from the project directory:

```powershell
dotnet build .\LightSwitchMonitor.csproj
```

The debug executable is generated at:

```text
bin\Debug\net10.0-windows\LightSwitchMonitor.exe
```

## Run

Launch the application from:

```text
bin\Debug\net10.0-windows\LightSwitchMonitor.exe
```

After startup, LightSwitch Monitor appears in the Windows notification area.

- Left-click the tray icon to open the settings window.
- Right-click the tray icon to open the context menu.
- Use the slider to adjust inactive-monitor dimming.
- Use the context menu to enable, disable, or exit the application.

## Settings

User settings are stored in:

```text
%APPDATA%\LightSwitchMonitor\settings.json
```

The settings file currently stores:

- whether dimming is enabled,
- the selected dimming percentage.

## Project Structure

```text
LightSwitchMonitor
├── App.xaml / App.xaml.cs              application startup and tray integration
├── MainWindow.xaml / MainWindow.xaml.cs
│                                       settings window and slider behavior
├── MonitorDimmerService.cs             monitor detection and overlay coordination
├── DimOverlayWindow.cs                 click-through dimming overlay window
├── AppSettings.cs                      user settings persistence
└── LightSwitchMonitor.csproj           WPF project configuration
```

## Technical Notes

- The app does not change hardware monitor brightness.
- Dimming is implemented visually through semi-transparent overlays.
- With only one connected monitor, overlays are hidden automatically.
- Closing the settings window does not stop the application; the process remains active in the tray.

