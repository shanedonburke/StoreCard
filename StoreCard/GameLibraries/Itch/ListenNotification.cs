﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreCard.GameLibraries.Itch;

internal class ListenNotification
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

    public ListenNotification(string secret, TcpInfo tcp, long time, string type)
    {
        Secret = secret;
        Tcp = tcp;
        Time = time;
        Type = type;
    }
}
