#region

using System;

#endregion

namespace StoreCard.Utils;

/// <summary>
/// Time utilities.
/// </summary>
public static class TimeUtils
{
    /// <summary>
    /// The current Unix epoch time in milliseconds.
    /// </summary>
    public static long UnixTimeMillis => DateTimeOffset.UtcNow.ToUnixTimeSeconds();
}
