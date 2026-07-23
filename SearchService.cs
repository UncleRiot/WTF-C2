using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace c2flux
{
    public sealed class SearchService
    {
        public void Search(
            FileSystemEntry rootEntry,
            SearchCriteria criteria,
            Action<SearchResult> resultCallback,
            Action<int> progressCallback,
            CancellationToken cancellationToken)
        {
            if (rootEntry == null)
                throw new ArgumentNullException(nameof(rootEntry));

            if (criteria == null)
                throw new ArgumentNullException(nameof(criteria));

            if (resultCallback == null)
                throw new ArgumentNullException(nameof(resultCallback));

            HashSet<string> visitedEntries = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            Stack<FileSystemEntry> pendingEntries = new Stack<FileSystemEntry>();
            pendingEntries.Push(rootEntry);

            int processed = 0;

            while (pendingEntries.Count > 0)
            {
                cancellationToken.ThrowIfCancellationRequested();

                FileSystemEntry entry = pendingEntries.Pop();

                if (entry == null || string.IsNullOrWhiteSpace(entry.FullPath))
                    continue;

                string identity = (entry.IsDirectory ? "D|" : "F|") + entry.FullPath;

                if (!visitedEntries.Add(identity))
                    continue;

                processed++;

                if (Matches(entry, criteria))
                {
                    resultCallback(CreateResult(entry));
                }

                if (entry.IsDirectory)
                {
                    PushEntries(entry.Children, pendingEntries);
                    PushEntries(entry.AllFiles, pendingEntries);
                }

                if (processed % 500 == 0)
                {
                    progressCallback?.Invoke(processed);
                }
            }

            progressCallback?.Invoke(processed);
        }

        private static void PushEntries(
            IReadOnlyList<FileSystemEntry> entries,
            Stack<FileSystemEntry> pendingEntries)
        {
            if (entries == null)
                return;

            for (int index = entries.Count - 1; index >= 0; index--)
            {
                FileSystemEntry entry = entries[index];

                if (entry != null)
                {
                    pendingEntries.Push(entry);
                }
            }
        }

        private static bool Matches(FileSystemEntry entry, SearchCriteria criteria)
        {
            if (!MatchesText(entry, criteria))
                return false;

            if (criteria.MinimumSizeBytes.HasValue &&
                entry.SizeBytes < criteria.MinimumSizeBytes.Value)
            {
                return false;
            }

            if (criteria.MaximumSizeBytes.HasValue &&
                entry.SizeBytes > criteria.MaximumSizeBytes.Value)
            {
                return false;
            }

            DateTime modifiedLocal = entry.LastWriteTimeUtc.Kind == DateTimeKind.Utc
                ? entry.LastWriteTimeUtc.ToLocalTime()
                : entry.LastWriteTimeUtc;

            if (criteria.ModifiedAfterLocal.HasValue &&
                modifiedLocal < criteria.ModifiedAfterLocal.Value)
            {
                return false;
            }

            if (criteria.ModifiedBeforeLocal.HasValue &&
                modifiedLocal > criteria.ModifiedBeforeLocal.Value)
            {
                return false;
            }

            if (criteria.FileExtensions.Count > 0)
            {
                if (entry.IsDirectory)
                    return false;

                string extension = Path.GetExtension(entry.Name ?? string.Empty);
                bool extensionMatches = false;

                foreach (string allowedExtension in criteria.FileExtensions)
                {
                    if (string.Equals(
                        extension,
                        allowedExtension,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        extensionMatches = true;
                        break;
                    }
                }

                if (!extensionMatches)
                    return false;
            }

            return true;
        }


        private static bool MatchesText(FileSystemEntry entry, SearchCriteria criteria)
        {
            string searchText = criteria.SearchText?.Trim() ?? string.Empty;

            if (searchText.Length == 0)
                return true;

            string name = entry.Name ?? string.Empty;

            return criteria.MatchMode switch
            {
                SearchMatchMode.StartsWith =>
                    name.StartsWith(searchText, StringComparison.OrdinalIgnoreCase),
                SearchMatchMode.ExactName =>
                    string.Equals(name, searchText, StringComparison.OrdinalIgnoreCase),
                SearchMatchMode.FileExtension =>
                    !entry.IsDirectory &&
                    string.Equals(
                        NormalizeExtension(Path.GetExtension(name)),
                        NormalizeExtension(searchText),
                        StringComparison.OrdinalIgnoreCase),
                _ =>
                    name.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (entry.FullPath ?? string.Empty).IndexOf(
                        searchText,
                        StringComparison.OrdinalIgnoreCase) >= 0
            };
        }

        private static string NormalizeExtension(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            string normalized = value.Trim();
            return normalized.StartsWith(".", StringComparison.Ordinal)
                ? normalized
                : "." + normalized;
        }

        private static SearchResult CreateResult(FileSystemEntry entry)
        {
            string root = Path.GetPathRoot(entry.FullPath) ?? string.Empty;
            DateTime modifiedLocal = entry.LastWriteTimeUtc.Kind == DateTimeKind.Utc
                ? entry.LastWriteTimeUtc.ToLocalTime()
                : entry.LastWriteTimeUtc;

            return new SearchResult
            {
                Drive = root.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar),
                FullPath = entry.FullPath,
                Name = entry.Name,
                SizeBytes = entry.SizeBytes,
                ModifiedLocal = modifiedLocal,
                IsDirectory = entry.IsDirectory
            };
        }
    }
}
