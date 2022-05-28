using System;

namespace StoreCard.Utils;

public class TimeUtils
{
    public static long UnixTimeMillis => DateTimeOffset.UtcNow.ToUnixTimeSeconds();
}
