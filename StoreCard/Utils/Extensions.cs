#region

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;

#endregion

namespace StoreCard.Utils;

/// <summary>
/// Extension methods.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Brings this window to the front (i.e., in front of all other windows) and focuses it.
    /// </summary>
    /// <param name="window"></param>
    public static void BringToFront(this Window window)
    {
        window.Activate();
        window.WindowState = WindowState.Normal;

        // This brings it to the front, but allows other windows to be in front afterward
        window.Topmost = true;
        window.Topmost = false;

        window.Focus();
    }

    /// <summary>
    /// Converts this bitmap to its Base64 representation.
    /// </summary>
    /// <param name="bitmap"></param>
    /// <returns>Base64 representation</returns>
    public static string ToBase64(this BitmapSource bitmap) => ImageUtils.ImageToBase64(bitmap);

    public static byte[] ToAsciiBytes(this string str)
    {
        byte[] utf8Bytes = Encoding.UTF8.GetBytes(str);
        string asciiStr = Encoding.ASCII.GetString(utf8Bytes);
        return Encoding.ASCII.GetBytes(asciiStr);
    }

    /// <summary>
    /// Deconstructs the given sequence into its first element, followed by the rest of the elements.
    /// </summary>
    /// <typeparam name="T">Type of the elements</typeparam>
    /// <param name="seq"></param>
    /// <param name="first">First element, or <c>null</c> if the sequence is empty</param>
    /// <param name="rest">Rest of the elements</param>
    public static void Deconstruct<T>(this IEnumerable<T> seq, out T? first, out IEnumerable<T> rest)
    {
        IEnumerable<T> enumerable = seq.ToList();
        first = enumerable.FirstOrDefault();
        rest = enumerable.Skip(1);
    }

    /// <summary>
    /// Deconstructs the given sequence into its first and second elements,
    /// followed by the rest of the elements.
    /// </summary>
    /// <typeparam name="T">Type of the elements</typeparam>
    /// <param name="seq"></param>
    /// <param name="first">First element, or <c>null</c> if the sequence is empty</param>
    /// <param name="second">Second element, or <c>null</c> if the sequence's length is less than 2</param>
    /// <param name="rest">Rest of the elements</param>
    public static void Deconstruct<T>(
        this IEnumerable<T> seq,
        out T? first,
        out T? second,
        out IEnumerable<T> rest)
        => (first, (second, rest)) = seq;
}
