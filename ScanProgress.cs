using System.Collections.Generic;

namespace c2flux
{
    public sealed class ScanProgress
    {
        public string CurrentPath { get; set; }
        public long ScannedBytes { get; set; }
        public int ScannedDirectories { get; set; }
        public int ScannedFiles { get; set; }
        public int SkippedDirectories { get; set; }
        public List<string> SkippedDirectoryDetails { get; set; }
        public FileSystemEntry LiveRootEntry { get; set; }
        public bool IsCacheVerification { get; set; }
        public bool IsCacheSavePhase { get; set; }
    }
}
