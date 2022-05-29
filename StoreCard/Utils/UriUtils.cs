using System;

namespace StoreCard.Utils;
public class UriUtils
{
    public static Uri BuildPackUri(string path)
    {
        string normalized = path[0] == '/' ? path.Substring(1) : path;
        return new Uri($"pack://application:,,,/{normalized}");
    }
}
