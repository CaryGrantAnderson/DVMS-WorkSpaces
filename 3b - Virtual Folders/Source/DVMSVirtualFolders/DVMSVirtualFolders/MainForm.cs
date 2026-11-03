using DevExpress.XtraBars;
using DevExpress.XtraBars.Docking;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraDialogs;
using DevExpress.XtraDialogs.FileExplorerExtensions;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Base.Controls.Preview;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace DVMSVirtualFolders
{
    internal sealed partial class MainForm : RibbonForm
    {
        private readonly VirtualFoldersDatabase database = new VirtualFoldersDatabase();
        private readonly ApplicationDatabases applicationDatabases = new ApplicationDatabases();
        private readonly Dictionary<long, TreeListNode> folderNodes = new Dictionary<long, TreeListNode>();
        //private readonly DevExpress.XtraDialogs.FileExplorerAssistant previewAssistant =
        //    new DevExpress.XtraDialogs.FileExplorerAssistant();
        private Point dragStart;
        private TreeListNode dragNode;

        public MainForm()
        {
            InitializeComponent();
            BuildRibbon();
            BuildTree();
            Load += MainForm_Load;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            database.Initialize();
            applicationDatabases.Initialize();
            applicationDatabases.Log("Application Started", Application.ExecutablePath);
            ReloadTree();
        }

        private void BuildRibbon()
        {
            ribbon.Dock = DockStyle.Top;
            Ribbon = ribbon;
            Controls.Add(ribbon);
            var home = new RibbonPage("Home");
            var folders = new RibbonPageGroup("Virtual Folders");
            var items = new RibbonPageGroup("Items");
            var view = new RibbonPageGroup("View");
            var application = new RibbonPageGroup("Application") { Alignment = RibbonPageGroupAlignment.Far };
            home.Groups.AddRange(new[] { folders, items, view, application });
            ribbon.Pages.Add(home);
            AddButton(folders, "New Folder", "svgimages/actions/newfolder.svg", (s, e) => AddVirtualFolder());
            AddButton(folders, "Rename", "svgimages/icon%20builder/actions_rename.svg", (s, e) => RenameSelected());
            AddButton(folders, "Remove", "svgimages/dashboards/deletetablerows.svg", (s, e) => RemoveSelected());
            AddButton(items, "Add Files", "svgimages/outlook%20inspired/newfile.svg", (s, e) => AddFiles());
            AddButton(items, "Add Folder", "svgimages/icon%20builder/actions_folderopen.svg", (s, e) => AddPhysicalFolder());
            AddButton(items, "Open", "svgimages/actions/open.svg", (s, e) => OpenSelected());
            AddButton(items, "Physical Location", "svgimages/icon%20builder/actions_folderopen.svg", (s, e) => ShowPhysicalLocation());
            AddButton(view, "Preview", "svgimages/print/preview.svg", (s, e) => previewPanel.Show());
            AddButton(view, "Open DB Folder", "svgimages/icon%20builder/actions_database.svg", (s, e) => Process.Start(applicationDatabases.DirectoryPath));
            AddButton(application, "Close", "svgimages/xaf/action_exit.svg", (s, e) => Close());
        }

        private void AddButton(RibbonPageGroup group, string caption, string icon, ItemClickEventHandler handler)
        {
            var button = new BarButtonItem { Caption = caption, RibbonStyle = RibbonItemStyles.Large };
            button.ImageOptions.SvgImage = DevExpress.Images.ImageResourceCache.Default.GetSvgImage(icon);
            button.ItemClick += handler; ribbon.Items.Add(button); group.ItemLinks.Add(button);
        }

        private void BuildTree()
        {
            tree.Dock = DockStyle.Fill;
            tree.AllowDrop = true;
            tree.OptionsBehavior.Editable = false;
            tree.OptionsView.ShowIndicator = false;
            tree.OptionsView.ShowHorzLines = false;
            tree.OptionsView.ShowVertLines = false;
            AddColumn("Name", "Virtual Folder / Item", 280);
            AddColumn("Type", "Type", 110);
            AddColumn("Source", "Source Folder", 230);
            AddColumn("Target", "Physical Target", 360);
            AddColumn("Status", "Status", 85);
            AddColumn("Counts", "Contents", 180);
            tree.FocusedNodeChanged += Tree_FocusedNodeChanged;
            tree.DoubleClick += (s, e) => OpenSelected();
            tree.KeyDown += Tree_KeyDown;
            tree.DragEnter += Tree_DragEnter;
            tree.DragOver += Tree_DragEnter;
            tree.DragDrop += Tree_DragDrop;
            tree.MouseDown += Tree_MouseDown;
            tree.MouseMove += Tree_MouseMove;
            Controls.Add(tree);
            tree.BringToFront();
        }

        private void AddColumn(string field, string caption, int width)
        {
            var column = tree.Columns.Add(); column.FieldName = field; column.Caption = caption;
            column.Visible = true; column.VisibleIndex = tree.Columns.Count - 1; column.Width = width;
        }

        private void ReloadTree(long? focusId = null)
        {
            List<VirtualFolderRecord> folders = database.LoadFolders();
            var counts = database.LoadDirectCounts();
            tree.BeginUpdate();
            try
            {
                tree.Nodes.Clear(); folderNodes.Clear();
                TreeListNode root = tree.AppendNode(new object[] { "Virtual Tree Root", "Root", "", "", "", folders.Count(x => !x.ParentId.HasValue) + " folders" }, null);
                root.Tag = new RootTag();
                AddFolderNodes(folders, counts, null, root, string.Empty);
                root.Expanded = true;
                tree.ExpandAll();
                tree.FocusedNode = focusId.HasValue && folderNodes.ContainsKey(focusId.Value) ? folderNodes[focusId.Value] : root;
            }
            finally { tree.EndUpdate(); }
        }

        private void AddFolderNodes(List<VirtualFolderRecord> folders, Dictionary<long, int[]> counts, long? parentId, TreeListNode parent, string prefix)
        {
            var children = folders.Where(x => x.ParentId == parentId).OrderBy(x => x.SortOrder).ThenBy(x => x.Name).ToList();
            for (int index = 0; index < children.Count; index++)
            {
                VirtualFolderRecord folder = children[index]; string number = prefix + (index + 1) + ".";
                int[] count = counts.ContainsKey(folder.Id) ? counts[folder.Id] : new int[3];
                TreeListNode node = tree.AppendNode(new object[] { number + " " + folder.Name, "Virtual Folder", "", "", "Virtual", count[0] + " subfolders, " + count[1] + " folders, " + count[2] + " files" }, parent);
                node.Tag = folder; folderNodes[folder.Id] = node;
                AddFolderNodes(folders, counts, folder.Id, node, number);
                foreach (VirtualItemRecord item in database.LoadItems(folder.Id))
                {
                    TreeListNode itemNode = tree.AppendNode(new object[] { item.DisplayName, item.IsFolder ? "Physical Folder" : "File", item.SourceFolder, item.TargetPath, item.Exists ? "Available" : "Missing", "" }, node);
                    itemNode.Tag = item;
                }
            }
        }

        private VirtualFolderRecord SelectedFolder()
        {
            TreeListNode node = tree.FocusedNode;
            while (node != null && !(node.Tag is VirtualFolderRecord)) node = node.ParentNode;
            return node == null ? null : node.Tag as VirtualFolderRecord;
        }

        private void AddVirtualFolder()
        {
            VirtualFolderRecord selected = SelectedFolder();
            string name = XtraInputBox.Show("Virtual folder name:", "New Virtual Folder", "New Folder");
            if (string.IsNullOrWhiteSpace(name)) return;
            long id = database.AddFolder(selected == null ? (long?)null : selected.Id, name.Trim());
            applicationDatabases.Log("Create Virtual Folder", name); ReloadTree(id);
        }

        private void RenameSelected()
        {
            var folder = tree.FocusedNode?.Tag as VirtualFolderRecord;
            if (folder == null) return;
            string name = XtraInputBox.Show("Virtual folder name:", "Rename Virtual Folder", folder.Name);
            if (string.IsNullOrWhiteSpace(name)) return;
            database.RenameFolder(folder.Id, name.Trim()); applicationDatabases.Log("Rename Virtual Folder", name); ReloadTree(folder.Id);
        }

        private void RemoveSelected()
        {
            if (tree.FocusedNode?.Tag is VirtualItemRecord item)
            {
                if (XtraMessageBox.Show("Remove this virtual link? The physical item will not be changed.", "Remove Link", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
                database.DeleteItem(item.Id); applicationDatabases.Log("Remove Virtual Link", item.TargetPath); ReloadTree(item.FolderId); return;
            }
            var folder = tree.FocusedNode?.Tag as VirtualFolderRecord;
            if (folder == null || XtraMessageBox.Show("Remove this virtual folder and all links below it? Physical files will not be changed.", "Remove Virtual Folder", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            long? parent = folder.ParentId; database.DeleteFolder(folder.Id); applicationDatabases.Log("Remove Virtual Folder", folder.Name); ReloadTree(parent);
        }

        private void AddFiles()
        {
            var folder = SelectedFolder(); if (folder == null) return;
            using (var dialog = new OpenFileDialog { Multiselect = true, Filter = "All files (*.*)|*.*" })
                if (dialog.ShowDialog(this) == DialogResult.OK) AddPaths(folder.Id, dialog.FileNames);
        }

        private void AddPhysicalFolder()
        {
            var folder = SelectedFolder(); if (folder == null) return;
            using (var dialog = new XtraFolderBrowserDialog()) if (dialog.ShowDialog(this) == DialogResult.OK) AddPaths(folder.Id, new[] { dialog.SelectedPath });
        }

        private void AddPaths(long folderId, IEnumerable<string> paths)
        {
            foreach (string path in paths.Where(x => File.Exists(x) || Directory.Exists(x))) { database.AddItem(folderId, path); applicationDatabases.Log("Add Virtual Link", path); }
            ReloadTree(folderId);
        }

        private void OpenSelected()
        {
            var item = tree.FocusedNode?.Tag as VirtualItemRecord; if (item == null) { tree.FocusedNode.Expanded = !tree.FocusedNode.Expanded; return; }
            if (!item.Exists) { XtraMessageBox.Show("The physical target cannot be found.", "Missing Target"); return; }
            try { Process.Start(new ProcessStartInfo { FileName = item.TargetPath, UseShellExecute = true }); applicationDatabases.Log("Open", item.TargetPath); }
            catch (Exception ex) { applicationDatabases.Log("Open", item.TargetPath, successful: false, error: ex.Message); XtraMessageBox.Show(ex.Message, "Open"); }
        }

        private void ShowPhysicalLocation()
        {
            var item = tree.FocusedNode?.Tag as VirtualItemRecord; if (item == null) return;
            string argument = item.IsFolder ? "\"" + item.TargetPath + "\"" : "/select,\"" + item.TargetPath + "\"";
            Process.Start(new ProcessStartInfo("explorer.exe", argument) { UseShellExecute = true });
        }

        private void Tree_FocusedNodeChanged(object sender, DevExpress.XtraTreeList.FocusedNodeChangedEventArgs e)
        {
            var item = e.Node?.Tag as VirtualItemRecord;
            string previewPath = item != null &&
                item.Exists && !item.IsFolder
                    ? item.TargetPath
                    : string.Empty;
            previewPanelExtension1.SetCurrentPath(previewPath);
        }

        private void Tree_KeyDown(object sender, KeyEventArgs e) { if (e.KeyCode == Keys.Enter) OpenSelected(); else if (e.KeyCode == Keys.Delete) RemoveSelected(); }
        private void Tree_DragEnter(object sender, DragEventArgs e) { e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Link : e.Data.GetDataPresent(typeof(VirtualFolderRecord)) ? DragDropEffects.Move : DragDropEffects.None; }
        private void Tree_DragDrop(object sender, DragEventArgs e)
        {
            Point point = tree.PointToClient(new Point(e.X, e.Y)); var hit = tree.CalcHitInfo(point); var targetFolder = hit.Node?.Tag as VirtualFolderRecord;
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) { if (targetFolder != null) AddPaths(targetFolder.Id, (string[])e.Data.GetData(DataFormats.FileDrop)); return; }
            var moving = e.Data.GetData(typeof(VirtualFolderRecord)) as VirtualFolderRecord; if (moving == null || hit.Node == null) return;
            long? parent; int index;
            if (hit.Node.Tag is RootTag) { parent = null; index = database.LoadFolders().Count(x => !x.ParentId.HasValue); }
            else if (targetFolder != null)
            {
                int third = Math.Max(1, hit.Bounds.Height / 3); bool before = point.Y < hit.Bounds.Top + third; bool after = point.Y > hit.Bounds.Bottom - third;
                if (before || after) { parent = targetFolder.ParentId; var siblings = database.LoadFolders().Where(x => x.ParentId == parent && x.Id != moving.Id).OrderBy(x => x.SortOrder).ToList(); int targetIndex = siblings.FindIndex(x => x.Id == targetFolder.Id); index = Math.Max(0, targetIndex + (after ? 1 : 0)); }
                else { parent = targetFolder.Id; index = database.LoadFolders().Count(x => x.ParentId == parent && x.Id != moving.Id); }
            }
            else return;
            try { database.MoveFolder(moving.Id, parent, index); applicationDatabases.Log("Move Virtual Folder", moving.Name); ReloadTree(moving.Id); }
            catch (Exception ex) { XtraMessageBox.Show(ex.Message, "Move Virtual Folder"); }
        }

        private void Tree_MouseDown(object sender, MouseEventArgs e) { dragStart = e.Location; dragNode = tree.CalcHitInfo(e.Location).Node; }
        private void Tree_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == 0 || dragNode == null) return; Size size = SystemInformation.DragSize;
            if (new Rectangle(dragStart.X - size.Width / 2, dragStart.Y - size.Height / 2, size.Width, size.Height).Contains(e.Location)) return;
            var folder = dragNode.Tag as VirtualFolderRecord; if (folder != null) tree.DoDragDrop(folder, DragDropEffects.Move); dragNode = null;
        }
    }

    internal sealed class RootTag { }
}
