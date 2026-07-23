using Microsoft.Win32;
using System;
using System.Windows.Forms;

namespace c2flux
{
    public static class ShellContextMenuService
    {
        private const string DirectoryShellKeyPath = @"Software\Classes\Directory\shell\WTF";
        private const string FolderShellKeyPath = @"Software\Classes\Folder\shell\WTF";
        private const string DriveShellKeyPath = @"Software\Classes\Drive\shell\WTF";
        private const string DirectoryBackgroundShellKeyPath = @"Software\Classes\Directory\Background\shell\WTF";

        public static void Apply(bool enabled)
        {
            if (enabled)
            {
                Register();
                return;
            }

            Unregister();
        }

        private static void Register()
        {
            RegisterShellEntry(DirectoryShellKeyPath, "%1", true);
            RegisterShellEntry(FolderShellKeyPath, "%1", true);
            RegisterShellEntry(DriveShellKeyPath, "%1", false);
            RegisterShellEntry(DirectoryBackgroundShellKeyPath, "%V", true);
        }

        private static void Unregister()
        {
            Registry.CurrentUser.DeleteSubKeyTree(DirectoryShellKeyPath, false);
            Registry.CurrentUser.DeleteSubKeyTree(FolderShellKeyPath, false);
            Registry.CurrentUser.DeleteSubKeyTree(DriveShellKeyPath, false);
            Registry.CurrentUser.DeleteSubKeyTree(DirectoryBackgroundShellKeyPath, false);
        }

        private static void RegisterShellEntry(string shellKeyPath, string pathArgument, bool quotePathArgument)
        {
            string executablePath = Application.ExecutablePath;
            string commandText = "\"" + executablePath + "\" " + (quotePathArgument ? "\"" + pathArgument + "\"" : pathArgument);

            using RegistryKey shellKey = Registry.CurrentUser.CreateSubKey(shellKeyPath);
            shellKey.SetValue(
                string.Empty,
                AppConstants.ShellContextMenuText);
            shellKey.SetValue("Icon", executablePath + ",0");

            using RegistryKey commandKey = shellKey.CreateSubKey("command");
            commandKey.SetValue(string.Empty, commandText);
        }
    }
}