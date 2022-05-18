using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace StoreCard.Native;

internal class Shell32
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
    };

    // Import SHGetFileInfo function
    [DllImport("shell32.dll")]
    public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes,
        ref Shfileinfo psfi,
        uint cbSizeFileInfo, uint uFlags);
}