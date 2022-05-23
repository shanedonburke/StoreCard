using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Media.Imaging;

namespace StoreCard.Utils;

internal static class Extensions
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
        return Images.ImageToBase64(bitmap);
    }

    public static void Deconstruct<T>(this IEnumerable<T> seq, out T? first, out IEnumerable<T> rest)
    {
        IEnumerable<T> enumerable = seq as T[] ?? seq.ToArray();
        first = enumerable.FirstOrDefault();
        rest = enumerable.Skip(1);
    }

    public static void Deconstruct<T>(
        this IEnumerable<T> seq,
        out T? first,
        out T? second,
        out IEnumerable<T> rest)
        => (first, (second, rest)) = seq;

    public static byte[] ReceiveAll(this Socket socket)
    {
        var buffer = new List<byte>();

        while (socket.Available > 0)
        {
            var currByte = new Byte[1];
            var byteCounter = socket.Receive(currByte, currByte.Length, SocketFlags.None);

            if (byteCounter.Equals(1))
            {
                buffer.Add(currByte[0]);
            }
        }

        return buffer.ToArray();
    }
}
