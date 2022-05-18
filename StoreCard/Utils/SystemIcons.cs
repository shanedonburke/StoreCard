using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using StoreCard.Native;

namespace StoreCard.Utils;

internal class SystemIcons
{
    public static ImageSource? GetFileIconByPath(string path)
    {
        var icon = Icon.ExtractAssociatedIcon(path);
        if (icon == null) return null;
        return Imaging.CreateBitmapSourceFromHIcon(
            icon.Handle,
            Int32Rect.Empty,
            BitmapSizeOptions.FromEmptyOptions());
    }

    public static ImageSource GetFolderIconByPath(string path)
    {
        var shinfo = new Shell32.Shfileinfo();

        // Call function with the path to the folder you want the icon for
        Shell32.SHGetFileInfo(
            path,
            0,
            ref shinfo,
            (uint) Marshal.SizeOf(shinfo),
            Shell32.ShgfiIcon | Shell32.ShgfiLargeicon);

        using var i = Icon.FromHandle(shinfo.hIcon);

        // Convert icon to a bitmap source
        return Imaging.CreateBitmapSourceFromHIcon(
            i.Handle,
            new Int32Rect(0, 0, i.Width, i.Height),
            BitmapSizeOptions.FromEmptyOptions());
    }
}