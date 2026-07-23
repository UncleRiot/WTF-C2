using System;
using System.IO;
using System.Text.Json;

namespace c2flux
{
    public static class ScanResultFileService
    {
        public static void Save(string filePath, FileSystemEntry rootEntry)
        {
            JsonSerializerOptions options = new JsonSerializerOptions { WriteIndented = false };
            File.WriteAllText(filePath, JsonSerializer.Serialize(rootEntry, options));
        }

        public static FileSystemEntry Load(string filePath)
        {
            string json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<FileSystemEntry>(json);
        }
    }
}
