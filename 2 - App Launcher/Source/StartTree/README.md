# DVMS Start Tree

Start Tree is a compact Windows desktop application launcher built with .NET 9,
C#, DevExpress WinForms 25.1, and SQLite.

## Interaction

- Left-click the icon to open or close the program tree.
- Click a program or select it and press Enter to launch it.
- Right-click and drag the icon to move it. Its position is saved.
- Right-click without moving to open the Settings menu.
- Press Escape or click elsewhere to close the program tree.

The tree is populated from the current-user and all-users Windows Start Menu
program folders. Discovered menu definitions and application settings are stored
in `%LOCALAPPDATA%\DVMS\StartTree\start-tree.db`.

## Build

Open `StartTree.slnx` in Visual Studio or run:

```powershell
dotnet build StartTree.slnx
```

The local `NuGet.Config` uses the installed DevExpress 25.1 package directory
and NuGet.org for Microsoft.Data.Sqlite.
