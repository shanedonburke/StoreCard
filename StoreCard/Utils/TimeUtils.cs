using System;

namespace StoreCard.Utils;

internal class TimeUtils
{
    public static long UnixTimeMillis => DateTimeOffset.UtcNow.ToUnixTimeSeconds();
}
