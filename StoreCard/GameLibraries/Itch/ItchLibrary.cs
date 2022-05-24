// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;
using StoreCard.Models.Items.Installed;
using StoreCard.Utils;
using SystemIcons = System.Drawing.SystemIcons;

namespace StoreCard.GameLibraries.Itch;

internal class ItchLibrary : GameLibrary
{
    private static readonly string s_dataFolder =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "itch");

    public static readonly string? ButlerExecPath = Directory.EnumerateFiles(
        Path.Combine(s_dataFolder, @"broth\butler\versions"),
        "butler.exe",
        SearchOption.AllDirectories).ToList().FirstOrDefault();

    public static readonly string ButlerDbPath = Path.Combine(s_dataFolder, @"db\butler.db");

    public override IEnumerable<InstalledGame> GetInstalledGames()
    {
        if (ButlerExecPath == null) yield break;

        Process butlerProc = new()
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = ButlerExecPath,
                Arguments =
                    $"daemon --keep-alive --json --transport tcp --dbpath \"{ButlerDbPath}\" --destiny-pid {Environment.ProcessId}",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            }
        };

        butlerProc.Start();

        ButlerClient client = new(ButlerExecPath, ButlerDbPath, butlerProc);
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

        foreach (ButlerCave cave in caves)
        {
            string installFolder = cave.InstallInfo.InstallFolder;

            BitmapSource? icon = null;

            foreach (string exePath in Directory.EnumerateFiles(installFolder, "*.exe", SearchOption.AllDirectories))
            {
                Icon? hIcon = Icon.ExtractAssociatedIcon(exePath);

                if (hIcon == SystemIcons.Application)
                {
                    continue;
                }

                icon = IconUtils.CreateBitmapSourceFromHIcon(hIcon ?? SystemIcons.Application);
            }

            yield return new InstalledItchGame(cave.Game.Title, icon, cave.Id);
        }
    }
}
