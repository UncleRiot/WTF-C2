using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Filesystem.Ntfs;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace c2flux
{
    public sealed class NtfsMftScanner
    {
        private const int ProgressReportIntervalNodes = 5000;

        private readonly AppSettings _settings;

        public NtfsMftScanner(AppSettings settings)
        {
            _settings = settings;
        }

        public static bool IsSupported(string rootPath)
        {
            if (!IsProcessElevated())
                return false;

            try
            {
                string driveRoot = Path.GetPathRoot(rootPath);

                if (string.IsNullOrWhiteSpace(driveRoot))
                    return false;

                DriveInfo driveInfo = new DriveInfo(driveRoot);

                return driveInfo.IsReady &&
                       driveInfo.DriveType == DriveType.Fixed &&
                       string.Equals(driveInfo.DriveFormat, "NTFS", StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }
        private static bool IsProcessElevated()
        {
            try
            {
                using System.Security.Principal.WindowsIdentity identity = System.Security.Principal.WindowsIdentity.GetCurrent();
                System.Security.Principal.WindowsPrincipal principal = new System.Security.Principal.WindowsPrincipal(identity);

                return principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
            }
            catch
            {
                return false;
            }
        }
        public Task<FileSystemEntry> ScanAsync(string rootPath, IProgress<ScanProgress> progress, CancellationToken cancellationToken, PauseToken pauseToken)
        {
            return Task.Factory.StartNew(() =>
            {
                Stopwatch totalStopwatch = Stopwatch.StartNew();
                Stopwatch phaseStopwatch = new Stopwatch();
                long allocatedBytesBefore = GC.GetAllocatedBytesForCurrentThread();
                long workingSetBefore = Process.GetCurrentProcess().WorkingSet64;
                int gen0CollectionsBefore = GC.CollectionCount(0);
                int gen1CollectionsBefore = GC.CollectionCount(1);
                int gen2CollectionsBefore = GC.CollectionCount(2);
                int progressReportCount = 0;

                cancellationToken.ThrowIfCancellationRequested();
                pauseToken.WaitWhilePaused(cancellationToken);

                string driveRoot = Path.GetPathRoot(rootPath);

                if (string.IsNullOrWhiteSpace(driveRoot))
                {
                    throw new InvalidOperationException(LocalizationService.GetText("Alert.InvalidNtfsDrive"));
                }

                DriveInfo driveInfo = new DriveInfo(driveRoot);

                phaseStopwatch.Restart();
                NtfsReader reader = new NtfsReader(
                    driveInfo,
                    RetrieveMode.Minimal | RetrieveMode.StandardInformations);
                List<INode> nodes = reader.GetNodes(driveRoot);
                phaseStopwatch.Stop();
                TimeSpan mftReadElapsed = phaseStopwatch.Elapsed;

                phaseStopwatch.Restart();

                FileSystemEntry rootEntry = CreateRootEntry(driveRoot);
                string normalizedRootPath = NormalizeDirectoryPath(rootEntry.FullPath);
                bool hasExcludedPaths = _settings.ExcludedPaths != null &&
                    _settings.ExcludedPaths.Any(path => !string.IsNullOrWhiteSpace(path));

                Dictionary<string, FileSystemEntry> directoryEntriesByPath = new Dictionary<string, FileSystemEntry>(StringComparer.OrdinalIgnoreCase)
                {
                    [normalizedRootPath] = rootEntry
                };

                int scannedDirectories = 1;
                int scannedFiles = 0;
                long scannedBytes = 0;
                int processedNodes = 0;

                foreach (INode node in nodes)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    pauseToken.WaitWhilePaused(cancellationToken);

                    if (node == null || string.IsNullOrWhiteSpace(node.FullName))
                        continue;

                    string fullPath = NormalizePath(node.FullName);

                    if (!fullPath.StartsWith(rootEntry.FullPath, StringComparison.OrdinalIgnoreCase))
                        continue;

                    bool isDirectory = node.Attributes.HasFlag(System.IO.Filesystem.Ntfs.Attributes.Directory);

                    if (isDirectory &&
                        string.Equals(NormalizeDirectoryPath(fullPath), normalizedRootPath, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    if (hasExcludedPaths && ScanPathFilter.IsExcluded(fullPath, _settings.ExcludedPaths))
                        continue;

                    if (isDirectory)
                    {
                        FileSystemEntry directoryEntry = EnsureDirectoryEntry(fullPath, rootEntry, directoryEntriesByPath);

                        if (directoryEntry != null)
                        {
                            scannedDirectories++;
                        }
                    }
                    else
                    {
                        long nodeSize = ConvertNodeSize(node.Size);

                        scannedFiles++;
                        scannedBytes += nodeSize;

                        string parentPath = GetParentDirectoryPath(fullPath);

                        if (!string.IsNullOrWhiteSpace(parentPath))
                        {
                            FileSystemEntry parentEntry = EnsureDirectoryEntry(parentPath, rootEntry, directoryEntriesByPath);

                            if (parentEntry != null)
                            {
                                parentEntry.SizeBytes += nodeSize;

                                FileSystemEntry fileEntry = new FileSystemEntry
                                {
                                    Name = Path.GetFileName(fullPath),
                                    FullPath = fullPath,
                                    SizeBytes = nodeSize,
                                    IsDirectory = false,
                                    LastWriteTimeUtc = node.LastChangeTime
                                };

                                rootEntry.AllFiles.Add(fileEntry);

                                if (_settings.ShowFilesInTree)
                                {
                                    parentEntry.Children.Add(fileEntry);
                                }
                            }
                        }
                    }

                    processedNodes++;

                    if (processedNodes % ProgressReportIntervalNodes == 0)
                    {
                        progressReportCount++;
                        progress?.Report(new ScanProgress
                        {
                            CurrentPath = fullPath,
                            ScannedBytes = scannedBytes,
                            ScannedDirectories = scannedDirectories,
                            ScannedFiles = scannedFiles,
                            LiveRootEntry = CreateLiveSnapshot(rootEntry)
                        });
                    }
                }

                phaseStopwatch.Stop();
                TimeSpan nodeProcessingElapsed = phaseStopwatch.Elapsed;

                phaseStopwatch.Restart();
                PropagateDirectorySizes(rootEntry);
                phaseStopwatch.Stop();
                TimeSpan sizeAggregationElapsed = phaseStopwatch.Elapsed;

                phaseStopwatch.Restart();
                SortChildrenRecursive(rootEntry);
                phaseStopwatch.Stop();
                TimeSpan sortingElapsed = phaseStopwatch.Elapsed;

                phaseStopwatch.Restart();
                FileSystemEntry finalSnapshot = CreateLiveSnapshot(rootEntry);
                phaseStopwatch.Stop();
                TimeSpan finalSnapshotElapsed = phaseStopwatch.Elapsed;

                progressReportCount++;
                progress?.Report(new ScanProgress
                {
                    CurrentPath = LocalizationService.GetText("Status.MftFastScanCompleted"),
                    ScannedBytes = rootEntry.SizeBytes,
                    ScannedDirectories = scannedDirectories,
                    ScannedFiles = scannedFiles,
                    LiveRootEntry = finalSnapshot
                });

                totalStopwatch.Stop();

                long allocatedBytesAfter = GC.GetAllocatedBytesForCurrentThread();
                long workingSetAfter = Process.GetCurrentProcess().WorkingSet64;

                AppAlertLog.AddVerboseInformation(
                    "Performance",
                    string.Format(
                        "NtfsMftScanner benchmark: {0:N0} ms",
                        totalStopwatch.Elapsed.TotalMilliseconds),
                    string.Join(
                        Environment.NewLine,
                        string.Format("Path: {0}", rootPath),
                        string.Format("NodesReturned: {0:N0}", nodes.Count),
                        string.Format("ProcessedNodes: {0:N0}", processedNodes),
                        string.Format("Directories: {0:N0}", scannedDirectories),
                        string.Format("Files: {0:N0}", scannedFiles),
                        string.Format("Bytes: {0:N0}", rootEntry.SizeBytes),
                        string.Format("MftReadMilliseconds: {0:N0}", mftReadElapsed.TotalMilliseconds),
                        string.Format("NodeProcessingMilliseconds: {0:N0}", nodeProcessingElapsed.TotalMilliseconds),
                        string.Format("SizeAggregationMilliseconds: {0:N0}", sizeAggregationElapsed.TotalMilliseconds),
                        string.Format("SortingMilliseconds: {0:N0}", sortingElapsed.TotalMilliseconds),
                        string.Format("FinalSnapshotMilliseconds: {0:N0}", finalSnapshotElapsed.TotalMilliseconds),
                        string.Format("TotalMilliseconds: {0:N0}", totalStopwatch.Elapsed.TotalMilliseconds),
                        string.Format("AllocatedBytesCurrentThread: {0:N0}", Math.Max(0, allocatedBytesAfter - allocatedBytesBefore)),
                        string.Format("WorkingSetBeforeBytes: {0:N0}", workingSetBefore),
                        string.Format("WorkingSetAfterBytes: {0:N0}", workingSetAfter),
                        string.Format("Gen0Collections: {0:N0}", GC.CollectionCount(0) - gen0CollectionsBefore),
                        string.Format("Gen1Collections: {0:N0}", GC.CollectionCount(1) - gen1CollectionsBefore),
                        string.Format("Gen2Collections: {0:N0}", GC.CollectionCount(2) - gen2CollectionsBefore),
                        string.Format("ProgressReports: {0:N0}", progressReportCount)));

                return rootEntry;
            }, cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        private FileSystemEntry CreateRootEntry(string rootPath)
        {
            string normalizedRootPath = NormalizeDirectoryPath(rootPath);

            return new FileSystemEntry
            {
                Name = normalizedRootPath,
                FullPath = normalizedRootPath,
                IsDirectory = true
            };
        }
        private long ConvertNodeSize(ulong size)
        {
            if (size > long.MaxValue)
            {
                return long.MaxValue;
            }

            return (long)size;
        }
        private FileSystemEntry EnsureDirectoryEntry(
            string directoryPath,
            FileSystemEntry rootEntry,
            Dictionary<string, FileSystemEntry> directoryEntriesByPath)
        {
            string normalizedDirectoryPath = NormalizeDirectoryPath(directoryPath);

            if (directoryEntriesByPath.TryGetValue(normalizedDirectoryPath, out FileSystemEntry existingEntry))
            {
                return existingEntry;
            }

            if (!normalizedDirectoryPath.StartsWith(rootEntry.FullPath, StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            string parentPath = GetParentDirectoryPath(normalizedDirectoryPath);
            FileSystemEntry parentEntry = string.IsNullOrWhiteSpace(parentPath)
                ? rootEntry
                : EnsureDirectoryEntry(parentPath, rootEntry, directoryEntriesByPath);

            if (parentEntry == null)
            {
                return null;
            }

            FileSystemEntry directoryEntry = new FileSystemEntry
            {
                Name = GetDirectoryName(normalizedDirectoryPath),
                FullPath = normalizedDirectoryPath,
                IsDirectory = true
            };

            parentEntry.Children.Add(directoryEntry);
            directoryEntriesByPath[normalizedDirectoryPath] = directoryEntry;

            return directoryEntry;
        }

        private void PropagateDirectorySizes(FileSystemEntry entry)
        {
            foreach (FileSystemEntry child in entry.Children)
            {
                if (!child.IsDirectory)
                    continue;

                PropagateDirectorySizes(child);
                entry.SizeBytes += child.SizeBytes;
            }
        }

        private void SortChildrenRecursive(FileSystemEntry entry)
        {
            foreach (FileSystemEntry child in entry.Children)
            {
                if (child.IsDirectory)
                {
                    SortChildrenRecursive(child);
                }
            }

            entry.Children.Sort((left, right) =>
            {
                int sizeCompare = right.SizeBytes.CompareTo(left.SizeBytes);

                if (sizeCompare != 0)
                {
                    return sizeCompare;
                }

                return string.Compare(left.Name, right.Name, StringComparison.OrdinalIgnoreCase);
            });
        }

        private FileSystemEntry CreateLiveSnapshot(FileSystemEntry rootEntry)
        {
            FileSystemEntry snapshot = new FileSystemEntry
            {
                Name = rootEntry.Name,
                FullPath = rootEntry.FullPath,
                SizeBytes = rootEntry.SizeBytes,
                IsDirectory = true
            };

            foreach (FileSystemEntry child in rootEntry.Children
                         .Where(child => child.IsDirectory || _settings.ShowFilesInTree)
                         .OrderByDescending(child => child.SizeBytes)
                         .ThenBy(child => child.Name)
                         .Take(100))
            {
                snapshot.Children.Add(new FileSystemEntry
                {
                    Name = child.Name,
                    FullPath = child.FullPath,
                    SizeBytes = child.SizeBytes,
                    IsDirectory = child.IsDirectory
                });
            }

            return snapshot;
        }

        private string NormalizePath(string path)
        {
            if (Path.IsPathFullyQualified(path))
            {
                return path;
            }

            return Path.GetFullPath(path);
        }

        private string NormalizeDirectoryPath(string path)
        {
            string normalizedPath = NormalizePath(path);

            if (!normalizedPath.EndsWith("\\", StringComparison.Ordinal))
            {
                normalizedPath += "\\";
            }

            return normalizedPath;
        }

        private string GetParentDirectoryPath(string path)
        {
            string normalizedPath = path.TrimEnd('\\');
            string parentPath = Path.GetDirectoryName(normalizedPath);

            if (string.IsNullOrWhiteSpace(parentPath))
            {
                return string.Empty;
            }

            return NormalizeDirectoryPath(parentPath);
        }

        private string GetDirectoryName(string directoryPath)
        {
            string normalizedPath = directoryPath.TrimEnd('\\');
            string name = Path.GetFileName(normalizedPath);

            return string.IsNullOrWhiteSpace(name) ? directoryPath : name;
        }
    }
}