#region

using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using StoreCard.Native;

#endregion

namespace StoreCard.Utils;

/// <summary>
/// Utilities for working with icons.
/// </summary>
public static class IconUtils
{
    /// <summary>
    /// Converts an <c>Icon</c> to a <c>BitmapSource</c>.
    /// </summary>
    /// <param name="icon"></param>
    /// <returns>Bitmap source</returns>
    public static BitmapSource CreateBitmapSourceFromHIcon(Icon icon)
    {
        BitmapSource bitmapSource = Imaging.CreateBitmapSourceFromHIcon(
            icon.Handle,
            Int32Rect.Empty,
            BitmapSizeOptions.FromEmptyOptions());
        bitmapSource.Freeze();
        return bitmapSource;
    }

    /// <summary>
    /// Returns the icon of the file at the given path. The path must be to a file,
    /// not a folder. For folders, use <see cref="GetFolderIconByPath"/>.
    /// </summary>
    /// <param name="path">Absolute file path</param>
    /// <returns>Icon as a bitmap source</returns>
    public static BitmapSource? GetFileIconByPath(string path)
    {
        using var hIcon = Icon.ExtractAssociatedIcon(path);
        return hIcon == null ? null : CreateBitmapSourceFromHIcon(hIcon);
    }

    /// <summary>
    /// Returns the icon of the folder at the given path. The path must be to a folder,
    /// not a file. For files, see <see cref="GetFileIconByPath"/>.
    /// </summary>
    /// <param name="path">Absolute file path</param>
    /// <returns>Icon as a bitmap source</returns>
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
