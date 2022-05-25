using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;

namespace StoreCard.Utils;

internal class ImageUtils
{
    public static string ImageToBase64(BitmapSource bitmap)
    {
        var encoder = new PngBitmapEncoder();
        var frame = BitmapFrame.Create(bitmap);
        encoder.Frames.Add(frame);
        using var stream = new MemoryStream();
        encoder.Save(stream);
        return Convert.ToBase64String(stream.ToArray());
    }

    // From https://stackoverflow.com/a/593406
    public static BitmapImage Base64ToImage(string base64)
    {
        byte[] binaryData = Convert.FromBase64String(base64);

        var bi = new BitmapImage();
        bi.BeginInit();
        bi.StreamSource = new MemoryStream(binaryData);
        bi.EndInit();

        return bi;
    }

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
        bitmapImage.Freeze();

        return bitmapImage;
    }

    public static BitmapImage BytesToBitmapImage(byte[] bytes)
    {
        var ms = new MemoryStream(bytes);
        var bitmap = new Bitmap(ms);
        return BitmapToBitmapImage(bitmap);
    }
}
