using System;
using System.Linq;

namespace c2flux
{
    public static class TreeSortService
    {
        public static void Sort(FileSystemEntry entry, TreeSortMode mode)
        {
            if (entry == null)
                return;

            foreach (FileSystemEntry child in entry.Children.Where(child => child.IsDirectory))
            {
                Sort(child, mode);
            }

            System.Collections.Generic.IEnumerable<FileSystemEntry> ordered = entry.Children;

            switch (mode)
            {
                case TreeSortMode.SizeAscending:
                    ordered = entry.Children.OrderBy(child => child.SizeBytes).ThenBy(child => child.Name);
                    break;
                case TreeSortMode.NameAscending:
                    ordered = entry.Children.OrderBy(child => child.Name, StringComparer.CurrentCultureIgnoreCase);
                    break;
                case TreeSortMode.NameDescending:
                    ordered = entry.Children.OrderByDescending(child => child.Name, StringComparer.CurrentCultureIgnoreCase);
                    break;
                case TreeSortMode.DateDescending:
                    ordered = entry.Children.OrderByDescending(child => child.LastWriteTimeUtc).ThenBy(child => child.Name);
                    break;
                case TreeSortMode.DateAscending:
                    ordered = entry.Children.OrderBy(child => child.LastWriteTimeUtc).ThenBy(child => child.Name);
                    break;
                default:
                    ordered = entry.Children.OrderByDescending(child => child.SizeBytes).ThenBy(child => child.Name);
                    break;
            }

            System.Collections.Generic.List<FileSystemEntry> sorted = ordered.ToList();
            entry.Children.Clear();
            entry.Children.AddRange(sorted);
        }
    }
}
