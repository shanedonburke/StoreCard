// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using StoreCard.Models.Items.Installed;
using StoreCard.Utils;

namespace StoreCard.GameLibraries.Itch;

internal class ItchLibrary : GameLibrary
{
    private static readonly string s_dataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "itch");

    public override IEnumerable<InstalledGame> GetInstalledGames()
    {
        string butlerPath = Path.Combine(s_dataFolder, @"broth\butler\versions");
        var find = Directory.EnumerateFiles(
            butlerPath,
            "*.exe",
            SearchOption.AllDirectories).ToList();
        string? butlerExecPath = new SafeFileEnumerator(
            butlerPath,
            "*.exe",
            SearchOption.AllDirectories).FirstOrDefault()?.FullName;

        if (butlerExecPath == null) yield break;

        Process butlerProc = new()
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = butlerExecPath,
                Arguments = $"--json --dbpath {Path.Combine(s_dataFolder, @"db\butler.db")}",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            }
        };

        butlerProc.Start();

        ListenNotification? listenNotif = null;

        while (listenNotif == null)
        {
            string? jsonLine = butlerProc.StandardOutput.ReadLine();

            if (jsonLine == null) continue;

            listenNotif = JsonConvert.DeserializeObject<ListenNotification>(jsonLine);
        }
        Debug.WriteLine("here");
        yield break;
    }
}
