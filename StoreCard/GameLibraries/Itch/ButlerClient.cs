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
using StoreCard.Utils;

namespace StoreCard.GameLibraries.Itch;

internal class ButlerClient
{
    private static class Methods
    {
        public const string MetaAuthenticate = "Meta.Authenticate";

        public const string FetchCaves = "Fetch.Caves";

        public const string Launch = "Launch";
    }

    private class AuthenticateResult
    {
        [JsonProperty("ok")] public readonly bool Ok;

        public AuthenticateResult(bool ok)
        {
            Ok = ok;
        }
    }

    private class FetchCavesResult
    {
        [JsonProperty("items")] public List<ButlerCave> Items;

        public FetchCavesResult(List<ButlerCave> items)
        {
            Items = items;
        }
    }

    private readonly Socket _socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

    private readonly string _execPath = ButlerPaths.ButlerExecutable!;

    private readonly string _dbPath = ButlerPaths.ButlerDatabase;

    private string? _secret;

    public void Start()
    {
        Process butlerProc = new()
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = _execPath,
                Arguments =
                    $"daemon --json --transport tcp --dbpath \"{_dbPath}\" --destiny-pid {Environment.ProcessId}",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            }
        };
        butlerProc.Start();

        ButlerListenNotification? listenNotif = null;

        while (listenNotif == null)
        {
            string? jsonLine = butlerProc.StandardOutput.ReadLine();

            if (jsonLine == null) continue;

            try
            {
                listenNotif = JsonConvert.DeserializeObject<ButlerListenNotification>(jsonLine,
                    new JsonSerializerSettings {MissingMemberHandling = MissingMemberHandling.Error});
            }
            catch (JsonSerializationException)
            {
            }
        }

        _secret = listenNotif.Secret;

        string[] split = listenNotif.Tcp.Address.Split(":");
        string server = split[0];
        int port = int.Parse(split[1]);

        _socket.Connect(server, port);
    }

    public bool Authenticate()
    {
        if (_secret == null)
        {
            Start();
        }

        Dictionary<string, object> parameters = new() {{"secret", _secret!}};
        return SendRequest<AuthenticateResult>(Methods.MetaAuthenticate, parameters) != null;
    }

    public List<ButlerCave>? FetchCaves()
    {
        return SendRequest<FetchCavesResult>(Methods.FetchCaves, new Dictionary<string, object>())?.Items;
    }

    public void Launch(string caveId)
    {
        Dictionary<string, object> parameters = new()
        {
            {"caveId", caveId},
            {"prereqsDir", ButlerPaths.ButlerPrereqsFolder}
        };
        SendRequest<object>(Methods.Launch, parameters);
    }

    private TResult? SendRequest<TResult>(string method, Dictionary<string, object> parameters)
    {
        Dictionary<string, object> req = new()
        {
            {"jsonrpc", "2.0"}, {"method", method}, {"id", new Random().Next()}, {"params", parameters}
        };

        string json = JsonConvert.SerializeObject(req);
        byte[] utf8Bytes = Encoding.UTF8.GetBytes(json + "\n");
        string ascii = Encoding.ASCII.GetString(utf8Bytes);

        _socket.Send(Encoding.ASCII.GetBytes(ascii));

        string response;

        do
        {
            Thread.Sleep(100);
            byte[] buffer = new byte[65536];
            _socket.Receive(buffer);
            response = Encoding.ASCII.GetString(buffer);
        } while (response.Trim() == string.Empty);

        // The buffer may contain multiple lines of JSON objects (e.g., logs)
        string resJson = response.Split("\n")[0];

        try
        {
            return JsonConvert.DeserializeObject<ButlerResponse<TResult>>(resJson,
                new JsonSerializerSettings {MissingMemberHandling = MissingMemberHandling.Ignore}) is { } res
                ? res.Result
                : default;
        }
        catch (JsonSerializationException)
        {
            return default;
        }
    }
}
