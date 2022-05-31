using System;

namespace StoreCard.Utils;

/// <summary>
/// Utilities for working with resource URIs.
/// </summary>
public static class UriUtils
{
    /// <summary>
    /// Build a URI to reference an application resource.
    /// </summary>
    /// <param name="path">Relative path to resource</param>
    /// <returns>A URI</returns>
    public static Uri BuildPackUri(string path)
    {
        string normalized = path[0] == '/' ? path[1..] : path;
        return new Uri($"pack://application:,,,/{normalized}");
    }
}
