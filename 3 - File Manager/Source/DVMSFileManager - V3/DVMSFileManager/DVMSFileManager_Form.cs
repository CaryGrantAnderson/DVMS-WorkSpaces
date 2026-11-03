using DevExpress.Dialogs.Core.View;
using DevExpress.Dialogs.Core.ViewModel;
using DevExpress.XtraDialogs.FileExplorerExtensions;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Reflection.Emit;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32.SafeHandles;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;

namespace DVMSFileManager
{
    public partial class DVMSFileManagerForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        private const string SearchThisFolderCommandName =
            "dvms-search-this-folder";
        private const int MaximumNavigationHistoryItems = 50;
        private const int MaximumRecentFolderItems = 20;
        private readonly List<string> fileNavigationHistory =
            new List<string>();
        private readonly List<string> recentFolders =
            new List<string>();
        private PopupMenu backHistoryPopupMenu;
        private PopupMenu recentFoldersPopupMenu;
        private string homeFolderPath;
        private SettingsDatabase settingsDatabase;
        private OperationLogDatabase operationLogDatabase;
        private LogViewerForm logViewerForm;
        private DevExpress.XtraBars.Docking.DockPanel wiwaDockPanel;
        private WiwaControl wiwaControl;
        private ApplicationSettings applicationSettings;
        private string defaultThemeName;
        private string defaultPaletteName;
        private DevExpress.XtraBars.Docking.DockPanel searchDockPanel;
        private SearchControl searchControl;
        private string contextMenuSearchFolderPath;
        private BarButtonItem ribbonBackButton;
        private BarButtonItem ribbonForwardButton;
        private BarButtonItem ribbonUpButton;
        private BarButtonItem ribbonHomeButton;
        private BarSubItem ribbonRecentFoldersMenu;
        private readonly List<BarItem> ribbonRecentFolderItems =
            new List<BarItem>();

        public DVMSFileManagerForm()
        {
            InitializeComponent();
            // Build the ribbon during construction so Visual Studio's form
            // designer displays the pages, buttons, and SVG icons. The
            // designer creates the form but does not raise its Load event.
            InitializeFileManagerRibbon();
        }


        private void DVMSFileManagerForm_Load(object sender, EventArgs e)
        {
            InitializeApplicationSettings();
            InitializeOperationLogging();
            InitializecheckedComboBoxEdit_FoldersView();

            InitializeAppsTree();

            InitializeFilesViewSelector();
            InitializeFileNavigation();
            ApplyFilesView(applicationSettings.DefaultView);
            InitializeMetadata();
            InitializeFileProperties();
            //ApplyFilesView(ViewMode.MediumIcon);
            //ApplyFilesView(ViewMode.LargeIcon);

            if (applicationSettings.RestoreLastFolder &&
                Directory.Exists(applicationSettings.LastFolder))
            {
                NavigateToFolder(applicationSettings.LastFolder);
            }


        }

        private void InitializeApplicationSettings()
        {
            var lookAndFeel =
                DevExpress.LookAndFeel.UserLookAndFeel.Default;
            defaultThemeName = lookAndFeel.ActiveSkinName;
            defaultPaletteName = lookAndFeel.ActiveSvgPaletteName;

            settingsDatabase = new SettingsDatabase();
            settingsDatabase.Initialize();
            applicationSettings = settingsDatabase.Load(
                gridControlExtension1.CurrentPath);

            if (string.IsNullOrWhiteSpace(applicationSettings.ThemeName))
            {
                applicationSettings.ThemeName = defaultThemeName;
                applicationSettings.PaletteName = defaultPaletteName;
                settingsDatabase.Save(applicationSettings);
            }

            checkEdit_ShowDescriptionPreview.Checked =
                applicationSettings.ShowDescriptions;
            barCheck_ViewPreviewColumn.Checked =
                applicationSettings.ShowPreviewColumn;

            folderSizeScanLimit.Dispose();
            folderSizeScanLimit = new SemaphoreSlim(
                applicationSettings.FolderSizeScanConcurrency,
                applicationSettings.FolderSizeScanConcurrency);

            if (!string.IsNullOrWhiteSpace(applicationSettings.ThemeName))
            {
                try
                {
                    ApplySavedTheme(applicationSettings);
                }
                catch (Exception)
                {
                    // Retain the current theme if a saved skin is no longer
                    // installed in this DevExpress version.
                }
            }

            DevExpress.LookAndFeel.UserLookAndFeel.Default.StyleChanged +=
                UserLookAndFeel_StyleChanged;
        }

        private void InitializeOperationLogging()
        {
            operationLogDatabase = new OperationLogDatabase();
            operationLogDatabase.Initialize();
            operationLogDatabase.Log(
                "Application Started",
                details: Application.ExecutablePath);
        }

        private void ShowSettingsWindow()
        {
            using (var form = new SettingsForm(
                applicationSettings,
                settingsDatabase.DatabasePath,
                gridControlExtension1.CurrentPath,
                defaultThemeName,
                defaultPaletteName))
            {
                form.SettingsApplied += (sender, e) =>
                    ApplyAndSaveSettings(form.Settings);
                form.ShowDialog(this);
            }
        }

