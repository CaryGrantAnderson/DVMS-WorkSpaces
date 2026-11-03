using DevExpress.XtraEditors;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;

namespace DVMS.StartTree;

internal sealed class ProgramTreeForm : XtraForm
{
    private readonly StartTreeDatabase _database;
    private readonly TreeList _tree = new();
    private readonly OutsideClickMonitor _outsideClickMonitor;
    private Form? _anchor;

    public ProgramTreeForm(StartTreeDatabase database)
    {
        _database = database;
        _outsideClickMonitor = new OutsideClickMonitor(
            () => Bounds,
            () => _anchor?.Bounds,
            () => BeginInvoke(Hide));
        AutoScaleMode = AutoScaleMode.Dpi;
        ClientSize = new Size(380, 560);
        FormBorderStyle = FormBorderStyle.None;
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.Manual;
        Text = "Programs";

        ConfigureTree();
        Controls.Add(_tree);
        Deactivate += (_, _) =>
        {
            if (!_database.LoadSettings().KeepMenuOpenAfterLaunch)
            {
                Hide();
            }
        };
    }

    public void Reload()
    {
        _tree.BeginUnboundLoad();
        try
        {
            _tree.ClearNodes();
            var nodesByKey = new Dictionary<string, TreeListNode>(StringComparer.OrdinalIgnoreCase);

            foreach (var item in OrderForTree(_database.LoadMenuItems()))
            {
                var parent = item.ParentKey is not null && nodesByKey.TryGetValue(item.ParentKey, out var found)
                    ? found
                    : null;
                var node = _tree.AppendNode(new object[] { item.DisplayName }, parent);
                node.Tag = item;
                node.ImageIndex = item.IsFolder ? 0 : 1;
                nodesByKey[item.ItemKey] = node;
            }
        }
        finally
        {
            _tree.EndUnboundLoad();
        }

        foreach (TreeListNode root in _tree.Nodes)
        {
            root.Expanded = true;
        }
    }

    public void ShowNear(Form anchor)
    {
        _anchor = anchor;
        var screen = Screen.FromControl(anchor).WorkingArea;
        var x = anchor.Right + 8;
        if (x + Width > screen.Right)
        {
            x = anchor.Left - Width - 8;
        }

        x = Math.Clamp(x, screen.Left, Math.Max(screen.Left, screen.Right - Width));

        var y = Math.Clamp(anchor.Top, screen.Top, Math.Max(screen.Top, screen.Bottom - Height));
        Location = new Point(x, y);
        Show();
        Activate();
        _tree.Focus();
    }

    protected override void OnVisibleChanged(EventArgs e)
    {
        base.OnVisibleChanged(e);
        if (Visible)
        {
            _outsideClickMonitor.Start();
        }
        else
        {
            _outsideClickMonitor.Stop();
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _outsideClickMonitor.Dispose();
        }

        base.Dispose(disposing);
    }

    private void ConfigureTree()
    {
        _tree.Dock = DockStyle.Fill;
        _tree.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
        _tree.OptionsBehavior.Editable = false;
        _tree.OptionsBehavior.ReadOnly = true;
        _tree.OptionsSelection.EnableAppearanceFocusedCell = false;
        _tree.OptionsView.ShowColumns = false;
        _tree.OptionsView.ShowHorzLines = false;
        _tree.OptionsView.ShowIndicator = false;
        _tree.OptionsView.ShowVertLines = false;
        _tree.TreeViewColumn = _tree.Columns.AddVisible("Name");
        _tree.RowHeight = 28;
        _tree.MouseClick += (_, e) =>
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            var hit = _tree.CalcHitInfo(e.Location);
            if (hit.Node?.Tag is MenuItemDefinition { IsFolder: false })
            {
                _tree.FocusedNode = hit.Node;
                LaunchFocusedItem();
            }
        };
        _tree.KeyDown += (_, e) =>
        {
            if (e.KeyCode == Keys.Enter)
            {
                LaunchFocusedItem();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                Hide();
                e.Handled = true;
            }
        };
    }

    private void LaunchFocusedItem()
    {
        var node = _tree.FocusedNode;
        if (node?.Tag is not MenuItemDefinition item || item.IsFolder)
        {
            return;
        }

        LauncherService.Launch(item);
        if (!_database.LoadSettings().KeepMenuOpenAfterLaunch)
        {
            Hide();
        }
    }

    private static IEnumerable<MenuItemDefinition> OrderForTree(
        IReadOnlyCollection<MenuItemDefinition> items)
    {
        var byParent = items.ToLookup(x => x.ParentKey, StringComparer.OrdinalIgnoreCase);
        return Walk(null);

        IEnumerable<MenuItemDefinition> Walk(string? parent)
        {
            foreach (var item in byParent[parent]
                         .OrderBy(x => x.SortOrder)
                         .ThenBy(x => x.DisplayName, StringComparer.CurrentCultureIgnoreCase))
            {
                yield return item;
                foreach (var child in Walk(item.ItemKey))
                {
                    yield return child;
                }
            }
        }
    }
}
