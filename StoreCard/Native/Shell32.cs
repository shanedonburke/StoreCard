#region

using System;
using System.Runtime.InteropServices;

#endregion

namespace StoreCard.Native;

/// <summary>
/// Wrapper for <c>Shell32.dll</c>.
/// Used to get file information.
/// </summary>
public class Shell32
{
    // Constant flags for SHGetFileInfo 
    public const uint ShgfiIcon = 0x100;
    public const uint ShgfiLargeicon = 0x0;

    [StructLayout(LayoutKind.Sequential)]
    public struct Shfileinfo
    {
        public IntPtr hIcon;

        public int iIcon;

        public uint dwAttributes;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string szDisplayName;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
        public string szTypeName;
    }

    [DllImport("shell32.dll")]
    public static extern IntPtr SHGetFileInfo(
        string pszPath,
        uint dwFileAttributes,
        ref Shfileinfo psfi,
        uint cbSizeFileInfo,
        uint uFlags);
}