        private void ApplyAndSaveSettings(ApplicationSettings settings)
        {
            int previousConcurrency =
                applicationSettings.FolderSizeScanConcurrency;
            applicationSettings = settings.Clone();
            applicationSettings.Validate();
            settingsDatabase.Save(applicationSettings);

            homeFolderPath = applicationSettings.HomeFolder;
            checkEdit_ShowDescriptionPreview.Checked =
                applicationSettings.ShowDescriptions;
            barCheck_ViewPreviewColumn.Checked =
                applicationSettings.ShowPreviewColumn;

            if (recentFolders.Count >
                applicationSettings.MaximumRecentFolders)
            {
                recentFolders.RemoveRange(
                    applicationSettings.MaximumRecentFolders,
                    recentFolders.Count -
                    applicationSettings.MaximumRecentFolders);
                RebuildNavigationMenus();
            }

            if (previousConcurrency !=
                applicationSettings.FolderSizeScanConcurrency)
            {
                CancelFolderSizeCalculations();
                folderSizeScanLimit = new SemaphoreSlim(
                    applicationSettings.FolderSizeScanConcurrency,
                    applicationSettings.FolderSizeScanConcurrency);
            }

            if (!applicationSettings.CalculateFolderSizes)
                InvalidateFolderSizeCache();
            else
                ScheduleDetailsViewColumnConfiguration();

            if (!string.IsNullOrWhiteSpace(applicationSettings.ThemeName))
            {
                try
                {
                    ApplySavedTheme(applicationSettings);
                }
                catch (Exception exception)
                {
                    XtraMessageBox.Show(
                        this,
                        "The selected theme could not be applied.\r\n\r\n" +
                        exception.Message,
                        "Settings",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }

            UpdateFileNavigationButtons();
            gridControl_Files.RefreshDataSource();
        }

        private static void ApplySavedTheme(ApplicationSettings settings)
        {
            if (string.IsNullOrWhiteSpace(settings.PaletteName))
            {
                DevExpress.LookAndFeel.UserLookAndFeel.Default
                    .SetSkinStyle(settings.ThemeName);
            }
            else
            {
                DevExpress.LookAndFeel.UserLookAndFeel.Default
                    .SetSkinStyle(
                        settings.ThemeName,
                        settings.PaletteName);
            }
        }

        private void UserLookAndFeel_StyleChanged(
            object sender,
            EventArgs e)
        {
            if (applicationSettings == null || settingsDatabase == null)
                return;

            var lookAndFeel =
                DevExpress.LookAndFeel.UserLookAndFeel.Default;
            applicationSettings.ThemeName = lookAndFeel.ActiveSkinName;
            applicationSettings.PaletteName =
                lookAndFeel.ActiveSvgPaletteName;
            settingsDatabase.Save(applicationSettings);
        }

        #region File Navigation

        private void InitializeFileNavigation()
        {
            homeFolderPath = applicationSettings == null
                ? gridControlExtension1.CurrentPath
                : applicationSettings.HomeFolder;

            backHistoryPopupMenu = new PopupMenu(components)
            {
                Ribbon = ribbonControl1
            };
            recentFoldersPopupMenu = new PopupMenu(components)
            {
                Ribbon = ribbonControl1
            };

            simpleButton_FileBack.DropDownControl =
                backHistoryPopupMenu;
            dropDownButton_RecentFolders.DropDownControl =
                recentFoldersPopupMenu;
            dropDownButton_RecentFolders.ActAsDropDown = true;

            simpleButton_FileBack.Click += (sender, e) =>
                NavigateFilesBack();
            simpleButton_FileForward.Click += (sender, e) =>
                NavigateFilesForward();
            simpleButton_FileUp.Click += (sender, e) =>
                NavigateFilesUp();
            simpleButton_FileHome.Click += (sender, e) =>
                NavigateToFolder(homeFolderPath);
            simpleButton_FileRefresh.Click += (sender, e) =>
                RefreshCurrentFolder();

            gridControlExtension1.AllowGoBackChanged +=
                (sender, e) => UpdateFileNavigationButtons();
            gridControlExtension1.AllowGoForwardChanged +=
                (sender, e) => UpdateFileNavigationButtons();
            gridControlExtension1.AllowGoUpChanged +=
                (sender, e) => UpdateFileNavigationButtons();

            gridControl_Files.MouseDown += gridControl_Files_MouseDown;
            gridControlExtension1.ContextMenuShowing +=
                gridControlExtension1_ContextMenuShowing;
            gridControlExtension1.BeforeExecuteItemCommand +=
                gridControlExtension1_BeforeExecuteItemCommand;
            RecordVisitedFolder(gridControlExtension1.CurrentPath);
            UpdateFileNavigationButtons();
        }

        private void UpdateFileNavigationButtons()
        {
            simpleButton_FileBack.Enabled =
                gridControlExtension1.CanGoBack ||
                GetBackHistoryFolders().Any();
            simpleButton_FileForward.Enabled = gridControlExtension1.CanGoForward;
            simpleButton_FileUp.Enabled = gridControlExtension1.CanGoUp;
            simpleButton_FileHome.Enabled =
                !string.IsNullOrWhiteSpace(homeFolderPath) &&
                !PathsEqual(
                    gridControlExtension1.CurrentPath,
                    homeFolderPath);

            if (ribbonBackButton != null)
                ribbonBackButton.Enabled = gridControlExtension1.CanGoBack;
            if (ribbonForwardButton != null)
                ribbonForwardButton.Enabled = gridControlExtension1.CanGoForward;
            if (ribbonUpButton != null)
                ribbonUpButton.Enabled = gridControlExtension1.CanGoUp;
            if (ribbonHomeButton != null)
                ribbonHomeButton.Enabled = simpleButton_FileHome.Enabled;
        }

        private void RecordVisitedFolder(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return;

            if (fileNavigationHistory.Count == 0 ||
                !PathsEqual(fileNavigationHistory.Last(), path))
            {
                fileNavigationHistory.Add(path);
                if (fileNavigationHistory.Count >
                    MaximumNavigationHistoryItems)
                {
                    fileNavigationHistory.RemoveAt(0);
                }
            }

            recentFolders.RemoveAll(item => PathsEqual(item, path));
            recentFolders.Insert(0, path);
            int maximumRecentFolders = applicationSettings == null
                ? MaximumRecentFolderItems
                : applicationSettings.MaximumRecentFolders;
            if (recentFolders.Count > maximumRecentFolders)
                recentFolders.RemoveRange(
                    maximumRecentFolders,
                    recentFolders.Count - maximumRecentFolders);

            RebuildNavigationMenus();
            UpdateFileNavigationButtons();
        }

        private IEnumerable<string> GetBackHistoryFolders()
        {
            string currentPath = gridControlExtension1.CurrentPath;
            var returnedPaths = new HashSet<string>(
                StringComparer.OrdinalIgnoreCase);

            for (int index = fileNavigationHistory.Count - 1;
                 index >= 0;
                 index--)
            {
                string path = fileNavigationHistory[index];
                if (!PathsEqual(path, currentPath) &&
                    returnedPaths.Add(path))
                {
                    yield return path;
                }
            }
        }

        private void RebuildNavigationMenus()
        {
            if (backHistoryPopupMenu == null ||
                recentFoldersPopupMenu == null)
            {
                return;
            }

            PopulateFolderMenu(
                backHistoryPopupMenu,
                GetBackHistoryFolders(),
                "No back history");
            PopulateFolderMenu(
                recentFoldersPopupMenu,
                recentFolders,
                "No recently visited folders");
            RebuildRibbonRecentFoldersMenu();
        }

        private void PopulateFolderMenu(
            PopupMenu popupMenu,
            IEnumerable<string> paths,
            string emptyCaption)
        {
            popupMenu.ClearLinks();
            List<string> menuPaths = paths.ToList();

            if (menuPaths.Count == 0)
            {
                popupMenu.AddItem(new BarButtonItem
                {
                    Caption = emptyCaption,
                    Enabled = false
                });
                return;
            }

            foreach (string path in menuPaths)
            {
                var item = new BarButtonItem
                {
                    Caption = GetFolderMenuCaption(path),
                    Hint = path
                };
                string selectedPath = path;
                item.ItemClick += (sender, e) =>
                    NavigateToFolder(selectedPath);
                popupMenu.AddItem(item);
            }
        }

        private static string GetFolderMenuCaption(string path)
        {
            string trimmedPath = path.TrimEnd(
                Path.DirectorySeparatorChar,
                Path.AltDirectorySeparatorChar);
            string folderName = Path.GetFileName(trimmedPath);

            return string.IsNullOrWhiteSpace(folderName)
                ? path
                : folderName + "    " + path;
        }

        private void NavigateToFolder(string path)
        {
            if (string.IsNullOrWhiteSpace(path) ||
                PathsEqual(gridControlExtension1.CurrentPath, path))
            {
                return;
            }

            try
            {
                gridControlExtension1.SetCurrentPath(path);
            }
            catch (Exception exception)
            {
                operationLogDatabase?.Log(
                    "Navigate Folder", path, successful: false,
                    errorMessage: exception.Message);
                XtraMessageBox.Show(
                    this,
                    "DVMS could not open the selected folder.\r\n\r\n" +
                    exception.Message,
                    "Open Folder",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private static bool PathsEqual(string first, string second)
        {
            return string.Equals(
                first?.TrimEnd('\\', '/'),
                second?.TrimEnd('\\', '/'),
                StringComparison.OrdinalIgnoreCase);
        }

        private void NavigateFilesBack()
        {
            if (gridControlExtension1.CanGoBack)
                gridControlExtension1.GoBack();
        }

        private void NavigateFilesForward()
        {
            if (gridControlExtension1.CanGoForward)
                gridControlExtension1.GoForward();
        }

        private void NavigateFilesUp()
        {
            if (gridControlExtension1.CanGoUp)
                gridControlExtension1.GoUp();
        }

        private void RefreshCurrentFolder()
        {
            InvalidateFolderSizeCache();
            gridControlExtension1.RefreshData();
            ScheduleDetailsViewColumnConfiguration();
        }

        private void gridControl_Files_MouseDown(
            object sender,
            MouseEventArgs e)
        {
            if (e.Button == MouseButtons.XButton1)
                NavigateFilesBack();
            else if (e.Button == MouseButtons.XButton2)
                NavigateFilesForward();
        }

        protected override bool ProcessCmdKey(
            ref Message msg,
            Keys keyData)
        {
            if (keyData == (Keys.Alt | Keys.Left))
            {
                NavigateFilesBack();
                return true;
            }

            if (keyData == (Keys.Alt | Keys.Right))
            {
                NavigateFilesForward();
                return true;
            }

            if (keyData == (Keys.Alt | Keys.Up) ||
                (keyData == Keys.Back &&
                 gridControl_Files.ContainsFocus))
            {
                NavigateFilesUp();
                return true;
            }

            if (keyData == Keys.F5)
            {
                RefreshCurrentFolder();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void InitializeFileManagerRibbon()
        {
            if (ribbonPage_Home != null)
            {
                InitializeSerializedRibbonCommands();
                return;
            }

            ribbonControl1.Pages.Clear();

            RibbonPage homePage = CreateRibbonPage("Home");
            RibbonPage navigatePage = CreateRibbonPage("Navigate");
            RibbonPage viewPage = CreateRibbonPage("View");
            RibbonPage metadataPage = CreateRibbonPage("Metadata");
            RibbonPage toolsPage = CreateRibbonPage("Tools");
            RibbonPage debugPage = CreateRibbonPage("Debug");

            RibbonPageGroup homeActions =
                CreateRibbonGroup(homePage, "Files");
            AddRibbonButton(
                homeActions,
                "Refresh",
                "svgimages/icon%20builder/actions_refresh.svg",
                (sender, e) => RefreshCurrentFolder(),
                true);
            AddRibbonButton(
                homeActions,
                "Properties",
                "svgimages/setup/properties.svg",
                (sender, e) => ShowDockPanel(dockPanel_FileProperties),
                true);
            AddRibbonButton(
                homeActions,
                "Notes",
                "svgimages/business%20objects/bo_note.svg",
                (sender, e) => EditFocusedFileNotes(),
                true);

            RibbonPageGroup applicationGroup =
                CreateRibbonGroup(homePage, "Application");
            applicationGroup.Alignment = RibbonPageGroupAlignment.Far;
            AddRibbonButton(
                applicationGroup,
                "Close",
                "svgimages/xaf/action_exit.svg",
                (sender, e) => Close(),
                true);

            RibbonPageGroup navigationGroup =
                CreateRibbonGroup(navigatePage, "Navigation");
            ribbonBackButton = AddRibbonButton(
                navigationGroup,
                "Back",
                "svgimages/navigation/backward.svg",
                (sender, e) => NavigateFilesBack(),
                true);
            ribbonForwardButton = AddRibbonButton(
                navigationGroup,
                "Forward",
                "svgimages/navigation/forward.svg",
                (sender, e) => NavigateFilesForward(),
                true);
            ribbonUpButton = AddRibbonButton(
                navigationGroup,
                "Up",
                "svgimages/actions/up.svg",
                (sender, e) => NavigateFilesUp(),
                true);
            ribbonHomeButton = AddRibbonButton(
                navigationGroup,
                "Home",
                "svgimages/icon%20builder/actions_home.svg",
                (sender, e) => NavigateToFolder(homeFolderPath),
                true);

            ribbonRecentFoldersMenu = new BarSubItem
            {
                Caption = "Recent Folders"
            };
            SetRibbonSvgImage(
                ribbonRecentFoldersMenu,
                "svgimages/icon%20builder/actions_folderopen.svg");
            ribbonControl1.Items.Add(ribbonRecentFoldersMenu);
            navigationGroup.ItemLinks.Add(ribbonRecentFoldersMenu);
            RebuildRibbonRecentFoldersMenu();

            RibbonPageGroup viewModes =
                CreateRibbonGroup(viewPage, "Display");
            AddViewModeRibbonButton(
                viewModes, "Extra Large", ViewMode.ExtraLargeIcon);
            AddViewModeRibbonButton(
                viewModes, "Large Icons", ViewMode.LargeIcon);
            AddViewModeRibbonButton(
                viewModes, "Medium Icons", ViewMode.MediumIcon);
            AddViewModeRibbonButton(
                viewModes, "Small Icons", ViewMode.SmallIcon);
            AddViewModeRibbonButton(
                viewModes, "List", ViewMode.List);
            AddViewModeRibbonButton(
                viewModes, "Details", ViewMode.Details);
            AddViewModeRibbonButton(
                viewModes, "Tiles", ViewMode.Tiles);
            AddViewModeRibbonButton(
                viewModes, "Content", ViewMode.Content);

            RibbonPageGroup viewOptions =
                CreateRibbonGroup(viewPage, "Options");
            AddRibbonCheckButton(
                viewOptions,
                "Descriptions",
                "svgimages/richedit/preview.svg",
                checkEdit_ShowDescriptionPreview.Checked,
                item => checkEdit_ShowDescriptionPreview.Checked =
                    item.Checked);
            AddDockPanelRibbonButton(
                viewOptions,
                "Preview Panel",
                "svgimages/print/preview.svg",
                dockPanel_Preview);
            AddDockPanelRibbonButton(
                viewOptions,
                "Metadata Panel",
                "svgimages/icon%20builder/actions_database.svg",
                dockPanel_MetaData);
            AddDockPanelRibbonButton(
                viewOptions,
                "Properties Panel",
                "svgimages/setup/properties.svg",
                dockPanel_FileProperties);

            RibbonPageGroup metadataEditing =
                CreateRibbonGroup(metadataPage, "File Metadata");
            AddRibbonButton(
                metadataEditing,
                "Show Metadata",
                "svgimages/icon%20builder/actions_database.svg",
                (sender, e) => ShowDockPanel(dockPanel_MetaData),
                true);
            AddRibbonButton(
                metadataEditing,
                "Edit Notes",
                "svgimages/business%20objects/bo_note.svg",
                (sender, e) => EditFocusedFileNotes(),
                true);
            AddRibbonButton(
                metadataEditing,
                "File Properties",
                "svgimages/setup/properties.svg",
                (sender, e) => ShowDockPanel(dockPanel_FileProperties),
                true);
            AddRibbonButton(
                metadataEditing,
                "Preview",
                "svgimages/richedit/preview.svg",
                (sender, e) => ShowDockPanel(dockPanel_Preview),
                true);

            RibbonPageGroup databaseGroup =
                CreateRibbonGroup(toolsPage, "Database");
            AddRibbonButton(
                databaseGroup,
                "Open DB Folder",
                "svgimages/icon%20builder/actions_database.svg",
                (sender, e) => OpenMetadataDatabaseFolder(),
                true);
            AddRibbonButton(
                databaseGroup,
                "Log Viewer",
                "svgimages/spreadsheet/pivottable.svg",
                (sender, e) => ShowLogViewer(),
                true);
            AddRibbonButton(
                databaseGroup,
                "Open Log Folder",
                "svgimages/icon%20builder/actions_folderopen.svg",
                (sender, e) => OpenLogFolder(),
                true);
            AddRibbonButton(
                databaseGroup,
                "WIWA Timeline",
                "svgimages/chart/charttype_line.svg",
                (sender, e) => ShowWiwaTimeline(),
                true);

            RibbonPageGroup diagnosticsGroup =
                CreateRibbonGroup(debugPage, "Special Folders");
            SetRibbonSvgImage(
                barButtonItem_DisplayCurrentSpecialFolders,
                "svgimages/business%20objects/bo_folder.svg");
            SetRibbonSvgImage(
                barButtonItem_DisplayAllSpecialFolders,
                "svgimages/icon%20builder/actions_folderopen.svg");
            SetRibbonSvgImage(
                barButtonItem_GetExistingSpecialFolders,
                "svgimages/xaf/action_search.svg");
            diagnosticsGroup.ItemLinks.Add(
                barButtonItem_DisplayCurrentSpecialFolders);
            diagnosticsGroup.ItemLinks.Add(
                barButtonItem_DisplayAllSpecialFolders);
            diagnosticsGroup.ItemLinks.Add(
                barButtonItem_GetExistingSpecialFolders);

            ribbonControl1.SelectedPage = homePage;
            UpdateFileNavigationButtons();
        }

        private void InitializeSerializedRibbonCommands()
        {
            ribbonBackButton = barButton_NavBack;
            ribbonForwardButton = barButton_NavForward;
            ribbonUpButton = barButton_NavUp;
            ribbonHomeButton = barButton_NavHome;
            ribbonRecentFoldersMenu = barSubItem_NavRecent;

            barButton_HomeRefresh.ItemClick +=
                (sender, e) => RefreshCurrentFolder();
            barButton_HomeProperties.ItemClick +=
                (sender, e) => ShowDockPanel(dockPanel_FileProperties);
            barButton_HomeNotes.ItemClick +=
                (sender, e) => EditFocusedFileNotes();
            barButton_AppClose.ItemClick +=
                (sender, e) => Close();

            barButton_NavBack.ItemClick +=
                (sender, e) => NavigateFilesBack();
            barButton_NavForward.ItemClick +=
                (sender, e) => NavigateFilesForward();
            barButton_NavUp.ItemClick +=
                (sender, e) => NavigateFilesUp();
            barButton_NavHome.ItemClick +=
                (sender, e) => NavigateToFolder(homeFolderPath);

            WireViewModeButton(
                barButton_ViewExtraLarge, ViewMode.ExtraLargeIcon);
            WireViewModeButton(
                barButton_ViewLarge, ViewMode.LargeIcon);
            WireViewModeButton(
                barButton_ViewMedium, ViewMode.MediumIcon);
            WireViewModeButton(
                barButton_ViewSmall, ViewMode.SmallIcon);
            WireViewModeButton(barButton_ViewList, ViewMode.List);
            WireViewModeButton(barButton_ViewDetails, ViewMode.Details);
            WireViewModeButton(barButton_ViewTiles, ViewMode.Tiles);
            WireViewModeButton(barButton_ViewContent, ViewMode.Content);

            barCheck_ViewDescriptions.Checked =
                checkEdit_ShowDescriptionPreview.Checked;
            barCheck_ViewDescriptions.ItemClick += (sender, e) =>
                checkEdit_ShowDescriptionPreview.Checked =
                    barCheck_ViewDescriptions.Checked;
            barCheck_ViewPreviewColumn.Checked =
                applicationSettings != null &&
                applicationSettings.ShowPreviewColumn;
            barCheck_ViewPreviewColumn.ItemClick += (sender, e) =>
            {
                if (applicationSettings == null)
                    return;

                applicationSettings.ShowPreviewColumn =
                    barCheck_ViewPreviewColumn.Checked;
                settingsDatabase.Save(applicationSettings);
                ScheduleDetailsViewColumnConfiguration();
            };

            WireDockPanelCheckItem(
                barCheck_ViewPreview, dockPanel_Preview);
            WireDockPanelCheckItem(
                barCheck_ViewMetadata, dockPanel_MetaData);
            WireDockPanelCheckItem(
                barCheck_ViewProperties, dockPanel_FileProperties);

            barButton_MetadataShow.ItemClick +=
                (sender, e) => ShowDockPanel(dockPanel_MetaData);
            barButton_MetadataNotes.ItemClick +=
                (sender, e) => EditFocusedFileNotes();
            barButton_MetadataProperties.ItemClick +=
                (sender, e) => ShowDockPanel(dockPanel_FileProperties);
            barButton_MetadataPreview.ItemClick +=
                (sender, e) => ShowDockPanel(dockPanel_Preview);
            barButton_ToolsDatabaseFolder.ItemClick +=
                (sender, e) => OpenMetadataDatabaseFolder();
            barButton_ToolsSettings.ItemClick +=
                (sender, e) => ShowSettingsWindow();
            barButton_ToolsLogViewer.ItemClick +=
                (sender, e) => ShowLogViewer();
            barButton_ToolsLogFolder.ItemClick +=
                (sender, e) => OpenLogFolder();
            barButton_ToolsWiwa.ItemClick +=
                (sender, e) => ShowWiwaTimeline();
            barButton_Search.ItemClick +=
                (sender, e) => ShowSearchWindow();
            barButton_DefaultTheme.ItemClick +=
                (sender, e) => RestoreDefaultTheme();

            SetRibbonSvgImage(
                barButton_ToolsDatabaseFolder,
                "svgimages/icon%20builder/actions_database.svg");
            SetRibbonSvgImage(
                barButton_ToolsLogViewer,
                "svgimages/spreadsheet/pivottable.svg");
            SetRibbonSvgImage(
                barButton_ToolsLogFolder,
                "svgimages/icon%20builder/actions_folderopen.svg");
            SetRibbonSvgImage(
                barButton_ToolsWiwa,
                "svgimages/chart/charttype_line.svg");
            SetRibbonSvgImage(
                barButton_MetadataShow,
                "svgimages/icon%20builder/actions_database.svg");
            SetRibbonSvgImage(
                barButton_ViewTiles,
                "svgimages/dashboards/tilelabels.svg");
            SetRibbonSvgImage(
                barCheck_ViewMetadata,
                "svgimages/icon%20builder/actions_database.svg");
            SetRibbonSvgImage(
                barButton_Search,
                "svgimages/xaf/action_search.svg");

            RebuildRibbonRecentFoldersMenu();
            ribbonControl1.SelectedPage = ribbonPage_Home;
            UpdateFileNavigationButtons();
        }

        private void WireViewModeButton(
            BarButtonItem button,
            ViewMode viewMode)
        {
            button.ItemClick += (sender, e) => ApplyFilesView(viewMode);
        }

        private void WireDockPanelCheckItem(
            BarCheckItem item,
            DevExpress.XtraBars.Docking.DockPanel dockPanel)
        {
            item.Checked = dockPanel.Visibility ==
                DevExpress.XtraBars.Docking.DockVisibility.Visible;
            item.ItemClick += (sender, e) =>
            {
                if (item.Checked)
                    ShowDockPanel(dockPanel);
                else
                    dockPanel.Hide();
            };
        }

        private RibbonPage CreateRibbonPage(string caption)
        {
            var page = new RibbonPage(caption);
            ribbonControl1.Pages.Add(page);
            return page;
        }

        private static RibbonPageGroup CreateRibbonGroup(
            RibbonPage page,
            string caption)
        {
            var group = new RibbonPageGroup(caption);
            page.Groups.Add(group);
            return group;
        }

        private BarButtonItem AddRibbonButton(
            RibbonPageGroup group,
            string caption,
            string svgResourceName,
            ItemClickEventHandler clickHandler,
            bool large = false)
        {
            var button = new BarButtonItem
            {
                Caption = caption,
                RibbonStyle = large
                    ? RibbonItemStyles.Large
                    : RibbonItemStyles.SmallWithText
            };
            SetRibbonSvgImage(button, svgResourceName);
            button.ItemClick += clickHandler;
            ribbonControl1.Items.Add(button);
            group.ItemLinks.Add(button);
            return button;
        }

        private void AddViewModeRibbonButton(
            RibbonPageGroup group,
            string caption,
            ViewMode viewMode)
        {
            string icon = viewMode == ViewMode.Details ||
                          viewMode == ViewMode.List
                ? "svgimages/scheduling/listview.svg"
                : "svgimages/business%20objects/bo_folder.svg";

            AddRibbonButton(
                group,
                caption,
                icon,
                (sender, e) => ApplyFilesView(viewMode));
        }

        private void AddRibbonCheckButton(
            RibbonPageGroup group,
            string caption,
            string svgResourceName,
            bool isChecked,
            Action<BarCheckItem> checkedChanged)
        {
            var item = new BarCheckItem
            {
                Caption = caption,
                Checked = isChecked
            };
            SetRibbonSvgImage(item, svgResourceName);
            item.ItemClick += (sender, e) => checkedChanged(item);
            ribbonControl1.Items.Add(item);
            group.ItemLinks.Add(item);
        }

        private void AddDockPanelRibbonButton(
            RibbonPageGroup group,
            string caption,
            string svgResourceName,
            DevExpress.XtraBars.Docking.DockPanel dockPanel)
        {
            AddRibbonCheckButton(
                group,
                caption,
                svgResourceName,
                dockPanel.Visibility ==
                    DevExpress.XtraBars.Docking.DockVisibility.Visible,
                item =>
                {
                    if (item.Checked)
                        ShowDockPanel(dockPanel);
                    else
                        dockPanel.Hide();
                });
        }

        private static void SetRibbonSvgImage(
            BarItem item,
            string resourceName)
        {
            item.ImageOptions.SvgImage =
                DevExpress.Images.ImageResourceCache.Default
                    .GetSvgImage(resourceName);
        }

        private void RebuildRibbonRecentFoldersMenu()
        {
            if (ribbonRecentFoldersMenu == null)
                return;

            ribbonRecentFoldersMenu.ItemLinks.Clear();
            foreach (BarItem oldItem in ribbonRecentFolderItems)
            {
                ribbonControl1.Items.Remove(oldItem);
                oldItem.Dispose();
            }
            ribbonRecentFolderItems.Clear();

            foreach (string path in recentFolders)
            {
                string selectedPath = path;
                var item = new BarButtonItem
                {
                    Caption = GetFolderMenuCaption(path),
                    Hint = path
                };
                SetRibbonSvgImage(
                    item,
                    "svgimages/business%20objects/bo_folder.svg");
                item.ItemClick += (sender, e) =>
                    NavigateToFolder(selectedPath);
                ribbonControl1.Items.Add(item);
                ribbonRecentFolderItems.Add(item);
                ribbonRecentFoldersMenu.ItemLinks.Add(item);
            }

            if (recentFolders.Count == 0)
            {
                var emptyItem = new BarButtonItem
                {
                    Caption = "No recently visited folders",
                    Enabled = false
                };
                ribbonControl1.Items.Add(emptyItem);
                ribbonRecentFolderItems.Add(emptyItem);
                ribbonRecentFoldersMenu.ItemLinks.Add(emptyItem);
            }
        }

        private static void ShowDockPanel(
            DevExpress.XtraBars.Docking.DockPanel dockPanel)
        {
            dockPanel.Show();
        }

        private void EditFocusedFileNotes()
        {
            if (currentMetadata == null)
            {
                XtraMessageBox.Show(
                    this,
                    "Select a file or folder first.",
                    "Edit Notes",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            EditMetadataNotes(currentMetadata);
        }

        private void OpenMetadataDatabaseFolder()
        {
            if (metadataDatabase == null)
                return;

            string directory = Path.GetDirectoryName(
                metadataDatabase.DatabasePath);
            if (string.IsNullOrWhiteSpace(directory))
                return;

            Process.Start(new ProcessStartInfo
            {
                FileName = directory,
                UseShellExecute = true
            });
        }

        private void OpenLogFolder()
        {
            if (operationLogDatabase == null)
                return;

            Process.Start(new ProcessStartInfo
            {
                FileName = Path.GetDirectoryName(
                    operationLogDatabase.DatabasePath),
                UseShellExecute = true
            });
        }

        private void ShowLogViewer()
        {
            if (operationLogDatabase == null)
                return;

            if (logViewerForm == null || logViewerForm.IsDisposed)
            {
                logViewerForm = new LogViewerForm(operationLogDatabase);
                logViewerForm.FormClosed += (sender, e) =>
                    logViewerForm = null;
                logViewerForm.Show(this);
            }
            else
            {
                logViewerForm.RefreshLog();
                if (logViewerForm.WindowState == FormWindowState.Minimized)
                    logViewerForm.WindowState = FormWindowState.Normal;
                logViewerForm.BringToFront();
                logViewerForm.Activate();
            }
        }

        private void ShowWiwaTimeline()
        {
            if (operationLogDatabase == null)
                return;

            if (wiwaDockPanel == null || wiwaDockPanel.IsDisposed)
            {
                wiwaControl = new WiwaControl(operationLogDatabase);
                wiwaControl.CloseRequested += (sender, e) =>
                    wiwaDockPanel?.Hide();
                wiwaDockPanel = dockManager1.AddPanel(
                    wiwaControl,
                    new Point(
                        Math.Max(50, Left + 150),
                        Math.Max(50, Top + 120)),
                    "WIWA Timeline");
                wiwaDockPanel.FloatSize = new Size(1250, 720);
            }

            wiwaControl.RefreshTimeline();
            wiwaDockPanel.Show();
        }

        private void RestoreDefaultTheme()
        {
            if (applicationSettings == null)
                return;

            applicationSettings.ThemeName = defaultThemeName;
            applicationSettings.PaletteName = defaultPaletteName;
            ApplySavedTheme(applicationSettings);
            settingsDatabase.Save(applicationSettings);
        }

        private void ShowSearchWindow(string location = null)
        {
            if (searchDockPanel == null || searchDockPanel.IsDisposed)
            {
                searchControl = new SearchControl(
                    applicationSettings,
                    settingsDatabase,
                    operationLogDatabase,
                    metadataDatabase.DatabasePath,
                    location ?? gridControlExtension1.CurrentPath);
                searchDockPanel = dockManager1.AddPanel(
                    searchControl,
                    new Point(
                        Math.Max(50, Left + 180),
                        Math.Max(50, Top + 140)),
                    "Search");
                searchDockPanel.FloatSize = new Size(1100, 700);
            }

            if (!string.IsNullOrWhiteSpace(location))
                searchControl.SetLocation(location);
            searchDockPanel.Show();
        }

        private void gridControlExtension1_ContextMenuShowing(
            object sender,
            DevExpress.Dialogs.ContextMenu.ContextMenuShowingEventArgs e)
        {
            var item = e.FocusedItem;
            if (item == null || !item.IsFolder ||
                string.IsNullOrWhiteSpace(item.FullPath))
            {
                return;
            }

            contextMenuSearchFolderPath = item.FullPath;

            e.MenuItems.InsertCommand(
                0,
                "Search This Folder…",
                SearchThisFolderCommandName,
                true);
            e.MenuItems.InsertSeparator(1);
        }

        private void gridControlExtension1_BeforeExecuteItemCommand(
            object sender,
            DevExpress.Dialogs.Core.BeforeExecuteItemCommandEventArgs e)
        {
            var loggedItem = e.Item ?? e.SelectedItems?.FirstOrDefault();
            operationLogDatabase?.Log(
                "File Command: " + (e.CommandName ?? "Unknown"),
                loggedItem?.FullPath,
                details: e.SelectedItems == null
                    ? null
                    : e.SelectedItems.Count() + " selected item(s)");

            if (!string.Equals(
                    e.CommandName,
                    SearchThisFolderCommandName,
                    StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            e.Cancel = true;
            var item = e.Item ?? e.SelectedItems?.FirstOrDefault(
                selectedItem => selectedItem != null &&
                    selectedItem.IsFolder);
            string folderPath = item != null && item.IsFolder
                ? item.FullPath
                : contextMenuSearchFolderPath;

            if (!string.IsNullOrWhiteSpace(folderPath))
            {
                BeginInvoke(new Action(() =>
                    ShowSearchWindow(folderPath)));
            }
        }

        #endregion


        #region Windows Special Folders

        private List<Environment.SpecialFolder> GetConfiguredSpecialFolders()
        {
            var folders = new List<Environment.SpecialFolder>();

            foreach (var rootNode in treeListExtension1.RootNodes)
            {
                var specialFolderNode =
                    rootNode as EnvironmentSpecialFolderNode;

                if (specialFolderNode != null)
                    folders.Add(specialFolderNode.Folder);
            }

            return folders;
        }

        private void DisplayConfiguredSpecialFolders()
        {
            var folders = GetConfiguredSpecialFolders();

            if (folders.Count == 0)
            {
                XtraMessageBox.Show("No special folders are configured.");
            }
            else
            {
                var sb = new StringBuilder();
                foreach (var folder in folders)
                    sb.AppendLine(folder.ToString());
                XtraMessageBox.Show(sb.ToString(), "Configured Special Folders");
            }
        }

        private List<SpecialFolderInfo> GetAllWindowsSpecialFolders()
        {
            var result = new List<SpecialFolderInfo>();

            foreach (Environment.SpecialFolder folder in
                     Enum.GetValues(typeof(Environment.SpecialFolder)))
            {
                string path = Environment.GetFolderPath(folder);

                result.Add(new SpecialFolderInfo
                {
                    Folder = folder,
                    Name = folder.ToString(),
                    Path = path
                });
            }

            return result;
        }

        private void DisplayAllWindowsSpecialFolders()
        {
            var folders = GetAllWindowsSpecialFolders();
            var sb = new StringBuilder();
            foreach (var folder in folders)
            {
                sb.AppendLine($"{folder.Name}: {folder.Path}");
            }
            XtraMessageBox.Show(sb.ToString(), "All Windows Special Folders");
        }

        private sealed class SpecialFolderInfo
        {
            public Environment.SpecialFolder Folder { get; set; }
            public string Name { get; set; }
            public string Path { get; set; }
        }

        private List<SpecialFolderInfo> GetExistingSpecialFolders()
        {
            var result = new List<SpecialFolderInfo>();
            var usedPaths = new HashSet<string>(
                StringComparer.OrdinalIgnoreCase);

            foreach (Environment.SpecialFolder folder in
                     Enum.GetValues(typeof(Environment.SpecialFolder)))
            {
                string path = Environment.GetFolderPath(folder);

                if (string.IsNullOrWhiteSpace(path))
                    continue;

                if (!usedPaths.Add(path))
                    continue;

                result.Add(new SpecialFolderInfo
                {
                    Folder = folder,
                    Name = folder.ToString(),
                    Path = path
                });
            }

            return result;
        }


        private void DisplayExistingWindowsSpecialFolders()
        {
            var folders = GetExistingSpecialFolders();
            var sb = new StringBuilder();
            foreach (var folder in folders)
            {
                sb.AppendLine($"{folder.Name}: {folder.Path}");
            }
            XtraMessageBox.Show(sb.ToString(), "All Windows Special Folders");
        }

        private void AddSpecialFolderRoots()
        {
            treeListExtension1.RootNodes.Clear();

            AddSpecialFolder(Environment.SpecialFolder.Desktop);
            AddSpecialFolder(Environment.SpecialFolder.MyDocuments);
            AddSpecialFolder(Environment.SpecialFolder.MyPictures);
            AddSpecialFolder(Environment.SpecialFolder.MyMusic);
            AddSpecialFolder(Environment.SpecialFolder.MyVideos);
            AddSpecialFolder(Environment.SpecialFolder.Favorites);
        }

        private void AddSpecialFolder(
            Environment.SpecialFolder folder)
        {
            var node = new EnvironmentSpecialFolderNode
            {
                Folder = folder,
                AutoExpand = false
            };

            treeListExtension1.RootNodes.Add(node);
        }


        #endregion Windows Special Folders

        #region Windows Special Folders View CheckedComboxBox Code

        private static readonly Guid ThisPcKnownFolderId =
        new Guid("0AC0837C-BBF8-452A-850D-79D08E667CA7");

        private static readonly Guid NetworkKnownFolderId =
            new Guid("D20BEEC4-5CA8-4905-AE3B-BF251EA09B53");

        private static readonly Guid RecycleBinKnownFolderId =
            new Guid("B7534046-3ECB-4C18-BE4E-64CD4CB7D6AC");

        private void InitializecheckedComboBoxEdit_FoldersView()
        {
            checkedComboBoxEdit_FoldersView.Properties.NullText =
                "Select navigation folders...";

            checkedComboBoxEdit_FoldersView.Properties.SelectAllItemCaption =
                "Select All";

            checkedComboBoxEdit_FoldersView.Properties.SeparatorChar = ';';
            checkedComboBoxEdit_FoldersView.Properties.Items.Clear();

            foreach (NavigationLocation location
                     in CreateDefaultNavigationLocations())
            {
                checkedComboBoxEdit_FoldersView.Properties.Items.Add(
                    location,
                    location.DisplayName,
                    CheckState.Checked,
                    true);
            }

            checkedComboBoxEdit_FoldersView.EditValueChanged +=
                checkedComboBoxEdit_FoldersView_EditValueChanged;

            PopulateNavigationTree(
                GetSelectedNavigationLocations());
        }

        private static List<NavigationLocation>
            CreateDefaultNavigationLocations()
        {
            var locations = new List<NavigationLocation>();

            AddPathLocation(
                locations,
                "Home",
                Environment.GetFolderPath(
                    Environment.SpecialFolder.UserProfile),
                NavigationLocationType.Folder);

            AddSpecialFolder(
                locations,
                "Desktop",
                Environment.SpecialFolder.DesktopDirectory);

            AddSpecialFolder(
                locations,
                "Documents",
                Environment.SpecialFolder.MyDocuments);

            AddPathLocation(
                locations,
                "Downloads",
                Path.Combine(
                    Environment.GetFolderPath(
                        Environment.SpecialFolder.UserProfile),
                    "Downloads"),
                NavigationLocationType.Folder);

            AddSpecialFolder(
                locations,
                "Pictures",
                Environment.SpecialFolder.MyPictures);

            AddSpecialFolder(
                locations,
                "Music",
                Environment.SpecialFolder.MyMusic);

            AddSpecialFolder(
                locations,
                "Videos",
                Environment.SpecialFolder.MyVideos);

            string oneDrivePath =
                Environment.GetEnvironmentVariable("OneDrive");

            AddPathLocation(
                locations,
                "OneDrive",
                oneDrivePath,
                NavigationLocationType.Folder);

            locations.Add(
                NavigationLocation.CreateShellLocation(
                    "This PC",
                    ThisPcKnownFolderId));

            locations.Add(
                NavigationLocation.CreateShellLocation(
                    "Network",
                    NetworkKnownFolderId));

            locations.Add(
                NavigationLocation.CreateShellLocation(
                    "Recycle Bin",
                    RecycleBinKnownFolderId));

            return locations;
        }

        private static void AddSpecialFolder(
            ICollection<NavigationLocation> locations,
            string displayName,
            Environment.SpecialFolder specialFolder)
        {
            string path =
                Environment.GetFolderPath(specialFolder);

            if (string.IsNullOrWhiteSpace(path))
                return;

            locations.Add(
                NavigationLocation.CreateSpecialFolder(
                    displayName,
                    specialFolder,
                    path));
        }

        private static void AddPathLocation(
            ICollection<NavigationLocation> locations,
            string displayName,
            string path,
            NavigationLocationType locationType)
        {
            if (string.IsNullOrWhiteSpace(path))
                return;

            if (!Directory.Exists(path))
                return;

            locations.Add(
                new NavigationLocation
                {
                    Id = displayName,
                    DisplayName = displayName,
                    LocationType = locationType,
                    Path = path
                });
        }

        private void checkedComboBoxEdit_FoldersView_EditValueChanged(
            object sender,
            EventArgs e)
        {
            List<NavigationLocation> selectedLocations = GetSelectedNavigationLocations();

            // This is where the TreeList can be rebuilt.
            PopulateNavigationTree(selectedLocations);
        }

        private List<NavigationLocation>
            GetSelectedNavigationLocations()
        {
            var selected =
                new List<NavigationLocation>();

            foreach (CheckedListBoxItem item in
                     checkedComboBoxEdit_FoldersView.Properties.Items)
            {
                if (item.CheckState != CheckState.Checked)
                    continue;

                var location =
                    item.Value as NavigationLocation;

                if (location != null)
                    selected.Add(location);
            }

            return selected;
        }

        private void PopulateNavigationTree(
            IEnumerable<NavigationLocation> locations)
        {
            treeListExtension1.BeginInit();

            try
            {
                treeListExtension1.RootNodes.Clear();

                foreach (NavigationLocation location in locations)
                {
                    switch (location.LocationType)
                    {
                        case NavigationLocationType.SpecialFolder:
                            AddEnvironmentSpecialFolder(location);
                            break;

                        case NavigationLocationType.Folder:
                            AddPhysicalFolder(location);
                            break;

                        case NavigationLocationType.ShellGuid:
                            AddShellFolder(location);
                            break;
                    }
                }
            }
            finally
            {
                treeListExtension1.EndInit();
            }

            treeListExtension1.RefreshData();
        }

        private void AddEnvironmentSpecialFolder(
            NavigationLocation location)
        {
            if (!location.SpecialFolder.HasValue)
                return;

            var node =
                new DevExpress.XtraDialogs
                    .FileExplorerExtensions
                    .EnvironmentSpecialFolderNode
                {
                    Folder = location.SpecialFolder.Value,
                    AutoExpand = false,
                    Tag = location
                };

            treeListExtension1.RootNodes.Add(node);
        }

        private void AddPhysicalFolder(
            NavigationLocation location)
        {
            if (string.IsNullOrWhiteSpace(location.Path))
                return;

            if (!Directory.Exists(location.Path))
                return;

            var node =
                new DevExpress.XtraDialogs
                    .FileExplorerExtensions
                    .PathNode(location.Path)
                {
                    AutoExpand = false,
                    Tag = location
                };

            treeListExtension1.RootNodes.Add(node);
        }

        private void AddShellFolder(
            NavigationLocation location)
        {
            if (!location.ShellGuid.HasValue)
                return;

            var node =
                new DevExpress.XtraDialogs
                    .FileExplorerExtensions
                    .GuidNode(location.ShellGuid.Value)
                {
                    AutoExpand = false,
                    Tag = location
                };

            treeListExtension1.RootNodes.Add(node);
        }




        #endregion Windows Special Folders View CheckedComboxBox Code

        #region Apps Tree Code

        private readonly List<CuratedAppEntry> curatedApps = new List<CuratedAppEntry>();

        private enum AppsTreeSource
        {
            MyApps,
            MyUserApps,
            AllUsersApps
        }

        private enum AppsTreeNodeType
        {
            Category,
            Folder,
            Application,
            Placeholder
        }

        private sealed class AppsTreeSourceItem
        {
            public AppsTreeSource Source { get; set; }

            public string DisplayName { get; set; }

            public override string ToString()
            {
                return DisplayName;
            }
        }

        private sealed class AppsTreeNodeInfo
        {
            public AppsTreeNodeType NodeType { get; set; }

            public string DisplayName { get; set; }

            public string Path { get; set; }

            public AppsTreeSource? Source { get; set; }
        }

        private sealed class CuratedAppEntry
        {
            public string DisplayName { get; set; }

            public string LaunchPath { get; set; }

            public string Arguments { get; set; }

            public string WorkingDirectory { get; set; }
        }

        private void InitializeAppsTree()
        {
            InitializeAppsViewSelector();
            InitializeAppsTreeList();
            LoadInitialCuratedApps();
            ReloadAppsTree();
        }

        private void InitializeAppsViewSelector()
        {
            //checkedComboBoxEdit_AppsView =
            //    new CheckedComboBoxEdit();

            //checkedComboBoxEdit_AppsView.Name =
            //    "checkedComboBoxEdit_AppsView";

            checkedComboBoxEdit_AppsView.Properties.NullText =
                "Select application sources...";

            checkedComboBoxEdit_AppsView.Properties
                .SelectAllItemCaption = "Select All";

            checkedComboBoxEdit_AppsView.Properties
                .SeparatorChar = ';';

            //checkedComboBoxEdit_AppsView.Location =
            //    new System.Drawing.Point(8, 7);

            //checkedComboBoxEdit_AppsView.Size =
            //    new System.Drawing.Size(275, 22);

            //checkedComboBoxEdit_AppsView.Anchor =
            //    AnchorStyles.Top |
            //    AnchorStyles.Left |
            //    AnchorStyles.Right;

            checkedComboBoxEdit_AppsView.Properties.Items.Add(
                new AppsTreeSourceItem
                {
                    Source = AppsTreeSource.MyApps,
                    DisplayName = "My Apps"
                },
                "My Apps",
                CheckState.Checked,
                true);

            checkedComboBoxEdit_AppsView.Properties.Items.Add(
                new AppsTreeSourceItem
                {
                    Source = AppsTreeSource.MyUserApps,
                    DisplayName = "My User Apps"
                },
                "My User Apps",
                CheckState.Checked,
                true);

            checkedComboBoxEdit_AppsView.Properties.Items.Add(
                new AppsTreeSourceItem
                {
                    Source = AppsTreeSource.AllUsersApps,
                    DisplayName = "All Users Apps"
                },
                "All Users Apps",
                CheckState.Checked,
                true);

            checkedComboBoxEdit_AppsView.EditValueChanged +=
                checkedComboBoxEdit_AppsView_EditValueChanged;

            //panelControl_AppsTree_Top.Controls.Add(
            //    checkedComboBoxEdit_AppsView);

            //checkedComboBoxEdit_AppsView.BringToFront();
        }

        private void InitializeAppsTreeList()
        {
            //treeList_AppsTree = new TreeList();

            //treeList_AppsTree.Name = "treeList_AppsTree";
            //treeList_AppsTree.Dock = DockStyle.Fill;

            treeList_AppsTree.OptionsBehavior.Editable = false;
            treeList_AppsTree.OptionsBehavior.AllowRecursiveNodeChecking =
                true;

            treeList_AppsTree.OptionsSelection
                .EnableAppearanceFocusedCell = false;

            treeList_AppsTree.OptionsView.ShowColumns = false;
            treeList_AppsTree.OptionsView.ShowIndicator = false;
            treeList_AppsTree.OptionsView.ShowHorzLines = false;
            treeList_AppsTree.OptionsView.ShowVertLines = false;

            treeList_AppsTree.OptionsSelection.EnableAppearanceFocusedRow = true;
            treeList_AppsTree.TreeLevelWidth = 18;

            // Replace problematic call with explicit column creation to avoid CS1501.
            var col = treeList_AppsTree.Columns.Add();
            col.FieldName = "Name";
            col.Caption = "Application";
            col.Visible = true;

            treeList_AppsTree.DoubleClick +=
                treeList_AppsTree_DoubleClick;

            treeList_AppsTree.KeyDown +=
                treeList_AppsTree_KeyDown;

            ////////////////////controlContainer3.Controls.Add(treeList_AppsTree);

            // Ensure the existing top and bottom panels remain visible.
            ////////////////////treeList_AppsTree.SendToBack();
            ////////////////////panelControl_AppsTree_Top.BringToFront();
            ////////////////////panelControl_AppsTree_Bottom.BringToFront();
        }


        private void checkedComboBoxEdit_AppsView_EditValueChanged(
    object sender,
    EventArgs e)
        {
            ReloadAppsTree();
        }

        private void ReloadAppsTree()
        {
            if (treeList_AppsTree == null)
                return;

            HashSet<AppsTreeSource> selectedSources =
                GetSelectedAppsTreeSources();

            treeList_AppsTree.BeginUnboundLoad();

            try
            {
                treeList_AppsTree.Nodes.Clear();

                if (selectedSources.Contains(
                    AppsTreeSource.MyApps))
                {
                    LoadMyAppsRoot();
                }

                if (selectedSources.Contains(
                    AppsTreeSource.MyUserApps))
                {
                    LoadStartMenuProgramsRoot(
                        AppsTreeSource.MyUserApps,
                        "My User Apps",
                        Environment.SpecialFolder.Programs);
                }

                if (selectedSources.Contains(
                    AppsTreeSource.AllUsersApps))
                {
                    LoadStartMenuProgramsRoot(
                        AppsTreeSource.AllUsersApps,
                        "All Users Apps",
                        Environment.SpecialFolder.CommonPrograms);
                }
            }
            finally
            {
                treeList_AppsTree.EndUnboundLoad();
            }

            ExpandAppsTreeRoots();
        }

        private HashSet<AppsTreeSource> GetSelectedAppsTreeSources()
        {
            var selected =
                new HashSet<AppsTreeSource>();

            foreach (CheckedListBoxItem checkedItem in
                     checkedComboBoxEdit_AppsView
                         .Properties.Items)
            {
                if (checkedItem.CheckState !=
                    CheckState.Checked)
                {
                    continue;
                }

                AppsTreeSourceItem sourceItem =
                    checkedItem.Value as AppsTreeSourceItem;

                if (sourceItem != null)
                    selected.Add(sourceItem.Source);
            }

            return selected;
        }

        private void LoadInitialCuratedApps()
        {
            /*
             * This is intentionally empty for now.
             *
             * Later, load the user's curated applications from
             * SQLite and add them to curatedApps.
             *
             * Example:
             *
             * curatedApps.Add(new CuratedAppEntry
             * {
             *     DisplayName = "Visual Studio",
             *     LaunchPath =
             *         @"C:\Program Files\Microsoft Visual Studio"
             *         + @"\2022\Community\Common7\IDE\devenv.exe",
             *     Arguments = "",
             *     WorkingDirectory = ""
             * });
             */
        }

        private void LoadMyAppsRoot()
        {
            TreeListNode rootNode = AppendAppsNode(
                null,
                "My Apps",
                new AppsTreeNodeInfo
                {
                    NodeType = AppsTreeNodeType.Category,
                    DisplayName = "My Apps",
                    Source = AppsTreeSource.MyApps
                });

            if (curatedApps.Count == 0)
            {
                AppendAppsNode(
                    rootNode,
                    "(Use the Apps Tree menu to add curated apps)",
                    new AppsTreeNodeInfo
                    {
                        NodeType =
                            AppsTreeNodeType.Placeholder,

                        DisplayName =
                            "Use the Apps Tree menu to add curated apps",

                        Source = AppsTreeSource.MyApps
                    });

                return;
            }

            foreach (CuratedAppEntry app in
                     curatedApps.OrderBy(
                         item => item.DisplayName,
                         StringComparer.CurrentCultureIgnoreCase))
            {
                AppendAppsNode(
                    rootNode,
                    app.DisplayName,
                    new AppsTreeNodeInfo
                    {
                        NodeType =
                            AppsTreeNodeType.Application,

                        DisplayName = app.DisplayName,
                        Path = app.LaunchPath,
                        Source = AppsTreeSource.MyApps
                    });
            }
        }

        private void LoadStartMenuProgramsRoot(
    AppsTreeSource source,
    string displayName,
    Environment.SpecialFolder specialFolder)
        {
            TreeListNode rootNode = AppendAppsNode(
                null,
                displayName,
                new AppsTreeNodeInfo
                {
                    NodeType = AppsTreeNodeType.Category,
                    DisplayName = displayName,
                    Source = source
                });

            string programsPath =
                Environment.GetFolderPath(specialFolder);

            if (string.IsNullOrWhiteSpace(programsPath) ||
                !Directory.Exists(programsPath))
            {
                AppendAppsNode(
                    rootNode,
                    "(Folder is unavailable)",
                    new AppsTreeNodeInfo
                    {
                        NodeType =
                            AppsTreeNodeType.Placeholder,

                        DisplayName =
                            "Folder is unavailable",

                        Path = programsPath,
                        Source = source
                    });

                return;
            }

            LoadAppsDirectory(
                rootNode,
                programsPath,
                source);
        }

        private void LoadAppsDirectory(
    TreeListNode parentNode,
    string directoryPath,
    AppsTreeSource source)
        {
            IEnumerable<string> childDirectories;

            try
            {
                childDirectories =
                    Directory.EnumerateDirectories(directoryPath)
                        .OrderBy(
                            path => Path.GetFileName(path),
                            StringComparer.CurrentCultureIgnoreCase)
                        .ToList();
            }
            catch (UnauthorizedAccessException)
            {
                AppendAccessDeniedNode(
                    parentNode,
                    directoryPath,
                    source);

                return;
            }
            catch (IOException)
            {
                return;
            }

            foreach (string childDirectory in
                     childDirectories)
            {
                string directoryName =
                    Path.GetFileName(childDirectory);

                TreeListNode directoryNode =
                    AppendAppsNode(
                        parentNode,
                        directoryName,
                        new AppsTreeNodeInfo
                        {
                            NodeType =
                                AppsTreeNodeType.Folder,

                            DisplayName = directoryName,
                            Path = childDirectory,
                            Source = source
                        });

                LoadAppsDirectory(
                    directoryNode,
                    childDirectory,
                    source);

                // Do not display empty Start Menu folders.
                if (directoryNode.Nodes.Count == 0)
                    treeList_AppsTree.DeleteNode(directoryNode);
            }

            IEnumerable<string> applicationFiles;

            try
            {
                applicationFiles =
                    Directory.EnumerateFiles(directoryPath)
                        .Where(IsApplicationEntry)
                        .OrderBy(
                            GetApplicationDisplayName,
                            StringComparer.CurrentCultureIgnoreCase)
                        .ToList();
            }
            catch (UnauthorizedAccessException)
            {
                AppendAccessDeniedNode(
                    parentNode,
                    directoryPath,
                    source);

                return;
            }
            catch (IOException)
            {
                return;
            }

            foreach (string applicationPath in
                     applicationFiles)
            {
                string displayName =
                    GetApplicationDisplayName(applicationPath);

                AppendAppsNode(
                    parentNode,
                    displayName,
                    new AppsTreeNodeInfo
                    {
                        NodeType =
                            AppsTreeNodeType.Application,

                        DisplayName = displayName,
                        Path = applicationPath,
                        Source = source
                    });
            }
        }

        private static bool IsApplicationEntry(string path)
        {
            string extension =
                Path.GetExtension(path);

            if (string.IsNullOrWhiteSpace(extension))
                return false;

            switch (extension.ToLowerInvariant())
            {
                case ".lnk":
                case ".url":
                case ".appref-ms":
                case ".exe":
                case ".bat":
                case ".cmd":
                case ".com":
                case ".msc":
                    return true;

                default:
                    return false;
            }
        }

        private static string GetApplicationDisplayName(
            string path)
        {
            string fileName =
                Path.GetFileNameWithoutExtension(path);

            return string.IsNullOrWhiteSpace(fileName)
                ? Path.GetFileName(path)
                : fileName;
        }

        private TreeListNode AppendAppsNode(
    TreeListNode parentNode,
    string displayName,
    AppsTreeNodeInfo nodeInfo)
        {
            TreeListNode node =
                treeList_AppsTree.AppendNode(
                    new object[] { displayName },
                    parentNode);

            node.Tag = nodeInfo;

            return node;
        }

        private void AppendAccessDeniedNode(
            TreeListNode parentNode,
            string path,
            AppsTreeSource source)
        {
            AppendAppsNode(
                parentNode,
                "(Access denied)",
                new AppsTreeNodeInfo
                {
                    NodeType =
                        AppsTreeNodeType.Placeholder,

                    DisplayName = "Access denied",
                    Path = path,
                    Source = source
                });
        }

        private void ExpandAppsTreeRoots()
        {
            foreach (TreeListNode rootNode in
                     treeList_AppsTree.Nodes)
            {
                rootNode.Expanded = true;
            }
        }

        private void treeList_AppsTree_DoubleClick(
    object sender,
    EventArgs e)
        {
            LaunchFocusedAppsTreeItem();
        }

        private void treeList_AppsTree_KeyDown(
            object sender,
            KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
                return;

            LaunchFocusedAppsTreeItem();

            e.Handled = true;
            e.SuppressKeyPress = true;
        }

        private void LaunchFocusedAppsTreeItem()
        {
            TreeListNode focusedNode =
                treeList_AppsTree.FocusedNode;

            if (focusedNode == null)
                return;

            AppsTreeNodeInfo nodeInfo =
                focusedNode.Tag as AppsTreeNodeInfo;

            if (nodeInfo == null)
                return;

            if (nodeInfo.NodeType ==
                AppsTreeNodeType.Folder)
            {
                focusedNode.Expanded =
                    !focusedNode.Expanded;

                return;
            }

            if (nodeInfo.NodeType !=
                AppsTreeNodeType.Application)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(
                nodeInfo.Path))
            {
                return;
            }


            // Debug:
            // If you are having trouble launching applications, this will tell you if the DVMS executable is missing.
            // This happened on my machine as pCloud was syncing the DVMS executable and it was temporarily unavailable.
            string executablePath = Application.ExecutablePath;

            if (!File.Exists(executablePath))
            {
                operationLogDatabase?.Log(
                    "Launch Application",
                    nodeInfo.Path,
                    successful: false,
                    errorMessage: "The running DVMS executable is missing.");
                MessageBox.Show(
                    "The running DVMS executable no longer exists:"
                    + "\r\n\r\n"
                    + executablePath,
                    "Missing Application Executable",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }
            // End Debug.




            try
            {
                // Note:  If this fails with a file not found error but the link works otherwise
                //        then the problem is that you have the program default as ANYCPU or 32-bit.
                //        Changing it to x64 fixes the problem!  Framework only of course.

                Process.Start(
                    new ProcessStartInfo
                    {
                        FileName = nodeInfo.Path,
                        UseShellExecute = true
                    });
                operationLogDatabase?.Log(
                    "Launch Application", nodeInfo.Path);
            }
            catch (Exception exception)
            {
                operationLogDatabase?.Log(
                    "Launch Application",
                    nodeInfo.Path,
                    successful: false,
                    errorMessage: exception.Message);
                XtraMessageBox.Show(
                    this,
                    "DVMS could not open the selected application."
                    + "\r\n\r\n"
                    + exception.Message,
                    "Open Application",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                //MessageBox.Show(
                //    "DVMS could not open the selected application."
                //    + "\r\n\r\n"
                //    + "Application entry:"
                //    + "\r\n"
                //    + nodeInfo.Path
                //    + "\r\n\r\n"
                //    + "DVMS executable:"
                //    + "\r\n"
                //    + Application.ExecutablePath
                //    + "\r\n\r\n"
                //    + exception.GetType().FullName
                //    + "\r\n"
                //    + exception.Message,
                //    "Open Application",
                //    MessageBoxButtons.OK,
                //    MessageBoxIcon.Error);
            }
        }



        // Example of how to add curated apps programmatically:
        //
        // AddCuratedApp(
        //    "Notepad",
        //    "notepad.exe",
        //    "",
        //    "");

        private void AddCuratedApp(
            string displayName,
            string launchPath,
            string arguments,
            string workingDirectory)
        {
            if (string.IsNullOrWhiteSpace(launchPath))
                return;

            curatedApps.Add(
                new CuratedAppEntry
                {
                    DisplayName =
                        string.IsNullOrWhiteSpace(displayName)
                            ? GetApplicationDisplayName(launchPath)
                            : displayName,

                    LaunchPath = launchPath,
                    Arguments = arguments ?? "",
                    WorkingDirectory =
                        workingDirectory ?? ""
                });

            ReloadAppsTree();
        }



        #endregion Apps Tree Code


        #region File Grid Code

        private bool changingFilesView;
        private bool configuringDetailsColumns;
        private readonly System.Windows.Forms.Timer detailsColumnsTimer =
            new System.Windows.Forms.Timer { Interval = 300 };
        private readonly Dictionary<string, DateTime?> fileCreatedDates =
            new Dictionary<string, DateTime?>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, long> folderSizesByPath =
            new Dictionary<string, long>(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> folderSizeCalculationsInProgress =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private SemaphoreSlim folderSizeScanLimit =
            new SemaphoreSlim(2, 2);
        private CancellationTokenSource folderSizeCancellation =
            new CancellationTokenSource();
        private bool folderSizeCalculatorIsShuttingDown;
        private MetadataDatabase metadataDatabase;
        private FileMetadataViewModel currentMetadata;
        private bool loadingMetadata;
        private Dictionary<string, int> noteCountsByPath =
            new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        private Dictionary<string, MetadataPresentation>
            metadataPresentationByPath =
                new Dictionary<string, MetadataPresentation>(
                    StringComparer.OrdinalIgnoreCase);
        private Dictionary<string, string> descriptionsByPath =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, Image> metadataIconCache =
            new Dictionary<string, Image>(StringComparer.OrdinalIgnoreCase);
        private int filesDataSourceVersion;
        private int reconciledDataSourceVersion = -1;
        private bool reconcilingVisibleMetadata;

        private void InitializeMetadata()
        {
            metadataDatabase = new MetadataDatabase();
            metadataDatabase.Initialize();
            RefreshNoteCounts();
            RefreshMetadataPresentation();

            gridControlExtension1.QueryLinkImage -=
                gridControlExtension1_QueryLinkImage;
            gridControlExtension1.QueryLinkImage +=
                gridControlExtension1_QueryLinkImage;

            FormClosed -= DVMSFileManagerForm_MetadataFormClosed;
            FormClosed += DVMSFileManagerForm_MetadataFormClosed;

            propertyGridControl_MetaData.SelectedObject = null;
            propertyGridControl_MetaData.CellValueChanged -=
                propertyGridControl_MetaData_CellValueChanged;
            propertyGridControl_MetaData.CellValueChanged +=
                propertyGridControl_MetaData_CellValueChanged;

            checkEdit_ShowDescriptionPreview.CheckedChanged -=
                checkEdit_ShowDescriptionPreview_CheckedChanged;
            checkEdit_ShowDescriptionPreview.CheckedChanged +=
                checkEdit_ShowDescriptionPreview_CheckedChanged;
        }

        private void propertyGridControl_MetaData_CellValueChanged(
            object sender,
            DevExpress.XtraVerticalGrid.Events.CellValueChangedEventArgs e)
        {
            if (loadingMetadata || currentMetadata == null)
                return;

            metadataDatabase.SaveMetadata(currentMetadata);
            RefreshMetadataPresentation();
            propertyGridControl_MetaData.Refresh();
            gridControl_Files.RefreshDataSource();
        }

        private void InitializeFileProperties()
        {
            propertyGridControl_Files.SelectedObject = null;

            gridControlExtension1.FocusedLinkChanged -=
                gridControlExtension1_FocusedLinkChanged;

            gridControlExtension1.FocusedLinkChanged +=
                gridControlExtension1_FocusedLinkChanged;

            DisplayFileProperties(gridControlExtension1.FocusedItem);
        }

        private void gridControlExtension1_FocusedLinkChanged(
            object sender,
            FocusedLinkChangedEventArgs e)
        {
            DisplayFileProperties(e.FocusedLink);

            // FileExplorerAssistant focuses the first row after its
            // asynchronous data and Shell-column calculation has ended.
            // Reorder on the next UI message, after that calculation returns.
            if (gridControl_Files.IsHandleCreated)
            {
                gridControl_Files.BeginInvoke(
                    new Action(() =>
                    {
                        ConfigureDetailsViewColumns();
                        ReconcileVisibleFileMetadata();
                    }));
            }
        }

        private async void ReconcileVisibleFileMetadata()
        {
            if (reconcilingVisibleMetadata ||
                reconciledDataSourceVersion == filesDataSourceVersion)
            {
                return;
            }

            DevExpress.XtraGrid.Views.Grid.GridView detailsView =
                gridControl_Files.ViewCollection
                    .OfType<DevExpress.XtraGrid.Views.Grid.GridView>()
                    .FirstOrDefault(view => string.Equals(
                        view.Name,
                        "detailsGridView",
                        StringComparison.OrdinalIgnoreCase))
                ?? gridView1;

            var visibleFiles = new List<KeyValuePair<string, string>>();
            for (int rowHandle = 0;
                 rowHandle < detailsView.DataRowCount;
                 rowHandle++)
            {
                var item = detailsView.GetRow(rowHandle) as
                    DevExpress.Dialogs.Core.Items.ShellItemLink;

                if (item != null &&
                    !string.IsNullOrWhiteSpace(item.FullPath))
                {
                    visibleFiles.Add(
                        new KeyValuePair<string, string>(
                            item.FullPath,
                            item.Name));
                }
            }

            int versionBeingReconciled = filesDataSourceVersion;
            reconcilingVisibleMetadata = true;

            try
            {
                int updatedCount = await Task.Run(() =>
                {
                    var identities = new List<FileIdentityKey>();

                    foreach (KeyValuePair<string, string> file
                             in visibleFiles)
                    {
                        identities.Add(
                            FileSystemProperties.CreateIdentityKey(
                                file.Key,
                                file.Value));
                    }

                    return metadataDatabase.ReconcileKnownFiles(
                        identities);
                });

                reconciledDataSourceVersion = versionBeingReconciled;

                if (updatedCount > 0)
                {
                    RefreshNoteCounts();
                    RefreshMetadataPresentation();
                    gridControl_Files.RefreshDataSource();
                    gridControl_Files.Invalidate();
                }
            }
            catch (Exception exception)
                when (exception is IOException ||
                      exception is UnauthorizedAccessException ||
                      exception is SystemException)
            {
                // Individual inaccessible or unsupported items simply retain
                // their last-known path until they can be read again.
            }
            finally
            {
                reconcilingVisibleMetadata = false;

                if (reconciledDataSourceVersion != filesDataSourceVersion)
                    ReconcileVisibleFileMetadata();
            }
        }

        private void DisplayFileProperties(
            DevExpress.Dialogs.Core.Items.ShellItemLink item)
        {
            FileSystemProperties fileProperties = item == null
                ? null
                : FileSystemProperties.Create(item);

            propertyGridControl_Files.SelectedObject = fileProperties;
            DisplayMetadata(item, fileProperties);
        }

        private void DisplayMetadata(
            DevExpress.Dialogs.Core.Items.ShellItemLink item,
            FileSystemProperties fileProperties)
        {
            loadingMetadata = true;
            try
            {
                if (item == null ||
                    fileProperties == null ||
                    string.IsNullOrWhiteSpace(item.FullPath))
                {
                    currentMetadata = null;
                    propertyGridControl_MetaData.SelectedObject = null;
                    return;
                }

                currentMetadata = metadataDatabase.LoadOrCreate(
                    new FileIdentityKey
                    {
                        VolumeSerialNumber =
                            fileProperties.VolumeSerialNumber,
                        WindowsFileId = fileProperties.FileId,
                        FullPath = item.FullPath,
                        FileName = item.Name
                    },
                    EditMetadataNotes);

                // LoadOrCreate follows a renamed or moved file by its
                // Volume + File ID and updates Files.FullPath. Refresh the
                // path-keyed presentation caches immediately so its metadata
                // is rendered at the new location.
                RefreshNoteCounts();
                RefreshMetadataPresentation();

                propertyGridControl_MetaData.SelectedObject =
                    currentMetadata;
            }
            finally
            {
                loadingMetadata = false;
            }

            gridControl_Files.RefreshDataSource();
            gridControl_Files.Invalidate();
        }

        private void EditMetadataNotes(FileMetadataViewModel metadata)
        {
            using (var editor = new NotesEditorForm(
                Path.GetFileName(metadata.FullPath),
                metadata.NoteItems))
            {
                if (editor.ShowDialog(this) != DialogResult.OK)
                    return;

                metadata.NoteItems = editor.Notes;
                MetadataDatabase.RefreshNotesSummary(metadata);
                metadataDatabase.SaveNotes(
                    metadata.FileKey,
                    metadata.NoteItems);
                metadataDatabase.SaveMetadata(metadata);

                RefreshNoteCounts();
                RefreshMetadataPresentation();
                propertyGridControl_MetaData.Refresh();
                gridControl_Files.RefreshDataSource();
            }
        }

        private void RefreshNoteCounts()
        {
            noteCountsByPath = metadataDatabase.GetNoteCountsByPath();
        }

        private void RefreshMetadataPresentation()
        {
            metadataPresentationByPath =
                metadataDatabase.GetPresentationByPath();
            descriptionsByPath = metadataDatabase.GetDescriptionsByPath();

            foreach (Image image in metadataIconCache.Values)
                image.Dispose();
            metadataIconCache.Clear();
        }

        private void checkEdit_ShowDescriptionPreview_CheckedChanged(
            object sender,
            EventArgs e)
        {
            ApplyDescriptionPreviewSetting();
        }

        private void ApplyDescriptionPreviewSetting()
        {
            DevExpress.XtraGrid.Views.Grid.GridView detailsView =
                gridControl_Files.ViewCollection
                    .OfType<DevExpress.XtraGrid.Views.Grid.GridView>()
                    .FirstOrDefault(view => string.Equals(
                        view.Name,
                        "detailsGridView",
                        StringComparison.OrdinalIgnoreCase))
                ?? gridView1;

            detailsView.PreviewFieldName = "DVMSDescription";
            detailsView.OptionsView.AutoCalcPreviewLineCount = true;
            detailsView.PreviewLineCount = -1;
            detailsView.OptionsView.ShowPreview =
                checkEdit_ShowDescriptionPreview.Checked;
            detailsView.LayoutChanged();
        }

        private void gridControlExtension1_QueryLinkImage(
            object sender,
            QueryLinkImageEventArgs e)
        {
            if (e.Link == null ||
                string.IsNullOrWhiteSpace(e.Link.FullPath))
            {
                return;
            }

            MetadataPresentation presentation;
            if (!metadataPresentationByPath.TryGetValue(
                    e.Link.FullPath,
                    out presentation) ||
                string.IsNullOrWhiteSpace(presentation.CustomIconPath) ||
                !File.Exists(presentation.CustomIconPath))
            {
                return;
            }

            string cacheKey = presentation.CustomIconPath + "|" +
                e.ImageSize.Width + "x" + e.ImageSize.Height;

            Image image;
            if (!metadataIconCache.TryGetValue(cacheKey, out image))
            {
                try
                {
                    using (Image source = Image.FromFile(
                        presentation.CustomIconPath))
                    {
                        image = new Bitmap(
                            source,
                            Math.Max(1, e.ImageSize.Width),
                            Math.Max(1, e.ImageSize.Height));
                    }

                    metadataIconCache[cacheKey] = image;
                }
                catch (Exception exception)
                    when (exception is IOException ||
                          exception is UnauthorizedAccessException ||
                          exception is ArgumentException)
                {
                    return;
                }
            }

            e.Image = image;
        }

        private void DVMSFileManagerForm_MetadataFormClosed(
            object sender,
            FormClosedEventArgs e)
        {
            DevExpress.LookAndFeel.UserLookAndFeel.Default.StyleChanged -=
                UserLookAndFeel_StyleChanged;
            folderSizeCalculatorIsShuttingDown = true;

            CancellationTokenSource cancellation =
                folderSizeCancellation;
            folderSizeCancellation = null;
            if (cancellation != null)
            {
                try
                {
                    cancellation.Cancel();
                }
                catch (ObjectDisposedException)
                {
                }
                cancellation.Dispose();
            }

            folderSizeScanLimit.Dispose();

            foreach (Image image in metadataIconCache.Values)
                image.Dispose();
            metadataIconCache.Clear();
        }

        private sealed class FileSystemProperties
        {
            [Category("General"), DisplayName("Name")]
            public string Name { get; private set; }

            [Category("General"), DisplayName("Type")]
            public string Type { get; private set; }

            [Category("General"), DisplayName("File extension")]
            public string Extension { get; private set; }

            [Category("General"), DisplayName("Item kind")]
            public string ItemKind { get; private set; }

            [Category("Location"), DisplayName("Folder path")]
            public string FolderPath { get; private set; }

            [Category("Location"), DisplayName("Full path")]
            public string FullPath { get; private set; }

            [Category("File identity"), DisplayName("Volume name")]
            public string VolumeName { get; private set; }

            [Category("File identity"), DisplayName("Volume serial number")]
            public string VolumeSerialNumber { get; private set; }

            [Category("File identity"), DisplayName("Volume GUID path")]
            public string VolumeGuidPath { get; private set; }

            [Category("File identity"), DisplayName("File ID")]
            public string FileId { get; private set; }

            [Category("File identity"), DisplayName("Combined identity")]
            public string CombinedIdentity { get; private set; }

            [Category("Size"), DisplayName("Size")]
            public string Size { get; private set; }

            [Category("Size"), DisplayName("Size in bytes")]
            public long? SizeInBytes { get; private set; }

            [Category("Dates"), DisplayName("Date created")]
            public DateTime? DateCreated { get; private set; }

            [Category("Dates"), DisplayName("Date modified")]
            public DateTime? DateModified { get; private set; }

            [Category("Dates"), DisplayName("Date accessed")]
            public DateTime? DateAccessed { get; private set; }

            [Category("Security"), DisplayName("Owner")]
            public string Owner { get; private set; }

            [Category("Attributes"), DisplayName("Attributes")]
            public string Attributes { get; private set; }

            [Category("Attributes"), DisplayName("Read-only")]
            public bool? IsReadOnly { get; private set; }

            [Category("Attributes"), DisplayName("Hidden")]
            public bool? IsHidden { get; private set; }

            [Category("Attributes"), DisplayName("Offline status")]
            public string OfflineStatus { get; private set; }

            public static FileSystemProperties Create(
                DevExpress.Dialogs.Core.Items.ShellItemLink item)
            {
                string path = item.FullPath;
                bool isFile = !string.IsNullOrWhiteSpace(path) &&
                    File.Exists(path);
                bool isDirectory = !string.IsNullOrWhiteSpace(path) &&
                    Directory.Exists(path);

                var properties = new FileSystemProperties
                {
                    Name = item.Name,
                    Type = item.TypeName,
                    Extension = item.Extension,
                    ItemKind = item.IsFolder ? "Folder" : "File",
                    FullPath = path,
                    FolderPath = GetFolderPath(path, item.IsFolder),
                    SizeInBytes = item.Size,
                    Size = FormatFileSize(item.Size),
                    DateModified = item.DateModified
                };

                if (isFile || isDirectory)
                {
                    FileSystemInfo fileSystemInfo = isFile
                        ? (FileSystemInfo)new FileInfo(path)
                        : new DirectoryInfo(path);

                    try
                    {
                        properties.DateCreated =
                            fileSystemInfo.CreationTime;
                        properties.DateModified =
                            fileSystemInfo.LastWriteTime;
                        properties.DateAccessed =
                            fileSystemInfo.LastAccessTime;

                        FileAttributes attributes =
                            fileSystemInfo.Attributes;

                        properties.Attributes = attributes.ToString();
                        properties.IsReadOnly =
                            (attributes & FileAttributes.ReadOnly) != 0;
                        properties.IsHidden =
                            (attributes & FileAttributes.Hidden) != 0;
                        properties.OfflineStatus =
                            (attributes & FileAttributes.Offline) != 0
                                ? "Offline"
                                : "Available";
                    }
                    catch (IOException)
                    {
                    }
                    catch (UnauthorizedAccessException)
                    {
                    }

                    properties.Owner = GetFileSystemOwner(
                        path,
                        isDirectory);

                    FileIdentityInfo identity =
                        WindowsFileIdentity.TryRead(path);

                    if (identity != null)
                    {
                        properties.VolumeName = identity.VolumeName;
                        properties.VolumeSerialNumber =
                            identity.VolumeSerialNumber;
                        properties.VolumeGuidPath =
                            identity.VolumeGuidPath;
                        properties.FileId = identity.FileId;
                        properties.CombinedIdentity =
                            identity.CombinedIdentity;
                    }
                }

                return properties;
            }

            private static string GetFolderPath(
                string path,
                bool isFolder)
            {
                if (string.IsNullOrWhiteSpace(path))
                    return string.Empty;

                if (isFolder)
                {
                    var directory = new DirectoryInfo(path);
                    return directory.Parent == null
                        ? path
                        : directory.Parent.FullName;
                }

                return Path.GetDirectoryName(path) ?? string.Empty;
            }

            public static FileIdentityKey CreateIdentityKey(
                string path,
                string fileName)
            {
                FileIdentityInfo identity =
                    WindowsFileIdentity.TryRead(path);

                return new FileIdentityKey
                {
                    VolumeSerialNumber = identity?.VolumeSerialNumber,
                    WindowsFileId = identity?.FileId,
                    FullPath = path,
                    FileName = fileName
                };
            }

            private static string GetFileSystemOwner(
                string path,
                bool isDirectory)
            {
                try
                {
                    FileSystemSecurity security = isDirectory
                        ? (FileSystemSecurity)Directory.GetAccessControl(path)
                        : File.GetAccessControl(path);

                    IdentityReference owner = security.GetOwner(
                        typeof(NTAccount));

                    return owner == null
                        ? string.Empty
                        : owner.Value;
                }
                catch (Exception exception)
                    when (exception is IOException ||
                          exception is UnauthorizedAccessException ||
                          exception is SystemException)
                {
                    return string.Empty;
                }
            }

            private static string FormatFileSize(long? byteCount)
            {
                if (!byteCount.HasValue)
                    return string.Empty;

                double size = byteCount.Value;
                string[] units = { "bytes", "KB", "MB", "GB", "TB" };
                int unitIndex = 0;

                while (size >= 1024 && unitIndex < units.Length - 1)
                {
                    size /= 1024;
                    unitIndex++;
                }

                return unitIndex == 0
                    ? string.Format("{0:N0} {1}", size, units[unitIndex])
                    : string.Format("{0:N2} {1}", size, units[unitIndex]);
            }

            private sealed class FileIdentityInfo
            {
                public string VolumeName { get; set; }
                public string VolumeSerialNumber { get; set; }
                public string VolumeGuidPath { get; set; }
                public string FileId { get; set; }
                public string CombinedIdentity { get; set; }
            }

            private static class WindowsFileIdentity
            {
                private const uint FileReadAttributes = 0x0080;
                private const uint FileShareRead = 0x00000001;
                private const uint FileShareWrite = 0x00000002;
                private const uint FileShareDelete = 0x00000004;
                private const uint OpenExisting = 3;
                private const uint FileFlagBackupSemantics = 0x02000000;
                private const int FileIdInfoClass = 18;

                public static FileIdentityInfo TryRead(string path)
                {
                    try
                    {
                        string volumeRoot = GetVolumeRoot(path);
                        var identity = new FileIdentityInfo
                        {
                            VolumeGuidPath = GetVolumeGuidPath(volumeRoot)
                        };

                        PopulateVolumeInformation(volumeRoot, identity);

                        using (SafeFileHandle handle = CreateFile(
                            path,
                            FileReadAttributes,
                            FileShareRead | FileShareWrite | FileShareDelete,
                            IntPtr.Zero,
                            OpenExisting,
                            FileFlagBackupSemantics,
                            IntPtr.Zero))
                        {
                            if (handle == null || handle.IsInvalid)
                                return identity;

                            var nativeInfo = new FileIdInfo
                            {
                                FileId = new FileId128
                                {
                                    Identifier = new byte[16]
                                }
                            };

                            if (GetFileInformationByHandleEx(
                                    handle,
                                    FileIdInfoClass,
                                    ref nativeInfo,
                                    (uint)Marshal.SizeOf(typeof(FileIdInfo))))
                            {
                                identity.VolumeSerialNumber =
                                    nativeInfo.VolumeSerialNumber
                                        .ToString("X16");
                                identity.FileId = BitConverter.ToString(
                                        nativeInfo.FileId.Identifier)
                                    .Replace("-", string.Empty);
                            }
                            else
                            {
                                ByHandleFileInformation fallbackInfo;

                                if (GetFileInformationByHandle(
                                        handle,
                                        out fallbackInfo))
                                {
                                    identity.VolumeSerialNumber =
                                        fallbackInfo.VolumeSerialNumber
                                            .ToString("X8");

                                    ulong fallbackFileId =
                                        ((ulong)fallbackInfo.FileIndexHigh << 32) |
                                        fallbackInfo.FileIndexLow;

                                    identity.FileId =
                                        fallbackFileId.ToString("X16");
                                }
                            }

                            if (!string.IsNullOrEmpty(
                                    identity.VolumeSerialNumber) &&
                                !string.IsNullOrEmpty(identity.FileId))
                            {
                                identity.CombinedIdentity =
                                    identity.VolumeSerialNumber + ":" +
                                    identity.FileId;
                            }

                            return identity;
                        }
                    }
                    catch (Exception exception)
                        when (exception is IOException ||
                              exception is UnauthorizedAccessException ||
                              exception is SystemException)
                    {
                        return null;
                    }
                }

                private static string GetVolumeRoot(string path)
                {
                    var buffer = new StringBuilder(512);

                    return GetVolumePathName(
                            path,
                            buffer,
                            buffer.Capacity)
                        ? buffer.ToString()
                        : string.Empty;
                }

                private static void PopulateVolumeInformation(
                    string volumeRoot,
                    FileIdentityInfo identity)
                {
                    if (string.IsNullOrEmpty(volumeRoot))
                        return;

                    var volumeName = new StringBuilder(261);

                    uint serial;
                    uint maximumComponentLength;
                    uint fileSystemFlags;

                    if (GetVolumeInformation(
                            volumeRoot,
                            volumeName,
                            volumeName.Capacity,
                            out serial,
                            out maximumComponentLength,
                            out fileSystemFlags,
                            null,
                            0))
                    {
                        string label = volumeName.ToString();

                        identity.VolumeName = string.IsNullOrWhiteSpace(label)
                            ? volumeRoot.TrimEnd('\\')
                            : label + " (" + volumeRoot.TrimEnd('\\') + ")";

                        identity.VolumeSerialNumber =
                            serial.ToString("X8");
                    }
                    else
                    {
                        identity.VolumeName = volumeRoot.TrimEnd('\\');
                    }
                }

                private static string GetVolumeGuidPath(string volumeRoot)
                {
                    if (string.IsNullOrEmpty(volumeRoot))
                        return string.Empty;

                    var volumeGuid = new StringBuilder(64);

                    return GetVolumeNameForVolumeMountPoint(
                            volumeRoot,
                            volumeGuid,
                            volumeGuid.Capacity)
                        ? volumeGuid.ToString()
                        : string.Empty;
                }

                [StructLayout(LayoutKind.Sequential)]
                private struct FileId128
                {
                    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
                    public byte[] Identifier;
                }

                [StructLayout(LayoutKind.Sequential)]
                private struct FileIdInfo
                {
                    public ulong VolumeSerialNumber;
                    public FileId128 FileId;
                }

                [StructLayout(LayoutKind.Sequential)]
                private struct ByHandleFileInformation
                {
                    public uint FileAttributes;
                    public System.Runtime.InteropServices.ComTypes.FILETIME
                        CreationTime;
                    public System.Runtime.InteropServices.ComTypes.FILETIME
                        LastAccessTime;
                    public System.Runtime.InteropServices.ComTypes.FILETIME
                        LastWriteTime;
                    public uint VolumeSerialNumber;
                    public uint FileSizeHigh;
                    public uint FileSizeLow;
                    public uint NumberOfLinks;
                    public uint FileIndexHigh;
                    public uint FileIndexLow;
                }

                [DllImport("kernel32.dll",
                    CharSet = CharSet.Unicode,
                    SetLastError = true)]
                private static extern SafeFileHandle CreateFile(
                    string fileName,
                    uint desiredAccess,
                    uint shareMode,
                    IntPtr securityAttributes,
                    uint creationDisposition,
                    uint flagsAndAttributes,
                    IntPtr templateFile);

                [DllImport("kernel32.dll", SetLastError = true)]
                [return: MarshalAs(UnmanagedType.Bool)]
                private static extern bool GetFileInformationByHandleEx(
                    SafeFileHandle file,
                    int fileInformationClass,
                    ref FileIdInfo fileInformation,
                    uint bufferSize);

                [DllImport("kernel32.dll", SetLastError = true)]
                [return: MarshalAs(UnmanagedType.Bool)]
                private static extern bool GetFileInformationByHandle(
                    SafeFileHandle file,
                    out ByHandleFileInformation fileInformation);

                [DllImport("kernel32.dll",
                    CharSet = CharSet.Unicode,
                    SetLastError = true)]
                [return: MarshalAs(UnmanagedType.Bool)]
                private static extern bool GetVolumePathName(
                    string fileName,
                    StringBuilder volumePathName,
                    int bufferLength);

                [DllImport("kernel32.dll",
                    CharSet = CharSet.Unicode,
                    SetLastError = true)]
                [return: MarshalAs(UnmanagedType.Bool)]
                private static extern bool
                    GetVolumeNameForVolumeMountPoint(
                        string volumeMountPoint,
                        StringBuilder volumeName,
                        int bufferLength);

                [DllImport("kernel32.dll",
                    CharSet = CharSet.Unicode,
                    SetLastError = true)]
                [return: MarshalAs(UnmanagedType.Bool)]
                private static extern bool GetVolumeInformation(
                    string rootPathName,
                    StringBuilder volumeNameBuffer,
                    int volumeNameSize,
                    out uint volumeSerialNumber,
                    out uint maximumComponentLength,
                    out uint fileSystemFlags,
                    StringBuilder fileSystemNameBuffer,
                    int fileSystemNameSize);
            }
        }

        private sealed class FilesViewItem
        {
            public string DisplayName { get; set; }

            public ViewMode ViewMode { get; set; }

            public override string ToString()
            {
                return DisplayName;
            }
        }

        private void InitializeFilesViewSelector()
        {
            comboBoxEdit_WinExplorerView.Properties.TextEditStyle =
                TextEditStyles.DisableTextEditor;

            comboBoxEdit_WinExplorerView.Properties.NullText =
                "Select view...";

            comboBoxEdit_WinExplorerView.Properties.Items.Clear();

            foreach (FilesViewItem viewItem in
                     CreateFilesViewItems())
            {
                comboBoxEdit_WinExplorerView.Properties.Items.Add(
                    viewItem);
            }

            // Allow the combo box to change ViewMode at runtime. The
            // AlwaysUsePredefined behavior intentionally ignores later
            // ViewMode assignments and locks the initialization-time mode.
            gridControlExtension1.ViewModeBehavior =
                ViewModeBehavior.UsePredefinedIfOriginNotReaded;

            SelectFilesViewItem(
                gridControlExtension1.ViewMode);

            comboBoxEdit_WinExplorerView.SelectedIndexChanged +=
                comboBoxEdit_WinExplorerView_SelectedIndexChanged;

            gridControl_Files.DataSourceChanged -=
                gridControl_Files_DataSourceChanged;
            gridControl_Files.DataSourceChanged +=
                gridControl_Files_DataSourceChanged;

            gridControlExtension1.CurrentItemChanged -=
                gridControlExtension1_CurrentItemChangedForDetails;
            gridControlExtension1.CurrentItemChanged +=
                gridControlExtension1_CurrentItemChangedForDetails;

            detailsColumnsTimer.Tick -= detailsColumnsTimer_Tick;
            detailsColumnsTimer.Tick += detailsColumnsTimer_Tick;

            ScheduleDetailsViewColumnConfiguration();

        }

        private void gridControl_Files_DataSourceChanged(
            object sender,
            EventArgs e)
        {
            filesDataSourceVersion++;
            CancelFolderSizeCalculations();
            ScheduleDetailsViewColumnConfiguration();
        }

        private void gridControlExtension1_CurrentItemChangedForDetails(
            object sender,
            CurrentItemChangedEventArgs e)
        {
            fileCreatedDates.Clear();
            ScheduleDetailsViewColumnConfiguration();
        }

        private void ScheduleDetailsViewColumnConfiguration()
        {
            if (!gridControl_Files.IsHandleCreated)
                return;

            if (!detailsColumnsTimer.Enabled)
                detailsColumnsTimer.Start();
        }

        private void detailsColumnsTimer_Tick(
            object sender,
            EventArgs e)
        {
            detailsColumnsTimer.Stop();
            ConfigureDetailsViewColumns();
        }

        private void ConfigureDetailsViewColumns()
        {
            if (gridControl_Files.IsDisposed)
                return;

            DevExpress.XtraGrid.Views.Grid.GridView detailsView =
                gridControl_Files.ViewCollection
                    .OfType<DevExpress.XtraGrid.Views.Grid.GridView>()
                    .FirstOrDefault(view =>
                        string.Equals(
                            view.Name,
                            "detailsGridView",
                            StringComparison.OrdinalIgnoreCase))
                ?? gridView1;

            DevExpress.XtraGrid.Columns.GridColumn createdColumn =
                detailsView.Columns.ColumnByFieldName(
                    "DVMSDateCreated");

            if (createdColumn == null)
            {
                createdColumn =
                    new DevExpress.XtraGrid.Columns.GridColumn
                    {
                        Name = "detailsViewColumn_DateCreated",
                        FieldName = "DVMSDateCreated",
                        Caption = "Date Created",
                        UnboundDataType = typeof(DateTime)
                    };

                createdColumn.DisplayFormat.FormatType =
                    DevExpress.Utils.FormatType.DateTime;
                createdColumn.DisplayFormat.FormatString = "g";
                createdColumn.OptionsColumn.AllowEdit = false;

                detailsView.Columns.Add(createdColumn);
            }

            DevExpress.XtraGrid.Columns.GridColumn notesColumn =
                detailsView.Columns.ColumnByFieldName("DVMSNotes");

            if (notesColumn == null)
            {
                notesColumn = new DevExpress.XtraGrid.Columns.GridColumn
                {
                    Name = "detailsViewColumn_Notes",
                    FieldName = "DVMSNotes",
                    Caption = "Notes",
                    UnboundDataType = typeof(string),
                    Width = 72
                };
                notesColumn.OptionsColumn.AllowEdit = false;
                notesColumn.OptionsColumn.FixedWidth = true;
                notesColumn.AppearanceCell.TextOptions.HAlignment =
                    DevExpress.Utils.HorzAlignment.Center;
                detailsView.Columns.Add(notesColumn);
            }

            DevExpress.XtraGrid.Columns.GridColumn descriptionColumn =
                detailsView.Columns.ColumnByFieldName("DVMSDescription");

            if (descriptionColumn == null)
            {
                descriptionColumn = new DevExpress.XtraGrid.Columns.GridColumn
                {
                    Name = "detailsViewColumn_Description",
                    FieldName = "DVMSDescription",
                    Caption = "Description",
                    UnboundDataType = typeof(string),
                    Visible = false
                };
                descriptionColumn.OptionsColumn.AllowEdit = false;
                descriptionColumn.OptionsColumn.AllowShowHide = false;
                descriptionColumn.OptionsColumn.ShowInCustomizationForm = false;
                detailsView.Columns.Add(descriptionColumn);
            }

            DevExpress.XtraGrid.Columns.GridColumn displaySizeColumn =
                detailsView.Columns.ColumnByFieldName("DVMSDisplaySize");

            if (displaySizeColumn == null)
            {
                displaySizeColumn =
                    new DevExpress.XtraGrid.Columns.GridColumn
                    {
                        Name = "detailsViewColumn_DisplaySize",
                        FieldName = "DVMSDisplaySize",
                        Caption = "Size",
                        UnboundDataType = typeof(string),
                        Width = 100,
                        ToolTip =
                            "Folder sizes include accessible contents only; " +
                            "hidden, system, linked, and inaccessible items " +
                            "are excluded."
                    };
                displaySizeColumn.OptionsColumn.AllowEdit = false;
                detailsView.Columns.Add(displaySizeColumn);
            }

            detailsView.CustomUnboundColumnData -=
                detailsView_CustomUnboundColumnData;
            detailsView.CustomUnboundColumnData +=
                detailsView_CustomUnboundColumnData;

            detailsView.RowCellStyle -= detailsView_RowCellStyle;
            detailsView.RowCellStyle += detailsView_RowCellStyle;

            detailsView.MouseDown -= detailsView_NotesMouseDown;
            detailsView.MouseDown += detailsView_NotesMouseDown;

            detailsView.ColumnPositionChanged -=
                detailsView_ColumnPositionChanged;
            detailsView.ColumnPositionChanged +=
                detailsView_ColumnPositionChanged;

            detailsView.TopRowChanged -= detailsView_TopRowChanged;
            detailsView.TopRowChanged += detailsView_TopRowChanged;

            detailsView.CustomRowFilter -= detailsView_CustomRowFilter;
            detailsView.CustomRowFilter += detailsView_CustomRowFilter;

            DevExpress.XtraGrid.Columns.GridColumn iconColumn =
                detailsView.Columns.ColumnByFieldName("Icon");
            DevExpress.XtraGrid.Columns.GridColumn nameColumn =
                detailsView.Columns.ColumnByFieldName("Name");
            DevExpress.XtraGrid.Columns.GridColumn sizeColumn =
                detailsView.Columns.ColumnByFieldName("Size");
            DevExpress.XtraGrid.Columns.GridColumn typeColumn =
                detailsView.Columns.ColumnByFieldName("TypeName");
            DevExpress.XtraGrid.Columns.GridColumn modifiedColumn =
                detailsView.Columns.ColumnByFieldName("DateModified");

            configuringDetailsColumns = true;

            try
            {
                // This is deliberately the same minimal sequence used by the
                // successful Reorder Columns test button. Do not hide or
                // rebuild the assistant's Shell-managed columns here.
                SetDetailsColumn(iconColumn, string.Empty, 0);
                SetDetailsColumn(nameColumn, "Name", 1);
                if (sizeColumn != null)
                {
                    sizeColumn.Visible = false;
                    sizeColumn.OptionsColumn.AllowShowHide = false;
                    sizeColumn.OptionsColumn.ShowInCustomizationForm = false;
                }
                SetDetailsColumn(displaySizeColumn, "Size", 2);
                SetDetailsColumn(typeColumn, "Type", 3);
                SetDetailsColumn(modifiedColumn, "Date Modified", 4);
                SetDetailsColumn(createdColumn, "Date Created", 5);
                SetDetailsColumn(notesColumn, "Notes", 6);
                if (applicationSettings != null &&
                    applicationSettings.ShowPreviewColumn)
                {
                    SetDetailsColumn(descriptionColumn, "Preview", 7);
                }
                else
                {
                    descriptionColumn.Visible = false;
                }
                ApplyDescriptionPreviewSetting();
                QueueVisibleFolderSizeCalculations(detailsView);
            }
            finally
            {
                configuringDetailsColumns = false;
            }
        }

        private void detailsView_ColumnPositionChanged(
            object sender,
            EventArgs e)
        {
            if (!configuringDetailsColumns)
                ScheduleDetailsViewColumnConfiguration();
        }

        private void detailsView_TopRowChanged(object sender, EventArgs e)
        {
            QueueVisibleFolderSizeCalculations(
                sender as DevExpress.XtraGrid.Views.Grid.GridView);
        }

        private void detailsView_CustomRowFilter(
            object sender,
            DevExpress.XtraGrid.Views.Base.RowFilterEventArgs e)
        {
            if (applicationSettings == null ||
                applicationSettings.ShowEmptyRemovableDrives)
            {
                return;
            }

            var detailsView = sender as
                DevExpress.XtraGrid.Views.Grid.GridView;
            if (detailsView == null)
                return;

            int rowHandle = detailsView.GetRowHandle(e.ListSourceRow);
            var item = detailsView.GetRow(rowHandle) as
                DevExpress.Dialogs.Core.Items.ShellItemLink;
            if (item != null && IsDriveWithoutMedia(item.FullPath))
            {
                e.Visible = false;
                e.Handled = true;
            }
        }

        private void QueueVisibleFolderSizeCalculations(
            DevExpress.XtraGrid.Views.Grid.GridView detailsView)
        {
            if (detailsView == null ||
                gridControlExtension1.ViewMode != ViewMode.Details ||
                (applicationSettings != null &&
                 !applicationSettings.CalculateFolderSizes))
            {
                return;
            }

            int firstVisibleIndex = Math.Max(0, detailsView.TopRowIndex);

            // The visible rows plus a modest scroll-ahead buffer.
            for (int offset = 0; offset < 60; offset++)
            {
                int rowHandle = detailsView.GetVisibleRowHandle(
                    firstVisibleIndex + offset);
                if (rowHandle < 0)
                    break;

                var item = detailsView.GetRow(rowHandle) as
                    DevExpress.Dialogs.Core.Items.ShellItemLink;
                if (item == null || !item.IsFolder ||
                    string.IsNullOrWhiteSpace(item.FullPath) ||
                    !Directory.Exists(item.FullPath))
                {
                    continue;
                }

                StartFolderSizeCalculation(item.FullPath, detailsView);
            }
        }

        private async void StartFolderSizeCalculation(
            string folderPath,
            DevExpress.XtraGrid.Views.Grid.GridView detailsView)
        {
            // Drive roots displayed by This PC use volume capacity. Never
            // recursively enumerate an entire drive to determine its size.
            if (IsDriveRoot(folderPath))
            {
                long totalCapacity;
                if (TryGetDriveCapacity(folderPath, out totalCapacity))
                    folderSizesByPath[folderPath] = totalCapacity;

                return;
            }

            if (folderSizesByPath.ContainsKey(folderPath) ||
                !folderSizeCalculationsInProgress.Add(folderPath))
            {
                return;
            }

            CancellationTokenSource cancellation =
                folderSizeCancellation;
            if (cancellation == null ||
                folderSizeCalculatorIsShuttingDown)
            {
                folderSizeCalculationsInProgress.Remove(folderPath);
                return;
            }

            CancellationToken token = cancellation.Token;
            SemaphoreSlim scanLimit = folderSizeScanLimit;
            bool scanSlotAcquired = false;

            try
            {
                // Cancellation is deliberately checked without throwing.
                // Visual Studio otherwise breaks on the expected
                // OperationCanceledException whenever navigation changes.
                await scanLimit.WaitAsync();
                scanSlotAcquired = true;

                if (token.IsCancellationRequested)
                    return;

                long? folderSize = await Task.Run(
                    () => CalculateAccessibleFolderSize(folderPath, token));

                if (folderSize.HasValue &&
                    !token.IsCancellationRequested &&
                    !IsDisposed && detailsView != null &&
                    detailsView.GridControl != null &&
                    !detailsView.GridControl.IsDisposed)
                {
                    folderSizesByPath[folderPath] = folderSize.Value;
                    detailsView.RefreshData();
                }
            }
            catch (ObjectDisposedException)
            {
            }
            finally
            {
                folderSizeCalculationsInProgress.Remove(folderPath);
                if (scanSlotAcquired)
                {
                    try
                    {
                        scanLimit.Release();
                    }
                    catch (ObjectDisposedException)
                    {
                    }
                }
            }
        }

        private static long? CalculateAccessibleFolderSize(
            string rootPath,
            CancellationToken token)
        {
            long totalSize = 0;
            var folders = new Stack<string>();
            folders.Push(rootPath);

            while (folders.Count > 0)
            {
                if (token.IsCancellationRequested)
                    return null;

                string folder = folders.Pop();
                FileSystemInfo[] children;

                try
                {
                    children = new DirectoryInfo(folder)
                        .EnumerateFileSystemInfos()
                        .ToArray();
                }
                catch (Exception exception)
                    when (exception is IOException ||
                          exception is UnauthorizedAccessException ||
                          exception is System.Security.SecurityException)
                {
                    continue;
                }

                foreach (FileSystemInfo child in children)
                {
                    if (token.IsCancellationRequested)
                        return null;

                    try
                    {
                        FileAttributes attributes = child.Attributes;
                        const FileAttributes excludedAttributes =
                            FileAttributes.Hidden |
                            FileAttributes.System |
                            FileAttributes.ReparsePoint;

                        if ((attributes & excludedAttributes) != 0)
                            continue;

                        var file = child as FileInfo;
                        if (file != null)
                        {
                            checked
                            {
                                totalSize += file.Length;
                            }
                        }
                        else if (child is DirectoryInfo)
                        {
                            folders.Push(child.FullName);
                        }
                    }
                    catch (Exception exception)
                        when (exception is IOException ||
                              exception is UnauthorizedAccessException ||
                              exception is System.Security.SecurityException ||
                              exception is OverflowException)
                    {
                    }
                }
            }

            return totalSize;
        }

        private static bool IsDriveRoot(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return false;

            try
            {
                string root = Path.GetPathRoot(path);
                return !string.IsNullOrWhiteSpace(root) &&
                    string.Equals(
                        path.TrimEnd('\\', '/'),
                        root.TrimEnd('\\', '/'),
                        StringComparison.OrdinalIgnoreCase);
            }
            catch (Exception exception)
                when (exception is ArgumentException ||
                      exception is PathTooLongException ||
                      exception is NotSupportedException)
            {
                return false;
            }
        }

        private static bool TryGetDriveCapacity(
            string path,
            out long totalCapacity)
        {
            totalCapacity = 0;

            try
            {
                string root = Path.GetPathRoot(path);
                if (string.IsNullOrWhiteSpace(root))
                    return false;

                var drive = new DriveInfo(root);
                if (!drive.IsReady)
                    return false;

                totalCapacity = drive.TotalSize;
                return true;
            }
            catch (Exception exception)
                when (exception is IOException ||
                      exception is UnauthorizedAccessException ||
                      exception is ArgumentException ||
                      exception is System.Security.SecurityException)
            {
                return false;
            }
        }

        private static bool IsDriveWithoutMedia(string path)
        {
            if (!IsDriveRoot(path))
                return false;

            try
            {
                string root = Path.GetPathRoot(path);
                if (string.IsNullOrWhiteSpace(root))
                    return false;

                var drive = new DriveInfo(root);
                return drive.DriveType == DriveType.Removable &&
                    !drive.IsReady;
            }
            catch (Exception exception)
                when (exception is IOException ||
                      exception is UnauthorizedAccessException ||
                      exception is ArgumentException ||
                      exception is System.Security.SecurityException)
            {
                return false;
            }
        }

        private void CancelFolderSizeCalculations()
        {
            if (folderSizeCalculatorIsShuttingDown)
                return;

            CancellationTokenSource previousCancellation =
                folderSizeCancellation;
            folderSizeCancellation = new CancellationTokenSource();

            if (previousCancellation == null)
                return;

            try
            {
                previousCancellation.Cancel();
            }
            catch (ObjectDisposedException)
            {
            }

            previousCancellation.Dispose();
        }

        private void InvalidateFolderSizeCache()
        {
            CancelFolderSizeCalculations();
            folderSizesByPath.Clear();
        }

        private static string FormatByteCount(long? byteCount)
        {
            if (!byteCount.HasValue)
                return string.Empty;

            string[] units = { "bytes", "KB", "MB", "GB", "TB", "PB" };
            double size = byteCount.Value;
            int unitIndex = 0;

            while (size >= 1024 && unitIndex < units.Length - 1)
            {
                size /= 1024;
                unitIndex++;
            }

            return unitIndex == 0
                ? byteCount.Value.ToString("N0") + " " + units[unitIndex]
                : size.ToString(size >= 100 ? "N0" : "N1") +
                  " " + units[unitIndex];
        }

        private static void SetDetailsColumn(
            DevExpress.XtraGrid.Columns.GridColumn column,
            string caption,
            int visibleIndex)
        {
            if (column == null)
                return;

            column.Caption = caption;
            column.Visible = true;
            column.VisibleIndex = visibleIndex;
            column.OptionsColumn.AllowMove = false;
            column.OptionsColumn.AllowShowHide = false;
        }

        private void detailsView_CustomUnboundColumnData(
            object sender,
            DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            if (!e.IsGetData)
            {
                return;
            }

            var item = e.Row as
                DevExpress.Dialogs.Core.Items.ShellItemLink;

            if (item == null || string.IsNullOrWhiteSpace(item.FullPath))
                return;

            if (e.Column.FieldName == "DVMSNotes")
            {
                int noteCount;
                if (noteCountsByPath.TryGetValue(
                        item.FullPath,
                        out noteCount) && noteCount > 0)
                {
                    e.Value = "🗒 " +
                        (noteCount > 99 ? "99+" : noteCount.ToString());
                }

                return;
            }

            if (e.Column.FieldName == "DVMSDescription")
            {
                string description;
                if (descriptionsByPath.TryGetValue(
                        item.FullPath,
                        out description))
                {
                    e.Value = description;
                }

                return;
            }

            if (e.Column.FieldName == "DVMSDisplaySize")
            {
                if (IsDriveRoot(item.FullPath))
                {
                    if (IsDriveWithoutMedia(item.FullPath))
                    {
                        e.Value = "No media";
                        return;
                    }

                    long totalCapacity;
                    e.Value = TryGetDriveCapacity(
                        item.FullPath,
                        out totalCapacity)
                        ? FormatByteCount(totalCapacity)
                        : "Unavailable";
                    return;
                }

                if (!item.IsFolder)
                {
                    e.Value = FormatByteCount(item.Size);
                    return;
                }

                if (applicationSettings != null &&
                    !applicationSettings.CalculateFolderSizes)
                {
                    e.Value = string.Empty;
                    return;
                }

                long folderSize;
                if (folderSizesByPath.TryGetValue(
                        item.FullPath,
                        out folderSize))
                {
                    e.Value = FormatByteCount(folderSize);
                }
                else
                {
                    e.Value = "Calculating…";
                    StartFolderSizeCalculation(
                        item.FullPath,
                        sender as DevExpress.XtraGrid.Views.Grid.GridView);
                }

                return;
            }

            if (e.Column.FieldName != "DVMSDateCreated")
                return;

            DateTime? createdDate;

            if (!fileCreatedDates.TryGetValue(
                    item.FullPath,
                    out createdDate))
            {
                try
                {
                    if (File.Exists(item.FullPath))
                    {
                        createdDate = File.GetCreationTime(item.FullPath);
                    }
                    else if (Directory.Exists(item.FullPath))
                    {
                        createdDate = Directory.GetCreationTime(item.FullPath);
                    }
                }
                catch (IOException)
                {
                    createdDate = null;
                }
                catch (UnauthorizedAccessException)
                {
                    createdDate = null;
                }

                fileCreatedDates[item.FullPath] = createdDate;
            }

            e.Value = createdDate;
        }

        private void detailsView_RowCellStyle(
            object sender,
            DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            if (e.Column == null)
                return;

            var detailsView = sender as
                DevExpress.XtraGrid.Views.Grid.GridView;
            var item = detailsView?.GetRow(e.RowHandle) as
                DevExpress.Dialogs.Core.Items.ShellItemLink;

            if (item == null || string.IsNullOrWhiteSpace(item.FullPath))
                return;

            if (IsDriveWithoutMedia(item.FullPath))
            {
                e.Appearance.ForeColor =
                    DevExpress.LookAndFeel.DXSkinColors.ForeColors.DisabledText;
                e.Appearance.FontStyleDelta = FontStyle.Italic;
                return;
            }

            if (e.Column.FieldName == "DVMSDisplaySize")
            {
                ApplySizeCellColor(e, item);
                return;
            }

            if (e.Column.FieldName != "Name")
                return;

            MetadataPresentation presentation;
            if (!metadataPresentationByPath.TryGetValue(
                    item.FullPath,
                    out presentation))
            {
                return;
            }

            if (presentation.FileNameColorArgb.HasValue)
            {
                e.Appearance.ForeColor = Color.FromArgb(
                    presentation.FileNameColorArgb.Value);
            }

            if (presentation.IsFileNameBold)
                e.Appearance.FontStyleDelta = FontStyle.Bold;
        }

        private void ApplySizeCellColor(
            DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e,
            DevExpress.Dialogs.Core.Items.ShellItemLink item)
        {
            if (applicationSettings == null ||
                !applicationSettings.ColorizeSizes ||
                IsDriveRoot(item.FullPath))
            {
                return;
            }

            long? byteCount = null;
            if (item.IsFolder)
            {
                long folderSize;
                if (folderSizesByPath.TryGetValue(
                        item.FullPath,
                        out folderSize))
                {
                    byteCount = folderSize;
                }
            }
            else
            {
                byteCount = item.Size;
            }

            if (!byteCount.HasValue || byteCount.Value < 0)
                return;

            long smallLimit =
                applicationSettings.SmallSizeUpperLimitMb *
                1024L * 1024L;
            long mediumLimit =
                applicationSettings.MediumSizeUpperLimitMb *
                1024L * 1024L;

            if (byteCount.Value < smallLimit)
                e.Appearance.ForeColor =
                    applicationSettings.SmallSizeColor;
            else if (byteCount.Value < mediumLimit)
                e.Appearance.ForeColor =
                    applicationSettings.MediumSizeColor;
            else
                e.Appearance.ForeColor =
                    applicationSettings.LargeSizeColor;
        }

        private void detailsView_NotesMouseDown(
            object sender,
            MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            var detailsView = sender as
                DevExpress.XtraGrid.Views.Grid.GridView;
            if (detailsView == null)
                return;

            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo hit =
                detailsView.CalcHitInfo(e.Location);

            if (!hit.InRowCell ||
                hit.Column == null ||
                hit.Column.FieldName != "DVMSNotes")
            {
                return;
            }

            var item = detailsView.GetRow(hit.RowHandle) as
                DevExpress.Dialogs.Core.Items.ShellItemLink;
            if (item == null)
                return;

            DevExpress.Utils.DXMouseEventArgs mouseArgs =
                DevExpress.Utils.DXMouseEventArgs.GetMouseArgs(
                    gridControl_Files,
                    e);
            if (mouseArgs != null)
                mouseArgs.Handled = true;

            detailsView.FocusedRowHandle = hit.RowHandle;
            DisplayFileProperties(item);

            if (currentMetadata != null)
            {
                BeginInvoke(new Action(() =>
                    EditMetadataNotes(currentMetadata)));
            }
        }

        private static List<FilesViewItem>
        CreateFilesViewItems()
        {
            return new List<FilesViewItem>
            {
                new FilesViewItem
                {
                    DisplayName = "Details",
                    ViewMode = ViewMode.Details
                },

                new FilesViewItem
                {
                    DisplayName = "Tiles",
                    ViewMode = ViewMode.Tiles
                },

                new FilesViewItem
                {
                    DisplayName = "Small Icons",
                    ViewMode = ViewMode.SmallIcon
                },

                new FilesViewItem
                {
                    DisplayName = "Medium Icons",
                    ViewMode = ViewMode.MediumIcon
                },

                new FilesViewItem
                {
                    DisplayName = "Large Icons",
                    ViewMode = ViewMode.LargeIcon
                },

                new FilesViewItem
                {
                    DisplayName = "Extra Large Icons",
                    ViewMode = ViewMode.ExtraLargeIcon
                },

                new FilesViewItem
                {
                    DisplayName = "List",
                    ViewMode = ViewMode.List
                },

                new FilesViewItem
                {
                    DisplayName = "Content",
                    ViewMode = ViewMode.Content
                }
            };
        }

        private void comboBoxEdit_WinExplorerView_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            if (changingFilesView)
                return;

            FilesViewItem selectedView =
                comboBoxEdit_WinExplorerView.SelectedItem
                    as FilesViewItem;

            if (selectedView == null)
                return;

            ApplyFilesView(selectedView.ViewMode);
        }

        private void ApplyFilesView(
            ViewMode viewMode)
        {
            if (changingFilesView)
                return;

            try
            {
                changingFilesView = true;

                gridControlExtension1.ViewModeBehavior =
                    ViewModeBehavior.UsePredefinedIfOriginNotReaded;

                int iconSize = GetFilesViewIconSize(viewMode);

                gridControlExtension1.IconSize =
                    new Size(iconSize, iconSize);

                gridControlExtension1.ViewMode =
                    viewMode;
            }
            finally
            {
                changingFilesView = false;
            }

            SelectFilesViewItem(viewMode);
        }

        private static int GetFilesViewIconSize(
            ViewMode viewMode)
        {
            switch (viewMode)
            {
                case ViewMode.ExtraLargeIcon:
                    return 256;

                case ViewMode.LargeIcon:
                    return 96;

                case ViewMode.MediumIcon:
                case ViewMode.Tiles:
                    return 48;

                case ViewMode.Content:
                    return 32;

                case ViewMode.SmallIcon:
                case ViewMode.List:
                case ViewMode.Details:
                default:
                    return 16;
            }
        }


        private void SelectFilesViewItem(
    ViewMode viewMode)
        {
            if (comboBoxEdit_WinExplorerView == null)
                return;

            FilesViewItem matchingItem =
                comboBoxEdit_WinExplorerView.Properties.Items
                    .OfType<FilesViewItem>()
                    .FirstOrDefault(
                        item => item.ViewMode == viewMode);

            if (matchingItem == null)
                return;

            bool previousChangingState =
                changingFilesView;

            try
            {
                changingFilesView = true;

                comboBoxEdit_WinExplorerView.SelectedItem =
                    matchingItem;
            }
            finally
            {
                changingFilesView =
                    previousChangingState;
            }
        }




        #endregion File Grid Code


        #region Event Handlers

        private void treeList1_FocusedNodeChanged(object sender, DevExpress.XtraTreeList.FocusedNodeChangedEventArgs e)
        {

        }

        private void dockPanel_Preview_Click(object sender, EventArgs e)
        {

        }

        private void dockPanel_MetaData_Click(object sender, EventArgs e)
        {

        }

        private void barButtonItem_DisplaySpecialFolders_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DisplayConfiguredSpecialFolders();
        }

        private void barButtonItem_DisplayAllSpecialFolders_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DisplayAllWindowsSpecialFolders();
        }

        private void barButtonItem_GetExistingSpecialFolders_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DisplayExistingWindowsSpecialFolders();
        }


        #endregion Event Handlers

        private void breadCrumbEdit1_PathChanged(object sender, BreadCrumbPathChangedEventArgs e)
        {
            RecordVisitedFolder(e.Path);
            operationLogDatabase?.Log("Navigate Folder", e.Path);
            if (applicationSettings != null &&
                settingsDatabase != null &&
                !string.IsNullOrWhiteSpace(e.Path))
            {
                applicationSettings.LastFolder = e.Path;
                settingsDatabase.SaveLastFolder(e.Path);
            }
        }
    }


    #region Navigation Location Classes

    internal enum NavigationLocationType
    {
        Folder,
        SpecialFolder,
        ShellGuid
    }

    internal sealed class NavigationLocation
    {
        public string Id { get; set; }

        public string DisplayName { get; set; }

        public NavigationLocationType LocationType { get; set; }

        public string Path { get; set; }

        public Environment.SpecialFolder? SpecialFolder
        {
            get;
            set;
        }

        public Guid? ShellGuid { get; set; }

        public static NavigationLocation CreateSpecialFolder(
            string displayName,
            Environment.SpecialFolder folder,
            string path)
        {
            return new NavigationLocation
            {
                Id = "SpecialFolder:" + folder,
                DisplayName = displayName,
                LocationType =
                    NavigationLocationType.SpecialFolder,
                SpecialFolder = folder,
                Path = path
            };
        }

        public static NavigationLocation CreateShellLocation(
            string displayName,
            Guid shellGuid)
        {
            return new NavigationLocation
            {
                Id = "ShellGuid:" + shellGuid.ToString("D"),
                DisplayName = displayName,
                LocationType =
                    NavigationLocationType.ShellGuid,
                ShellGuid = shellGuid
            };
        }

        public override string ToString()
        {
            return DisplayName;
        }
    }

    #endregion Navigation Location Classes
}
