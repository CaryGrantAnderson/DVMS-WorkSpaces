using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DVMSFileManager
{
    internal sealed class SearchResult
    {
        public string Name { get; set; }
        public string Folder { get; set; }
        public string Path { get; set; }
        public string Source { get; set; }
        public long? Size { get; set; }
        public DateTime? Modified { get; set; }
    }

    internal sealed class SearchControl : XtraUserControl
    {
        readonly TextEdit query = new TextEdit();
        readonly CheckEdit everything = new CheckEdit { Text = "Everything" };
        readonly CheckEdit windows = new CheckEdit { Text = "Windows Index" };
        readonly CheckEdit metadata = new CheckEdit { Text = "DVMS Metadata" };
        readonly CheckEdit subdirectories = new CheckEdit { Text = "Search Subdirectories" };
        readonly SimpleButton search = new SimpleButton { Text = "Search" };
        readonly CheckedComboBoxEdit locations = new CheckedComboBoxEdit();
        readonly LabelControl status = new LabelControl();
        readonly GridControl grid = new GridControl();
        readonly ApplicationSettings settings;
        readonly SettingsDatabase settingsDb;
        readonly OperationLogDatabase operationLog;
        readonly string metadataDbPath;

        public SearchControl(ApplicationSettings settings, SettingsDatabase settingsDb, OperationLogDatabase operationLog, string metadataDbPath, string currentFolder)
        {
            this.settings = settings; this.settingsDb = settingsDb; this.operationLog = operationLog; this.metadataDbPath = metadataDbPath;
            var top = new PanelControl { Dock = DockStyle.Top, Height = 76 };
            query.SetBounds(8, 9, 430, 24); everything.SetBounds(450, 9, 100, 24);
            windows.SetBounds(555, 9, 120, 24); metadata.SetBounds(680, 9, 130, 24);
            search.SetBounds(820, 7, 90, 28); status.SetBounds(920, 12, 165, 20);
            var locationLabel = new LabelControl { Text = "Search Locations:" }; locationLabel.SetBounds(8, 49, 105, 20);
            locations.SetBounds(118, 44, 690, 24); locations.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard; locations.Properties.SeparatorChar = ';';
            subdirectories.SetBounds(820, 44, 170, 24);
            foreach (string drive in DriveInfo.GetDrives().Select(d => d.Name)) locations.Properties.Items.Add(drive, false);
            if (!string.IsNullOrWhiteSpace(settings.HomeFolder) && !locations.Properties.Items.Cast<DevExpress.XtraEditors.Controls.CheckedListBoxItem>().Any(i => string.Equals(Convert.ToString(i.Value), settings.HomeFolder, StringComparison.OrdinalIgnoreCase))) locations.Properties.Items.Add(settings.HomeFolder, false);
            top.Controls.AddRange(new Control[] { query, everything, windows, metadata, search, status, locationLabel, locations, subdirectories });
            grid.Dock = DockStyle.Fill; grid.MainView = new GridView(grid); grid.ViewCollection.Add(grid.MainView);
            Controls.Add(grid); Controls.Add(top);
            everything.Checked = settings.SearchEverything; windows.Checked = settings.SearchWindowsIndex; metadata.Checked = settings.SearchDvmsMetadata;
            subdirectories.Checked = settings.SearchSubdirectories;
            search.Click += async (s, e) => await RunSearch(); query.KeyDown += async (s, e) => { if (e.KeyCode == Keys.Enter) await RunSearch(); };
            SetLocation(currentFolder);
        }

        public void SetLocation(string path)
        {
            locations.SetEditValue(path ?? string.Empty);
            if (!string.IsNullOrWhiteSpace(path)) locations.Text = path;
        }

        async Task RunSearch()
        {
            string text = query.Text.Trim(); if (text.Length == 0) return;
            settings.SearchEverything = everything.Checked; settings.SearchWindowsIndex = windows.Checked; settings.SearchDvmsMetadata = metadata.Checked; settings.SearchSubdirectories = subdirectories.Checked; settingsDb.Save(settings);
            search.Enabled = false; UseWaitCursor = true; Cursor.Current = Cursors.WaitCursor; status.Text = "Searching…"; var watch = Stopwatch.StartNew();
            try
            {
                var tasks = new List<Task<List<SearchResult>>>();
                if (everything.Checked) tasks.Add(Task.Run(() => SearchEverything(text)));
                if (windows.Checked) tasks.Add(Task.Run(() => SearchWindows(text)));
                if (metadata.Checked) tasks.Add(Task.Run(() => SearchMetadata(text)));
                var sets = await Task.WhenAll(tasks);
                var merged = sets.SelectMany(x => x).Where(InSelectedLocation).GroupBy(x => x.Path, StringComparer.OrdinalIgnoreCase)
                    .Select(g => { var r = g.First(); r.Source = string.Join(" + ", g.Select(x => x.Source).Distinct()); return r; }).Take(1000).ToList();
                grid.DataSource = merged; watch.Stop(); status.Text = merged.Count + " results · " + watch.Elapsed.TotalSeconds.ToString("0.00") + "s";
                operationLog?.Log(
                    "Search",
                    locations.Text,
                    details: "Query=" + text + "; Subdirectories=" +
                        subdirectories.Checked + "; Results=" + merged.Count +
                        "; Providers=" + string.Join(", ", new[] {
                            everything.Checked ? "Everything" : null,
                            windows.Checked ? "Windows Index" : null,
                            metadata.Checked ? "DVMS Metadata" : null
                        }.Where(x => x != null)));
            }
            catch (Exception exception)
            {
                operationLog?.Log(
                    "Search", locations.Text,
                    details: "Query=" + text,
                    successful: false,
                    errorMessage: exception.Message);
                status.Text = "Search failed";
                XtraMessageBox.Show(this, exception.Message, "Search", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                search.Enabled = true; UseWaitCursor = false; Cursor.Current = Cursors.Default;
            }
        }

        bool InSelectedLocation(SearchResult result)
        {
            var selected = GetSelectedLocations(); if (selected.Count == 0) return true;
            string folder = Path.GetDirectoryName(result.Path) ?? string.Empty;
            return selected.Any(path => subdirectories.Checked
                ? result.Path.StartsWith(path.TrimEnd('\\') + "\\", StringComparison.OrdinalIgnoreCase) || string.Equals(result.Path, path, StringComparison.OrdinalIgnoreCase)
                : string.Equals(folder.TrimEnd('\\'), path.TrimEnd('\\'), StringComparison.OrdinalIgnoreCase));
        }

        List<string> GetSelectedLocations()
        {
            return locations.Text.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim().Trim('"').TrimEnd('\\'))
                .Where(x => x.Length > 0).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        }

        SearchResult CreateResult(string name, string path, string source, long? size = null, DateTime? modified = null)
        {
            return new SearchResult { Name = name, Folder = Path.GetDirectoryName(path), Path = path, Source = source, Size = size, Modified = modified };
        }

        List<SearchResult> SearchEverything(string text)
        {
            string exe = FindEs(); if (exe == null) return new List<SearchResult>();
            string scope = string.Join(" | ", GetSelectedLocations().Select(path => subdirectories.Checked ? "path:\"" + path.Replace("\"", "") + "\"" : "parent:\"" + path.Replace("\"", "") + "\""));
            string expression = (scope.Length == 0 ? string.Empty : "<" + scope + "> ") + text.Replace("\"", "");
            var p = new Process { StartInfo = new ProcessStartInfo(exe, "-n 2000 -timeout 4000 -full-path-and-name \"" + expression + "\"") { UseShellExecute = false, RedirectStandardOutput = true, CreateNoWindow = true } };
            p.Start(); string output = p.StandardOutput.ReadToEnd(); p.WaitForExit(5000);
            return output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Select(path => CreateResult(Path.GetFileName(path), path, "Everything")).ToList();
        }

        string FindEs()
        {
            string[] paths = { settings.EverythingCliPath, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "es.exe"), Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Everything", "es.exe"), Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Everything", "es.exe") };
            return paths.FirstOrDefault(File.Exists);
        }

        List<SearchResult> SearchWindows(string text)
        {
            var list = new List<SearchResult>();
            try { using (var c = new OleDbConnection("Provider=Search.CollatorDSO.1;Extended Properties='Application=Windows';")) { c.Open(); string safe = text.Replace("'", "''").Replace("%", "[%]").Replace("*", "%").Replace("?", "_"); if (!safe.Contains("%") && !safe.Contains("_")) safe = "%" + safe + "%"; var selected = GetSelectedLocations(); string scope = selected.Count == 0 ? string.Empty : " AND (" + string.Join(" OR ", selected.Select(path => (subdirectories.Checked ? "SCOPE='file:" : "DIRECTORY='file:") + path.Replace("\\", "/").Replace("'", "''") + "'")) + ")"; using (var cmd = new OleDbCommand("SELECT TOP 2000 System.ItemNameDisplay,System.ItemPathDisplay,System.Size,System.DateModified FROM SystemIndex WHERE System.ItemNameDisplay LIKE '" + safe + "'" + scope, c)) using (var r = cmd.ExecuteReader()) while (r.Read()) { string path = Convert.ToString(r[1]); list.Add(CreateResult(Convert.ToString(r[0]), path, "Windows Index", r.IsDBNull(2) ? (long?)null : Convert.ToInt64(r[2]), r.IsDBNull(3) ? (DateTime?)null : Convert.ToDateTime(r[3]))); } } } catch { }
            return list;
        }

        List<SearchResult> SearchMetadata(string text)
        {
            var list = new List<SearchResult>(); string like = text.Replace("%", "[%]").Replace("*", "%").Replace("?", "_"); if (!like.Contains("%") && !like.Contains("_")) like = "%" + like + "%";
            try { using (var c = new SQLiteConnection("Data Source=" + metadataDbPath + ";Version=3;")) { c.Open(); using (var cmd = c.CreateCommand()) { cmd.CommandText = @"SELECT DISTINCT f.FileName,f.FullPath FROM Files f LEFT JOIN FileMetadata m ON m.FileKey=f.FileKey LEFT JOIN FileTags ft ON ft.FileKey=f.FileKey LEFT JOIN Tags t ON t.TagKey=ft.TagKey LEFT JOIN FileNotes n ON n.FileKey=f.FileKey WHERE f.FileName LIKE @q OR m.DisplayName LIKE @q OR m.Description LIKE @q OR m.Status LIKE @q OR m.Workspace LIKE @q OR t.TagName LIKE @q OR n.Title LIKE @q OR n.NoteText LIKE @q LIMIT 2000"; cmd.Parameters.AddWithValue("@q", like); using (var r = cmd.ExecuteReader()) while (r.Read()) { string path = r.GetString(1); list.Add(CreateResult(r.GetString(0), path, "DVMS Metadata")); } } } } catch { }
            return list;
        }
    }
}
