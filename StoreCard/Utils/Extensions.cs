// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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
}
