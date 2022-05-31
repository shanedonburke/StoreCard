#region

using System;

#endregion

namespace StoreCard.Utils;

public static class TimeUtils
{
    public static long UnixTimeMillis => DateTimeOffset.UtcNow.ToUnixTimeSeconds();
}
