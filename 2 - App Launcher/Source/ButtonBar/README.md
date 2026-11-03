# DVMS Button Bar

A reusable .NET 9 desktop application launcher built with C#, DevExpress
WinForms 25.1, a wrapping flow layout, and SQLite.

## Features

- Add programs by browsing or dragging files and shortcuts onto the bar.
- Edit button name, target, arguments, working folder, and custom icon.
- Reorder or remove buttons from each button's right-click menu.
- Show or hide names and select small, medium, or large buttons.
- Optional always-on-top behavior.
- Best-effort placement of newly launched windows on the Button Bar's monitor.
- Optional WAV or MP3 sound for any button, including sound-only buttons.
- A configurable shared sound directory for relative sound paths.
- Clone existing button definitions.
- Hide the menu bar and restore it from any button's right-click menu.
- Automatically preserve the button area when the menu is shown or hidden.
- Larger program icons at every button-size setting.
- Successful program launches recorded in the SQLite `LaunchHistory` table.
- Read-only launched-program log viewer under Settings, with filtering and refresh.
- Process termination tracking with process ID, completion status, end time, duration, and exit code.
- Optional ETW monitoring of user files opened, read, written, and closed by a launched process and its children.
- Read-only opened-file activity viewer under Settings.
- Independent locks for button changes, window resizing, and window movement.
- Persistent position, size, view, lock, and button definitions.
- Calculator, Notepad, and Windows Settings samples on first use.

Data is stored at `%LOCALAPPDATA%\DVMS\ButtonBar\button-bar.db`.

## Build

```powershell
dotnet build ButtonBar.slnx
```

The local NuGet configuration uses the installed DevExpress 25.1 package
directory and NuGet.org for Microsoft.Data.Sqlite.
