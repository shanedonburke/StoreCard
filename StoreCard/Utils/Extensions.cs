using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;

namespace StoreCard.Utils;

public static class Extensions
{
    public static void BringToFront(this Window window)
    {
        window.Activate();
        window.WindowState = WindowState.Normal;
        window.Topmost = true;
        window.Topmost = false;
        window.Focus();
    }

    public static string ToBase64(this BitmapSource bitmap)
    {
        return ImageUtils.ImageToBase64(bitmap);
    }

    public static void Deconstruct<T>(this IEnumerable<T> seq, out T? first, out IEnumerable<T> rest)
    {
        IEnumerable<T> enumerable = seq.ToList();
        first = enumerable.FirstOrDefault();
        rest = enumerable.Skip(1);
    }

    public static void Deconstruct<T>(
        this IEnumerable<T> seq,
        out T? first,
        out T? second,
        out IEnumerable<T> rest)
        => (first, (second, rest)) = seq;
}
