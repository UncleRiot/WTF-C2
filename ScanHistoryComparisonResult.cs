using System.Collections.Generic;

namespace c2flux
{
    public sealed class ScanHistoryComparisonResult
    {
        public ScanHistoryInfo BaselineScan { get; set; }
        public ScanHistoryInfo CompareScan { get; set; }
        public long BaselineSizeBytes { get; set; }
        public long CompareSizeBytes { get; set; }
        public long SizeDeltaBytes { get; set; }
        public int BaselineFileCount { get; set; }
        public int CompareFileCount { get; set; }
        public int NewFileCount { get; set; }
        public int DeletedFileCount { get; set; }
        public int ChangedFileCount { get; set; }
        public List<ScanHistoryFileChange> NewFiles { get; } = new List<ScanHistoryFileChange>();
        public List<ScanHistoryFileChange> DeletedFiles { get; } = new List<ScanHistoryFileChange>();
        public List<ScanHistoryFileChange> ChangedFiles { get; } = new List<ScanHistoryFileChange>();
        public List<ScanHistoryFolderGrowth> FolderGrowth { get; } = new List<ScanHistoryFolderGrowth>();
    }

    public sealed class ScanHistoryFileChange
    {
        public string Path { get; set; }
        public string ParentPath { get; set; }
        public long BaselineSizeBytes { get; set; }
        public long CompareSizeBytes { get; set; }
        public long DeltaBytes { get; set; }
        public string Size { get; set; }
        public string BaselineSize { get; set; }
        public string CompareSize { get; set; }
        public string Delta { get; set; }
        public string LastWriteTimeUtc { get; set; }
    }

    public sealed class ScanHistoryFolderGrowth
    {
        public string Path { get; set; }
        public long BaselineSizeBytes { get; set; }
        public long CompareSizeBytes { get; set; }
        public long DeltaBytes { get; set; }
        public int NewFileCount { get; set; }
        public int ChangedFileCount { get; set; }
        public string BaselineSize { get; set; }
        public string CompareSize { get; set; }
        public string Delta { get; set; }
    }
}
