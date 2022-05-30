#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using StoreCard.Utils;

#endregion

namespace StoreCard.GameLibraries.Itch;

/// <summary>
/// An interface for interacting with the Butler daemon, which is used by the itch launcher
/// for various purposes. We use it to get info about and launch installed games.
/// See http://docs.itch.ovh/butlerd/master/#/ for documentation.
/// </summary>
public sealed class ButlerClient
{
    /// <summary>
    /// The classes we've created for responses aren't complete, but that's okay
    /// </summary>
    private static readonly JsonSerializerSettings s_deserializeResponseSettings =
        new() {MissingMemberHandling = MissingMemberHandling.Ignore};

    /// <summary>
    /// Path to the DB created by Butler.
    /// </summary>
    private readonly string _dbPath = ButlerPaths.ButlerDatabase;

    /// <summary>
    /// Path to the Butler executable.
    /// </summary>
    private readonly string _execPath = ButlerPaths.ButlerExecutable!;

    /// <summary>
    /// Socket used to connect to the daemon.
    /// </summary>
    private readonly Socket _socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

    /// <summary>
    /// A secret value is printed when the daemon starts. We use it to authenticate in our first request.
    /// </summary>
    private string? _secret;

    /// <summary>
    /// The Butler system process.
    /// </summary>
    private Process _butlerProc;

    public void Start()
    {
        _butlerProc = new Process
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
        _butlerProc.Start();

        ButlerListenNotification? listenNotif = null;

        while (listenNotif == null)
        {
            string? jsonLine = _butlerProc.StandardOutput.ReadLine();

            if (jsonLine == null)
            {
                continue;
            }

            try
            {
                listenNotif = JsonConvert.DeserializeObject<ButlerListenNotification>(
                    jsonLine,
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
        return SendRequest<AuthenticateResult>(
            Methods.MetaAuthenticate, parameters) != null;
    }

    public List<ButlerCave>? FetchCaves() =>
        SendRequest<FetchCavesResult>(
            Methods.FetchCaves,
            new Dictionary<string, object>())?.Items;

    public void Launch(string caveId)
    {
        Dictionary<string, object> parameters = new()
        {
            {"caveId", caveId}, {"prereqsDir", ButlerPaths.ButlerPrereqsFolder}
        };
        SendRequest<object>(Methods.Launch, parameters);
    }

    public void KillDaemon()
    {
        _butlerProc.Kill();
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
        string resJson = response.Split('\n')[0];

        try
        {
            return JsonConvert.DeserializeObject<ButlerResponse<TResult>>(
                resJson, s_deserializeResponseSettings) is { } res
                ? res.Result
                : default;
        }
        catch (JsonSerializationException e)
        {
            Logger.LogExceptionMessage("Failed to deserialize Butler response", e);
            return default;
        }
    }

    private static class Methods
    {
        public const string MetaAuthenticate = "Meta.Authenticate";

        public const string FetchCaves = "Fetch.Caves";

        public const string Launch = "Launch";
    }

    private sealed class AuthenticateResult
    {
        [JsonProperty("ok")] public readonly bool Ok;

        public AuthenticateResult(bool ok) => Ok = ok;
    }

    private sealed class FetchCavesResult
    {
        [JsonProperty("items")] public List<ButlerCave> Items;

        public FetchCavesResult(List<ButlerCave> items) => Items = items;
    }
}
