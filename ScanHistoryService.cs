using System;
using System.Collections.Generic;
using System.Globalization;

namespace c2flux
{
    public static class ScanHistoryService
    {
        public static string DefaultDatabasePath => ScanHistoryDatabaseService.DefaultDatabasePath;

        public static string DatabasePath => ScanHistoryDatabaseService.DatabasePath;

        public static void ConfigureDatabasePath(string databasePath)
        {
            ScanHistoryDatabaseService.ConfigureDatabasePath(databasePath);
        }

        public static void ConfigureRetention(int maximumScansPerPath)
        {
            ScanHistoryDatabaseService.ConfigureRetention(maximumScansPerPath);
        }

        public static bool IsDatabaseMaintenanceRequired()
        {
            return ScanHistoryDatabaseService.IsMaintenanceRequired();
        }

        public static string NormalizeDatabasePath(string databasePath)
        {
            return ScanHistoryDatabaseService.NormalizeDatabasePath(databasePath);
        }

        public static void MoveDatabase(string targetDatabasePath)
        {
            ScanHistoryDatabaseService.MoveDatabase(targetDatabasePath);
        }

        public static string Save(FileSystemEntry rootEntry, IProgress<int> progress = null)
        {
            return ScanHistoryDatabaseService.Save(rootEntry, progress);
        }

        public static IReadOnlyList<ScanHistoryInfo> List()
        {
            return ScanHistoryDatabaseService.List();
        }

        public static ScanHistorySnapshot Load(string scanId)
        {
            return ScanHistoryDatabaseService.Load(scanId);
        }
    }

    public sealed class ScanHistoryInfo
    {
        public string FilePath { get; set; }
        public string ScanId { get; set; }
        public DateTime CreatedUtc { get; set; }
        public string RootPath { get; set; }
        public long RootSizeBytes { get; set; }
        public int FileCount { get; set; }
        public int DirectoryCount { get; set; }

        public string DisplayName
        {
            get
            {
                return CreatedUtc.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.CurrentCulture) +
                    " - " +
                    RootPath +
                    " - " +
                    SizeFormatter.Format(RootSizeBytes);
            }
        }

        public override string ToString()
        {
            return DisplayName;
        }
    }

    public sealed class ScanHistorySnapshot
    {
        public int Version { get; set; }
        public string ScanId { get; set; }
        public DateTime CreatedUtc { get; set; }
        public string RootPath { get; set; }
        public long RootSizeBytes { get; set; }
        public FileSystemEntry RootEntry { get; set; }
    }
}
