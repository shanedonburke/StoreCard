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
/// We interact with the daemon through its JSON-RPC API, which we connect to with a TCP socket.
/// See http://docs.itch.ovh/butlerd/master/#/ for documentation.
/// </summary>
public sealed class ButlerClient
{
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
    private Process? _butlerProc;

    public void Start()
    {
        _butlerProc = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = _execPath,
                Arguments =
                    // destiny-pid makes the daemon exit when StoreCard does
                    $"daemon --json --transport tcp --dbpath \"{_dbPath}\" --destiny-pid {Environment.ProcessId}",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            }
        };

        _butlerProc.Start();

        // The notification printed by the daemon when it's ready
        ButlerListenNotification? listenNotif = null;

        while (listenNotif == null)
        {
            // Each line of stdout is a JSON object
            string? jsonLine = _butlerProc.StandardOutput.ReadLine();

            if (jsonLine == null)
            {
                continue;
            }

            try
            {
                listenNotif = JsonConvert.DeserializeObject<ButlerListenNotification>(
                    jsonLine,
                    // If missing members are ignored, an object of a different type may "pass" as the
                    // listen notifcation because it has some of the same members
                    new JsonSerializerSettings {MissingMemberHandling = MissingMemberHandling.Error});
            }
            catch (JsonSerializationException)
            {
                // This line of stdout wasn't the notification, so try again
            }
        }

        _secret = listenNotif.Secret;

        // Address in the form host:port
        string[] split = listenNotif.Tcp.Address.Split(":");
        string host = split[0];
        int port = int.Parse(split[1]);

        _socket.Connect(host, port);
    }

    /// <summary>
    /// Send the authentication request. This must be called before any other request methods
    /// (e.g., getting caves or launching games).
    /// </summary>
    /// <returns>True if the authentication succeeded, false if it did not</returns>
    public bool Authenticate()
    {
        // We get the secret when we start the daemon
        if (_secret == null)
        {
            Start();
        }

        Dictionary<string, object> parameters = new() {{"secret", _secret!}};
        return SendRequest<AuthenticateResult>(Methods.MetaAuthenticate, parameters) != null;
    }

    /// <summary>
    /// Gets the list of caves, i.e., info about installed games.
    /// </summary>
    /// <returns></returns>
    public List<ButlerCave>? FetchCaves() =>
        SendRequest<FetchCavesResult>(
            Methods.FetchCaves,
            new Dictionary<string, object>())?.Items;

    /// <summary>
    /// Launches a game.
    /// </summary>
    /// <param name="caveId">ID of the cave representing the game</param>
    public void Launch(string caveId)
    {
        Dictionary<string, object> parameters = new()
        {
            {"caveId", caveId}, {"prereqsDir", ButlerPaths.ButlerPrereqsFolder}
        };
        // We don't care about the response
        SendRequest<object>(Methods.Launch, parameters);
    }

    /// <summary>
    /// Kills the Butler daemon process.
    /// </summary>
    public void KillDaemon()
    {
        _butlerProc?.Kill();
    }

    /// <summary>
    /// Sends a JSON-RPC request to the Butler daemon.
    /// </summary>
    /// <typeparam name="TResult">Type of the request-specific result</typeparam>
    /// <param name="method">Name of the API method to invoke</param>
    /// <param name="parameters">Parameters to be serialized into the request</param>
    /// <returns></returns>
    private TResult? SendRequest<TResult>(string method, Dictionary<string, object> parameters)
    {
        // Necessary parameters plus request-specific ones.
        Dictionary<string, object> req = new()
        {
            // The ID doesn't matter, but it must be present
            {"jsonrpc", "2.0"}, {"method", method}, {"id", new Random().Next()}, {"params", parameters}
        };

        string json = JsonConvert.SerializeObject(req) + "\n";

        // Strings are UTF-8 by default, but the daemon requires ASCII
        _socket.Send(json.ToAsciiBytes());

        string response;

        do
        {
            // Check for response every 100 ms.
            Thread.Sleep(100);
            byte[] buffer = new byte[65536];
            _socket.Receive(buffer);
            response = Encoding.ASCII.GetString(buffer);
        } while (response.Trim() == string.Empty);

        // The buffer may contain multiple lines of JSON objects (e.g., logs + response).
        // The first one will be the response.
        string resJson = response.Split('\n')[0];

        try
        {
            // Return the result part of the response if we can deserialize it correctly.
            // We are only sending one request at a time, so we don't need to worry about the wrong
            // response being deserialized.
            return JsonConvert.DeserializeObject<ButlerResponse<TResult>>(
                    resJson,
                    // The response classes we've created may not be complete, but that's okay
                    new JsonSerializerSettings {MissingMemberHandling = MissingMemberHandling.Ignore})
                is { } res
                ? res.Result
                : default;
        }
        catch (JsonSerializationException e)
        {
            Logger.LogExceptionMessage("Failed to deserialize Butler response", e);
            return default;
        }
    }

    /// <summary>
    /// Names of the Butler daemon's API methods. The method name is specified in JSON-RPC requests
    /// sent to the daemon.
    /// </summary>
    private static class Methods
    {
        public const string MetaAuthenticate = "Meta.Authenticate";

        public const string FetchCaves = "Fetch.Caves";

        public const string Launch = "Launch";
    }

    /// <summary>
    /// Result of the authentication request.
    /// </summary>
    private sealed class AuthenticateResult
    {
        [JsonProperty("ok")] public readonly bool Ok;

        public AuthenticateResult(bool ok) => Ok = ok;
    }

    /// <summary>
    /// Result of a request to fetch caves.
    /// </summary>
    private sealed class FetchCavesResult
    {
        [JsonProperty("items")] public List<ButlerCave> Items;

        public FetchCavesResult(List<ButlerCave> items) => Items = items;
    }
}
