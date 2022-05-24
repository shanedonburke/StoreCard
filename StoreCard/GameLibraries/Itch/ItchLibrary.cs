// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StoreCard.Models.Items.Installed;
using StoreCard.Utils;

namespace StoreCard.GameLibraries.Itch;

internal class ItchLibrary : GameLibrary
{
    private static readonly string s_dataFolder =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "itch");

    public override IEnumerable<InstalledGame> GetInstalledGames()
    {
        string butlerRoot = Path.Combine(s_dataFolder, @"broth\butler\versions");
        string? butlerExecPath = Directory.EnumerateFiles(
            butlerRoot,
            "butler.exe",
            SearchOption.AllDirectories).ToList().FirstOrDefault();
        if (butlerExecPath == null) yield break;

        string dbPath = Path.Combine(s_dataFolder, @"db\butler.db");
        Process butlerProc = new()
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = butlerExecPath,
                Arguments =
                    $"daemon --keep-alive --json --transport tcp --dbpath \"{dbPath}\" --destiny-pid {Environment.ProcessId}",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            }
        };

        butlerProc.Start();

        ButlerClient client = new(butlerExecPath, dbPath, butlerProc);
        client.Start();

        if (!client.Authenticate())
        {
            Debug.WriteLine("Failed to authenticate with Butler client.");
            yield break;
        }

        if (client.FetchCaves() is not { } caves)
        {
            Debug.WriteLine("Failed to fetch caves.");
            yield break;
        }

        foreach (var cave in caves)
        {
            Debug.WriteLine(cave.Id);
        }
    }
}
