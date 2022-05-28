using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using StoreCard.Native;

namespace StoreCard.Utils;

public class IconUtils
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

        // Pass in folder path
        Shell32.SHGetFileInfo(
            path,
            0,
            ref shinfo,
            (uint)Marshal.SizeOf(shinfo),
            Shell32.ShgfiIcon | Shell32.ShgfiLargeicon);

        using var hIcon = Icon.FromHandle(shinfo.hIcon);
        return CreateBitmapSourceFromHIcon(hIcon);
    }
}
