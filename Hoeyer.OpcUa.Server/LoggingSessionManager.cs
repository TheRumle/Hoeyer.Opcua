using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Logging;
using Opc.Ua;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server;

internal sealed class LoggingSessionManager : SessionManager
{
    private readonly ILogger _logger;
    private readonly IDisposable? _scope;
    private readonly Guid _instanceId = Guid.NewGuid();

    public LoggingSessionManager(
        ILogger logger,
        IServerInternal server,
        ApplicationConfiguration configuration)
        : base(server, configuration)
    {
        _logger = logger;
        var keys = new Dictionary<string, object>
        {
            ["SessionManagerId"] = _instanceId,
            ["ApplicationUri"] = configuration.ApplicationUri
        };
        _scope = _logger.BeginScope(keys);
    }

    public override async ValueTask<CreateSessionResult> CreateSessionAsync(
        OperationContext context,
        X509Certificate2 serverCertificate,
        string sessionName,
        byte[] clientNonce,
        ApplicationDescription clientDescription,
        string endpointUrl,
        X509Certificate2 clientCertificate,
        X509Certificate2Collection clientCertificateChain,
        double requestedSessionTimeout,
        uint maxResponseMessageSize,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "CreateSessionAsync. SessionName={SessionName}, ClientApplicationUri={ApplicationUri}, ClientApplicationName={ApplicationName}, Endpoint={Endpoint}, RequestedTimeout={RequestedTimeout}, MaxResponseMessageSize={MaxResponseMessageSize}",
            sessionName,
            clientDescription?.ApplicationUri,
            clientDescription?.ApplicationName?.Text,
            endpointUrl,
            requestedSessionTimeout,
            maxResponseMessageSize);

        var result = await base.CreateSessionAsync(
            context,
            serverCertificate,
            sessionName,
            clientNonce,
            clientDescription,
            endpointUrl,
            clientCertificate,
            clientCertificateChain,
            requestedSessionTimeout,
            maxResponseMessageSize,
            cancellationToken);

        LogSession(result.Session, "Created session");
        return result;
    }

    public override async ValueTask<(bool IdentityContextChanged, byte[] ServerNonce)> ActivateSessionAsync(
        OperationContext context,
        NodeId authenticationToken,
        SignatureData clientSignature,
        ExtensionObject userIdentityToken,
        SignatureData userTokenSignature,
        StringCollection localeIds,
        CancellationToken cancellationToken = default)
    {
        var result = await base.ActivateSessionAsync(
            context,
            authenticationToken,
            clientSignature,
            userIdentityToken,
            userTokenSignature,
            localeIds,
            cancellationToken);

        LogSession(context.Session, "Activated session");
        return result;
    }

    public override void CloseSession(NodeId sessionId)
    {
        _logger.LogInformation(
            "CloseSession. SessionId={SessionId}",
            sessionId);
        base.CloseSession(sessionId);
    }


    protected override ISession CreateSession(
        OperationContext context,
        IServerInternal server,
        X509Certificate2 serverCertificate,
        NodeId sessionCookie,
        byte[] clientNonce,
        Nonce serverNonce,
        string sessionName,
        ApplicationDescription clientDescription,
        string endpointUrl,
        X509Certificate2 clientCertificate,
        X509Certificate2Collection clientCertificateChain,
        double sessionTimeout,
        uint maxResponseMessageSize,
        int maxRequestAge,
        int maxContinuationPoints)
    {
        _logger.LogDebug(
            "Attempting to create session: SessionName={SessionName}, SessionCookie={SessionCookie}, Timeout={Timeout}",
            sessionName,
            sessionCookie,
            sessionTimeout);

        var session = base.CreateSession(
            context,
            server,
            serverCertificate,
            sessionCookie,
            clientNonce,
            serverNonce,
            sessionName,
            clientDescription,
            endpointUrl,
            clientCertificate,
            clientCertificateChain,
            sessionTimeout,
            maxResponseMessageSize,
            maxRequestAge,
            maxContinuationPoints);

        LogSession(session, "Session created");
        return session;
    }

    protected override void Dispose(bool disposing)
    {
        _logger.LogInformation(
            "Disposing session manager. Disposing={Disposing}",
            disposing);

        try
        {
            base.Dispose(disposing);
        }
        finally
        {
            _scope?.Dispose();
        }
    }

    private void LogSession(ISession session, string preamble = "")
    {
        _logger.LogInformation(
            preamble + ": SessionId={SessionId}, User={DisplayName}",
            session?.Id,
            session?.Identity?.DisplayName);
    }
}