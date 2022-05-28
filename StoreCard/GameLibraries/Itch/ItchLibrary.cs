// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;
using StoreCard.Models.Items.Installed.Games;
using StoreCard.Utils;
using SystemIcons = System.Drawing.SystemIcons;

namespace StoreCard.GameLibraries.Itch;

internal class ItchLibrary : GameLibrary
{
    public bool IsInstalled => ButlerPaths.ButlerExecutable != null && File.Exists(ButlerPaths.ButlerDatabase);

    public override IEnumerable<InstalledGame> GetInstalledGames()
    {
        if (!IsInstalled) yield break;

        Process butlerProc = new()
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = ButlerPaths.ButlerExecutable,
                Arguments =
                    $"daemon --keep-alive --json --transport tcp --dbpath \"{ButlerPaths.ButlerDatabase}\" --destiny-pid {Environment.ProcessId}",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            }
        };

        butlerProc.Start();

        ButlerClient client = new();
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
