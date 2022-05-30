namespace StoreCard.GameLibraries.Itch;

/// <summary>
/// Represents the JSON object printed to stdout when the Butler daemon is started.
/// </summary>
public sealed class ButlerListenNotification
{
    /// <summary>
    /// The secret value that must be passed in the authentication request.
    /// </summary>
    public readonly string Secret;

    public readonly TcpInfo Tcp;

    public readonly long Time;

    public readonly string Type;

    public ButlerListenNotification(string secret, TcpInfo tcp, long time, string type)
    {
        Secret = secret;
        Tcp = tcp;
        Time = time;
        Type = type;
    }

    /// <summary>
    /// Connection info used to connect to the daemon via socket.
    /// </summary>
    public sealed class TcpInfo
    {
        /// <summary>
        /// Address in the form 'host:post'.
        /// </summary>
        public readonly string Address;

        public TcpInfo(string address) => Address = address;
    }
}
