using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace c2flux
{
    public sealed class ScanHistoryForm : Form
    {
        private readonly AppSettings _settings;
        private readonly ScanHistoryCompareService _compareService = new ScanHistoryCompareService();

        private Panel panelTop;
        private Panel panelBottom;
        private AntdUI.Label labelBaselineScan;
        private AntdUI.Label labelCompareScan;
        private ComboBox comboBoxBaselineScan;
        private ComboBox comboBoxCompareScan;
        private AntdUI.Button buttonCompare;
        private AntdUI.Button buttonClose;
        private AntdUI.Button buttonRefresh;
        private AntdUI.Label labelStatus;
        private TabControl tabControlResults;
        private TabPage tabPageScans;
        private TabPage tabPageOverview;
        private TabPage tabPageSummary;
        private TabPage tabPageFolderGrowth;
        private TabPage tabPageNewFiles;
        private TabPage tabPageChangedFiles;
        private TabPage tabPageDeletedFiles;
        private DataGridView dataGridViewScans;
        private ScanHistoryGrowthOverviewControl growthOverviewControl;
        private DataGridView dataGridViewSummary;
        private DataGridView dataGridViewFolderGrowth;
        private DataGridView dataGridViewNewFiles;
        private DataGridView dataGridViewChangedFiles;
        private DataGridView dataGridViewDeletedFiles;

        private List<ScanHistoryInfo> scanHistoryInfos = new List<ScanHistoryInfo>();
        private ScanHistoryComparisonResult currentComparisonResult;

        public ScanHistoryForm(AppSettings settings)
        {
            _settings = settings ?? new AppSettings();

            AntdThemeService.Apply(_settings.Layout);
            InitializeComponent();
            Shown += ScanHistoryForm_Shown;
            AntdThemeService.Apply(this, _settings.Layout);
            AntdThemeService.ApplyTable(dataGridViewScans);
            growthOverviewControl.ApplyTheme();
            AntdThemeService.ApplyTable(dataGridViewSummary);
            AntdThemeService.ApplyTable(dataGridViewFolderGrowth);
            AntdThemeService.ApplyTable(dataGridViewNewFiles);
            AntdThemeService.ApplyTable(dataGridViewChangedFiles);
            AntdThemeService.ApplyTable(dataGridViewDeletedFiles);
            ApplyScanHistoryAntdUITheme();
        }

        private void InitializeComponent()
        {
            Text = LocalizationService.GetText("ScanHistory.Title");
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(940, 560);
            Size = new Size(1120, 700);
            ShowIcon = false;

            Color backgroundPrimary = AntdThemeService.BackgroundPrimary;
            Color backgroundSecondary = AntdThemeService.BackgroundSecondary;

            panelTop = new Panel
            {
                Dock = DockStyle.Top,
                Height = 102,
                Padding = new Padding(16, 12, 16, 8),
                BackColor = backgroundSecondary
            };

            panelBottom = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 52,
                Padding = new Padding(16, 8, 16, 10),
                BackColor = backgroundSecondary
            };

            labelBaselineScan = new AntdUI.Label
            {
                Name = "labelBaselineScan",
                Text = LocalizationService.GetText("ScanHistory.BaselineScan"),
                Location = new Point(16, 16),
                Size = new Size(110, 24),
                TextAlign = ContentAlignment.MiddleLeft
            };

            comboBoxBaselineScan = CreateScanComboBox("comboBoxBaselineScan", 132, 14);

            labelCompareScan = new AntdUI.Label
            {
                Name = "labelCompareScan",
                Text = LocalizationService.GetText("ScanHistory.CompareScan"),
                Location = new Point(16, 52),
                Size = new Size(110, 24),
                TextAlign = ContentAlignment.MiddleLeft
            };

            comboBoxCompareScan = CreateScanComboBox("comboBoxCompareScan", 132, 50);

            buttonCompare = new AntdUI.Button
            {
                Name = "buttonCompare",
                Text = LocalizationService.GetText("ScanHistory.Compare"),
                Location = new Point(704, 14),
                Size = new Size(112, 28),
                Type = AntdUI.TTypeMini.Default
            };
            buttonCompare.Click += buttonCompare_Click;

            buttonRefresh = new AntdUI.Button
            {
                Name = "buttonRefresh",
                Text = LocalizationService.GetText("ScanHistory.Refresh"),
                Location = new Point(704, 50),
                Size = new Size(112, 28),
                Type = AntdUI.TTypeMini.Default
            };
            buttonRefresh.Click += buttonRefresh_Click;

            labelStatus = new AntdUI.Label
            {
                Name = "labelStatus",
                Text = string.Empty,
                Location = new Point(832, 16),
                Size = new Size(240, 60),
                TextAlign = ContentAlignment.MiddleLeft
            };

            tabControlResults = new ScanHistoryTabControl
            {
                Dock = DockStyle.Fill,
                Padding = new Point(12, 4)
            };

            tabPageScans = CreateTabPage("tabPageScans", LocalizationService.GetText("ScanHistory.Scans"));
            tabPageOverview = CreateTabPage("tabPageOverview", LocalizationService.GetText("ScanHistory.Overview"));
            tabPageSummary = CreateTabPage("tabPageSummary", LocalizationService.GetText("ScanHistory.Summary"));
            tabPageFolderGrowth = CreateTabPage("tabPageFolderGrowth", LocalizationService.GetText("ScanHistory.FolderGrowth"));
            tabPageNewFiles = CreateTabPage("tabPageNewFiles", LocalizationService.GetText("ScanHistory.NewFiles"));
            tabPageChangedFiles = CreateTabPage("tabPageChangedFiles", LocalizationService.GetText("ScanHistory.ChangedFiles"));
            tabPageDeletedFiles = CreateTabPage("tabPageDeletedFiles", LocalizationService.GetText("ScanHistory.DeletedFiles"));

            dataGridViewScans = CreateGrid("dataGridViewScans");
            growthOverviewControl = new ScanHistoryGrowthOverviewControl
            {
                Name = "growthOverviewControl"
            };
            growthOverviewControl.PathActivated += growthOverviewControl_PathActivated;
            dataGridViewSummary = CreateGrid("dataGridViewSummary");
            dataGridViewFolderGrowth = CreateGrid("dataGridViewFolderGrowth");
            dataGridViewNewFiles = CreateGrid("dataGridViewNewFiles");
            dataGridViewChangedFiles = CreateGrid("dataGridViewChangedFiles");
            dataGridViewDeletedFiles = CreateGrid("dataGridViewDeletedFiles");

            ConfigureScansGrid();
            ConfigureSummaryGrid();
            ConfigureFolderGrowthGrid();
            ConfigureFileChangeGrid(dataGridViewNewFiles);
            ConfigureFileChangeGrid(dataGridViewChangedFiles);
            ConfigureFileChangeGrid(dataGridViewDeletedFiles);

            tabPageScans.Controls.Add(dataGridViewScans);
            tabPageOverview.Controls.Add(growthOverviewControl);
            tabPageSummary.Controls.Add(dataGridViewSummary);
            tabPageFolderGrowth.Controls.Add(dataGridViewFolderGrowth);
            tabPageNewFiles.Controls.Add(dataGridViewNewFiles);
            tabPageChangedFiles.Controls.Add(dataGridViewChangedFiles);
            tabPageDeletedFiles.Controls.Add(dataGridViewDeletedFiles);

            tabControlResults.TabPages.Add(tabPageScans);
            tabControlResults.TabPages.Add(tabPageOverview);
            tabControlResults.TabPages.Add(tabPageSummary);
            tabControlResults.TabPages.Add(tabPageFolderGrowth);
            tabControlResults.TabPages.Add(tabPageNewFiles);
            tabControlResults.TabPages.Add(tabPageChangedFiles);
            tabControlResults.TabPages.Add(tabPageDeletedFiles);

            buttonClose = new AntdUI.Button
            {
                Name = "buttonClose",
                Text = LocalizationService.GetText("Common.Close"),
                Anchor = AnchorStyles.Right | AnchorStyles.Top,
                Location = new Point(1014, 10),
                Size = new Size(90, 30),
                DialogResult = DialogResult.OK,
                Type = AntdUI.TTypeMini.Default
            };

            panelTop.Controls.Add(labelBaselineScan);
            panelTop.Controls.Add(comboBoxBaselineScan);
            panelTop.Controls.Add(labelCompareScan);
            panelTop.Controls.Add(comboBoxCompareScan);
            panelTop.Controls.Add(buttonCompare);
            panelTop.Controls.Add(buttonRefresh);
            panelTop.Controls.Add(labelStatus);
            panelBottom.Controls.Add(buttonClose);

            Panel resultsHostPanel = AntdThemeService.CreateTableHost(
                tabControlResults,
                backgroundPrimary,
                0,
                0);

            Controls.Add(resultsHostPanel);
            Controls.Add(panelBottom);
            Controls.Add(panelTop);

            AcceptButton = buttonCompare;
            CancelButton = buttonClose;

            Resize += ScanHistoryForm_Resize;
            ScanHistoryForm_Resize(this, EventArgs.Empty);
        }

        private ComboBox CreateScanComboBox(string name, int left, int top)
        {
            ComboBox comboBox = new ScanHistoryComboBox
            {
                Name = name,
                Location = new Point(left, top),
                Size = new Size(552, 26),
                DisplayMember = nameof(ScanHistoryInfo.DisplayName),
                Font = SystemFonts.MessageBoxFont
            };

            return comboBox;
        }

        private static TabPage CreateTabPage(string name, string text)
        {
            TabPage tabPage = new TabPage
            {
                Name = name,
                Text = text
            };

            AntdThemeService.ConfigureTablePage(
                tabPage,
                AntdThemeService.BackgroundPrimary);

            return tabPage;
        }

        private static DataGridView CreateGrid(string name)
        {
            DataGridView grid = new DataGridView
            {
                Name = name,
                Dock = DockStyle.Fill,
                AllowDrop = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToOrderColumns = false,
                AllowUserToResizeRows = false,
                AutoGenerateColumns = false,
                BackgroundColor = AntdThemeService.BackgroundSecondary,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                ColumnHeadersHeight = 28,
                EditMode = DataGridViewEditMode.EditProgrammatically,
                MultiSelect = false,
                ReadOnly = true,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            grid.MouseMove += grid_MouseMove;
            grid.CellMouseMove += grid_CellMouseMove;
            grid.ColumnHeaderMouseClick += grid_ColumnHeaderMouseClick;

            return grid;
        }

        private static void grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (sender is DataGridView grid && e.Button != MouseButtons.None)
            {
                grid.Capture = false;
            }
        }

        private static void grid_CellMouseMove(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (sender is DataGridView grid && Control.MouseButtons != MouseButtons.None)
            {
                grid.Capture = false;
            }
        }

                private void ApplyScanHistoryAntdUITheme()
        {
            Color backgroundPrimary = AntdThemeService.BackgroundPrimary;
            Color backgroundSecondary = AntdThemeService.BackgroundSecondary;
            Color textPrimary = AntdThemeService.TextPrimary;

            panelTop.BackColor = backgroundPrimary;
            panelBottom.BackColor = backgroundPrimary;

            if (tabControlResults is ScanHistoryTabControl scanHistoryTabControl)
            {
                scanHistoryTabControl.ApplyTheme(backgroundPrimary, backgroundSecondary, textPrimary);
            }

            ApplyScanComboBoxTheme(comboBoxBaselineScan, backgroundPrimary, textPrimary);
            ApplyScanComboBoxTheme(comboBoxCompareScan, backgroundPrimary, textPrimary);
        }

        private static void ApplyScanComboBoxTheme(ComboBox comboBox, Color backColor, Color foreColor)
        {
            if (comboBox == null)
                return;

            comboBox.BackColor = backColor;
            comboBox.ForeColor = foreColor;
            comboBox.FlatStyle = FlatStyle.Flat;
            comboBox.DrawMode = DrawMode.OwnerDrawFixed;
            comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox.ItemHeight = 20;
            comboBox.Invalidate();
        }

        private void ConfigureScansGrid()
        {
            AddTextColumn(dataGridViewScans, "CreatedLocal", LocalizationService.GetText("ScanHistory.Date"), 150);
            AddTextColumn(dataGridViewScans, "RootPath", LocalizationService.GetText("ScanHistory.RootPath"), 360);
            AddTextColumn(dataGridViewScans, "RootSize", LocalizationService.GetText("ScanHistory.TotalSize"), 100);
            AddTextColumn(dataGridViewScans, "FileCount", LocalizationService.GetText("Common.Files"), 80);
            AddTextColumn(dataGridViewScans, "DirectoryCount", LocalizationService.GetText("Common.Folders"), 80);
        }

        private void ConfigureSummaryGrid()
        {
            AddTextColumn(dataGridViewSummary, "Metric", LocalizationService.GetText("ScanHistory.Metric"), 260);
            AddTextColumn(dataGridViewSummary, "Value", LocalizationService.GetText("ScanHistory.Value"), 420);
        }

        private void ConfigureFolderGrowthGrid()
        {
            AddTextColumn(dataGridViewFolderGrowth, "Path", LocalizationService.GetText("ScanHistory.Path"), 420);
            AddTextColumn(dataGridViewFolderGrowth, "BaselineSize", LocalizationService.GetText("ScanHistory.BaselineSize"), 120);
            AddTextColumn(dataGridViewFolderGrowth, "CompareSize", LocalizationService.GetText("ScanHistory.CompareSize"), 120);
            AddTextColumn(dataGridViewFolderGrowth, "Delta", LocalizationService.GetText("ScanHistory.Delta"), 110);
            AddTextColumn(dataGridViewFolderGrowth, "NewFileCount", LocalizationService.GetText("ScanHistory.NewFiles"), 90);
            AddTextColumn(dataGridViewFolderGrowth, "ChangedFileCount", LocalizationService.GetText("ScanHistory.ChangedFiles"), 110);
        }

        private void ConfigureFileChangeGrid(DataGridView grid)
        {
            AddTextColumn(grid, "Path", LocalizationService.GetText("ScanHistory.Path"), 420);
            AddTextColumn(grid, "BaselineSize", LocalizationService.GetText("ScanHistory.BaselineSize"), 120);
            AddTextColumn(grid, "CompareSize", LocalizationService.GetText("ScanHistory.CompareSize"), 120);
            AddTextColumn(grid, "Delta", LocalizationService.GetText("ScanHistory.Delta"), 110);
            AddTextColumn(grid, "LastWriteTimeUtc", LocalizationService.GetText("ScanHistory.LastWriteUtc"), 150);
        }

        private static void AddTextColumn(DataGridView grid, string dataPropertyName, string headerText, int width)
        {
            DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn
            {
                DataPropertyName = dataPropertyName,
                HeaderText = headerText,
                Name = dataPropertyName,
                SortMode = DataGridViewColumnSortMode.Programmatic,
                Width = width
            };

            grid.Columns.Add(column);
        }

        private async void ScanHistoryForm_Shown(object sender, EventArgs e)
        {
            Shown -= ScanHistoryForm_Shown;

            if (!ScanHistoryService.IsDatabaseMaintenanceRequired())
            {
                LoadScanHistoryInfos();
                return;
            }

            using DatabaseMaintenanceForm maintenanceForm =
                new DatabaseMaintenanceForm(_settings.Layout);

            Enabled = false;
            maintenanceForm.Show(this);
            maintenanceForm.Refresh();

            try
            {
                IReadOnlyList<ScanHistoryInfo> loadedScanHistoryInfos =
                    await Task.Run(() => ScanHistoryService.List());

                BindScanHistoryInfos(loadedScanHistoryInfos);
            }
            catch (Exception exception)
            {
                MessageBox.Show(
                    this,
                    exception.Message,
                    LocalizationService.GetText("Common.Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                maintenanceForm.Close();
                Enabled = true;
                Activate();
            }
        }

        private void LoadScanHistoryInfos()
        {
            BindScanHistoryInfos(ScanHistoryService.List());
        }

        private void BindScanHistoryInfos(
            IReadOnlyList<ScanHistoryInfo> loadedScanHistoryInfos)
        {
            scanHistoryInfos = loadedScanHistoryInfos
                .OrderBy(
                    scanHistoryInfo => System.IO.Path.GetPathRoot(scanHistoryInfo.RootPath) ?? scanHistoryInfo.RootPath,
                    StringComparer.OrdinalIgnoreCase)
                .ThenByDescending(scanHistoryInfo => scanHistoryInfo.CreatedUtc)
                .ToList();

            dataGridViewScans.DataSource = scanHistoryInfos
                .Select(scanHistoryInfo => new ScanHistoryListRow(scanHistoryInfo))
                .ToList();

            comboBoxBaselineScan.DataSource = scanHistoryInfos.ToList();
            comboBoxCompareScan.DataSource = scanHistoryInfos.ToList();

            if (scanHistoryInfos.Count >= 2)
            {
                comboBoxCompareScan.SelectedIndex = 0;
                comboBoxBaselineScan.SelectedIndex = 1;
            }

            buttonCompare.Enabled = scanHistoryInfos.Count >= 2;
            labelStatus.Text = string.Format(
                LocalizationService.GetText("ScanHistory.ScanCount"),
                scanHistoryInfos.Count);
        }

        private async void buttonCompare_Click(object sender, EventArgs e)
        {
            if (comboBoxBaselineScan.SelectedItem is not ScanHistoryInfo baselineScanInfo ||
                comboBoxCompareScan.SelectedItem is not ScanHistoryInfo compareScanInfo)
                return;

            if (string.Equals(baselineScanInfo.FilePath, compareScanInfo.FilePath, StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show(
                    this,
                    LocalizationService.GetText("ScanHistory.SelectDifferentScans"),
                    Text,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (baselineScanInfo.CreatedUtc >= compareScanInfo.CreatedUtc)
            {
                MessageBox.Show(
                    this,
                    LocalizationService.GetText("ScanHistory.InvalidChronologicalOrder"),
                    Text,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            Cursor oldCursor = Cursor.Current;
            string originalTitle = LocalizationService.GetText("ScanHistory.Title");
            int lastDisplayedPercent = -1;

            Progress<int> progress = new Progress<int>(percent =>
            {
                int normalizedPercent = Math.Max(0, Math.Min(100, percent));

                if (normalizedPercent == lastDisplayedPercent)
                    return;

                lastDisplayedPercent = normalizedPercent;
                Text = LocalizationService.Format(
                    "ScanHistory.CompareProgressTitle",
                    normalizedPercent);
            });

            Cursor.Current = Cursors.WaitCursor;
            buttonCompare.Enabled = false;
            buttonRefresh.Enabled = false;
            comboBoxBaselineScan.Enabled = false;
            comboBoxCompareScan.Enabled = false;

            try
            {
                currentComparisonResult = await Task.Run(
                    () => _compareService.Compare(
                        baselineScanInfo,
                        compareScanInfo,
                        progress));

                BindComparisonResult(currentComparisonResult);
                tabControlResults.SelectedTab = tabPageOverview;
            }
            finally
            {
                Text = originalTitle;
                Cursor.Current = oldCursor;
                buttonCompare.Enabled = scanHistoryInfos.Count >= 2;
                buttonRefresh.Enabled = true;
                comboBoxBaselineScan.Enabled = true;
                comboBoxCompareScan.Enabled = true;
            }
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            LoadScanHistoryInfos();
        }

        private void BindComparisonResult(ScanHistoryComparisonResult result)
        {
            growthOverviewControl.BindResult(result);
            dataGridViewSummary.DataSource = CreateSummaryRows(result);
            dataGridViewFolderGrowth.DataSource = result.FolderGrowth.ToList();
            dataGridViewNewFiles.DataSource = result.NewFiles.ToList();
            dataGridViewChangedFiles.DataSource = result.ChangedFiles.ToList();
            dataGridViewDeletedFiles.DataSource = result.DeletedFiles.ToList();

            ClearSortGlyphs(dataGridViewSummary);
            ClearSortGlyphs(dataGridViewFolderGrowth);
            ClearSortGlyphs(dataGridViewNewFiles);
            ClearSortGlyphs(dataGridViewChangedFiles);
            ClearSortGlyphs(dataGridViewDeletedFiles);
        }

        private void growthOverviewControl_PathActivated(
            object sender,
            GrowthOverviewPathEventArgs e)
        {
            if (currentComparisonResult == null || string.IsNullOrWhiteSpace(e.Path))
                return;

            List<ScanHistoryFileChange> matchingFiles;

            if (e.IsFile)
            {
                matchingFiles = currentComparisonResult.NewFiles
                    .Where(item => string.Equals(
                        item.Path,
                        e.Path,
                        StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
            else
            {
                string normalizedFolderPath = e.Path.TrimEnd(
                    System.IO.Path.DirectorySeparatorChar,
                    System.IO.Path.AltDirectorySeparatorChar);
                string folderPrefix = normalizedFolderPath +
                                      System.IO.Path.DirectorySeparatorChar;

                matchingFiles = currentComparisonResult.NewFiles
                    .Where(item =>
                        string.Equals(
                            item.ParentPath,
                            normalizedFolderPath,
                            StringComparison.OrdinalIgnoreCase) ||
                        item.Path.StartsWith(
                            folderPrefix,
                            StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            dataGridViewNewFiles.DataSource = matchingFiles;
            ClearSortGlyphs(dataGridViewNewFiles);
            tabControlResults.SelectedTab = tabPageNewFiles;

            if (matchingFiles.Count > 0)
            {
                dataGridViewNewFiles.ClearSelection();
                dataGridViewNewFiles.Rows[0].Selected = true;
                dataGridViewNewFiles.CurrentCell = dataGridViewNewFiles.Rows[0].Cells[0];
            }
        }

        private static List<SummaryRow> CreateSummaryRows(ScanHistoryComparisonResult result)
        {
            return new List<SummaryRow>
            {
                new SummaryRow("Baseline scan", result.BaselineScan.DisplayName),
                new SummaryRow("Compare scan", result.CompareScan.DisplayName),
                new SummaryRow("Baseline size", SizeFormatter.Format(result.BaselineSizeBytes)),
                new SummaryRow("Compare size", SizeFormatter.Format(result.CompareSizeBytes)),
                new SummaryRow("Size delta", FormatSignedSize(result.SizeDeltaBytes)),
                new SummaryRow("Baseline files", result.BaselineFileCount.ToString()),
                new SummaryRow("Compare files", result.CompareFileCount.ToString()),
                new SummaryRow("New files", result.NewFileCount.ToString()),
                new SummaryRow("Deleted files", result.DeletedFileCount.ToString()),
                new SummaryRow("Changed files", result.ChangedFileCount.ToString())
            };
        }

        private static string FormatSignedSize(long bytes)
        {
            if (bytes > 0)
                return "+" + SizeFormatter.Format(bytes);

            if (bytes < 0)
                return "-" + SizeFormatter.Format(Math.Abs(bytes));

            return SizeFormatter.Format(0);
        }

        private static void grid_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (sender is not DataGridView grid || e.ColumnIndex < 0 || e.ColumnIndex >= grid.Columns.Count)
                return;

            DataGridViewColumn column = grid.Columns[e.ColumnIndex];
            ListSortDirection direction = column.HeaderCell.SortGlyphDirection == SortOrder.Ascending
                ? ListSortDirection.Descending
                : ListSortDirection.Ascending;

            SortGrid(grid, column, direction);
        }

        private static void SortGrid(DataGridView grid, DataGridViewColumn column, ListSortDirection direction)
        {
            string propertyName = column.DataPropertyName;

            if (grid.DataSource is List<ScanHistoryListRow> scanRows)
            {
                grid.DataSource = SortRows(scanRows, propertyName, direction);
            }
            else if (grid.DataSource is List<SummaryRow> summaryRows)
            {
                grid.DataSource = SortRows(summaryRows, propertyName, direction);
            }
            else if (grid.DataSource is List<ScanHistoryFolderGrowth> folderGrowthRows)
            {
                grid.DataSource = SortRows(folderGrowthRows, propertyName, direction);
            }
            else if (grid.DataSource is List<ScanHistoryFileChange> fileChangeRows)
            {
                grid.DataSource = SortRows(fileChangeRows, propertyName, direction);
            }
            else
            {
                return;
            }

            ClearSortGlyphs(grid);
            column = grid.Columns[column.Name];
            column.HeaderCell.SortGlyphDirection = direction == ListSortDirection.Ascending
                ? SortOrder.Ascending
                : SortOrder.Descending;
        }

        private static List<T> SortRows<T>(
            IEnumerable<T> rows,
            string propertyName,
            ListSortDirection direction)
        {
            List<T> sortedRows = rows.ToList();

            sortedRows.Sort((left, right) =>
            {
                object leftValue = GetPropertyValue(left, propertyName);
                object rightValue = GetPropertyValue(right, propertyName);
                int compareResult = CompareValues(leftValue, rightValue);
                return direction == ListSortDirection.Ascending ? compareResult : -compareResult;
            });

            return sortedRows;
        }

        private static object GetPropertyValue(object instance, string propertyName)
        {
            if (instance == null || string.IsNullOrWhiteSpace(propertyName))
                return null;

            return TypeDescriptor.GetProperties(instance)[propertyName]?.GetValue(instance);
        }

        private static int CompareValues(object leftValue, object rightValue)
        {
            if (ReferenceEquals(leftValue, rightValue))
                return 0;

            if (leftValue == null)
                return -1;

            if (rightValue == null)
                return 1;

            if (leftValue is IComparable comparableLeft)
                return comparableLeft.CompareTo(rightValue);

            return string.Compare(
                leftValue.ToString(),
                rightValue.ToString(),
                StringComparison.CurrentCultureIgnoreCase);
        }

        private static void ClearSortGlyphs(DataGridView grid)
        {
            foreach (DataGridViewColumn column in grid.Columns)
            {
                column.HeaderCell.SortGlyphDirection = SortOrder.None;
            }
        }

        private void ScanHistoryForm_Resize(object sender, EventArgs e)
        {
            int topButtonLeft = Math.Max(704, ClientSize.Width - 416);
            buttonCompare.Left = topButtonLeft;
            buttonRefresh.Left = topButtonLeft;
            labelStatus.Left = topButtonLeft + 128;
            labelStatus.Width = Math.Max(180, ClientSize.Width - labelStatus.Left - 24);

            int comboWidth = Math.Max(320, topButtonLeft - comboBoxBaselineScan.Left - 20);
            comboBoxBaselineScan.Width = comboWidth;
            comboBoxCompareScan.Width = comboWidth;

            buttonClose.Left = Math.Max(16, panelBottom.ClientSize.Width - buttonClose.Width - 16);
        }

        private sealed class ScanHistoryComboBox : ComboBox
        {
            public ScanHistoryComboBox()
            {
                DropDownStyle = ComboBoxStyle.DropDownList;
                DrawMode = DrawMode.OwnerDrawFixed;
                FlatStyle = FlatStyle.Flat;
                IntegralHeight = false;
                ItemHeight = 20;
            }

            protected override void OnDrawItem(DrawItemEventArgs e)
            {
                if (e.Index < 0 || e.Index >= Items.Count)
                    return;

                bool selected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;

                Color backgroundColor = selected
                    ? SystemColors.Highlight
                    : BackColor;

                Color textColor = selected
                    ? SystemColors.HighlightText
                    : ForeColor;

                using (SolidBrush backgroundBrush = new SolidBrush(backgroundColor))
                {
                    e.Graphics.FillRectangle(backgroundBrush, e.Bounds);
                }

                Rectangle iconBounds = new Rectangle(
                    e.Bounds.Left + 6,
                    e.Bounds.Top + Math.Max(0, (e.Bounds.Height - 14) / 2),
                    14,
                    14);

                DrawScanHistoryIcon(e.Graphics, iconBounds, selected);

                Rectangle textBounds = new Rectangle(
                    e.Bounds.Left + 28,
                    e.Bounds.Top,
                    Math.Max(0, e.Bounds.Width - 34),
                    e.Bounds.Height);

                string text = GetItemText(Items[e.Index]);

                TextRenderer.DrawText(
                    e.Graphics,
                    text,
                    Font,
                    textBounds,
                    textColor,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

                e.DrawFocusRectangle();
            }

            private static void DrawScanHistoryIcon(Graphics graphics, Rectangle bounds, bool selected)
            {
                Color outlineColor = selected
                    ? SystemColors.HighlightText
                    : AntdThemeService.TextPrimary;

                Color accentColor = selected
                    ? SystemColors.HighlightText
                    : AntdThemeService.Accent;

                Rectangle databaseBounds = new Rectangle(bounds.Left, bounds.Top + 2, bounds.Width - 1, bounds.Height - 4);

                using (Pen outlinePen = new Pen(outlineColor))
                using (Pen accentPen = new Pen(accentColor))
                {
                    graphics.DrawEllipse(
                        accentPen,
                        databaseBounds.Left,
                        databaseBounds.Top,
                        databaseBounds.Width,
                        4);

                    graphics.DrawLine(
                        outlinePen,
                        databaseBounds.Left,
                        databaseBounds.Top + 2,
                        databaseBounds.Left,
                        databaseBounds.Bottom - 2);

                    graphics.DrawLine(
                        outlinePen,
                        databaseBounds.Right,
                        databaseBounds.Top + 2,
                        databaseBounds.Right,
                        databaseBounds.Bottom - 2);

                    graphics.DrawArc(
                        outlinePen,
                        databaseBounds.Left,
                        databaseBounds.Bottom - 4,
                        databaseBounds.Width,
                        4,
                        0,
                        180);

                    graphics.DrawLine(
                        accentPen,
                        databaseBounds.Left + 3,
                        databaseBounds.Top + 7,
                        databaseBounds.Right - 3,
                        databaseBounds.Top + 7);
                }
            }
        }

        private sealed class ScanHistoryTabControl : TabControl
        {
            private Color _backgroundPrimary = Color.FromArgb(32, 32, 32);
            private Color _backgroundSecondary = Color.FromArgb(45, 45, 45);
            private Color _textPrimary = Color.White;

            public ScanHistoryTabControl()
            {
                DrawMode = TabDrawMode.OwnerDrawFixed;
                SizeMode = TabSizeMode.Normal;
                ItemSize = new Size(112, 28);
                Padding = new Point(12, 4);
            }

            public void ApplyTheme(Color backgroundPrimary, Color backgroundSecondary, Color textPrimary)
            {
                _backgroundPrimary = backgroundPrimary;
                _backgroundSecondary = backgroundSecondary;
                _textPrimary = textPrimary;

                BackColor = _backgroundPrimary;
                ForeColor = _textPrimary;
                DrawMode = TabDrawMode.OwnerDrawFixed;

                foreach (TabPage tabPage in TabPages)
                {
                    tabPage.BackColor = _backgroundPrimary;
                    tabPage.ForeColor = _textPrimary;
                }

                Invalidate();
            }

            protected override void OnDrawItem(DrawItemEventArgs e)
            {
                if (e.Index < 0 || e.Index >= TabPages.Count)
                    return;

                bool selected = e.Index == SelectedIndex;
                Rectangle tabBounds = GetTabRect(e.Index);
                tabBounds.Inflate(-1, 0);

                Color tabBackColor = selected
                    ? _backgroundPrimary
                    : _backgroundSecondary;

                Color borderColor = ControlPaint.Light(_backgroundSecondary, 0.25f);
                Color accentColor = AntdThemeService.Accent;

                using (SolidBrush backgroundBrush = new SolidBrush(tabBackColor))
                {
                    e.Graphics.FillRectangle(backgroundBrush, tabBounds);
                }

                using (Pen borderPen = new Pen(borderColor))
                {
                    e.Graphics.DrawRectangle(borderPen, tabBounds);
                }

                if (selected)
                {
                    using Pen accentPen = new Pen(accentColor, 2);
                    e.Graphics.DrawLine(
                        accentPen,
                        tabBounds.Left + 1,
                        tabBounds.Bottom - 2,
                        tabBounds.Right - 1,
                        tabBounds.Bottom - 2);
                }

                TextRenderer.DrawText(
                    e.Graphics,
                    TabPages[e.Index].Text,
                    Font,
                    tabBounds,
                    _textPrimary,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
            }

            protected override void OnPaintBackground(PaintEventArgs e)
            {
                using SolidBrush backgroundBrush = new SolidBrush(_backgroundPrimary);
                e.Graphics.FillRectangle(backgroundBrush, ClientRectangle);
            }
        }

        private sealed class DatabaseMaintenanceForm : Form
        {
            public DatabaseMaintenanceForm(AppLayout layout)
            {
                AntdThemeService.Apply(layout);

                Text = LocalizationService.GetText("ScanHistory.DatabaseMaintenanceTitle");
                StartPosition = FormStartPosition.CenterParent;
                FormBorderStyle = FormBorderStyle.FixedDialog;
                ClientSize = new Size(420, 126);
                MinimumSize = Size;
                MaximumSize = Size;
                ControlBox = false;
                ShowInTaskbar = false;
                BackColor = AntdThemeService.BackgroundPrimary;
                ForeColor = AntdThemeService.TextPrimary;

                AntdUI.Label labelMessage = new AntdUI.Label
                {
                    Name = "labelMessage",
                    Text = LocalizationService.GetText("ScanHistory.DatabaseMaintenanceMessage"),
                    Location = new Point(20, 18),
                    Size = new Size(380, 48),
                    TextAlign = ContentAlignment.MiddleLeft
                };

                ProgressBar progressBar = new ProgressBar
                {
                    Name = "progressBar",
                    Location = new Point(20, 80),
                    Size = new Size(380, 22),
                    Style = ProgressBarStyle.Marquee,
                    MarqueeAnimationSpeed = 30
                };

                Controls.Add(labelMessage);
                Controls.Add(progressBar);

                AntdThemeService.Apply(this, layout);
            }
        }

        private sealed class ScanHistoryListRow
        {
            public ScanHistoryListRow(ScanHistoryInfo scanHistoryInfo)
            {
                CreatedLocal = scanHistoryInfo.CreatedUtc.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
                RootPath = scanHistoryInfo.RootPath;
                RootSize = SizeFormatter.Format(scanHistoryInfo.RootSizeBytes);
                FileCount = scanHistoryInfo.FileCount;
                DirectoryCount = scanHistoryInfo.DirectoryCount;
            }

            public string CreatedLocal { get; }
            public string RootPath { get; }
            public string RootSize { get; }
            public int FileCount { get; }
            public int DirectoryCount { get; }
        }

        private sealed class SummaryRow
        {
            public SummaryRow(string metric, string value)
            {
                Metric = metric;
                Value = value;
            }

            public string Metric { get; }
            public string Value { get; }
        }
    }
}
