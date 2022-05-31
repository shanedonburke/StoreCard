#region

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;

#endregion

namespace StoreCard.Utils;

/// <summary>
/// Utilities for working with images.
/// </summary>
public static class ImageUtils
{
    /// <summary>
    /// Convert an image to its Base64 representation
    /// </summary>
    /// <param name="bitmap"></param>
    /// <returns></returns>
    public static string ImageToBase64(BitmapSource bitmap)
    {
        var encoder = new PngBitmapEncoder();
        var frame = BitmapFrame.Create(bitmap);
        encoder.Frames.Add(frame);
        using var stream = new MemoryStream();
        encoder.Save(stream);
        return Convert.ToBase64String(stream.ToArray());
    }

    /// <summary>
    /// Convert the Base64 representation of a bitmap image to an actual image.
    /// From <see href="https://stackoverflow.com/a/593406">this post</see>.
    /// </summary>
    /// <param name="base64"></param>
    /// <returns></returns>
    public static BitmapImage Base64ToImage(string base64)
    {
        byte[] binaryData = Convert.FromBase64String(base64);

        var bi = new BitmapImage();
        bi.BeginInit();
        bi.StreamSource = new MemoryStream(binaryData);
        bi.EndInit();

        return bi;
    }

    /// <summary>
    /// Converts a <see cref="Bitmap"/> to a <see cref="BitmapImage"/>.
    /// </summary>
    /// <param name="bitmap"></param>
    /// <returns></returns>
    public static BitmapImage BitmapToBitmapImage(Bitmap bitmap)
    {
        using var memory = new MemoryStream();
        bitmap.Save(memory, ImageFormat.Png);
        memory.Position = 0;

        var bitmapImage = new BitmapImage();
        bitmapImage.BeginInit();
        bitmapImage.StreamSource = memory;
        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
        bitmapImage.EndInit();

        // Must freeze image to access in other threads
        bitmapImage.Freeze();

        return bitmapImage;
    }

    /// <summary>
    /// Converts an array of bytes representing a <see cref="BitmapImage"/> to an actual iamge.
    /// </summary>
    /// <param name="bytes"></param>
    /// <returns></returns>
    public static BitmapImage BytesToBitmapImage(byte[] bytes)
    {
        var ms = new MemoryStream(bytes);
        var bitmap = new Bitmap(ms);
        return BitmapToBitmapImage(bitmap);
    }
}
