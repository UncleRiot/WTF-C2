using System;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace c2flux
{
    public static class AntdThemeService
    {
        private sealed class AntdUiCultureLocalization : AntdUI.ILocalization
        {
            public string GetLocalizedString(string key)
            {
                CultureInfo culture = CultureInfo.CurrentUICulture;
                DateTimeFormatInfo dateTimeFormat = culture.DateTimeFormat;

                return key switch
                {
                    "ID" => culture.Name,
                    "Cancel" => LocalizationService.GetText("Search.Cancel"),
                    "OK" => "OK",
                    "Now" => culture.TwoLetterISOLanguageName == "de" ? "Jetzt" : "Now",
                    "ToDay" => culture.TwoLetterISOLanguageName == "de" ? "Heute" : "Today",
                    "YearFormat" => "yyyy",
                    "MonthFormat" => "MMMM",
                    "Mon" => dateTimeFormat.GetShortestDayName(DayOfWeek.Monday),
                    "Tue" => dateTimeFormat.GetShortestDayName(DayOfWeek.Tuesday),
                    "Wed" => dateTimeFormat.GetShortestDayName(DayOfWeek.Wednesday),
                    "Thu" => dateTimeFormat.GetShortestDayName(DayOfWeek.Thursday),
                    "Fri" => dateTimeFormat.GetShortestDayName(DayOfWeek.Friday),
                    "Sat" => dateTimeFormat.GetShortestDayName(DayOfWeek.Saturday),
                    "Sun" => dateTimeFormat.GetShortestDayName(DayOfWeek.Sunday),
                    _ => null
                };
            }
        }

        private static bool _useDarkMode = IsWindowsAppDarkModeEnabled();
        private static readonly ToolTip MainToolTip = new ToolTip
        {
            AutoPopDelay = 10000,
            InitialDelay = 500,
            ReshowDelay = 100,
            ShowAlways = true
        };

        public const int HorizontalMargin = 15;
        public const int MainToolbarHeight = 44;
        public const int MainToolbarSecondRowTop = 44;
        public const int MainToolbarItemSpacing = -4;
        public const int MainStatusTextSpacing = 8;

        // ============================================================
        // Tabellen - allgemeingültig
        // ============================================================

        // Tabellenkopf
        public const int TableHeaderHeight = 32;

        // Tabellenzeile
        public const int TableRowHeight = 30;

        // Innenabstände der AntdUI-Table
        public const int TableGap = 8;
        public const int TableCellGap = 6;

        // Tatsächlich sichtbarer Tabellenkopf für klassische DataGridView-Tabellen
        public const int DataGridViewHeaderHeight = 40;

        // Tatsächlich sichtbare Tabellenzeile für klassische DataGridView-Tabellen
        public const int DataGridViewRowHeight = 36;

        // Innenabstand für Tabellenkopf und Tabellenzellen
        public const int TableCellHorizontalPadding = 8;

        // Fortschrittsbalken in Tabellenzellen
        public const int TableProgressWidth = 100;
        public const int TableProgressHeight = 16;
        public const int TableProgressRadius = 4;

        // ============================================================
        // Analysis
        // ============================================================

        // Tabs File types / Largest files
        public const int AnalysisTabWidth = 120;
        public const int AnalysisTabHeight = 30;
        public const int AnalysisTabHorizontalPadding = 12;
        public const int AnalysisTabVerticalPadding = 4;
        public const float AnalysisTabAccentHeight = 2F;

        // Tabelle File types - Spaltenbreiten in Prozent
        public const int AnalysisFileTypeColumnWidthPercent = 14;
        public const int AnalysisUsageColumnWidthPercent = 18;
        public const int AnalysisSizeGbColumnWidthPercent = 22;
        public const int AnalysisSizeMbColumnWidthPercent = 46;

        // Tabelle Largest files - Spaltenbreiten in Prozent
        public const int AnalysisLargestFilesNameColumnWidthPercent = 22;
        public const int AnalysisLargestFilesFormattedSizeColumnWidthPercent = 14;
        public const int AnalysisLargestFilesSizeBytesColumnWidthPercent = 14;
        public const int AnalysisLargestFilesLastWriteTimeColumnWidthPercent = 20;
        public const int AnalysisLargestFilesFullPathColumnWidthPercent = 30;

        // Usage-Füllbalken
        public const int AnalysisUsageBarHorizontalPadding = 12;
        public const int AnalysisUsageBarHeight = 16;
        public const int AnalysisUsageBarRadius = 4;

        // ============================================================
        // Search
        // ============================================================

        // Fenster
        public const int SearchWindowWidth = 1100;
        public const int SearchWindowHeight = 720;
        public const int SearchWindowMinimumWidth = 860;
        public const int SearchWindowMinimumHeight = 560;

        // Hauptinhalt
        public const int SearchContentPaddingLeft = 12;
        public const int SearchContentPaddingTop = 12;
        public const int SearchContentPaddingRight = 12;
        public const int SearchContentPaddingBottom = 12;

        // Beschriftungen
        public const int SearchLabelMarginLeft = 3;
        public const int SearchLabelMarginTop = 7;
        public const int SearchLabelMarginRight = 8;
        public const int SearchLabelMarginBottom = 3;

        // Auswahlfeld Search source
        public const int SearchSourceSelectHeight = 32;
        public const int SearchSourceSelectDropDownItemHeight = 28;
        public const int SearchSourceSelectRadius = 6;

        // Eingabefeld Search text
        public const int SearchTextInputHeight = 32;
        public const int SearchTextInputRadius = 6;

        // Auswahlfeld Match mode
        public const int SearchMatchModeSelectHeight = 32;
        public const int SearchMatchModeSelectDropDownItemHeight = 28;
        public const int SearchMatchModeSelectRadius = 6;

        // Auswahlfeld Saved scan
        public const int SearchSavedScanSelectHeight = 32;
        public const int SearchSavedScanSelectDropDownItemHeight = 28;
        public const int SearchSavedScanSelectRadius = 6;

        // Button Filters
        public const int SearchFiltersButtonHeight = 32;
        public const int SearchFiltersButtonRadius = 6;

        // Eingabefeld File types
        public const int SearchFileTypesInputHeight = 32;
        public const int SearchFileTypesInputRadius = 6;

        // Checkboxen Filter
        public const int SearchFilterCheckboxHeight = 28;

        // Eingabefeld Minimum size (MB)
        public const int SearchMinimumSizeInputHeight = 32;
        public const int SearchMinimumSizeInputRadius = 6;

        // Eingabefeld Maximum size (MB)
        public const int SearchMaximumSizeInputHeight = 32;
        public const int SearchMaximumSizeInputRadius = 6;

        // Datumsfeld Modified after
        public const int SearchModifiedAfterDatePickerHeight = 32;
        public const int SearchModifiedAfterDatePickerRadius = 6;

        // Datumsfeld Modified before
        public const int SearchModifiedBeforeDatePickerHeight = 32;
        public const int SearchModifiedBeforeDatePickerRadius = 6;

        // Button Reset filters
        public const int SearchResetFiltersButtonWidth = 110;
        public const int SearchResetFiltersButtonHeight = 32;
        public const int SearchResetFiltersButtonRadius = 6;

        // Filterbereich
        public const int SearchFiltersPaddingLeft = 8;
        public const int SearchFiltersPaddingTop = 4;
        public const int SearchFiltersPaddingRight = 0;
        public const int SearchFiltersPaddingBottom = 8;

        // Tabelle
        public const int SearchResultsHeaderHeight = TableHeaderHeight;
        public const int SearchResultsRowHeight = TableRowHeight;

        // Status und Buttons
        public const int SearchFooterButtonWidth = 92;
        public const int SearchFooterButtonHeight = 32;
        public const int SearchProgressHeight = 4;

        // ============================================================
        // Storage History
        // ============================================================

        // Fenster
        public const int StorageHistoryWindowWidth = 1120;
        public const int StorageHistoryWindowHeight = 650;
        public const int StorageHistoryWindowMinimumWidth = 900;
        public const int StorageHistoryWindowMinimumHeight = 500;

        // Kopfbereich
        public const int StorageHistoryHeaderPadding = 8;
        public const int StorageHistoryHeaderRowHeight = 36;

        // Text Drive
        public const int StorageHistoryPathLabelWidth = 58;
        public const int StorageHistoryPathLabelHeight = 32;
        public const int StorageHistoryPathLabelMarginRight = 0;

        // Auswahlfeld Drive
        public const int StorageHistoryPathSelectWidth = 260;
        public const int StorageHistoryPathSelectHeight = 32;
        public const int StorageHistoryPathSelectDropDownWidth = 260;
        public const int StorageHistoryPathSelectItemHeight = 24;
        public const int StorageHistoryPathSelectMarginLeft = 0;

        // Text Display
        public const int StorageHistoryDisplayLabelWidth = 60;
        public const int StorageHistoryDisplayLabelHeight = 32;

        // Auswahlfeld Display
        public const int StorageHistoryDisplaySelectWidth = 150;
        public const int StorageHistoryDisplaySelectHeight = 32;

        // Text Intensity
        public const int StorageHistoryIntensityLabelWidth = 70;
        public const int StorageHistoryIntensityLabelHeight = 32;

        // Slider Intensity
        public const int StorageHistoryIntensitySliderWidth = 140;
        public const int StorageHistoryIntensitySliderHeight = 32;
        public const int StorageHistoryIntensitySliderLineSize = 4;
        public const int StorageHistoryIntensitySliderDotSize = 10;
        public const int StorageHistoryIntensitySliderDotSizeActive = 12;

        // Text Intensity value
        public const int StorageHistoryIntensityValueLabelWidth = 48;
        public const int StorageHistoryIntensityValueLabelHeight = 32;

        // Button Delete history
        public const int StorageHistoryDeleteButtonWidth = 112;
        public const int StorageHistoryDeleteButtonHeight = 32;

        // Button Close
        public const int StorageHistoryCloseButtonWidth = 90;
        public const int StorageHistoryCloseButtonHeight = 32;

        // Tabelle
        public const int StorageHistoryGridHeaderHeight = 32;
        public const int StorageHistoryGridRowHeight = 28;

        // Aufteilung Tabelle / Diagramm
        public const int StorageHistoryEmbeddedGridWidth = 416;
        public const int StorageHistoryWindowGridWidth = 416;
        public const int StorageHistoryWindowGridMinimumWidth = 280;
        public const int StorageHistoryWindowChartMinimumWidth = 320;

        // ============================================================
        // Settings - Dialog
        // ============================================================

        // Settings-Fenster
        public const int SettingsDialogWidth = 520;
        public const int SettingsDialogHeight = 454;

        // Tab General
        public const int SettingsDialogGeneralTabLeft = 18;
        public const int SettingsDialogGeneralTabTop = 16;
        public const int SettingsDialogGeneralTabWidth = 80;
        public const int SettingsDialogGeneralTabHeight = 32;

        // Tab Export
        public const int SettingsDialogExportTabLeft = 102;
        public const int SettingsDialogExportTabTop = 16;
        public const int SettingsDialogExportTabWidth = 80;
        public const int SettingsDialogExportTabHeight = 32;

        // Tab Colors
        public const int SettingsDialogColorsTabLeft = 210;
        public const int SettingsDialogColorsTabTop = 16;
        public const int SettingsDialogColorsTabWidth = 92;
        public const int SettingsDialogColorsTabHeight = 32;

        // Tab UI
        public const int SettingsDialogUiTabLeft = 186;
        public const int SettingsDialogUiTabTop = 16;
        public const int SettingsDialogUiTabWidth = 80;
        public const int SettingsDialogUiTabHeight = 32;

        // Tab Statistics
        public const int SettingsDialogStatisticsTabLeft = 270;
        public const int SettingsDialogStatisticsTabTop = 16;
        public const int SettingsDialogStatisticsTabWidth = 100;
        public const int SettingsDialogStatisticsTabHeight = 32;

        // Tab Logging
        public const int SettingsDialogLoggingTabLeft = 374;
        public const int SettingsDialogLoggingTabTop = 16;
        public const int SettingsDialogLoggingTabWidth = 100;
        public const int SettingsDialogLoggingTabHeight = 32;

        // Inhaltsbereich
        public const int SettingsDialogPageHostLeft = 18;
        public const int SettingsDialogPageHostTop = 54;
        public const int SettingsDialogPageHostWidth = 484;
        public const int SettingsDialogPageHostHeight = 338;

        // Button OK
        public const int SettingsDialogOkButtonLeft = 312;
        public const int SettingsDialogOkButtonTop = 406;
        public const int SettingsDialogOkButtonWidth = 90;
        public const int SettingsDialogOkButtonHeight = 32;

        // Button Cancel
        public const int SettingsDialogCancelButtonLeft = 412;
        public const int SettingsDialogCancelButtonTop = 406;
        public const int SettingsDialogCancelButtonWidth = 90;
        public const int SettingsDialogCancelButtonHeight = 32;

        // ============================================================
        // Settings - General
        // ============================================================

        // Checkbox Show files in tree
        public const int SettingsGeneralShowFilesCheckboxLeft = 24;
        public const int SettingsGeneralShowFilesCheckboxTop = 24;
        public const int SettingsGeneralShowFilesCheckboxWidth = 420;
        public const int SettingsGeneralShowFilesCheckboxHeight = 24;

        // Checkbox Skip reparse points
        public const int SettingsGeneralSkipReparsePointsCheckboxLeft = 24;
        public const int SettingsGeneralSkipReparsePointsCheckboxTop = 60;
        public const int SettingsGeneralSkipReparsePointsCheckboxWidth = 420;
        public const int SettingsGeneralSkipReparsePointsCheckboxHeight = 24;

        // Checkbox Show partition panel
        public const int SettingsGeneralShowPartitionPanelCheckboxLeft = 24;
        public const int SettingsGeneralShowPartitionPanelCheckboxTop = 96;
        public const int SettingsGeneralShowPartitionPanelCheckboxWidth = 420;
        public const int SettingsGeneralShowPartitionPanelCheckboxHeight = 24;

        // Checkbox Start elevated
        public const int SettingsGeneralStartElevatedCheckboxLeft = 24;
        public const int SettingsGeneralStartElevatedCheckboxTop = 132;
        public const int SettingsGeneralStartElevatedCheckboxWidth = 420;
        public const int SettingsGeneralStartElevatedCheckboxHeight = 24;

        // Checkbox Show elevation prompt
        public const int SettingsGeneralShowElevationPromptCheckboxLeft = 24;
        public const int SettingsGeneralShowElevationPromptCheckboxTop = 168;
        public const int SettingsGeneralShowElevationPromptCheckboxWidth = 420;
        public const int SettingsGeneralShowElevationPromptCheckboxHeight = 24;

        // Checkbox Shell context menu
        public const int SettingsGeneralShellContextMenuCheckboxLeft = 24;
        public const int SettingsGeneralShellContextMenuCheckboxTop = 204;
        public const int SettingsGeneralShellContextMenuCheckboxWidth = 420;
        public const int SettingsGeneralShellContextMenuCheckboxHeight = 24;

        // Checkbox Auto check for updates
        public const int SettingsGeneralAutoCheckForUpdatesCheckboxLeft = 24;
        public const int SettingsGeneralAutoCheckForUpdatesCheckboxTop = 240;
        public const int SettingsGeneralAutoCheckForUpdatesCheckboxWidth = 420;
        public const int SettingsGeneralAutoCheckForUpdatesCheckboxHeight = 24;

        // Text Language
        public const int SettingsGeneralLanguageLabelLeft = 34;
        public const int SettingsGeneralLanguageLabelTop = 286;
        public const int SettingsGeneralLanguageLabelWidth = 70;
        public const int SettingsGeneralLanguageLabelHeight = 32;

        // Auswahlfeld Language
        public const int SettingsGeneralLanguageSelectLeft = 104;
        public const int SettingsGeneralLanguageSelectTop = 284;
        public const int SettingsGeneralLanguageSelectWidth = 216;
        public const int SettingsGeneralLanguageSelectHeight = 32;

        // Button Add language
        public const int SettingsGeneralAddLanguageButtonLeft = 320;
        public const int SettingsGeneralAddLanguageButtonTop = 284;
        public const int SettingsGeneralAddLanguageButtonWidth = 32;
        public const int SettingsGeneralAddLanguageButtonHeight = 32;

        // Button Delete language
        public const int SettingsGeneralDeleteLanguageButtonLeft = 346;
        public const int SettingsGeneralDeleteLanguageButtonTop = 284;
        public const int SettingsGeneralDeleteLanguageButtonWidth = 32;
        public const int SettingsGeneralDeleteLanguageButtonHeight = 32;

        // Text Layout
        public const int SettingsGeneralLayoutLabelLeft = 34;
        public const int SettingsGeneralLayoutLabelTop = 326;
        public const int SettingsGeneralLayoutLabelWidth = 70;
        public const int SettingsGeneralLayoutLabelHeight = 32;

        // Auswahlfeld Layout
        public const int SettingsGeneralLayoutSelectLeft = 104;
        public const int SettingsGeneralLayoutSelectTop = 324;
        public const int SettingsGeneralLayoutSelectWidth = 216;
        public const int SettingsGeneralLayoutSelectHeight = 32;

        // Scrollbereich General
        public const int SettingsGeneralScrollContentWidth = 460;
        public const int SettingsGeneralScrollContentHeight = 380;

        // ============================================================
        // Settings - Export
        // ============================================================

        // Checkbox Export path
        public const int SettingsExportPathCheckboxLeft = 24;
        public const int SettingsExportPathCheckboxTop = 24;
        public const int SettingsExportPathCheckboxWidth = 420;
        public const int SettingsExportPathCheckboxHeight = 24;

        // Checkbox Export size (GB)
        public const int SettingsExportSizeGbCheckboxLeft = 24;
        public const int SettingsExportSizeGbCheckboxTop = 60;
        public const int SettingsExportSizeGbCheckboxWidth = 420;
        public const int SettingsExportSizeGbCheckboxHeight = 24;

        // Checkbox Export size (MB)
        public const int SettingsExportSizeMbCheckboxLeft = 24;
        public const int SettingsExportSizeMbCheckboxTop = 96;
        public const int SettingsExportSizeMbCheckboxWidth = 420;
        public const int SettingsExportSizeMbCheckboxHeight = 24;

        // Text Maximum levels/depth
        public const int SettingsExportMaxDepthLabelLeft = 34;
        public const int SettingsExportMaxDepthLabelTop = 146;
        public const int SettingsExportMaxDepthLabelWidth = 160;
        public const int SettingsExportMaxDepthLabelHeight = 28;

        // Eingabefeld Maximum levels/depth
        public const int SettingsExportMaxDepthInputLeft = 194;
        public const int SettingsExportMaxDepthInputTop = 144;
        public const int SettingsExportMaxDepthInputWidth = 56;
        public const int SettingsExportMaxDepthInputHeight = 34;

        // ============================================================
        // Settings - UI
        // ============================================================

        // Text Fill indicator
        public const int SettingsUiFillIndicatorLabelLeft = 34;
        public const int SettingsUiFillIndicatorLabelTop = 24;
        public const int SettingsUiFillIndicatorLabelWidth = 120;
        public const int SettingsUiFillIndicatorLabelHeight = 28;

        // Button Select color
        public const int SettingsUiSelectColorButtonLeft = 150;
        public const int SettingsUiSelectColorButtonTop = 24;
        public const int SettingsUiSelectColorButtonWidth = 140;
        public const int SettingsUiSelectColorButtonHeight = 28;

        // Farbvorschau
        public const int SettingsUiColorPreviewPanelLeft = 300;
        public const int SettingsUiColorPreviewPanelTop = 24;
        public const int SettingsUiColorPreviewPanelWidth = 42;
        public const int SettingsUiColorPreviewPanelHeight = 28;

        // Text Bar chart height
        public const int SettingsUiBarChartHeightLabelLeft = 34;
        public const int SettingsUiBarChartHeightLabelTop = 76;
        public const int SettingsUiBarChartHeightLabelWidth = 120;
        public const int SettingsUiBarChartHeightLabelHeight = 28;

        // Eingabefeld Bar chart height
        public const int SettingsUiBarChartHeightInputLeft = 150;
        public const int SettingsUiBarChartHeightInputTop = 74;
        public const int SettingsUiBarChartHeightInputWidth = 56;
        public const int SettingsUiBarChartHeightInputHeight = 34;

        // Text Default bar chart height
        public const int SettingsUiBarChartHeightDefaultLabelLeft = 210;
        
        public const int SettingsUiBarChartHeightDefaultLabelTop = 76;
        public const int SettingsUiBarChartHeightDefaultLabelWidth = 160;
        public const int SettingsUiBarChartHeightDefaultLabelHeight = 28;

        // ============================================================
        // Settings - Statistics
        // ============================================================

        // Checkbox Save scan history
        public const int SettingsStatisticsSaveScanHistoryCheckboxLeft = 24;
        public const int SettingsStatisticsSaveScanHistoryCheckboxTop = 24;
        public const int SettingsStatisticsSaveScanHistoryCheckboxWidth = 420;
        public const int SettingsStatisticsSaveScanHistoryCheckboxHeight = 24;

        // Text Database path
        public const int SettingsStatisticsDatabasePathLabelLeft = 24;
        public const int SettingsStatisticsDatabasePathLabelTop = 60;
        public const int SettingsStatisticsDatabasePathLabelWidth = 420;
        public const int SettingsStatisticsDatabasePathLabelHeight = 24;

        // Eingabefeld Database path
        public const int SettingsStatisticsDatabasePathInputLeft = 24;
        public const int SettingsStatisticsDatabasePathInputTop = 88;
        public const int SettingsStatisticsDatabasePathInputWidth = 330;
        public const int SettingsStatisticsDatabasePathInputHeight = 25;

        // Button Browse database
        public const int SettingsStatisticsBrowseDatabaseButtonLeft = 364;
        public const int SettingsStatisticsBrowseDatabaseButtonTop = 86;
        public const int SettingsStatisticsBrowseDatabaseButtonWidth = 90;
        public const int SettingsStatisticsBrowseDatabaseButtonHeight = 28;

        // Text Database move hint
        public const int SettingsStatisticsDatabaseMoveHintLabelLeft = 24;
        public const int SettingsStatisticsDatabaseMoveHintLabelTop = 122;
        public const int SettingsStatisticsDatabaseMoveHintLabelWidth = 430;
        public const int SettingsStatisticsDatabaseMoveHintLabelHeight = 24;

        // Text Database size
        public const int SettingsStatisticsDatabaseSizeLabelLeft = 24;
        public const int SettingsStatisticsDatabaseSizeLabelTop = 150;
        public const int SettingsStatisticsDatabaseSizeLabelWidth = 430;
        public const int SettingsStatisticsDatabaseSizeLabelHeight = 24;

        // Text Maximum scans per path
        public const int SettingsStatisticsMaximumScansLabelLeft = 24;
        public const int SettingsStatisticsMaximumScansLabelTop = 182;
        public const int SettingsStatisticsMaximumScansLabelWidth = 184;
        public const int SettingsStatisticsMaximumScansLabelHeight = 25;

        // Eingabefeld Maximum scans per path
        public const int SettingsStatisticsMaximumScansInputLeft = 208;
        public const int SettingsStatisticsMaximumScansInputTop = 182;
        public const int SettingsStatisticsMaximumScansInputWidth = 30;
        public const int SettingsStatisticsMaximumScansInputHeight = 25;

        // ============================================================
        // Settings - Logging
        // ============================================================

        // Text Log level
        public const int SettingsLoggingLogLevelLabelLeft = 34;
        public const int SettingsLoggingLogLevelLabelTop = 24;
        public const int SettingsLoggingLogLevelLabelWidth = 75;
        public const int SettingsLoggingLogLevelLabelHeight = 28;

        // Auswahlfeld Log level
        public const int SettingsLoggingLogLevelSelectLeft = 120;

        public const int SettingsLoggingLogLevelSelectTop = 22;
        public const int SettingsLoggingLogLevelSelectWidth = 150;
        public const int SettingsLoggingLogLevelSelectHeight = 32;

        // Checkbox Auto save log
        public const int SettingsLoggingAutoSaveCheckboxLeft = 26;
        public const int SettingsLoggingAutoSaveCheckboxTop = 64;
        public const int SettingsLoggingAutoSaveCheckboxWidth = 420;
        public const int SettingsLoggingAutoSaveCheckboxHeight = 24;

        // Text Maximum log file size
        public const int SettingsLoggingMaximumFileSizeLabelLeft = 34;
        public const int SettingsLoggingMaximumFileSizeLabelTop = 96;
        public const int SettingsLoggingMaximumFileSizeLabelWidth = 96;

        public const int SettingsLoggingMaximumFileSizeLabelHeight = 28;

        // Eingabefeld Maximum log file size
        public const int SettingsLoggingMaximumFileSizeInputLeft = 126;
        public const int SettingsLoggingMaximumFileSizeInputTop = 94;
        public const int SettingsLoggingMaximumFileSizeInputWidth = 56;
        public const int SettingsLoggingMaximumFileSizeInputHeight = 34;

        // Text Unit (MB)
        public const int SettingsLoggingMaximumFileSizeUnitLabelLeft = 192;
        public const int SettingsLoggingMaximumFileSizeUnitLabelTop = 96;
        public const int SettingsLoggingMaximumFileSizeUnitLabelWidth = 50;
        public const int SettingsLoggingMaximumFileSizeUnitLabelHeight = 28;
        public static readonly Size MainDriveSelectSize = new Size(280, 44);
        public static readonly Padding MainDriveSelectDropDownPadding = new Padding(10, 2, 10, 2);

        public static Color BackgroundPrimary =>
            _useDarkMode ? Color.FromArgb(32, 32, 32) : SystemColors.Window;

        public static Color BackgroundSecondary =>
            _useDarkMode ? Color.FromArgb(45, 45, 45) : SystemColors.Control;

        public static Color BackgroundTertiary =>
            _useDarkMode ? Color.FromArgb(55, 55, 55) : SystemColors.ControlLight;

        public static Color TextPrimary =>
            _useDarkMode ? Color.White : SystemColors.ControlText;

        public static Color Accent =>
            SystemColors.Highlight;

        public static Color AccentText =>
            SystemColors.HighlightText;

        public static Color SurfaceHighlight =>
            _useDarkMode ? Color.FromArgb(80, 80, 80) : SystemColors.ControlDark;

        public static Color HeaderBackground =>
            _useDarkMode ? Color.FromArgb(24, 24, 24) : SystemColors.Control;

        public static Color AnalysisTabBackColor =>
            BackgroundPrimary;

        public static Color AnalysisTabSelectedBackColor =>
            BackgroundSecondary;

        public static Color AnalysisTabTextColor =>
            TextPrimary;

        public static Color AnalysisTabSelectedTextColor =>
            TextPrimary;

        public static Color AnalysisTabBorderColor =>
            Border;

        public static Color AnalysisTabAccentColor =>
            Accent;

        public static Color AnalysisUsageBarTrackColor =>
            _useDarkMode
                ? Color.FromArgb(64, 64, 64)
                : Color.FromArgb(225, 225, 225);

        public static Color AnalysisUsageBarFillColor =>
            _useDarkMode
                ? Color.FromArgb(75, 145, 230)
                : Color.FromArgb(22, 119, 255);

        public static Color AnalysisUsageBarTextColor =>
            Color.White;

        public static Font DefaultFont =>
            SystemFonts.MessageBoxFont;

        public static Color InputBackground =>
            _useDarkMode ? Color.FromArgb(38, 38, 38) : Color.White;

        public static Color Border =>
            _useDarkMode ? Color.FromArgb(68, 68, 68) : Color.FromArgb(217, 217, 217);

        public static Color HoverBackground =>
            _useDarkMode ? Color.FromArgb(58, 58, 58) : Color.FromArgb(245, 245, 245);

        public static Color PressedBackground =>
            _useDarkMode ? Color.FromArgb(72, 72, 72) : Color.FromArgb(230, 244, 255);

        // Tabs File types / Largest files - vollständige zentrale Darstellung
        public static void ConfigureAnalysisTabs(
            AntdUI.Tabs tabs)
        {
            tabs.Type = AntdUI.TabType.Card;
            tabs.Centered = false;
            tabs.ItemSize = AnalysisTabWidth;
            tabs.Gap = AnalysisTabHorizontalPadding;
            tabs.ForeColor = AnalysisTabTextColor;
            tabs.BackColor = BackgroundPrimary;
            tabs.Font = DefaultFont;
            tabs.TabStop = false;
            tabs.EnableSwitch = true;
            tabs.EnablePageScrolling = false;
            tabs.EnablePageCloseByMouseMiddle = false;
            tabs.EnablePageCloseByMouseDoubleClick = false;
            tabs.DragOrder = false;
            tabs.TabMenuVisible = true;
        }



        // Analysis-Tabellen verwenden dieselbe AntdUI-Table-Konfiguration wie die Table-Ansicht.
        public static void ConfigureAnalysisTable(
            AntdUI.Table table)
        {
            ApplyTable(table);
        }

        public static AntdUI.Label CreateStorageHistoryLabel(
            string name,
            string text,
            int width,
            int height)
        {
            return new AntdUI.Label
            {
                Name = name,
                Text = text,
                Size = new Size(width, height),
                TextAlign = ContentAlignment.MiddleLeft,
                Anchor = AnchorStyles.Left,
                Margin = name == "labelPath"
                    ? new Padding(
                        0,
                        0,
                        StorageHistoryPathLabelMarginRight,
                        0)
                    : Padding.Empty
            };
        }

        public static AntdUI.Select CreateStorageHistorySelect(
            string name,
            int width,
            int height)
        {
            AntdUI.Select select = CreateSettingsSelect(
                name,
                Point.Empty,
                new Size(width, height));

            select.Anchor = AnchorStyles.Left;
            select.ListAutoWidth = false;
            select.MaxCount = 8;

            return select;
        }

        public static AntdUI.Slider CreateStorageHistoryIntensitySlider(
            string name,
            int value)
        {
            return new AntdUI.Slider
            {
                Name = name,
                MinimumSize = new Size(
                    StorageHistoryIntensitySliderWidth,
                    StorageHistoryIntensitySliderHeight),
                Size = new Size(
                    StorageHistoryIntensitySliderWidth,
                    StorageHistoryIntensitySliderHeight),
                MinValue = 0,
                MaxValue = 100,
                Value = Math.Max(0, Math.Min(100, value)),
                LineSize = StorageHistoryIntensitySliderLineSize,
                DotSize = StorageHistoryIntensitySliderDotSize,
                DotSizeActive =
                    StorageHistoryIntensitySliderDotSizeActive,
                ShowValue = false,
                Fill = Accent,
                FillHover = Accent,
                FillActive = Accent,
                TrackColor = Border,
                Anchor = AnchorStyles.Left | AnchorStyles.Right
            };
        }

        public static AntdUI.Button CreateStorageHistoryButton(
            string name,
            string text,
            int width,
            int height,
            AntdUI.TTypeMini type)
        {
            return new AntdUI.Button
            {
                Name = name,
                Text = text,
                Size = new Size(width, height),
                Type = type,
                Radius = 6,
                BorderWidth = 1F,
                DefaultBorderColor = Border,
                BackColor = BackgroundSecondary,
                ForeColor = TextPrimary,
                BackHover = HoverBackground,
                BackActive = PressedBackground
            };
        }

        public static AntdUI.Select CreateStorageHistoryPathSelect(
            string name)
        {
            AntdUI.Select select = CreateSettingsSelect(
                name,
                Point.Empty,
                new Size(
                    StorageHistoryPathSelectWidth,
                    StorageHistoryPathSelectHeight));

            select.Anchor = AnchorStyles.Left;
            select.ListAutoWidth = false;
            select.MaxCount = 8;
            select.Margin = new Padding(
                StorageHistoryPathSelectMarginLeft,
                0,
                0,
                0);

            return select;
        }

        public static void ConfigureStorageHistoryGrid(
            DataGridView grid)
        {
            if (grid == null)
                return;

            ApplyTable(grid);

            grid.BackgroundColor = BackgroundPrimary;
            grid.BackColor = BackgroundPrimary;
            grid.ForeColor = TextPrimary;
            grid.BorderStyle = BorderStyle.None;
            grid.CellBorderStyle =
                DataGridViewCellBorderStyle.SingleHorizontal;
            grid.ColumnHeadersBorderStyle =
                DataGridViewHeaderBorderStyle.None;
            grid.RowHeadersBorderStyle =
                DataGridViewHeaderBorderStyle.None;
            grid.GridColor = Border;
            grid.EnableHeadersVisualStyles = false;
            grid.ColumnHeadersHeightSizeMode =
                DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            grid.ColumnHeadersHeight =
                StorageHistoryGridHeaderHeight;
            grid.RowTemplate.Height =
                StorageHistoryGridRowHeight;

            grid.ColumnHeadersDefaultCellStyle.BackColor =
                BackgroundSecondary;
            grid.ColumnHeadersDefaultCellStyle.ForeColor =
                TextPrimary;
            grid.ColumnHeadersDefaultCellStyle.SelectionBackColor =
                BackgroundSecondary;
            grid.ColumnHeadersDefaultCellStyle.SelectionForeColor =
                TextPrimary;
            grid.ColumnHeadersDefaultCellStyle.Font =
                DefaultFont;
            grid.ColumnHeadersDefaultCellStyle.Padding =
                new Padding(8, 0, 8, 0);

            grid.DefaultCellStyle.BackColor =
                BackgroundPrimary;
            grid.DefaultCellStyle.ForeColor =
                TextPrimary;
            grid.DefaultCellStyle.SelectionBackColor =
                Accent;
            grid.DefaultCellStyle.SelectionForeColor =
                AccentText;
            grid.DefaultCellStyle.Font =
                DefaultFont;
            grid.DefaultCellStyle.Padding =
                new Padding(4, 0, 4, 0);

            grid.AlternatingRowsDefaultCellStyle.BackColor =
                BackgroundSecondary;
            grid.AlternatingRowsDefaultCellStyle.ForeColor =
                TextPrimary;
            grid.AlternatingRowsDefaultCellStyle.SelectionBackColor =
                Accent;
            grid.AlternatingRowsDefaultCellStyle.SelectionForeColor =
                AccentText;

            ConfigureScrollBars(grid);
        }

        public static AntdUI.Checkbox CreateSettingsCheckBox(
            string name,
            string text,
            int left,
            int top,
            int width,
            int height,
            Color backColor)
        {
            return new AntdUI.Checkbox
            {
                Name = name,
                Text = text,
                Location = new Point(left, top),
                Size = new Size(width, height),
                BackColor = backColor
            };
        }

        public static AntdUI.Label CreateSettingsLabel(
            string name,
            string text,
            int left,
            int top,
            int width,
            int height)
        {
            return new AntdUI.Label
            {
                Name = name,
                Text = text,
                Location = new Point(left, top),
                Size = new Size(width, height),
                TextAlign = ContentAlignment.MiddleLeft
            };
        }

        public static AntdUI.Label CreateSettingsExportMaxDepthLabel(
            string name,
            string text)
        {
            return new AntdUI.Label
            {
                Name = name,
                Text = text,
                Location = new Point(
                    SettingsExportMaxDepthLabelLeft,
                    SettingsExportMaxDepthLabelTop),
                Size = new Size(
                    SettingsExportMaxDepthLabelWidth,
                    SettingsExportMaxDepthLabelHeight),
                TextAlign = ContentAlignment.MiddleLeft
            };
        }

        public static AntdUI.Input CreateSettingsExportMaxDepthInput(
            string name)
        {
            return new AntdUI.Input
            {
                Name = name,
                Location = new Point(
                    SettingsExportMaxDepthInputLeft,
                    SettingsExportMaxDepthInputTop),
                Size = new Size(
                    SettingsExportMaxDepthInputWidth,
                    SettingsExportMaxDepthInputHeight),
                TextAlign = HorizontalAlignment.Right
            };
        }

        public static AntdUI.Label CreateSettingsLayoutColorLabel(
            string name,
            string text,
            int left,
            int top,
            int width,
            int height)
        {
            return new AntdUI.Label
            {
                Name = name,
                Text = text,
                Location = new Point(left, top),
                Size = new Size(width, height),
                TextAlign = ContentAlignment.MiddleLeft
            };
        }

        public static AntdUI.Button CreateSettingsLayoutColorButton(
            string name,
            string text,
            int left,
            int top,
            int width,
            int height)
        {
            return new AntdUI.Button
            {
                Name = name,
                Text = text,
                Location = new Point(left, top),
                Size = new Size(width, height),
                Type = AntdUI.TTypeMini.Default
            };
        }

        public static Panel CreateSettingsLayoutColorPreview(
            string name,
            int left,
            int top,
            int width,
            int height)
        {
            return new Panel
            {
                Name = name,
                Location = new Point(left, top),
                Size = new Size(width, height),
                BorderStyle = BorderStyle.FixedSingle
            };
        }

        public static AntdUI.Label CreateSettingsBarChartHeightLabel(
            string name,
            string text)
        {
            return new AntdUI.Label
            {
                Name = name,
                Text = text,
                Location = new Point(
                    SettingsUiBarChartHeightLabelLeft,
                    SettingsUiBarChartHeightLabelTop),
                Size = new Size(
                    SettingsUiBarChartHeightLabelWidth,
                    SettingsUiBarChartHeightLabelHeight),
                TextAlign = ContentAlignment.MiddleLeft
            };
        }

        public static AntdUI.Input CreateSettingsBarChartHeightInput(
            string name)
        {
            return new AntdUI.Input
            {
                Name = name,
                Location = new Point(
                    SettingsUiBarChartHeightInputLeft,
                    SettingsUiBarChartHeightInputTop),
                Size = new Size(
                    SettingsUiBarChartHeightInputWidth,
                    SettingsUiBarChartHeightInputHeight),
                TextAlign = HorizontalAlignment.Right,
                MaxLength = 3
            };
        }

        public static AntdUI.Label CreateSettingsBarChartHeightDefaultLabel(
            string name,
            string text)
        {
            return new AntdUI.Label
            {
                Name = name,
                Text = text,
                Location = new Point(
                    SettingsUiBarChartHeightDefaultLabelLeft,
                    SettingsUiBarChartHeightDefaultLabelTop),
                Size = new Size(
                    SettingsUiBarChartHeightDefaultLabelWidth,
                    SettingsUiBarChartHeightDefaultLabelHeight),
                TextAlign = ContentAlignment.MiddleLeft
            };
        }

        public static AntdUI.Select CreateSettingsSelect(
            string name,
            Point location,
            Size size)
        {
            AntdUI.Select select = new AntdUI.Select
            {
                Name = name,
                Location = location,
                Size = size,
                Font = DefaultFont,
                BackColor = InputBackground,
                ForeColor = TextPrimary,
                BorderWidth = 1F,
                BorderColor = Border,
                Radius = 6,
                DropDownRadius = 8,
                DropDownArrow = true,
                DropDownPadding = new Size(10, 4),
                ListAutoWidth = true,
                MaxCount = 8,
                ReadOnly = false,
                ClickSwitchDropdown = true,
                CaretColor = Color.Transparent,
                CaretSpeed = 0,
                Cursor = Cursors.Default,
                UseContextMenu = false
            };

            select.VerifyChar += (
                object sender,
                AntdUI.InputVerifyCharEventArgs e) =>
            {
                e.Result = false;
            };

            return select;
        }

        public static AntdUI.Button CreateSettingsRoundButton(
            string name,
            string text,
            int left,
            int top,
            int width,
            int height)
        {
            return new AntdUI.Button
            {
                Name = name,
                Text = text,
                Location = new Point(left, top),
                Size = new Size(width, height),
                Font = DefaultFont,
                Type = AntdUI.TTypeMini.Default,
                Radius = Math.Min(width, height) / 2,
                BorderWidth = 1F,
                DefaultBorderColor = Border,
                BackColor = BackgroundSecondary,
                ForeColor = TextPrimary,
                BackHover = HoverBackground,
                BackActive = PressedBackground,
                Padding = new Padding(0)
            };
        }

        public static AntdUI.Select CreateMainSelect(
            string name,
            Size size)
        {
            AntdUI.Select select = new AntdUI.Select
            {
                Name = name,
                Size = size,
                Font = DefaultFont,
                BackColor = InputBackground,
                ForeColor = TextPrimary,
                BorderWidth = 1F,
                BorderColor = Border,
                Radius = 6,
                DropDownRadius = 8,
                DropDownArrow = true,
                DropDownPadding = MainDriveSelectDropDownPadding.Size,
                ListAutoWidth = true,
                MaxCount = 8
            };

            return select;
        }

        public static AntdUI.Panel CreateMainStatusPanel(
            string name)
        {
            return new AntdUI.Panel
            {
                Name = name,
                Dock = DockStyle.Bottom,
                Height = 30,
                Back = BackgroundSecondary,
                BackColor = BackgroundSecondary,
                ForeColor = TextPrimary,
                BorderWidth = 1F,
                BorderColor = Border,
                Radius = 0,
                Padding = new Padding(8, 4, 8, 4)
            };
        }

        public static AntdUI.Label CreateMainStatusLabel(
            string name,
            string text)
        {
            return new AntdUI.Label
            {
                Name = name,
                Dock = DockStyle.Fill,
                Text = text,
                Font = DefaultFont,
                ForeColor = TextPrimary,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleLeft,
                AutoSize = false,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };
        }

        public static AntdUI.Progress CreateStatusScanProgress(
            string name)
        {
            return new AntdUI.Progress
            {
                Name = name,
                Dock = DockStyle.Right,
                Width = 200,
                Font = DefaultFont,
                ForeColor = TextPrimary,
                Back = BackgroundTertiary,
                Fill = Accent,
                Radius = 4,
                Value = 0F,
                Text = "0.0 % | 0.0 s",
                UseSystemText = true,
                UseTextCenter = true,
                Animation = 0,
                Visible = true,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };
        }

        public static void ConfigureMainStatusPanel(
            AntdUI.Panel statusPanel,
            AntdUI.Label statusLabel,
            AntdUI.Progress scanProgress,
            StatusStrip alertStatusStrip)
        {
            if (statusPanel != null)
            {
                statusPanel.Back = BackgroundSecondary;
                statusPanel.BackColor = BackgroundSecondary;
                statusPanel.ForeColor = TextPrimary;
                statusPanel.BorderColor = Border;
                statusPanel.Invalidate();
            }

            if (statusLabel != null)
            {
                int alertWidth = alertStatusStrip?.PreferredSize.Width ?? 0;

                statusLabel.ForeColor = TextPrimary;
                statusLabel.Padding = new Padding(
                    alertWidth + MainStatusTextSpacing,
                    0,
                    0,
                    0);
                statusLabel.Invalidate();
            }

            if (scanProgress != null)
            {
                scanProgress.ForeColor = TextPrimary;
                scanProgress.Back = BackgroundTertiary;
                scanProgress.Fill = Accent;
                scanProgress.Invalidate();
            }
        }

        public static AntdUI.Button CreateMainButton(
            string name,
            string text)
        {
            AntdUI.Button button = new AntdUI.Button
            {
                Name = name,
                Text = text,
                AutoSize = true,
                Height = 32,
                Font = DefaultFont,
                Radius = 6,
                BorderWidth = 1F,
                DefaultBorderColor = Border,
                BackColor = BackgroundSecondary,
                ForeColor = TextPrimary,
                BackHover = HoverBackground,
                BackActive = PressedBackground
            };

            return button;
        }

        public static AntdUI.Button CreateMainToggleButton(
            string name,
            string text)
        {
            AntdUI.Button button = CreateMainButton(name, text);
            button.AutoToggle = true;
            button.ToggleType = AntdUI.TTypeMini.Primary;
            return button;
        }

        public static ToolStripControlHost CreateToolStripHost(Control control)
        {
            control.Margin = Padding.Empty;
            control.Padding = Padding.Empty;

            ToolStripControlHost host = new ToolStripControlHost(control)
            {
                Name = control.Name + "Host",
                AutoSize = false,
                Size = control.Size,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                Overflow = ToolStripItemOverflow.Never
            };

            return host;
        }

        public static FlowLayoutPanel CreateMainToolbarPanel()
        {
            FlowLayoutPanel toolbarPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                AllowDrop = true
            };

            ConfigureMainToolbarPanel(toolbarPanel);
            return toolbarPanel;
        }

        public static ToolStrip CreateMainToolStrip()
        {
            ToolStrip toolStrip = new ToolStrip
            {
                Dock = DockStyle.None,
                GripStyle = ToolStripGripStyle.Visible,
                AllowItemReorder = true,
                CanOverflow = false
            };

            ConfigureToolStrip(toolStrip);
            return toolStrip;
        }

        public static AntdUI.Checkbox CreateMainToolbarCheckBox(
            string name,
            string text)
        {
            return new AntdUI.Checkbox
            {
                Name = name,
                AutoSize = true,
                Text = text,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };
        }

        public static AntdUI.Panel CreateMainPane(
            string name,
            DockStyle dock)
        {
            return new AntdUI.Panel
            {
                Name = name,
                Dock = dock,
                Back = BackgroundPrimary,
                BackColor = BackgroundPrimary,
                ForeColor = TextPrimary,
                BorderWidth = 1F,
                BorderColor = Border,
                Radius = 0,
                Padding = new Padding(1)
            };
        }

        public static void SetToolTip(Control control, string text)
        {
            if (control == null)
                return;

            MainToolTip.SetToolTip(control, text ?? string.Empty);
        }

        public static void ConfigureScrollBars(Control control)
        {
            if (control == null)
                return;

            void ApplyScrollBarTheme()
            {
                if (!control.IsHandleCreated)
                    return;

                SetWindowTheme(
                    control.Handle,
                    _useDarkMode ? "DarkMode_Explorer" : "Explorer",
                    null);
            }

            if (control.IsHandleCreated)
            {
                ApplyScrollBarTheme();
            }
            else
            {
                control.HandleCreated += (sender, e) => ApplyScrollBarTheme();
            }

            foreach (Control child in control.Controls)
            {
                ConfigureScrollBars(child);
            }
        }

        public static void ApplyMainForm(
            Form form,
            AppLayout layout,
            MenuStrip menuStrip,
            FlowLayoutPanel toolStripPanel,
            AntdUI.Select driveComboBox,
            AntdUI.Checkbox showFilesCheckBox,
            ContextMenuStrip contextMenuStrip,
            SplitContainer splitContainerMain,
            SplitContainer splitContainerLeft,
            AntdUI.Panel rightViewHost,
            StatusStrip alertStatusStrip,
            AntdUI.Panel mainStatusPanel,
            AntdUI.Label mainStatusLabel,
            AntdUI.Progress scanProgress,
            DataGridView partitionGrid,
            AntdUI.Table entryGrid,
            params ToolStrip[] toolStrips)
        {
            Apply(form, layout);

            form.BackColor = BackgroundPrimary;
            form.ForeColor = TextPrimary;
            form.Font = DefaultFont;

            ConfigureMenuStrip(menuStrip);
            ConfigureMainToolbarPanel(toolStripPanel);

            foreach (ToolStrip toolStrip in toolStrips)
            {
                ConfigureToolStrip(toolStrip);
            }

            ConfigureMainSelect(driveComboBox);
            ConfigureCheckBox(showFilesCheckBox);
            ConfigureContextMenu(contextMenuStrip);
            ConfigureSplitContainer(splitContainerMain);
            ConfigureSplitContainer(splitContainerLeft);
            ConfigureSurface(rightViewHost);
            ConfigureStatusStrip(alertStatusStrip);
            ConfigureMainStatusPanel(
                mainStatusPanel,
                mainStatusLabel,
                scanProgress,
                alertStatusStrip);
            ApplyTable(partitionGrid);
            ApplyTable(entryGrid);
        }

        private static void ConfigureMenuStrip(MenuStrip menuStrip)
        {
            menuStrip.AutoSize = false;
            menuStrip.Height = 34;
            menuStrip.Padding = new Padding(8, 0, 8, 0);
            menuStrip.BackColor = BackgroundSecondary;
            menuStrip.ForeColor = TextPrimary;
            menuStrip.RenderMode = ToolStripRenderMode.ManagerRenderMode;
            menuStrip.Renderer = new MainFormToolStripRenderer();

            foreach (ToolStripItem item in menuStrip.Items)
            {
                item.Padding = new Padding(10, 0, 10, 0);
                ApplyWindowsToolStripItem(item, _useDarkMode);

                if (item is ToolStripMenuItem menuItem)
                {
                    ApplyWindowsMenuItem(menuItem, _useDarkMode);
                }
            }
        }

        private static void ConfigureMainToolbarPanel(
            FlowLayoutPanel toolbarPanel)
        {
            toolbarPanel.AutoSize = true;
            toolbarPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            toolbarPanel.FlowDirection = FlowDirection.LeftToRight;
            toolbarPanel.WrapContents = true;
            toolbarPanel.AllowDrop = true;
            toolbarPanel.BackColor = BackgroundSecondary;
            toolbarPanel.ForeColor = TextPrimary;
            toolbarPanel.Padding = new Padding(8, 6, 8, 6);
            toolbarPanel.Margin = Padding.Empty;
            toolbarPanel.MinimumSize = new Size(
                0,
                MainToolbarHeight + toolbarPanel.Padding.Vertical);
        }

        private static void ConfigureToolStrip(ToolStrip toolStrip)
        {
            toolStrip.AutoSize = true;
            toolStrip.Height = MainToolbarHeight;
            toolStrip.CanOverflow = false;
            toolStrip.BackColor = BackgroundSecondary;
            toolStrip.ForeColor = TextPrimary;
            toolStrip.GripStyle = ToolStripGripStyle.Visible;
            toolStrip.Padding = new Padding(2);
            toolStrip.Margin = Padding.Empty;
            toolStrip.RenderMode = ToolStripRenderMode.ManagerRenderMode;
            toolStrip.Renderer = new MainFormToolStripRenderer();

            for (int index = 0; index < toolStrip.Items.Count; index++)
            {
                ToolStripItem item = toolStrip.Items[index];

                item.AutoSize = true;
                item.Overflow = ToolStripItemOverflow.Never;
                item.Margin = index < toolStrip.Items.Count - 1
                    ? new Padding(0, 1, MainToolbarItemSpacing, 1)
                    : new Padding(0, 1, 0, 1);
                item.Padding = Padding.Empty;
                ApplyWindowsToolStripItem(item, _useDarkMode);
            }
        }

        public static void ConfigureMainSelect(AntdUI.Select comboBox)
        {
            comboBox.BackColor = InputBackground;
            comboBox.ForeColor = TextPrimary;
            comboBox.Font = DefaultFont;
            comboBox.Size = MainDriveSelectSize;
            comboBox.BorderWidth = 2F;
            comboBox.BorderColor = SurfaceHighlight;
            comboBox.Radius = 8;
            comboBox.DropDownRadius = 8;
            comboBox.DropDownArrow = true;
            comboBox.DropDownPadding = MainDriveSelectDropDownPadding.Size;
            comboBox.ListAutoWidth = true;
            comboBox.MaxCount = 8;
            comboBox.Invalidate();
        }

        private static void ConfigureCheckBox(AntdUI.Checkbox checkBox)
        {
            checkBox.BackColor = BackgroundSecondary;
            checkBox.ForeColor = TextPrimary;
            checkBox.Font = DefaultFont;
            checkBox.Margin = new Padding(8, 2, 8, 0);
        }

        private static void ConfigureContextMenu(ContextMenuStrip contextMenuStrip)
        {
            contextMenuStrip.BackColor = BackgroundSecondary;
            contextMenuStrip.ForeColor = TextPrimary;
            contextMenuStrip.Font = DefaultFont;
            contextMenuStrip.RenderMode = ToolStripRenderMode.ManagerRenderMode;
            contextMenuStrip.Renderer = new MainFormToolStripRenderer();

            foreach (ToolStripItem item in contextMenuStrip.Items)
            {
                ApplyWindowsToolStripItem(item, _useDarkMode);

                if (item is ToolStripMenuItem menuItem)
                {
                    ApplyWindowsMenuItem(menuItem, _useDarkMode);
                }
            }
        }

        private static void ConfigureSplitContainer(SplitContainer splitContainer)
        {
            splitContainer.BackColor = Border;
            splitContainer.ForeColor = TextPrimary;
            splitContainer.SplitterWidth = 6;
            splitContainer.Panel1.BackColor = BackgroundPrimary;
            splitContainer.Panel2.BackColor = BackgroundPrimary;
        }

        private static void ConfigureSurface(Control control)
        {
            control.BackColor = BackgroundPrimary;
            control.ForeColor = TextPrimary;
            control.Font = DefaultFont;

            if (control is AntdUI.Panel panel)
            {
                panel.Back = BackgroundPrimary;
                panel.BorderColor = Border;
                panel.BorderWidth = 1F;
            }

            ConfigureScrollBars(control);
        }

        private static void ConfigureStatusStrip(StatusStrip statusStrip)
        {
            if (statusStrip == null)
                return;

            statusStrip.AutoSize = true;
            statusStrip.Dock = DockStyle.Left;
            statusStrip.BackColor = BackgroundSecondary;
            statusStrip.ForeColor = TextPrimary;
            statusStrip.Padding = Padding.Empty;
            statusStrip.Margin = Padding.Empty;
            statusStrip.RenderMode = ToolStripRenderMode.ManagerRenderMode;
            statusStrip.Renderer = new MainFormToolStripRenderer();
            statusStrip.LayoutStyle =
                ToolStripLayoutStyle.HorizontalStackWithOverflow;

            foreach (ToolStripItem item in statusStrip.Items)
            {
                item.Margin = new Padding(2, 0, 2, 0);
                item.Padding = Padding.Empty;
                ApplyWindowsToolStripItem(item, _useDarkMode);
            }
        }

        // Search-Dialog - zentrale Darstellung
        public static void ConfigureSearchDialog(
            Form form,
            AntdUI.Select comboBoxSource,
            AntdUI.Select comboBoxSavedScan,
            AntdUI.Input textBoxSearch,
            AntdUI.Select comboBoxMatchMode,
            AntdUI.Button buttonToggleFilters,
            Panel panelFilters,
            AntdUI.Checkbox checkBoxMinimumSize,
            AntdUI.InputNumber numericMinimumSize,
            AntdUI.Checkbox checkBoxMaximumSize,
            AntdUI.InputNumber numericMaximumSize,
            AntdUI.Checkbox checkBoxModifiedAfter,
            AntdUI.DatePicker dateTimeModifiedAfter,
            AntdUI.Checkbox checkBoxModifiedBefore,
            AntdUI.DatePicker dateTimeModifiedBefore,
            AntdUI.Input textBoxFileTypes,
            AntdUI.Button buttonResetFilters,
            ContextMenuStrip contextMenuResults,
            ProgressBar progressBarSearch,
            AntdUI.Button buttonSearch,
            AntdUI.Button buttonCancel)
        {
            form.MinimumSize = new Size(
                SearchWindowMinimumWidth,
                SearchWindowMinimumHeight);
            form.Size = new Size(
                SearchWindowWidth,
                SearchWindowHeight);

            ConfigureSearchSelect(
                comboBoxSource,
                SearchSourceSelectHeight,
                SearchSourceSelectDropDownItemHeight,
                SearchSourceSelectRadius);

            ConfigureSearchSelect(
                comboBoxSavedScan,
                SearchSavedScanSelectHeight,
                SearchSavedScanSelectDropDownItemHeight,
                SearchSavedScanSelectRadius);

            ConfigureSearchInput(
                textBoxSearch,
                SearchTextInputHeight,
                SearchTextInputRadius);

            ConfigureSearchSelect(
                comboBoxMatchMode,
                SearchMatchModeSelectHeight,
                SearchMatchModeSelectDropDownItemHeight,
                SearchMatchModeSelectRadius);

            ConfigureSearchButton(
                buttonToggleFilters,
                SearchFooterButtonWidth,
                SearchFiltersButtonHeight,
                SearchFiltersButtonRadius,
                AntdUI.TTypeMini.Default);

            panelFilters.BackColor = BackgroundPrimary;

            ConfigureSearchCheckbox(checkBoxMinimumSize);
            ConfigureSearchInputNumber(
                numericMinimumSize,
                SearchMinimumSizeInputHeight,
                SearchMinimumSizeInputRadius);

            ConfigureSearchCheckbox(checkBoxMaximumSize);
            ConfigureSearchInputNumber(
                numericMaximumSize,
                SearchMaximumSizeInputHeight,
                SearchMaximumSizeInputRadius);

            ConfigureSearchCheckbox(checkBoxModifiedAfter);
            ConfigureSearchDatePicker(
                dateTimeModifiedAfter,
                SearchModifiedAfterDatePickerHeight,
                SearchModifiedAfterDatePickerRadius);

            ConfigureSearchCheckbox(checkBoxModifiedBefore);
            ConfigureSearchDatePicker(
                dateTimeModifiedBefore,
                SearchModifiedBeforeDatePickerHeight,
                SearchModifiedBeforeDatePickerRadius);

            ConfigureSearchInput(
                textBoxFileTypes,
                SearchFileTypesInputHeight,
                SearchFileTypesInputRadius);

            ConfigureSearchButton(
                buttonResetFilters,
                SearchResetFiltersButtonWidth,
                SearchResetFiltersButtonHeight,
                SearchResetFiltersButtonRadius,
                AntdUI.TTypeMini.Default);

            // Kontextmenü Suchergebnisse
            ConfigureContextMenu(contextMenuResults);

            ConfigureSearchButton(
                buttonSearch,
                SearchFooterButtonWidth,
                SearchFooterButtonHeight,
                SearchResetFiltersButtonRadius,
                AntdUI.TTypeMini.Primary);

            ConfigureSearchButton(
                buttonCancel,
                SearchFooterButtonWidth,
                SearchFooterButtonHeight,
                SearchResetFiltersButtonRadius,
                AntdUI.TTypeMini.Default);

            progressBarSearch.Height = SearchProgressHeight;
        }

        // Search-Auswahlfeld - zentrale AntdUI-Darstellung
        private static void ConfigureSearchSelect(
            AntdUI.Select select,
            int height,
            int itemHeight,
            int radius)
        {
            select.MinimumSize = new Size(0, height);
            select.Height = height;
            select.Font = DefaultFont;
            select.BackColor = InputBackground;
            select.ForeColor = TextPrimary;
            select.BorderWidth = 1F;
            select.BorderColor = Border;
            select.Radius = radius;
            select.DropDownRadius = radius;
            select.DropDownArrow = true;
            select.DropDownPadding = new Size(10, 4);
            select.ListAutoWidth = false;
            select.MaxCount = 12;
            select.ReadOnly = false;
            select.ClickSwitchDropdown = true;
            select.CaretColor = Color.Transparent;
            select.CaretSpeed = 0;
            select.Cursor = Cursors.Default;
            select.UseContextMenu = false;
        }

        // Search-Textfeld - zentrale AntdUI-Darstellung
        private static void ConfigureSearchInput(
            AntdUI.Input input,
            int height,
            int radius)
        {
            input.MinimumSize = new Size(0, height);
            input.Height = height;
            input.Font = DefaultFont;
            input.BackColor = InputBackground;
            input.ForeColor = TextPrimary;
            input.BorderWidth = 1F;
            input.BorderColor = Border;
            input.Radius = radius;
        }

        // Search-Checkbox - zentrale AntdUI-Darstellung
        private static void ConfigureSearchCheckbox(
            AntdUI.Checkbox checkbox)
        {
            checkbox.Font = DefaultFont;
            checkbox.ForeColor = TextPrimary;
            checkbox.BackColor = Color.Transparent;
        }

        // Search-Zahleneingabefeld - zentrale AntdUI-Darstellung
        private static void ConfigureSearchInputNumber(
            AntdUI.InputNumber input,
            int height,
            int radius)
        {
            input.MinimumSize = new Size(0, height);
            input.Height = height;
            input.Font = DefaultFont;
            input.BackColor = InputBackground;
            input.ForeColor = TextPrimary;
            input.BorderWidth = 1F;
            input.BorderColor = Border;
            input.Radius = radius;
        }

        // Search-Datumsfeld - zentrale AntdUI-Darstellung
        private static void ConfigureSearchDatePicker(
            AntdUI.DatePicker datePicker,
            int height,
            int radius)
        {
            datePicker.MinimumSize = new Size(0, height);
            datePicker.Height = height;
            datePicker.Font = DefaultFont;
            datePicker.BackColor = InputBackground;
            datePicker.ForeColor = TextPrimary;
            datePicker.BorderWidth = 1F;
            datePicker.BorderColor = Border;
            datePicker.Radius = radius;
            datePicker.DropDownArrow = true;
            datePicker.ShowIcon = true;
        }

        // Search-Button - zentrale AntdUI-Darstellung
        private static void ConfigureSearchButton(
            AntdUI.Button button,
            int width,
            int height,
            int radius,
            AntdUI.TTypeMini type)
        {
            button.MinimumSize = new Size(width, height);
            button.Size = new Size(width, height);
            button.Font = DefaultFont;
            button.Type = type;
            button.Radius = radius;
            button.BorderWidth = 1F;
            button.DefaultBorderColor = Border;
            button.BackColor = BackgroundSecondary;
            button.ForeColor = TextPrimary;
            button.BackHover = HoverBackground;
            button.BackActive = PressedBackground;
        }

        public static void Apply(AppLayout layout)
        {
            _useDarkMode = ShouldUseDarkMode(layout);
            AntdUI.Localization.Provider = new AntdUiCultureLocalization();
            AntdUI.Localization.SetLanguage(CultureInfo.CurrentUICulture.Name);
            AntdUI.Config.Mode = _useDarkMode ? AntdUI.TMode.Dark : AntdUI.TMode.Light;
        }

        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
        private static extern int SetWindowTheme(IntPtr hwnd, string pszSubAppName, string pszSubIdList);

        private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;

        public static void Apply(Form form, AppLayout layout)
        {
            Apply(layout);
            form.Icon = AppResources.ApplicationIcon;
            ApplyWindowsTheme(form, layout);
        }

        private static void ApplyWindowsTheme(Form form, AppLayout layout)
        {
            form.SuspendLayout();

            bool useDarkMode = ShouldUseDarkMode(layout);

            form.Font = SystemFonts.MessageBoxFont;
            form.SizeGripStyle = IsResizable(form) ? SizeGripStyle.Auto : SizeGripStyle.Hide;

            SetImmersiveDarkMode(form, useDarkMode);

            if (useDarkMode)
            {
                form.BackColor = Color.FromArgb(32, 32, 32);
                form.ForeColor = Color.White;
            }
            else
            {
                form.BackColor = SystemColors.Control;
                form.ForeColor = SystemColors.ControlText;
            }

            foreach (Control control in form.Controls)
            {
                ApplyWindowsControl(control, useDarkMode);
            }

            form.ResumeLayout(false);
            form.PerformLayout();
        }

        private static bool IsResizable(Form form)
        {
            return form.MinimumSize != form.MaximumSize;
        }

        private static bool IsAntdUIControl(Control control)
        {
            string controlNamespace = control.GetType().Namespace;

            return !string.IsNullOrWhiteSpace(controlNamespace) &&
                   controlNamespace.StartsWith("AntdUI.", StringComparison.Ordinal);
        }

        private static bool ShouldUseDarkMode(AppLayout layout)
        {
            if (layout == AppLayout.WindowsDarkMode)
                return true;

            if (layout == AppLayout.WindowsLightMode)
                return false;

            return IsWindowsAppDarkModeEnabled();
        }

        private static bool IsWindowsAppDarkModeEnabled()
        {
            try
            {
                using Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(
                    @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");

                object value = key?.GetValue("AppsUseLightTheme");

                if (value is int appsUseLightTheme)
                {
                    return appsUseLightTheme == 0;
                }
            }
            catch
            {
            }

            return false;
        }

        private static void ApplyWindowsControl(Control control, bool useDarkMode)
        {
            if (IsAntdUIControl(control))
            {
                control.Font = DefaultFont;
                control.ForeColor = TextPrimary;
                control.Invalidate(true);
                return;
            }

            Color windowBackColor;
            Color controlBackColor;
            Color headerBackColor;
            Color textColor;
            Color selectedBackColor;
            Color selectedTextColor;
            Color gridColor;

            if (useDarkMode)
            {
                windowBackColor = Color.FromArgb(32, 32, 32);
                controlBackColor = Color.FromArgb(45, 45, 45);
                headerBackColor = Color.FromArgb(24, 24, 24);
                textColor = Color.White;
                selectedBackColor = SystemColors.Highlight;
                selectedTextColor = SystemColors.HighlightText;
                gridColor = Color.FromArgb(80, 80, 80);
            }
            else
            {
                windowBackColor = SystemColors.Window;
                controlBackColor = SystemColors.Control;
                headerBackColor = SystemColors.Control;
                textColor = SystemColors.ControlText;
                selectedBackColor = SystemColors.Highlight;
                selectedTextColor = SystemColors.HighlightText;
                gridColor = SystemColors.ControlDark;
            }

            control.Font = SystemFonts.MessageBoxFont;
            control.ForeColor = textColor;

            if (control is TabControl tabControl)
            {
                tabControl.DrawMode = useDarkMode ? TabDrawMode.OwnerDrawFixed : TabDrawMode.Normal;
                tabControl.BackColor = useDarkMode ? windowBackColor : SystemColors.Control;
                tabControl.ForeColor = textColor;
                tabControl.DrawItem -= tabControl_DrawItem;

                if (useDarkMode)
                {
                    tabControl.DrawItem += tabControl_DrawItem;
                    DarkTabControlHeaderPainter.Attach(tabControl);
                }
                else
                {
                    DarkTabControlHeaderPainter.Detach(tabControl);
                }
            }
            else if (control is TabPage tabPage)
            {
                tabPage.BackColor = useDarkMode ? windowBackColor : SystemColors.Control;
                tabPage.ForeColor = textColor;
            }
            else if (control is TextBox textBox)
            {
                textBox.BorderStyle = BorderStyle.Fixed3D;
                textBox.BackColor = windowBackColor;
                textBox.ForeColor = textColor;
            }
            else if (control is ComboBox comboBox)
            {
                if (comboBox.Parent is not ToolStrip)
                {
                    comboBox.FlatStyle = useDarkMode ? FlatStyle.Flat : FlatStyle.Standard;
                    comboBox.BackColor = windowBackColor;
                    comboBox.ForeColor = textColor;
                    comboBox.DrawItem -= comboBox_DrawItem;
                    comboBox.DrawMode = useDarkMode
                        ? DrawMode.OwnerDrawFixed
                        : DrawMode.Normal;

                    if (useDarkMode)
                    {
                        comboBox.DrawItem += comboBox_DrawItem;
                    }
                }
            }
            else if (control is Button button)
            {
                button.FlatStyle = useDarkMode ? FlatStyle.Flat : FlatStyle.Standard;
                button.UseVisualStyleBackColor = !useDarkMode;
                button.BackColor = useDarkMode ? controlBackColor : SystemColors.Control;
                button.ForeColor = textColor;
                button.Cursor = Cursors.Default;

                if (useDarkMode)
                {
                    button.FlatAppearance.BorderColor = Color.FromArgb(120, 120, 120);
                    button.FlatAppearance.MouseOverBackColor = Color.FromArgb(55, 55, 55);
                    button.FlatAppearance.MouseDownBackColor = Color.FromArgb(65, 65, 65);
                }
            }
            else if (control is CheckBox checkBox)
            {
                checkBox.FlatStyle = useDarkMode ? FlatStyle.Flat : FlatStyle.Standard;
                checkBox.UseVisualStyleBackColor = !useDarkMode;
                checkBox.BackColor = useDarkMode ? windowBackColor : SystemColors.Control;
                checkBox.ForeColor = textColor;
            }
            else if (control is Label label)
            {
                label.BackColor = Color.Transparent;
                label.ForeColor = textColor;
            }
            else if (control is LinkLabel linkLabel)
            {
                linkLabel.BackColor = Color.Transparent;
                linkLabel.ForeColor = textColor;
                linkLabel.LinkColor = useDarkMode ? Color.LightSkyBlue : SystemColors.HotTrack;
                linkLabel.ActiveLinkColor = useDarkMode ? Color.DeepSkyBlue : SystemColors.Highlight;
                linkLabel.VisitedLinkColor = useDarkMode ? Color.Plum : SystemColors.HotTrack;
            }
            else if (control is MenuStrip menuStrip)
            {
                menuStrip.BackColor = controlBackColor;
                menuStrip.ForeColor = textColor;
                menuStrip.RenderMode = ToolStripRenderMode.ManagerRenderMode;
                menuStrip.Renderer = useDarkMode ? new DarkToolStripRenderer() : null;

                foreach (ToolStripItem item in menuStrip.Items)
                {
                    ApplyWindowsToolStripItem(item, useDarkMode);

                    if (item is ToolStripMenuItem menuItem)
                    {
                        ApplyWindowsMenuItem(menuItem, useDarkMode);
                    }
                }
            }
            else if (control is ToolStrip toolStrip)
            {
                toolStrip.BackColor = controlBackColor;
                toolStrip.ForeColor = textColor;
                toolStrip.RenderMode = ToolStripRenderMode.ManagerRenderMode;
                toolStrip.Renderer = useDarkMode ? new DarkToolStripRenderer() : null;
                toolStrip.GripStyle = ToolStripGripStyle.Hidden;

                foreach (ToolStripItem item in toolStrip.Items)
                {
                    ApplyWindowsToolStripItem(item, useDarkMode);
                }
            }
            else if (control is StatusStrip statusStrip)
            {
                statusStrip.BackColor = controlBackColor;
                statusStrip.ForeColor = textColor;
                statusStrip.RenderMode = ToolStripRenderMode.ManagerRenderMode;
                statusStrip.Renderer = useDarkMode ? new DarkToolStripRenderer() : null;

                foreach (ToolStripItem item in statusStrip.Items)
                {
                    ApplyWindowsToolStripItem(item, useDarkMode);
                }
            }
            else if (control is DataGridView dataGridView)
            {
                dataGridView.BackgroundColor = windowBackColor;
                dataGridView.GridColor = gridColor;
                dataGridView.BorderStyle = BorderStyle.Fixed3D;
                dataGridView.EnableHeadersVisualStyles = !useDarkMode;

                dataGridView.DefaultCellStyle.BackColor = windowBackColor;
                dataGridView.DefaultCellStyle.ForeColor = textColor;
                dataGridView.DefaultCellStyle.SelectionBackColor = selectedBackColor;
                dataGridView.DefaultCellStyle.SelectionForeColor = selectedTextColor;

                dataGridView.AlternatingRowsDefaultCellStyle.BackColor = windowBackColor;
                dataGridView.AlternatingRowsDefaultCellStyle.ForeColor = textColor;
                dataGridView.AlternatingRowsDefaultCellStyle.SelectionBackColor = selectedBackColor;
                dataGridView.AlternatingRowsDefaultCellStyle.SelectionForeColor = selectedTextColor;

                dataGridView.ColumnHeadersDefaultCellStyle.BackColor = headerBackColor;
                dataGridView.ColumnHeadersDefaultCellStyle.ForeColor = textColor;
                dataGridView.ColumnHeadersDefaultCellStyle.SelectionBackColor = headerBackColor;
                dataGridView.ColumnHeadersDefaultCellStyle.SelectionForeColor = textColor;

                dataGridView.RowHeadersDefaultCellStyle.BackColor = headerBackColor;
                dataGridView.RowHeadersDefaultCellStyle.ForeColor = textColor;
                dataGridView.RowHeadersDefaultCellStyle.SelectionBackColor = headerBackColor;
                dataGridView.RowHeadersDefaultCellStyle.SelectionForeColor = textColor;
            }
            else if (control is TreeView treeView)
            {
                treeView.BackColor = windowBackColor;
                treeView.ForeColor = textColor;
                treeView.BorderStyle = BorderStyle.Fixed3D;
            }
            else if (control is ListView listView)
            {
                listView.BackColor = windowBackColor;
                listView.ForeColor = textColor;
                listView.BorderStyle = BorderStyle.Fixed3D;
                listView.Font = SystemFonts.MessageBoxFont;
            }
            else if (control is Chart_PieChart || control is Chart_BarChart)
            {
                control.BackColor = windowBackColor;
                control.ForeColor = textColor;
                control.Font = SystemFonts.MessageBoxFont;
                control.Invalidate();
            }
            else if (control is SplitContainer splitContainer)
            {
                splitContainer.BackColor = controlBackColor;
                splitContainer.ForeColor = textColor;
            }
            else if (control is SplitterPanel splitterPanel)
            {
                splitterPanel.BackColor = windowBackColor;
                splitterPanel.ForeColor = textColor;
            }
            else if (control is TableLayoutPanel tableLayoutPanel)
            {
                tableLayoutPanel.BackColor = windowBackColor;
                tableLayoutPanel.ForeColor = textColor;
            }
            else if (control is ToolStripPanel toolStripPanel)
            {
                toolStripPanel.BackColor = controlBackColor;
                toolStripPanel.ForeColor = textColor;
            }
            else if (control is Panel panel)
            {
                panel.BackColor = windowBackColor;
                panel.ForeColor = textColor;
            }
            else
            {
                control.BackColor = windowBackColor;
            }

            ApplyNativeScrollBarTheme(control, useDarkMode);

            foreach (Control child in control.Controls)
            {
                ApplyWindowsControl(child, useDarkMode);
            }
        }

        private static void ApplyNativeScrollBarTheme(Control control, bool useDarkMode)
        {
            if (!(control is DataGridView) &&
                !(control is TreeView) &&
                !(control is ListView) &&
                !(control is PropertyGrid) &&
                !(control is RichTextBox) &&
                !(control is TextBox textBox && textBox.Multiline))
            {
                return;
            }

            ApplyNativeScrollBarThemeToHandle(control, useDarkMode);
        }

        private static void ApplyNativeScrollBarThemeToHandle(Control control, bool useDarkMode)
        {
            try
            {
                SetWindowTheme(
                    control.Handle,
                    useDarkMode ? "DarkMode_Explorer" : "Explorer",
                    null);
            }
            catch
            {
            }
        }

        private static void tabControl_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (sender is not TabControl tabControl)
                return;

            bool selected = e.Index == tabControl.SelectedIndex;

            Color backColor = selected
                ? Color.FromArgb(32, 32, 32)
                : Color.FromArgb(45, 45, 45);

            Color foreColor = Color.White;
            Color borderColor = Color.FromArgb(120, 120, 120);

            Rectangle tabBounds = tabControl.GetTabRect(e.Index);

            using SolidBrush backBrush = new SolidBrush(backColor);
            e.Graphics.FillRectangle(backBrush, tabBounds);

            using Pen borderPen = new Pen(borderColor);
            e.Graphics.DrawRectangle(borderPen, tabBounds);

            TextRenderer.DrawText(
                e.Graphics,
                tabControl.TabPages[e.Index].Text,
                tabControl.Font,
                tabBounds,
                foreColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }

        private static void comboBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (sender is not ComboBox comboBox)
                return;

            if (e.Index < 0)
                return;

            bool selected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;

            Color backColor = selected
                ? SystemColors.Highlight
                : Color.FromArgb(32, 32, 32);

            Color foreColor = selected
                ? SystemColors.HighlightText
                : Color.White;

            using SolidBrush backBrush = new SolidBrush(backColor);
            e.Graphics.FillRectangle(backBrush, e.Bounds);

            TextRenderer.DrawText(
                e.Graphics,
                comboBox.Items[e.Index].ToString(),
                e.Font,
                e.Bounds,
                foreColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
        }

        private static void ApplyWindowsMenuItem(ToolStripMenuItem item, bool useDarkMode)
        {
            Color backColor = useDarkMode
                ? Color.FromArgb(45, 45, 45)
                : SystemColors.Control;

            Color foreColor = useDarkMode
                ? Color.White
                : SystemColors.ControlText;

            item.BackColor = backColor;
            item.ForeColor = foreColor;
            item.Padding = new Padding(4, 0, 4, 0);

            foreach (ToolStripItem child in item.DropDownItems)
            {
                child.BackColor = backColor;
                child.ForeColor = foreColor;
                child.Padding = new Padding(4, 0, 4, 0);

                if (child is ToolStripMenuItem childMenuItem)
                {
                    ApplyWindowsMenuItem(childMenuItem, useDarkMode);
                }
            }
        }

        private static void ApplyWindowsToolStripItem(ToolStripItem item, bool useDarkMode)
        {
            item.BackColor = useDarkMode
                ? Color.FromArgb(45, 45, 45)
                : SystemColors.Control;

            item.ForeColor = useDarkMode
                ? Color.White
                : SystemColors.ControlText;

            if (item is ToolStripComboBox toolStripComboBox)
            {
                toolStripComboBox.ComboBox.BackColor = useDarkMode
                    ? Color.FromArgb(32, 32, 32)
                    : SystemColors.Window;

                toolStripComboBox.ComboBox.ForeColor = useDarkMode
                    ? Color.White
                    : SystemColors.WindowText;

                toolStripComboBox.ComboBox.Invalidate();
            }
        }

        private static void SetImmersiveDarkMode(Form form, bool enabled)
        {
            if (!form.IsHandleCreated)
            {
                form.HandleCreated += (sender, e) => SetImmersiveDarkMode(form, enabled);
                return;
            }

            int useDarkMode = enabled ? 1 : 0;
            DwmSetWindowAttribute(form.Handle, DWMWA_USE_IMMERSIVE_DARK_MODE, ref useDarkMode, sizeof(int));
        }

        private sealed class MainFormToolStripRenderer : ToolStripProfessionalRenderer
        {
            public MainFormToolStripRenderer()
                : base(new MainFormProfessionalColorTable())
            {
                RoundedEdges = false;
            }

            protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
            {
                using SolidBrush brush = new SolidBrush(BackgroundSecondary);
                e.Graphics.FillRectangle(brush, e.AffectedBounds);
            }

            protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
            {
                Rectangle bounds = new Rectangle(Point.Empty, e.Item.Size);
                Color color = e.Item.Selected ? HoverBackground : BackgroundSecondary;

                using SolidBrush brush = new SolidBrush(color);
                e.Graphics.FillRectangle(brush, bounds);
            }

            protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
            {
                Rectangle bounds = new Rectangle(Point.Empty, e.Item.Size);
                Color color = BackgroundSecondary;

                if (e.Item.Pressed ||
                    e.Item is ToolStripButton button && button.Checked)
                {
                    color = PressedBackground;
                }
                else if (e.Item.Selected)
                {
                    color = HoverBackground;
                }

                using SolidBrush brush = new SolidBrush(color);
                e.Graphics.FillRectangle(brush, bounds);

                if (e.Item.Selected || e.Item.Pressed)
                {
                    using Pen pen = new Pen(Border);
                    e.Graphics.DrawRectangle(
                        pen,
                        0,
                        0,
                        Math.Max(0, bounds.Width - 1),
                        Math.Max(0, bounds.Height - 1));
                }
            }

            protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
            {
                e.TextColor = TextPrimary;
                base.OnRenderItemText(e);
            }

            protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
            {
                using Pen pen = new Pen(Border);
                int y = e.Item.Height / 2;
                e.Graphics.DrawLine(pen, 6, y, Math.Max(6, e.Item.Width - 6), y);
            }

            protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
            {
                using Pen pen = new Pen(Border);
                e.Graphics.DrawLine(
                    pen,
                    0,
                    Math.Max(0, e.ToolStrip.Height - 1),
                    e.ToolStrip.Width,
                    Math.Max(0, e.ToolStrip.Height - 1));
            }
        }

        private sealed class MainFormProfessionalColorTable : ProfessionalColorTable
        {
            public override Color MenuStripGradientBegin => BackgroundSecondary;
            public override Color MenuStripGradientEnd => BackgroundSecondary;
            public override Color ToolStripGradientBegin => BackgroundSecondary;
            public override Color ToolStripGradientMiddle => BackgroundSecondary;
            public override Color ToolStripGradientEnd => BackgroundSecondary;
            public override Color ToolStripDropDownBackground => BackgroundSecondary;
            public override Color ImageMarginGradientBegin => BackgroundSecondary;
            public override Color ImageMarginGradientMiddle => BackgroundSecondary;
            public override Color ImageMarginGradientEnd => BackgroundSecondary;
            public override Color MenuItemSelected => HoverBackground;
            public override Color MenuItemSelectedGradientBegin => HoverBackground;
            public override Color MenuItemSelectedGradientEnd => HoverBackground;
            public override Color MenuItemPressedGradientBegin => PressedBackground;
            public override Color MenuItemPressedGradientMiddle => PressedBackground;
            public override Color MenuItemPressedGradientEnd => PressedBackground;
            public override Color ButtonSelectedGradientBegin => HoverBackground;
            public override Color ButtonSelectedGradientMiddle => HoverBackground;
            public override Color ButtonSelectedGradientEnd => HoverBackground;
            public override Color ButtonPressedGradientBegin => PressedBackground;
            public override Color ButtonPressedGradientMiddle => PressedBackground;
            public override Color ButtonPressedGradientEnd => PressedBackground;
            public override Color SeparatorDark => Border;
            public override Color SeparatorLight => Border;
            public override Color ToolStripBorder => Border;
            public override Color MenuBorder => Border;
        }

        private sealed class DarkTabControlHeaderPainter : NativeWindow
        {
            private const int WM_PAINT = 0x000F;
            private readonly TabControl _tabControl;

            private DarkTabControlHeaderPainter(TabControl tabControl)
            {
                _tabControl = tabControl;
                AssignHandle(tabControl.Handle);
            }

            public static void Attach(TabControl tabControl)
            {
                if (tabControl.Tag is DarkTabControlHeaderPainter)
                    return;

                tabControl.Tag = new DarkTabControlHeaderPainter(tabControl);
            }

            public static void Detach(TabControl tabControl)
            {
                if (tabControl.Tag is not DarkTabControlHeaderPainter painter)
                    return;

                painter.ReleaseHandle();
                tabControl.Tag = null;
                tabControl.Invalidate();
            }

            protected override void WndProc(ref Message m)
            {
                base.WndProc(ref m);

                if (m.Msg != WM_PAINT)
                    return;

                if (_tabControl.TabPages.Count == 0)
                    return;

                using Graphics graphics = Graphics.FromHwnd(_tabControl.Handle);

                int headerHeight = _tabControl.DisplayRectangle.Top;
                Rectangle lastTabBounds = _tabControl.GetTabRect(_tabControl.TabPages.Count - 1);

                Rectangle headerBounds = new Rectangle(
                    lastTabBounds.Right,
                    0,
                    Math.Max(0, _tabControl.Width - lastTabBounds.Right),
                    headerHeight);

                using SolidBrush headerBrush = new SolidBrush(Color.FromArgb(32, 32, 32));
                graphics.FillRectangle(headerBrush, headerBounds);

                using Pen borderPen = new Pen(Color.FromArgb(120, 120, 120));
                graphics.DrawLine(borderPen, 0, headerHeight - 1, _tabControl.Width, headerHeight - 1);
            }
        }

        private sealed class DarkToolStripRenderer : ToolStripProfessionalRenderer
        {
            public DarkToolStripRenderer()
                : base(new DarkProfessionalColorTable())
            {
            }

            protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
            {
                using SolidBrush brush = new SolidBrush(Color.FromArgb(45, 45, 45));
                e.Graphics.FillRectangle(brush, e.AffectedBounds);
            }

            protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
            {
                Rectangle bounds = new Rectangle(Point.Empty, e.Item.Size);

                Color backColor = e.Item.Selected
                    ? Color.FromArgb(65, 65, 65)
                    : Color.FromArgb(45, 45, 45);

                using SolidBrush brush = new SolidBrush(backColor);
                e.Graphics.FillRectangle(brush, bounds);
            }

            protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
            {
                Rectangle bounds = new Rectangle(Point.Empty, e.Item.Size);

                Color backColor;

                if (e.Item.Pressed || (e.Item is ToolStripButton button && button.Checked))
                {
                    backColor = Color.FromArgb(72, 72, 72);
                }
                else if (e.Item.Selected)
                {
                    backColor = Color.FromArgb(65, 65, 65);
                }
                else
                {
                    backColor = Color.FromArgb(45, 45, 45);
                }

                using SolidBrush brush = new SolidBrush(backColor);
                e.Graphics.FillRectangle(brush, bounds);

                if (e.Item.Selected || e.Item.Pressed)
                {
                    using Pen borderPen = new Pen(Color.FromArgb(95, 95, 95));
                    e.Graphics.DrawRectangle(
                        borderPen,
                        0,
                        0,
                        Math.Max(0, bounds.Width - 1),
                        Math.Max(0, bounds.Height - 1));
                }
            }

            protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
            {
                e.TextColor = Color.White;
                base.OnRenderItemText(e);
            }

            protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
            {
                using Pen pen = new Pen(Color.FromArgb(80, 80, 80));
                int y = e.Item.Height / 2;
                e.Graphics.DrawLine(pen, 4, y, e.Item.Width - 4, y);
            }

            protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
            {
                using SolidBrush brush = new SolidBrush(Color.FromArgb(45, 45, 45));
                e.Graphics.FillRectangle(brush, e.AffectedBounds);
            }

            protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
            {
                using Pen pen = new Pen(Color.FromArgb(80, 80, 80));
                Rectangle bounds = new Rectangle(Point.Empty, e.ToolStrip.Size - new Size(1, 1));
                e.Graphics.DrawRectangle(pen, bounds);
            }
        }

        private sealed class DarkProfessionalColorTable : ProfessionalColorTable
        {
            public override Color ToolStripDropDownBackground => Color.FromArgb(45, 45, 45);
            public override Color MenuBorder => Color.FromArgb(80, 80, 80);
            public override Color MenuItemBorder => Color.FromArgb(80, 80, 80);
            public override Color MenuItemSelected => Color.FromArgb(65, 65, 65);
            public override Color MenuItemSelectedGradientBegin => Color.FromArgb(65, 65, 65);
            public override Color MenuItemSelectedGradientEnd => Color.FromArgb(65, 65, 65);
            public override Color MenuItemPressedGradientBegin => Color.FromArgb(65, 65, 65);
            public override Color MenuItemPressedGradientMiddle => Color.FromArgb(65, 65, 65);
            public override Color MenuItemPressedGradientEnd => Color.FromArgb(65, 65, 65);
            public override Color ImageMarginGradientBegin => Color.FromArgb(45, 45, 45);
            public override Color ImageMarginGradientMiddle => Color.FromArgb(45, 45, 45);
            public override Color ImageMarginGradientEnd => Color.FromArgb(45, 45, 45);
            public override Color ToolStripBorder => Color.FromArgb(80, 80, 80);
            public override Color ToolStripGradientBegin => Color.FromArgb(45, 45, 45);
            public override Color ToolStripGradientMiddle => Color.FromArgb(45, 45, 45);
            public override Color ToolStripGradientEnd => Color.FromArgb(45, 45, 45);
        }

        public static Padding CreateHorizontalPadding(int top, int bottom)
        {
            return new Padding(HorizontalMargin, top, HorizontalMargin, bottom);
        }

        public static Panel CreateTableHost(
            Control content,
            Color backColor,
            int top,
            int bottom)
        {
            Panel panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = CreateHorizontalPadding(top, bottom),
                BackColor = backColor
            };

            content.Dock = DockStyle.Fill;
            panel.Controls.Add(content);

            return panel;
        }

        public static void ConfigureTablePage(TabPage tabPage, Color backColor)
        {
            if (tabPage == null)
                return;

            tabPage.Padding = new Padding(8);
            tabPage.BackColor = backColor;
        }

        public static Color TableProgressBackColor =>
            _useDarkMode
                ? Color.FromArgb(58, 58, 58)
                : Color.FromArgb(230, 230, 230);

        public static Color TableProgressFillColor =>
            _useDarkMode
                ? Color.FromArgb(70, 135, 220)
                : Color.FromArgb(90, 140, 210);

        public static void ApplyTable(AntdUI.Table table)
        {
            if (table == null)
                return;

            table.BackColor = BackgroundPrimary;
            table.ForeColor = TextPrimary;
            table.Font = DefaultFont;
            table.ColumnBack = BackgroundSecondary;
            table.ColumnFore = TextPrimary;
            table.RowHoverBg = SurfaceHighlight;
            table.RowSelectedBg = Accent;
            table.RowSelectedFore = AccentText;
            table.BorderColor = Border;
            table.BorderWidth = 1F;
            table.BorderCellWidth = 1F;
            table.Bordered = true;
            table.Radius = 6;
            table.Gap = TableGap;
            table.GapCell = TableCellGap;
            table.RowHeight = TableRowHeight;
            table.RowHeightHeader = TableHeaderHeight;
            table.FixedHeader = true;
            table.ScrollBarAvoidHeader = true;
            table.ShowTip = true;
            table.Invalidate();
        }

        public static void ApplyTable(DataGridView grid)
        {
            if (grid == null)
                return;

            Color headerBackColor = ControlPaint.Dark(BackgroundSecondary, 0.08f);
            Color gridColor = ControlPaint.Dark(BackgroundSecondary, 0.2f);

            grid.BackgroundColor = BackgroundPrimary;
            grid.BackColor = BackgroundPrimary;
            grid.ForeColor = TextPrimary;
            grid.GridColor = gridColor;
            grid.BorderStyle = BorderStyle.None;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            grid.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            grid.EnableHeadersVisualStyles = false;
            grid.ColumnHeadersHeightSizeMode =
                DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            grid.ColumnHeadersHeight = DataGridViewHeaderHeight;
            grid.RowTemplate.Height = DataGridViewRowHeight;
            grid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;

            grid.DefaultCellStyle.BackColor = BackgroundPrimary;
            grid.DefaultCellStyle.ForeColor = TextPrimary;
            grid.DefaultCellStyle.SelectionBackColor = Accent;
            grid.DefaultCellStyle.SelectionForeColor = AccentText;
            grid.DefaultCellStyle.Font = DefaultFont;
            grid.DefaultCellStyle.Padding =
                new Padding(TableCellHorizontalPadding, 0, TableCellHorizontalPadding, 0);

            grid.AlternatingRowsDefaultCellStyle.BackColor = BackgroundSecondary;
            grid.AlternatingRowsDefaultCellStyle.ForeColor = TextPrimary;
            grid.AlternatingRowsDefaultCellStyle.SelectionBackColor = Accent;
            grid.AlternatingRowsDefaultCellStyle.SelectionForeColor = AccentText;
            grid.AlternatingRowsDefaultCellStyle.Font = DefaultFont;
            grid.AlternatingRowsDefaultCellStyle.Padding =
                new Padding(TableCellHorizontalPadding, 0, TableCellHorizontalPadding, 0);

            grid.ColumnHeadersDefaultCellStyle.BackColor = headerBackColor;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = TextPrimary;
            grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = headerBackColor;
            grid.ColumnHeadersDefaultCellStyle.SelectionForeColor = TextPrimary;
            grid.ColumnHeadersDefaultCellStyle.Font = DefaultFont;
            grid.ColumnHeadersDefaultCellStyle.Padding =
                new Padding(TableCellHorizontalPadding, 0, TableCellHorizontalPadding, 0);

            foreach (DataGridViewColumn column in grid.Columns)
            {
                if (column.SortMode != DataGridViewColumnSortMode.NotSortable)
                {
                    column.SortMode = DataGridViewColumnSortMode.Programmatic;
                }
            }

            grid.DataBindingComplete -= TableGrid_DataBindingComplete;
            grid.DataBindingComplete += TableGrid_DataBindingComplete;
            grid.HandleCreated -= TableGrid_HandleCreated;
            grid.HandleCreated += TableGrid_HandleCreated;
            grid.ControlAdded -= TableGrid_ControlAdded;
            grid.ControlAdded += TableGrid_ControlAdded;

            ApplyTableGridRowHeights(grid);
            ApplyTableScrollBarTheme(grid);
            grid.Invalidate(true);
        }

        private static void TableGrid_DataBindingComplete(
            object sender,
            DataGridViewBindingCompleteEventArgs e)
        {
            if (sender is DataGridView grid)
            {
                ApplyTableGridRowHeights(grid);
                grid.Invalidate(true);
            }
        }

        private static void ApplyTableGridRowHeights(
            DataGridView grid)
        {
            if (grid == null)
                return;

            grid.ColumnHeadersHeight = DataGridViewHeaderHeight;
            grid.RowTemplate.Height = DataGridViewRowHeight;

            foreach (DataGridViewRow row in grid.Rows)
            {
                row.Height = DataGridViewRowHeight;
            }
        }

        public static void ApplyDetailsTable(DataGridView grid)
        {
            ApplyTable(grid);

            if (grid == null)
                return;

            grid.DefaultCellStyle.SelectionBackColor = BackgroundSecondary;
            grid.DefaultCellStyle.SelectionForeColor = TextPrimary;
            grid.AlternatingRowsDefaultCellStyle.SelectionBackColor = BackgroundSecondary;
            grid.AlternatingRowsDefaultCellStyle.SelectionForeColor = TextPrimary;
        }

        private static void TableGrid_HandleCreated(object sender, EventArgs e)
        {
            if (sender is DataGridView grid)
            {
                ApplyTableScrollBarTheme(grid);
            }
        }

        private static void TableGrid_ControlAdded(object sender, ControlEventArgs e)
        {
            if (sender is DataGridView grid)
            {
                ApplyTableScrollBarTheme(grid);
            }
        }

        private static void ApplyTableScrollBarTheme(DataGridView grid)
        {
            if (grid == null || !grid.IsHandleCreated)
                return;

            ApplyNativeScrollBarThemeToHandle(grid, _useDarkMode);

            foreach (Control child in grid.Controls)
            {
                if (child is ScrollBar)
                {
                    ApplyNativeScrollBarThemeToHandle(child, _useDarkMode);
                    child.BackColor = BackgroundSecondary;
                    child.ForeColor = TextPrimary;
                }
            }
        }

    }
}