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

        ListenNotification? listenNotif = null;

        while (listenNotif == null)
        {
            string? jsonLine = butlerProc.StandardOutput.ReadLine();
            if (jsonLine == null) continue;
            try
            {
                listenNotif = JsonConvert.DeserializeObject<ListenNotification>(jsonLine,
                    new JsonSerializerSettings {MissingMemberHandling = MissingMemberHandling.Error});
            }
            catch (JsonSerializationException)
            {
            }
        }

        (string? server, string? port, IEnumerable<string> rest) = listenNotif.Tcp.Address.Split(":");

        if (server == null || port == null)
        {
            butlerProc.Kill();
            yield break;
        }

        Dictionary<string, object> authReq = new()
        {
            {"jsonrpc", "2.0"},
            {"method", "Meta.Authenticate"},
            {"id", 0},
            {"params", new Dictionary<string, string> {{"secret", listenNotif.Secret}}}
        };
        string authReqJson = JsonConvert.SerializeObject(authReq);
        byte[] utf8Bytes = Encoding.UTF8.GetBytes(authReqJson + "\n");
        string ascii = Encoding.ASCII.GetString(utf8Bytes);

        Socket socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socket.Connect(server, int.Parse(port));
        socket.Send(Encoding.ASCII.GetBytes(ascii));
        // tcpClient.GetStream().Write(authReqBytes, 0, authReqBytes.Length);

        Dictionary<string, object> cavesReq = new()
        {
            {"jsonrpc", "2.0"}, {"method", "Fetch.Caves"}, {"id", 1}, {"params", new Dictionary<string, string>()}
        };
        string reqJson = JsonConvert.SerializeObject(cavesReq);
        byte[] cutf8Bytes = Encoding.UTF8.GetBytes(reqJson + "\n");
        string cascii = Encoding.ASCII.GetString(cutf8Bytes);
        socket.Send(Encoding.ASCII.GetBytes(cascii));

        // using StreamReader reader = new(tcpClient.GetStream(), Encoding.ASCII);
        byte[] buffer = new byte[1024];

        // using StreamReader reader = new StreamReader(stream);

        while (true)
        {
            if (socket.Receive(buffer) > 0)
            {
                Debug.WriteLine(Encoding.ASCII.GetString(buffer));
            }
        }
        Debug.WriteLine("Done");
    }
}
