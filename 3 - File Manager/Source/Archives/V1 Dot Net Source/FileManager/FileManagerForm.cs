using System.ComponentModel;
using System.Diagnostics;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;

namespace DVMS.FileManager;

internal sealed class FileManagerForm : XtraForm
{
    private readonly FileManagerDatabase _database;
    private readonly TreeList _folders = new();
    private readonly GridControl _files = new();
    private readonly GridView _fileView = new();
    private readonly ListBoxControl _favorites = new();
    private readonly TextEdit _address = new();
    private readonly ButtonEdit _search = new();
    private readonly MemoEdit _preview = new();
    private readonly LabelControl _status = new();
    private readonly BindingList<FileSystemItem> _items = new();
    private readonly Stack<string> _back = new();
    private readonly Stack<string> _forward = new();
    private string _currentPath = "";
    private string? _clipboardPath;
    private ClipboardOperation _clipboardOperation;
    private bool _navigatingTree;

    public FileManagerForm(FileManagerDatabase database)
    {
        _database = database;
        Text = "DVMS WorkSpaces File Manager";
        WindowState = FormWindowState.Maximized;
        MinimumSize = new Size(1050, 650);
        KeyPreview = true;

        ConfigureToolbar();
        ConfigureBody();
        ConfigureEvents();
        PopulateFolders();
        ReloadFavorites();

        var initial = database.GetSetting("LastPath");
        Navigate(Directory.Exists(initial) ? initial! : Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), false);
    }

    private void ConfigureToolbar()
    {
        var manager = new BarManager { Form = this };
        var bar = new Bar(manager, "File Manager") { DockStyle = BarDockStyle.Top };
        manager.MainMenu = bar;
        bar.AddItem(Item(manager, "Back", (_, _) => GoBack(), "Backward"));
        bar.AddItem(Item(manager, "Forward", (_, _) => GoForward(), "Forward"));
        bar.AddItem(Item(manager, "Up", (_, _) => GoUp(), "Up"));
        bar.AddItem(Item(manager, "Refresh", (_, _) => RefreshFiles(), "Refresh"));
        bar.AddItem(Item(manager, "New Folder", (_, _) => CreateFolder(), "NewFolder"));
        bar.AddItem(Item(manager, "Cut", (_, _) => SetClipboard(ClipboardOperation.Move), "Cut"));
        bar.AddItem(Item(manager, "Copy", (_, _) => SetClipboard(ClipboardOperation.Copy), "Copy"));
        bar.AddItem(Item(manager, "Paste", (_, _) => Paste(), "Paste"));
        bar.AddItem(Item(manager, "Rename", (_, _) => RenameSelected(), "Rename"));
        bar.AddItem(Item(manager, "Delete", (_, _) => DeleteSelected(), "Delete"));
        bar.AddItem(Item(manager, "Notes", (_, _) => EditMetadata(), "Edit"));
        bar.AddItem(Item(manager, "Favorite", (_, _) => AddFavorite(), "Favorites"));
    }

    private static BarButtonItem Item(BarManager manager, string caption, ItemClickEventHandler handler, string? image = null)
    {
        var item = new BarButtonItem(manager, caption);
        item.ItemClick += handler;
        if (image is not null) item.ImageOptions.ImageUri.Uri = image;
        return item;
    }

    private void ConfigureBody()
    {
        var top = new TableLayoutPanel { Dock = DockStyle.Top, Height = 42, Padding = new Padding(8, 6, 8, 4), ColumnCount = 3 };
        top.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 76));
        top.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        top.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 300));
        top.Controls.Add(new LabelControl { Text = "Location", Anchor = AnchorStyles.Left, AutoSizeMode = LabelAutoSizeMode.None, Dock = DockStyle.Fill, Appearance = { TextOptions = { VAlignment = VertAlignment.Center } } }, 0, 0);
        top.Controls.Add(_address, 1, 0);
        _search.Properties.NullValuePrompt = "Search this folder...";
        _search.Properties.Buttons[0].Kind = DevExpress.XtraEditors.Controls.ButtonPredefines.Search;
        top.Controls.Add(_search, 2, 0);
        Controls.Add(top);

        var outer = new SplitContainerControl { Dock = DockStyle.Fill, SplitterPosition = 270 };
        Controls.Add(outer);
        outer.BringToFront();
        top.BringToFront();

        var navigation = new SplitContainerControl { Dock = DockStyle.Fill, Horizontal = true, SplitterPosition = 410 };
        outer.Panel1.Controls.Add(navigation);
        ConfigureFolderTree();
        navigation.Panel1.Controls.Add(_folders);
        _favorites.Dock = DockStyle.Fill;
        _favorites.DisplayMember = nameof(FavoriteLocation.Name);
        navigation.Panel2.Controls.Add(_favorites);
        navigation.Panel2.Controls.Add(new LabelControl { Text = "  Favorites", Dock = DockStyle.Top, Height = 28, Appearance = { Font = new Font("Segoe UI", 10, FontStyle.Bold) } });

        var content = new SplitContainerControl { Dock = DockStyle.Fill, Horizontal = true, SplitterPosition = 520, FixedPanel = SplitFixedPanel.Panel2 };
        outer.Panel2.Controls.Add(content);
        ConfigureFileGrid();
        content.Panel1.Controls.Add(_files);
        _preview.Dock = DockStyle.Fill;
        _preview.Properties.ReadOnly = true;
        _preview.Properties.NullValuePrompt = "Select an item to see its notes and details.";
        content.Panel2.Controls.Add(_preview);
        content.Panel2.Controls.Add(new LabelControl { Text = "  Details and notes", Dock = DockStyle.Top, Height = 28, Appearance = { Font = new Font("Segoe UI", 10, FontStyle.Bold) } });

        _status.Dock = DockStyle.Bottom;
        _status.Height = 26;
        _status.Padding = new Padding(10, 4, 0, 0);
        Controls.Add(_status);
    }

    private void ConfigureFolderTree()
    {
        _folders.Dock = DockStyle.Fill;
        _folders.OptionsBehavior.Editable = false;
        _folders.OptionsView.ShowColumns = false;
        _folders.OptionsView.ShowIndicator = false;
        _folders.Columns.AddVisible("Name");
    }

    private void ConfigureFileGrid()
    {
        _files.Dock = DockStyle.Fill;
        _files.MainView = _fileView;
        _files.ViewCollection.Add(_fileView);
        _files.DataSource = _items;
        _fileView.OptionsBehavior.Editable = false;
        _fileView.OptionsSelection.MultiSelect = true;
        _fileView.OptionsView.ShowGroupPanel = true;
        _fileView.OptionsView.ShowAutoFilterRow = true;
        _fileView.Columns.AddVisible(nameof(FileSystemItem.Name), "Name").VisibleIndex = 0;
        _fileView.Columns.AddVisible(nameof(FileSystemItem.Description), "Description").VisibleIndex = 1;
        _fileView.Columns.AddVisible(nameof(FileSystemItem.Type), "Type").VisibleIndex = 2;
        var sizeColumn = _fileView.Columns.AddVisible(nameof(FileSystemItem.Size), "Size");
        sizeColumn.VisibleIndex = 3;
        _fileView.Columns.AddVisible(nameof(FileSystemItem.Modified), "Date modified").VisibleIndex = 4;
        _fileView.Columns.AddVisible(nameof(FileSystemItem.Created), "Date created").VisibleIndex = 5;
        _fileView.Columns.AddVisible(nameof(FileSystemItem.Note), "Note").VisibleIndex = 6;
        sizeColumn.DisplayFormat.FormatType = FormatType.Numeric;
        sizeColumn.DisplayFormat.FormatString = "n0";
        _fileView.RowStyle += (_, e) =>
        {
            if (e.RowHandle < 0) return;
            var item = _fileView.GetRow(e.RowHandle) as FileSystemItem;
            if (item is null || string.IsNullOrEmpty(item.ColorName)) return;
            var color = Color.FromName(item.ColorName);
            if (color.A > 0) e.Appearance.ForeColor = color;
        };
        _fileView.BestFitColumns();
    }

    private void ConfigureEvents()
    {
        _address.KeyDown += (_, e) => { if (e.KeyCode == Keys.Enter) { Navigate(_address.Text); e.SuppressKeyPress = true; } };
        _search.ButtonClick += (_, _) => ApplySearch();
        _search.KeyDown += (_, e) => { if (e.KeyCode == Keys.Enter) { ApplySearch(); e.SuppressKeyPress = true; } };
        _files.DoubleClick += (_, _) => OpenSelected();
        _fileView.FocusedRowChanged += (_, _) => UpdatePreview();
        _folders.BeforeExpand += (_, e) => EnsureChildren(e.Node);
        _folders.FocusedNodeChanged += (_, e) =>
        {
            if (_navigatingTree || e.Node?.Tag is not string path || !Directory.Exists(path)) return;
            Navigate(path);
        };
        _favorites.DoubleClick += (_, _) =>
        {
            if (_favorites.SelectedItem is FavoriteLocation favorite) Navigate(favorite.Path);
        };
        _favorites.KeyDown += (_, e) =>
        {
            if (e.KeyCode == Keys.Delete && _favorites.SelectedItem is FavoriteLocation favorite)
            {
                _database.RemoveFavorite(favorite.Id);
                ReloadFavorites();
            }
        };
        KeyDown += (_, e) =>
        {
            if (e.KeyCode == Keys.F5) RefreshFiles();
            else if (e.KeyCode == Keys.Back && !_address.ContainsFocus && !_search.ContainsFocus) GoBack();
            else if (e.KeyCode == Keys.Delete) DeleteSelected();
            else if (e.KeyCode == Keys.F2) RenameSelected();
            else if (e.Control && e.KeyCode == Keys.C) SetClipboard(ClipboardOperation.Copy);
            else if (e.Control && e.KeyCode == Keys.X) SetClipboard(ClipboardOperation.Move);
            else if (e.Control && e.KeyCode == Keys.V) Paste();
        };
        FormClosing += (_, _) => _database.SetSetting("LastPath", _currentPath);
    }

    private void PopulateFolders()
    {
        _folders.BeginUnboundLoad();
        _folders.Nodes.Clear();
        AddRoot("Home", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
        foreach (var drive in DriveInfo.GetDrives().Where(x => x.IsReady)) AddRoot($"{drive.Name}  {drive.VolumeLabel}", drive.RootDirectory.FullName);
        _folders.EndUnboundLoad();
    }

    private void AddRoot(string name, string path)
    {
        var node = _folders.AppendNode(new object[] { name }, null, path);
        AddPlaceholder(node);
    }

    private static void AddPlaceholder(TreeListNode node)
    {
        try { if (Directory.EnumerateDirectories((string)node.Tag).Any()) node.TreeList.AppendNode(new object[] { "Loading..." }, node, null); }
        catch { }
    }

    private void EnsureChildren(TreeListNode node)
    {
        if (node.Nodes.Count != 1 || node.Nodes[0].Tag is not null) return;
        node.Nodes.Clear();
        try
        {
            foreach (var directory in Directory.EnumerateDirectories((string)node.Tag).OrderBy(Path.GetFileName))
            {
                var child = _folders.AppendNode(new object[] { Path.GetFileName(directory) }, node, directory);
                AddPlaceholder(child);
            }
        }
        catch { }
    }

    private void Navigate(string path, bool addHistory = true)
    {
        path = Environment.ExpandEnvironmentVariables(path.Trim().Trim('"'));
        if (File.Exists(path)) path = Path.GetDirectoryName(path)!;
        if (!Directory.Exists(path)) { XtraMessageBox.Show(this, "That folder does not exist.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
        path = Path.GetFullPath(path);
        if (addHistory && !string.IsNullOrEmpty(_currentPath) && !string.Equals(_currentPath, path, StringComparison.OrdinalIgnoreCase))
        {
            _back.Push(_currentPath);
            _forward.Clear();
        }
        _currentPath = path;
        _address.Text = path;
        _database.RecordVisit(path);
        RefreshFiles();
        SelectTreePath(path);
    }

    private void RefreshFiles()
    {
        try
        {
            var loaded = FileSystemService.Enumerate(_currentPath, _database);
            _items.RaiseListChangedEvents = false;
            _items.Clear();
            foreach (var item in loaded) _items.Add(item);
            _items.RaiseListChangedEvents = true;
            _items.ResetBindings();
            ApplySearch();
            _status.Text = $"{loaded.Count(x => x.IsDirectory):n0} folders   {loaded.Count(x => !x.IsDirectory):n0} files";
        }
        catch (Exception ex) { XtraMessageBox.Show(this, ex.Message, "Unable to open folder", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
    }

    private void ApplySearch()
    {
        var term = _search.Text.Trim().Replace("'", "''");
        _fileView.ActiveFilterString = string.IsNullOrEmpty(term) ? "" : $"Contains([Name], '{term}') Or Contains([Description], '{term}') Or Contains([Note], '{term}')";
    }

    private FileSystemItem? Selected() => _fileView.GetFocusedRow() as FileSystemItem;

    private void OpenSelected()
    {
        var item = Selected();
        if (item is null) return;
        try { if (item.IsDirectory) Navigate(item.FullPath); else FileSystemService.Open(item.FullPath); }
        catch (Exception ex) { ShowError("Unable to open item", ex); }
    }

    private void UpdatePreview()
    {
        var item = Selected();
        _preview.Text = item is null ? "" : $"{item.FullPath}\r\n\r\n{item.Description}\r\n\r\n{item.Note}".Trim();
    }

    private void EditMetadata()
    {
        var item = Selected();
        if (item is null) return;
        using var editor = new MetadataEditorForm(new ItemMetadata(item.FullPath, item.Note, item.Description, item.ColorName));
        if (editor.ShowDialog(this) != DialogResult.OK) return;
        _database.SaveMetadata(editor.Build(item.FullPath));
        RefreshFiles();
    }

    private void CreateFolder()
    {
        var name = XtraInputBox.Show("Folder name:", "Create folder", "New folder");
        if (string.IsNullOrWhiteSpace(name)) return;
        try { Directory.CreateDirectory(Path.Combine(_currentPath, name.Trim())); RefreshFiles(); }
        catch (Exception ex) { ShowError("Unable to create folder", ex); }
    }

    private void RenameSelected()
    {
        var item = Selected();
        if (item is null) return;
        var name = XtraInputBox.Show("New name:", "Rename", item.Name);
        if (string.IsNullOrWhiteSpace(name) || name == item.Name) return;
        try
        {
            var destination = Path.Combine(Path.GetDirectoryName(item.FullPath)!, name.Trim());
            if (item.IsDirectory) Directory.Move(item.FullPath, destination); else File.Move(item.FullPath, destination);
            RefreshFiles();
        }
        catch (Exception ex) { ShowError("Unable to rename item", ex); }
    }

    private void DeleteSelected()
    {
        var selected = _fileView.GetSelectedRows().Select(row => _fileView.GetRow(row) as FileSystemItem).Where(x => x is not null).Cast<FileSystemItem>().ToList();
        if (selected.Count == 0) return;
        if (XtraMessageBox.Show(this, $"Permanently delete {selected.Count} selected item(s)?", "Confirm delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
        try
        {
            foreach (var item in selected) { if (item.IsDirectory) Directory.Delete(item.FullPath, true); else File.Delete(item.FullPath); }
            RefreshFiles();
        }
        catch (Exception ex) { ShowError("Unable to delete item", ex); }
    }

    private void SetClipboard(ClipboardOperation operation)
    {
        var item = Selected();
        if (item is null) return;
        _clipboardPath = item.FullPath;
        _clipboardOperation = operation;
        _status.Text = $"{operation}: {item.Name}";
    }

    private void Paste()
    {
        if (string.IsNullOrEmpty(_clipboardPath) || (!File.Exists(_clipboardPath) && !Directory.Exists(_clipboardPath))) return;
        try
        {
            var destination = FileSystemService.UniqueDestination(_currentPath, _clipboardPath);
            if (_clipboardOperation == ClipboardOperation.Copy) FileSystemService.Copy(_clipboardPath, destination);
            else
            {
                if (File.Exists(_clipboardPath)) File.Move(_clipboardPath, destination); else Directory.Move(_clipboardPath, destination);
                _clipboardPath = null;
            }
            RefreshFiles();
        }
        catch (Exception ex) { ShowError("Unable to paste item", ex); }
    }

    private void AddFavorite()
    {
        _database.AddFavorite(_currentPath);
        ReloadFavorites();
    }

    private void ReloadFavorites()
    {
        _favorites.DataSource = null;
        _favorites.DataSource = _database.GetFavorites();
    }

    private void GoBack()
    {
        if (_back.Count == 0) return;
        _forward.Push(_currentPath);
        Navigate(_back.Pop(), false);
    }

    private void GoForward()
    {
        if (_forward.Count == 0) return;
        _back.Push(_currentPath);
        Navigate(_forward.Pop(), false);
    }

    private void GoUp()
    {
        var parent = Directory.GetParent(_currentPath);
        if (parent is not null) Navigate(parent.FullName);
    }

    private void SelectTreePath(string path)
    {
        _navigatingTree = true;
        try
        {
            foreach (TreeListNode root in _folders.Nodes)
            {
                if (root.Tag is string rootPath && path.StartsWith(rootPath, StringComparison.OrdinalIgnoreCase))
                {
                    root.Expanded = true;
                    if (string.Equals(rootPath.TrimEnd('\\'), path.TrimEnd('\\'), StringComparison.OrdinalIgnoreCase)) _folders.FocusedNode = root;
                    break;
                }
            }
        }
        finally { _navigatingTree = false; }
    }

    private void ShowError(string title, Exception ex) => XtraMessageBox.Show(this, ex.Message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
}
