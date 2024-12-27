namespace Hoeyer.OpcUa.Proxy;

public record ConnectionState
{
    public readonly string Description;
    public static readonly ConnectionState Reconnecting = new ("Reconnecting");
    public static readonly ConnectionState Connecting = new("Connecting");
    public static readonly ConnectionState Connected = new("Connected, stable");
    public static readonly ConnectionState Initializing = new("Initializing");
    public static readonly ConnectionState Disconnecting = new("Disconnecting");
    public static readonly ConnectionState Disconnected = new("Disconnected");
    public static readonly ConnectionState FailedConnect = new("Connection failed");
    public static readonly ConnectionState FailedDisconnect = new("Disconnect failed");
    public static readonly ConnectionState FailedInitializing = new("Initialization failed");
    public static readonly ConnectionState Unknown = new("An unknown error occurred");
    public static readonly ConnectionState Running = new("Running but not connected");
    public static readonly ConnectionState PreInitialized = new("No initialization attempted");

    private ConnectionState(string description)
    {
        Description = description;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return Description;
    }
}