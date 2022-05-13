using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreCard.Utils;

internal class Time
{
    public static long UnixTimeMillis => DateTimeOffset.UtcNow.ToUnixTimeSeconds();
}