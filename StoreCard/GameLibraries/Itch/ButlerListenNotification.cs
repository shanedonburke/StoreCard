namespace StoreCard.GameLibraries.Itch;

public sealed class ButlerListenNotification
{
    public sealed class TcpInfo
    {
        public readonly string Address;

        public TcpInfo(string address) => Address = address;
    }

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
}
