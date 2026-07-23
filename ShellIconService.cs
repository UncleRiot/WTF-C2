using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;

namespace c2flux
{
    public enum ShellStockIconId : uint
    {
        FolderOpen = 4,
        Find = 22
    }

    public sealed class ShellIconService
    {
        private const int SHGFI_ICON = 0x100;
        private const int SHGFI_SMALLICON = 0x1;
        private const int SHGFI_USEFILEATTRIBUTES = 0x10;
        private const uint FILE_ATTRIBUTE_DIRECTORY = 0x10;
        private const uint FILE_ATTRIBUTE_NORMAL = 0x80;

        private const uint SHGSI_ICON = 0x000000100;
        private const uint SHGSI_SMALLICON = 0x000000001;

        [DllImport("shell32.dll")]
        private static extern IntPtr SHGetFileInfo(
            string pszPath,
            uint dwFileAttributes,
            ref SHFILEINFO psfi,
            uint cbFileInfo,
            uint uFlags);

        [DllImport("shell32.dll", SetLastError = false)]
        private static extern int SHGetStockIconInfo(
            ShellStockIconId siid,
            uint uFlags,
            ref SHSTOCKICONINFO psii);

        [DllImport("user32.dll")]
        private static extern bool DestroyIcon(IntPtr hIcon);

        public Bitmap GetSmallSystemIcon(string path)
        {
            SHFILEINFO shellFileInfo = new SHFILEINFO();

            bool directoryExists = Directory.Exists(path);
            bool fileExists = File.Exists(path);

            uint attributes = directoryExists
                ? FILE_ATTRIBUTE_DIRECTORY
                : FILE_ATTRIBUTE_NORMAL;

            uint flags = SHGFI_ICON | SHGFI_SMALLICON;

            if (!directoryExists && !fileExists)
            {
                flags |= SHGFI_USEFILEATTRIBUTES;
            }

            IntPtr result = SHGetFileInfo(
                path,
                attributes,
                ref shellFileInfo,
                (uint)Marshal.SizeOf(typeof(SHFILEINFO)),
                flags);

            if (result == IntPtr.Zero || shellFileInfo.hIcon == IntPtr.Zero)
            {
                return SystemIcons.Application.ToBitmap();
            }

            try
            {
                using Icon icon = (Icon)Icon.FromHandle(shellFileInfo.hIcon).Clone();
                return icon.ToBitmap();
            }
            finally
            {
                DestroyIcon(shellFileInfo.hIcon);
            }
        }

        public Bitmap GetSmallStockIcon(ShellStockIconId stockIconId)
        {
            SHSTOCKICONINFO stockIconInfo = new SHSTOCKICONINFO();
            stockIconInfo.cbSize = (uint)Marshal.SizeOf(typeof(SHSTOCKICONINFO));

            int result = SHGetStockIconInfo(
                stockIconId,
                SHGSI_ICON | SHGSI_SMALLICON,
                ref stockIconInfo);

            if (result != 0 || stockIconInfo.hIcon == IntPtr.Zero)
            {
                return SystemIcons.Application.ToBitmap();
            }

            try
            {
                using Icon icon = (Icon)Icon.FromHandle(stockIconInfo.hIcon).Clone();
                return icon.ToBitmap();
            }
            finally
            {
                DestroyIcon(stockIconInfo.hIcon);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct SHSTOCKICONINFO
        {
            public uint cbSize;
            public IntPtr hIcon;
            public int iSysImageIndex;
            public int iIcon;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szPath;
        }
    }
}
