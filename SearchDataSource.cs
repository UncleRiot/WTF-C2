using System;
using System.Collections.Generic;

namespace c2flux
{
    public enum SearchSource
    {
        CurrentScan,
        SavedScan,
        SelectedDrives,
        AllLoadedData
    }

    public sealed class SearchLoadedRoot
    {
        public string RootPath { get; set; }
        public FileSystemEntry RootEntry { get; set; }
    }

    public sealed class SearchDriveItem
    {
        public bool Selected { get; set; }
        public string Drive { get; set; }
        public string FileSystem { get; set; }
        public string DriveType { get; set; }
        public string PlannedScanner { get; set; }
        public string LastScan { get; set; }
        public FileSystemEntry LoadedRootEntry { get; set; }
    }
}
