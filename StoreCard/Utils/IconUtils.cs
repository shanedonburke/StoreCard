using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using StoreCard.Native;

namespace StoreCard.Utils;

internal class IconUtils
{
    public static BitmapSource CreateBitmapSourceFromHIcon(Icon icon)
    {
        BitmapSource bitmapSource = Imaging.CreateBitmapSourceFromHIcon(
            icon.Handle,
            Int32Rect.Empty,
            BitmapSizeOptions.FromEmptyOptions());
        bitmapSource.Freeze();
        return bitmapSource;
    }

    public static BitmapSource? GetFileIconByPath(string path)
    {
        var hIcon = Icon.ExtractAssociatedIcon(path);
        return hIcon == null ? null : CreateBitmapSourceFromHIcon(hIcon);
    }

    public static ImageSource GetFolderIconByPath(string path)
    {
        var shinfo = new Shell32.Shfileinfo();

        // Call function with the path to the folder you want the icon for
        Shell32.SHGetFileInfo(
            path,
            0,
            ref shinfo,
            (uint)Marshal.SizeOf(shinfo),
            Shell32.ShgfiIcon | Shell32.ShgfiLargeicon);

        using var i = Icon.FromHandle(shinfo.hIcon);

        // Convert icon to a bitmap source
        return Imaging.CreateBitmapSourceFromHIcon(
            i.Handle,
            new Int32Rect(0, 0, i.Width, i.Height),
            BitmapSizeOptions.FromEmptyOptions());
    }
}
