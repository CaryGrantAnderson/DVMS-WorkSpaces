# DVMS WorkSpaces File Manager

A Windows desktop file manager built with C#/.NET 9, DevExpress WinForms 25.1, and SQLite.

## Current capabilities

- Drive and folder navigation with Back, Forward, Up, address entry, and refresh
- Sortable, groupable, filterable DevExpress file grid
- Folder-local search across names, descriptions, and notes
- Open, create folder, rename, delete, cut, copy, and paste
- Persistent favorites
- Persistent notes, descriptions, and color markers for files and folders
- Last-location restore and a local recent-location history

Application data is stored in `%LOCALAPPDATA%\DVMS\FileManager\file-manager.db`.

## Build and run

Open `FileManager.slnx` in Visual Studio 2022 with the .NET 9 SDK installed, or run:

```powershell
dotnet build FileManager.slnx
dotnet run --project FileManager\FileManager.csproj
```

The configured DevExpress NuGet feed must be available for package restore.
