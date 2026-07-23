using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;



namespace c2flux
{
    public enum DatabasePathSelectionMode
    {
        None,
        MoveCurrentDatabase,
        UseExistingDatabase,
        CreateNewDatabase
    }

    public sealed class DatabaseMoveForm : Form
    {
        private readonly string _currentDatabasePath;

        public DatabaseMoveForm(AppLayout layout, string currentDatabasePath)
        {
            _currentDatabasePath = ScanHistoryService.NormalizeDatabasePath(currentDatabasePath);
            SelectedDatabasePath = _currentDatabasePath;
            SelectionMode = DatabasePathSelectionMode.None;

            AntdThemeService.Apply(layout);

            Text = LocalizationService.GetText("DatabaseBrowse.Title");
            Icon = AppResources.ApplicationIcon;
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(620, 280);
            MinimumSize = Size;
            MaximumSize = Size;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;

            AntdUI.Label labelCurrentPath = new AntdUI.Label
            {
                Text = LocalizationService.GetText("DatabaseBrowse.CurrentPath"),
                Location = new Point(20, 18),
                Size = new Size(580, 24),
                TextAlign = ContentAlignment.MiddleLeft
            };

            AntdUI.Input textBoxCurrentDatabasePath = new AntdUI.Input
            {
                Location = new Point(20, 44),
                Size = new Size(580, 25),
                Text = _currentDatabasePath,
                ReadOnly = true
            };

            AntdUI.Label labelHint = new AntdUI.Label
            {
                Text = LocalizationService.GetText("DatabaseBrowse.Hint"),
                Location = new Point(20, 82),
                Size = new Size(580, 42),
                TextAlign = ContentAlignment.TopLeft
            };

            AntdUI.Button buttonMoveCurrentDatabase = new AntdUI.Button
            {
                Text = LocalizationService.GetText("DatabaseBrowse.MoveCurrent"),
                Location = new Point(20, 138),
                Size = new Size(580, 32),
                Type = AntdUI.TTypeMini.Default,
                Enabled = File.Exists(_currentDatabasePath)
            };
            buttonMoveCurrentDatabase.Click += buttonMoveCurrentDatabase_Click;

            AntdUI.Button buttonUseExistingDatabase = new AntdUI.Button
            {
                Text = LocalizationService.GetText("DatabaseBrowse.UseExisting"),
                Location = new Point(20, 178),
                Size = new Size(580, 32),
                Type = AntdUI.TTypeMini.Default
            };
            buttonUseExistingDatabase.Click += buttonUseExistingDatabase_Click;

            AntdUI.Button buttonCreateNewDatabase = new AntdUI.Button
            {
                Text = LocalizationService.GetText("DatabaseBrowse.CreateNew"),
                Location = new Point(20, 218),
                Size = new Size(475, 32),
                Type = AntdUI.TTypeMini.Default
            };
            buttonCreateNewDatabase.Click += buttonCreateNewDatabase_Click;

            AntdUI.Button buttonCancel = new AntdUI.Button
            {
                Text = LocalizationService.GetText("Common.Cancel"),
                Location = new Point(505, 218),
                Size = new Size(95, 32),
                Type = AntdUI.TTypeMini.Default,
                DialogResult = DialogResult.Cancel
            };

            Controls.Add(labelCurrentPath);
            Controls.Add(textBoxCurrentDatabasePath);
            Controls.Add(labelHint);
            Controls.Add(buttonMoveCurrentDatabase);
            Controls.Add(buttonUseExistingDatabase);
            Controls.Add(buttonCreateNewDatabase);
            Controls.Add(buttonCancel);

            CancelButton = buttonCancel;

            AntdThemeService.Apply(this, layout);
        }

        public string SelectedDatabasePath { get; private set; }

        public DatabasePathSelectionMode SelectionMode { get; private set; }

        private void buttonMoveCurrentDatabase_Click(object sender, EventArgs e)
        {
            string selectedDatabasePath = SelectNewDatabasePath(
                LocalizationService.GetText("DatabaseBrowse.MoveSelectTitle"));

            if (string.IsNullOrWhiteSpace(selectedDatabasePath))
                return;

            DialogResult confirmationResult = MessageBox.Show(
                this,
                LocalizationService.GetText("DatabaseBrowse.MoveConfirm"),
                Text,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirmationResult != DialogResult.Yes)
                return;

            SelectedDatabasePath = selectedDatabasePath;
            SelectionMode = DatabasePathSelectionMode.MoveCurrentDatabase;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void buttonUseExistingDatabase_Click(object sender, EventArgs e)
        {
            using OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = LocalizationService.GetText("DatabaseBrowse.UseExistingSelectTitle"),
                Filter = LocalizationService.GetText("DatabaseBrowse.Filter"),
                InitialDirectory = GetExistingDirectoryPath(_currentDatabasePath),
                CheckFileExists = true,
                CheckPathExists = true,
                Multiselect = false
            };

            if (openFileDialog.ShowDialog(this) != DialogResult.OK)
                return;

            string selectedDatabasePath = ScanHistoryService.NormalizeDatabasePath(
                openFileDialog.FileName);

            if (string.Equals(
                    _currentDatabasePath,
                    selectedDatabasePath,
                    StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            SelectedDatabasePath = selectedDatabasePath;
            SelectionMode = DatabasePathSelectionMode.UseExistingDatabase;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void buttonCreateNewDatabase_Click(object sender, EventArgs e)
        {
            string selectedDatabasePath = SelectNewDatabasePath(
                LocalizationService.GetText("DatabaseBrowse.CreateNewSelectTitle"));

            if (string.IsNullOrWhiteSpace(selectedDatabasePath))
                return;

            SelectedDatabasePath = selectedDatabasePath;
            SelectionMode = DatabasePathSelectionMode.CreateNewDatabase;
            DialogResult = DialogResult.OK;
            Close();
        }

        private string SelectNewDatabasePath(string title)
        {
            while (true)
            {
                using SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Title = title,
                    Filter = LocalizationService.GetText("DatabaseBrowse.Filter"),
                    FileName = Path.GetFileName(_currentDatabasePath),
                    InitialDirectory = GetExistingDirectoryPath(_currentDatabasePath),
                    OverwritePrompt = false,
                    AddExtension = true,
                    DefaultExt = "db"
                };

                if (saveFileDialog.ShowDialog(this) != DialogResult.OK)
                    return null;

                string selectedDatabasePath = ScanHistoryService.NormalizeDatabasePath(
                    saveFileDialog.FileName);

                if (File.Exists(selectedDatabasePath))
                {
                    DialogResult existingFileResult = MessageBox.Show(
                        this,
                        LocalizationService.GetText("DatabaseBrowse.TargetExists"),
                        Text,
                        MessageBoxButtons.RetryCancel,
                        MessageBoxIcon.Warning);

                    if (existingFileResult == DialogResult.Retry)
                        continue;

                    return null;
                }

                return selectedDatabasePath;
            }
        }

        private static string GetExistingDirectoryPath(string filePath)
        {
            try
            {
                string directoryPath = Path.GetDirectoryName(filePath);

                if (!string.IsNullOrWhiteSpace(directoryPath) &&
                    Directory.Exists(directoryPath))
                {
                    return directoryPath;
                }
            }
            catch
            {
            }

            return AppContext.BaseDirectory;
        }
    }
}
