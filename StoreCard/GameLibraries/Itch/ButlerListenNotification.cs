// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace StoreCard.GameLibraries.Itch;

internal class ButlerListenNotification
{
    public class TcpInfo
    {
        public readonly string Address;

        public TcpInfo(string address) => Address = address;
    }

    public readonly string Secret;
    public readonly TcpInfo Tcp;
    public readonly long Time;
    public readonly string Type;

    public ButlerListenNotification(string secret, TcpInfo tcp, long time, string type)
    {
        Secret = secret;
        Tcp = tcp;
        Time = time;
        Type = type;
    }
}
