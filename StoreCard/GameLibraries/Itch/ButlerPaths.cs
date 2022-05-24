// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreCard.GameLibraries.Itch;

internal static class ButlerPaths
{
    private static readonly string s_itchDataFolder =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "itch");

    public static readonly string? ButlerExecutable = Directory.EnumerateFiles(
        Path.Combine(s_itchDataFolder, @"broth\butler\versions"),
        "butler.exe",
        SearchOption.AllDirectories).ToList().FirstOrDefault();

    public static readonly string ButlerDatabase = Path.Combine(s_itchDataFolder, @"db\butler.db");

    public static readonly string ButlerPrereqsFolder = Path.Combine(s_itchDataFolder, @"prereqs");
}
