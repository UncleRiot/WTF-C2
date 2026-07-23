using System;

namespace c2flux
{
    public sealed class StorageHistoryRecord
    {
        public string Path { get; set; }
        public DateTime RecordedAtUtc { get; set; }
        public long SizeBytes { get; set; }
        public long TotalCapacityBytes { get; set; }
        public long FreeSpaceBytes { get; set; }
    }
}
