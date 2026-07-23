using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace c2flux
{
    public sealed class NativeShellContextMenuCommand
    {
        public NativeShellContextMenuCommand(string text, Action execute)
        {
            Text = text ?? string.Empty;
            Execute = execute ?? throw new ArgumentNullException(nameof(execute));
        }

        public string Text { get; }
        public Action Execute { get; }
    }

    public static class NativeShellContextMenu
    {
        private const uint CustomCommandIdFirst = 1;
        private const uint ShellCommandIdFirst = 1000;
        private const uint ShellCommandIdLast = 0x7FFF;

        private const uint MF_STRING = 0x0000;
        private const uint MF_BYPOSITION = 0x0400;
        private const uint MF_SEPARATOR = 0x0800;

        private const uint TPM_RIGHTBUTTON = 0x0002;
        private const uint TPM_RETURNCMD = 0x0100;

        private const uint CMF_NORMAL = 0x00000000;
        private const uint CMIC_MASK_UNICODE = 0x00004000;
        private const uint CMIC_MASK_PTINVOKE = 0x20000000;

        private const int SW_SHOWNORMAL = 1;
        private const int WM_NULL = 0x0000;
        private const int WM_INITMENUPOPUP = 0x0117;
        private const int WM_MENUCHAR = 0x0120;
        private const int WM_DRAWITEM = 0x002B;
        private const int WM_MEASUREITEM = 0x002C;

        private static readonly Guid IID_IShellFolder =
            new Guid("000214E6-0000-0000-C000-000000000046");

        private static readonly Guid IID_IContextMenu =
            new Guid("000214E4-0000-0000-C000-000000000046");

        public static bool Show(
            IWin32Window owner,
            string path,
            Point screenLocation,
            IReadOnlyList<NativeShellContextMenuCommand> customCommands,
            AppLayout layout)
        {
            if (owner == null ||
                string.IsNullOrWhiteSpace(path) ||
                customCommands == null)
            {
                return false;
            }

            IntPtr absolutePidl = IntPtr.Zero;
            IntPtr parentFolderPointer = IntPtr.Zero;
            IntPtr contextMenuPointer = IntPtr.Zero;
            IntPtr menuHandle = IntPtr.Zero;
            IShellFolder parentFolder = null;
            IContextMenu contextMenu = null;

            try
            {
                int parseResult = SHParseDisplayName(
                    path,
                    IntPtr.Zero,
                    out absolutePidl,
                    0,
                    out _);

                if (parseResult != 0 || absolutePidl == IntPtr.Zero)
                    return false;

                Guid shellFolderGuid = IID_IShellFolder;
                int bindResult = SHBindToParent(
                    absolutePidl,
                    ref shellFolderGuid,
                    out parentFolderPointer,
                    out IntPtr childPidl);

                if (bindResult != 0 ||
                    parentFolderPointer == IntPtr.Zero ||
                    childPidl == IntPtr.Zero)
                {
                    return false;
                }

                parentFolder = (IShellFolder)Marshal.GetObjectForIUnknown(
                    parentFolderPointer);

                Guid contextMenuGuid = IID_IContextMenu;
                IntPtr[] childPidls = { childPidl };

                int contextResult = parentFolder.GetUIObjectOf(
                    owner.Handle,
                    1,
                    childPidls,
                    ref contextMenuGuid,
                    IntPtr.Zero,
                    out contextMenuPointer);

                if (contextResult != 0 || contextMenuPointer == IntPtr.Zero)
                    return false;

                contextMenu = (IContextMenu)Marshal.GetObjectForIUnknown(
                    contextMenuPointer);

                menuHandle = CreatePopupMenu();

                if (menuHandle == IntPtr.Zero)
                    return false;

                int menuPosition = 0;

                for (int index = 0; index < customCommands.Count; index++)
                {
                    InsertMenu(
                        menuHandle,
                        (uint)menuPosition,
                        MF_BYPOSITION | MF_STRING,
                        new UIntPtr(CustomCommandIdFirst + (uint)index),
                        EscapeMenuText(customCommands[index].Text));

                    menuPosition++;
                }

                if (customCommands.Count > 0)
                {
                    InsertMenu(
                        menuHandle,
                        (uint)menuPosition,
                        MF_BYPOSITION | MF_SEPARATOR,
                        UIntPtr.Zero,
                        null);

                    menuPosition++;
                }

                int queryResult = contextMenu.QueryContextMenu(
                    menuHandle,
                    (uint)menuPosition,
                    ShellCommandIdFirst,
                    ShellCommandIdLast,
                    CMF_NORMAL);

                if (queryResult < 0)
                    return false;

                NativeMenuTheme.Apply(layout);

                IContextMenu3 contextMenu3 = contextMenu as IContextMenu3;
                IContextMenu2 contextMenu2 = contextMenu as IContextMenu2;

                using ShellMenuMessageWindow messageWindow =
                    new ShellMenuMessageWindow(
                        owner.Handle,
                        contextMenu2,
                        contextMenu3);

                SetForegroundWindow(owner.Handle);

                uint selectedCommand = TrackPopupMenuEx(
                    menuHandle,
                    TPM_RIGHTBUTTON | TPM_RETURNCMD,
                    screenLocation.X,
                    screenLocation.Y,
                    owner.Handle,
                    IntPtr.Zero);

                PostMessage(owner.Handle, WM_NULL, IntPtr.Zero, IntPtr.Zero);

                if (selectedCommand == 0)
                    return true;

                uint customCommandIdLast =
                    CustomCommandIdFirst +
                    (uint)customCommands.Count -
                    1;

                if (selectedCommand >= CustomCommandIdFirst &&
                    selectedCommand <= customCommandIdLast)
                {
                    int customCommandIndex =
                        (int)(selectedCommand - CustomCommandIdFirst);

                    customCommands[customCommandIndex].Execute();
                    return true;
                }

                if (selectedCommand < ShellCommandIdFirst)
                    return true;

                InvokeShellCommand(
                    contextMenu,
                    owner.Handle,
                    selectedCommand - ShellCommandIdFirst,
                    screenLocation);

                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                if (menuHandle != IntPtr.Zero)
                {
                    DestroyMenu(menuHandle);
                }

                ReleaseComObject(contextMenu);
                ReleaseComObject(parentFolder);

                if (contextMenuPointer != IntPtr.Zero)
                {
                    Marshal.Release(contextMenuPointer);
                }

                if (parentFolderPointer != IntPtr.Zero)
                {
                    Marshal.Release(parentFolderPointer);
                }

                if (absolutePidl != IntPtr.Zero)
                {
                    Marshal.FreeCoTaskMem(absolutePidl);
                }
            }
        }

        private static void InvokeShellCommand(
            IContextMenu contextMenu,
            IntPtr ownerHandle,
            uint commandOffset,
            Point screenLocation)
        {
            CMINVOKECOMMANDINFOEX commandInfo =
                new CMINVOKECOMMANDINFOEX
                {
                    cbSize = Marshal.SizeOf<CMINVOKECOMMANDINFOEX>(),
                    fMask = CMIC_MASK_UNICODE | CMIC_MASK_PTINVOKE,
                    hwnd = ownerHandle,
                    lpVerb = new IntPtr(commandOffset),
                    lpVerbW = new IntPtr(commandOffset),
                    nShow = SW_SHOWNORMAL,
                    ptInvoke = new POINT
                    {
                        X = screenLocation.X,
                        Y = screenLocation.Y
                    }
                };

            contextMenu.InvokeCommand(ref commandInfo);
        }

        private static string EscapeMenuText(string text)
        {
            return (text ?? string.Empty).Replace("&", "&&");
        }

        private static void ReleaseComObject(object comObject)
        {
            if (comObject == null || !Marshal.IsComObject(comObject))
                return;

            Marshal.FinalReleaseComObject(comObject);
        }

        private sealed class ShellMenuMessageWindow : NativeWindow, IDisposable
        {
            private readonly IContextMenu2 _contextMenu2;
            private readonly IContextMenu3 _contextMenu3;

            public ShellMenuMessageWindow(
                IntPtr ownerHandle,
                IContextMenu2 contextMenu2,
                IContextMenu3 contextMenu3)
            {
                _contextMenu2 = contextMenu2;
                _contextMenu3 = contextMenu3;
                AssignHandle(ownerHandle);
            }

            public void Dispose()
            {
                ReleaseHandle();
            }

            protected override void WndProc(ref Message message)
            {
                if (message.Msg == WM_INITMENUPOPUP ||
                    message.Msg == WM_DRAWITEM ||
                    message.Msg == WM_MEASUREITEM ||
                    message.Msg == WM_MENUCHAR)
                {
                    if (_contextMenu3 != null)
                    {
                        int result = _contextMenu3.HandleMenuMsg2(
                            (uint)message.Msg,
                            message.WParam,
                            message.LParam,
                            out IntPtr returnValue);

                        if (result == 0)
                        {
                            message.Result = returnValue;
                            return;
                        }
                    }
                    else if (_contextMenu2 != null)
                    {
                        int result = _contextMenu2.HandleMenuMsg(
                            (uint)message.Msg,
                            message.WParam,
                            message.LParam);

                        if (result == 0)
                        {
                            message.Result = IntPtr.Zero;
                            return;
                        }
                    }
                }

                base.WndProc(ref message);
            }
        }

        private static class NativeMenuTheme
        {
            private const int SetPreferredAppModeOrdinal = 135;
            private const int FlushMenuThemesOrdinal = 136;

            private delegate int SetPreferredAppModeDelegate(
                PreferredAppMode preferredAppMode);

            private delegate void FlushMenuThemesDelegate();

            private enum PreferredAppMode
            {
                Default = 0,
                AllowDark = 1,
                ForceDark = 2,
                ForceLight = 3
            }

            public static void Apply(AppLayout layout)
            {
                if (Environment.OSVersion.Version.Major < 10)
                    return;

                IntPtr moduleHandle = LoadLibrary("uxtheme.dll");

                if (moduleHandle == IntPtr.Zero)
                    return;

                try
                {
                    IntPtr setPreferredAppModePointer = GetProcAddress(
                        moduleHandle,
                        new IntPtr(SetPreferredAppModeOrdinal));

                    IntPtr flushMenuThemesPointer = GetProcAddress(
                        moduleHandle,
                        new IntPtr(FlushMenuThemesOrdinal));

                    if (setPreferredAppModePointer == IntPtr.Zero ||
                        flushMenuThemesPointer == IntPtr.Zero)
                    {
                        return;
                    }

                    SetPreferredAppModeDelegate setPreferredAppMode =
                        Marshal.GetDelegateForFunctionPointer<SetPreferredAppModeDelegate>(
                            setPreferredAppModePointer);

                    FlushMenuThemesDelegate flushMenuThemes =
                        Marshal.GetDelegateForFunctionPointer<FlushMenuThemesDelegate>(
                            flushMenuThemesPointer);

                    PreferredAppMode preferredAppMode =
                        layout == AppLayout.WindowsDarkMode
                            ? PreferredAppMode.ForceDark
                            : layout == AppLayout.WindowsLightMode
                                ? PreferredAppMode.ForceLight
                                : PreferredAppMode.AllowDark;

                    setPreferredAppMode(preferredAppMode);
                    flushMenuThemes();
                }
                catch
                {
                }
                finally
                {
                    FreeLibrary(moduleHandle);
                }
            }
        }

        [ComImport]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("000214E6-0000-0000-C000-000000000046")]
        private interface IShellFolder
        {
            [PreserveSig]
            int ParseDisplayName(
                IntPtr hwnd,
                IntPtr bindContext,
                [MarshalAs(UnmanagedType.LPWStr)] string displayName,
                ref uint eaten,
                out IntPtr itemIdList,
                ref uint attributes);

            [PreserveSig]
            int EnumObjects(
                IntPtr hwnd,
                int flags,
                out IntPtr enumIdList);

            [PreserveSig]
            int BindToObject(
                IntPtr itemIdList,
                IntPtr bindContext,
                ref Guid interfaceId,
                out IntPtr result);

            [PreserveSig]
            int BindToStorage(
                IntPtr itemIdList,
                IntPtr bindContext,
                ref Guid interfaceId,
                out IntPtr result);

            [PreserveSig]
            int CompareIDs(
                IntPtr lParam,
                IntPtr firstItemIdList,
                IntPtr secondItemIdList);

            [PreserveSig]
            int CreateViewObject(
                IntPtr hwndOwner,
                ref Guid interfaceId,
                out IntPtr result);

            [PreserveSig]
            int GetAttributesOf(
                uint itemCount,
                [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)]
                IntPtr[] itemIdLists,
                ref uint attributes);

            [PreserveSig]
            int GetUIObjectOf(
                IntPtr hwndOwner,
                uint itemCount,
                [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)]
                IntPtr[] itemIdLists,
                ref Guid interfaceId,
                IntPtr reserved,
                out IntPtr result);

            [PreserveSig]
            int GetDisplayNameOf(
                IntPtr itemIdList,
                uint flags,
                out STRRET name);

            [PreserveSig]
            int SetNameOf(
                IntPtr hwnd,
                IntPtr itemIdList,
                [MarshalAs(UnmanagedType.LPWStr)] string name,
                uint flags,
                out IntPtr renamedItemIdList);
        }

        [ComImport]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("000214E4-0000-0000-C000-000000000046")]
        private interface IContextMenu
        {
            [PreserveSig]
            int QueryContextMenu(
                IntPtr menuHandle,
                uint indexMenu,
                uint commandIdFirst,
                uint commandIdLast,
                uint flags);

            [PreserveSig]
            int InvokeCommand(ref CMINVOKECOMMANDINFOEX commandInfo);

            [PreserveSig]
            int GetCommandString(
                UIntPtr commandId,
                uint flags,
                IntPtr reserved,
                IntPtr name,
                uint maximumCharacters);
        }

        [ComImport]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("000214F4-0000-0000-C000-000000000046")]
        private interface IContextMenu2 : IContextMenu
        {
            [PreserveSig]
            new int QueryContextMenu(
                IntPtr menuHandle,
                uint indexMenu,
                uint commandIdFirst,
                uint commandIdLast,
                uint flags);

            [PreserveSig]
            new int InvokeCommand(ref CMINVOKECOMMANDINFOEX commandInfo);

            [PreserveSig]
            new int GetCommandString(
                UIntPtr commandId,
                uint flags,
                IntPtr reserved,
                IntPtr name,
                uint maximumCharacters);

            [PreserveSig]
            int HandleMenuMsg(
                uint message,
                IntPtr wParam,
                IntPtr lParam);
        }

        [ComImport]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("BCFCE0A0-EC17-11D0-8D10-00A0C90F2719")]
        private interface IContextMenu3 : IContextMenu2
        {
            [PreserveSig]
            new int QueryContextMenu(
                IntPtr menuHandle,
                uint indexMenu,
                uint commandIdFirst,
                uint commandIdLast,
                uint flags);

            [PreserveSig]
            new int InvokeCommand(ref CMINVOKECOMMANDINFOEX commandInfo);

            [PreserveSig]
            new int GetCommandString(
                UIntPtr commandId,
                uint flags,
                IntPtr reserved,
                IntPtr name,
                uint maximumCharacters);

            [PreserveSig]
            new int HandleMenuMsg(
                uint message,
                IntPtr wParam,
                IntPtr lParam);

            [PreserveSig]
            int HandleMenuMsg2(
                uint message,
                IntPtr wParam,
                IntPtr lParam,
                out IntPtr result);
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct CMINVOKECOMMANDINFOEX
        {
            public int cbSize;
            public uint fMask;
            public IntPtr hwnd;
            public IntPtr lpVerb;
            public IntPtr lpParameters;
            public IntPtr lpDirectory;
            public int nShow;
            public uint dwHotKey;
            public IntPtr hIcon;
            public IntPtr lpTitle;
            public IntPtr lpVerbW;
            public IntPtr lpParametersW;
            public IntPtr lpDirectoryW;
            public IntPtr lpTitleW;
            public POINT ptInvoke;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int X;
            public int Y;
        }

        [StructLayout(LayoutKind.Explicit, Size = 520)]
        private struct STRRET
        {
            [FieldOffset(0)]
            public uint Type;

            [FieldOffset(4)]
            public IntPtr Pointer;

            [FieldOffset(4)]
            public uint Offset;

            [FieldOffset(4)]
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 260)]
            public byte[] CharacterBuffer;
        }

        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        private static extern int SHParseDisplayName(
            string name,
            IntPtr bindContext,
            out IntPtr itemIdList,
            uint attributesIn,
            out uint attributesOut);

        [DllImport("shell32.dll")]
        private static extern int SHBindToParent(
            IntPtr itemIdList,
            ref Guid interfaceId,
            out IntPtr parentFolder,
            out IntPtr childItemIdList);

        [DllImport("user32.dll")]
        private static extern IntPtr CreatePopupMenu();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DestroyMenu(IntPtr menuHandle);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool InsertMenu(
            IntPtr menuHandle,
            uint position,
            uint flags,
            UIntPtr newItemId,
            string newItem);

        [DllImport("user32.dll")]
        private static extern uint TrackPopupMenuEx(
            IntPtr menuHandle,
            uint flags,
            int x,
            int y,
            IntPtr ownerHandle,
            IntPtr parameters);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetForegroundWindow(IntPtr windowHandle);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool PostMessage(
            IntPtr windowHandle,
            int message,
            IntPtr wParam,
            IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr LoadLibrary(string fileName);

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool FreeLibrary(IntPtr moduleHandle);

        [DllImport("kernel32.dll", EntryPoint = "GetProcAddress")]
        private static extern IntPtr GetProcAddress(
            IntPtr moduleHandle,
            IntPtr procedureName);
    }
}
