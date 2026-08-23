using Hoeyer.Common.Extensions.LoggingExtensions;
using Hoeyer.OpcUa.Core.Extensions.Logging;
using Hoeyer.OpcUa.Core.Services.OpcUaServices;
using Hoeyer.OpcUa.Server.Abstractions;
using Hoeyer.OpcUa.Server.Abstractions.NodeManagement;
using Hoeyer.OpcUa.Server.Application;
using Hoeyer.OpcUa.Server.Extensions;
using Microsoft.Extensions.Logging;
using Opc.Ua;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server;

internal sealed class OpcEntityServer(
    IOpcUaTargetServerSetup applicationProductDetails,
    IEnumerable<IEntityNodeManagerFactory> entityManagerFactories,
    ILogger<OpcEntityServer> logger)
    : StandardServer
{
    private static readonly DateTime BuildDate = DateTime.UtcNow;
    public readonly IOpcUaTargetServerSetup ServerInfo = applicationProductDetails;

    private bool _disposed;

    public DomainMasterNodeManager? DomainManager { get; private set; }


    public override async Task<CallResponse> CallAsync(SecureChannelContext secureChannelContext,
        RequestHeader requestHeader,
        CallMethodRequestCollection methodsToCall, CancellationToken ct)
    {
        return await logger
            .LogCaughtExceptionAs(LogLevel.Error)
            .WithErrorMessage("An error occured while calling methods")
            .WithScope(requestHeader.ToLoggingObject().ToString())
            .WhenExecutingAsync(() => base.CallAsync(secureChannelContext, requestHeader, methodsToCall, ct));
    }

    protected override MasterNodeManager CreateMasterNodeManager(IServerInternal server,
        ApplicationConfiguration configuration)
    {
        return logger.Try(() =>
        {
            Task<IEntityNodeManager>[] managerCreationTasks = entityManagerFactories
                .Select(async factory => await factory.CreateEntityManager(server))
                .ToArray();

            Task.WhenAll(managerCreationTasks).Wait();
            var exceptions =
                managerCreationTasks.Select(e => e.Exception).Where(e => e != null).ToList();

            if (exceptions.Any())
            {
                throw new AggregateException(exceptions);
            }

            DomainManager = new DomainMasterNodeManager(server, configuration,
                managerCreationTasks.Select(e => e.Result).ToArray());
            return DomainManager;
        })!;
    }


    public override async Task<CreateSessionResponse> CreateSessionAsync(
        SecureChannelContext secureChannelContext,
        RequestHeader requestHeader,
        ApplicationDescription clientDescription,
        string serverUri,
        string endpointUrl,
        string sessionName,
        byte[] clientNonce,
        byte[] clientCertificate,
        double requestedSessionTimeout,
        uint maxResponseMessageSize,
        CancellationToken ct)
    {
        logger.LogInformation(
            "CreateSessionAsync: ClientName={ClientName}, ApplicationUri={ApplicationUri}, " +
            "ServerUri={ServerUri}, EndpointUrl={EndpointUrl}, SessionName={SessionName}, " +
            "RequestedTimeout={RequestedTimeout}, MaxResponseMessageSize={MaxResponseMessageSize}",
            clientDescription?.ApplicationName?.Text,
            clientDescription?.ApplicationUri,
            serverUri,
            endpointUrl,
            sessionName,
            requestedSessionTimeout,
            maxResponseMessageSize);

        try
        {
            var response = await base.CreateSessionAsync(
                secureChannelContext,
                requestHeader,
                clientDescription,
                serverUri,
                endpointUrl,
                sessionName,
                clientNonce,
                clientCertificate,
                requestedSessionTimeout,
                maxResponseMessageSize,
                ct);

            logger.LogInformation(
                "CreateSessionAsync completed: Status={Status}",
                response?.ResponseHeader?.ServiceResult);

            return response;
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "CreateSessionAsync FAILED: ClientName={ClientName}, ApplicationUri={ApplicationUri}, " +
                "ServerUri={ServerUri}, EndpointUrl={EndpointUrl}, SessionName={SessionName}",
                clientDescription?.ApplicationName?.Text,
                clientDescription?.ApplicationUri,
                serverUri,
                endpointUrl,
                sessionName);

            throw;
        }
    }

    public override async Task<ActivateSessionResponse> ActivateSessionAsync(SecureChannelContext secureChannelContext,
        RequestHeader requestHeader,
        SignatureData clientSignature, SignedSoftwareCertificateCollection clientSoftwareCertificates,
        StringCollection localeIds, ExtensionObject userIdentityToken, SignatureData userTokenSignature,
        CancellationToken ct)
    {
        return await logger
            .LogCaughtExceptionAs(LogLevel.Error)
            .WithErrorMessage("An error occured while calling methods")
            .WithScope("Activating session for {@SessionDetails}", new
            {
                ClientSignature = clientSignature.Signature,
                UserTokenSignature = userTokenSignature.Signature,
                UserIdentityToken = userIdentityToken.Body,
                RequestHeader = requestHeader.ToLoggingObject()
            })
            .WhenExecutingAsync(async () => await base.ActivateSessionAsync(secureChannelContext, requestHeader,
                clientSignature,
                clientSoftwareCertificates, localeIds, userIdentityToken, userTokenSignature, ct));
    }

    public override async Task<CloseSessionResponse> CloseSessionAsync(SecureChannelContext secureChannelContext,
        RequestHeader requestHeader, bool deleteSubscriptions,
        CancellationToken ct)
    {
        return await logger
            .LogCaughtExceptionAs(LogLevel.Error)
            .WithErrorMessage("An error occured while calling methods")
            .WithScope(requestHeader.ToLoggingObject().ToString())
            .WhenExecutingAsync(() =>
                base.CloseSessionAsync(secureChannelContext, requestHeader, deleteSubscriptions, ct));
    }

    protected override async ValueTask StartApplicationAsync(ApplicationConfiguration configuration,
        CancellationToken cancellationToken = new())
    {
        try
        {
            logger.LogInformation("Starting application with configuration {@Configuration}",
                configuration.ToLoggingObject());
            await base.StartApplicationAsync(configuration, cancellationToken);
        }
        catch (Exception e)
        {
            logger.LogCritical(e, " A critical error occured when trying to start the OpcEntityServer.");
            throw new OpcUaEntityServiceConfigurationException([e]);
        }
    }

    protected override ServerProperties LoadServerProperties()
    {
        var properties = base.LoadServerProperties();
        properties.ProductName = ServerInfo.ApplicationName;
        properties.ProductUri = ServerInfo.ApplicationNamespace.ToString();
        properties.SoftwareVersion = "0.0.01";
        properties.BuildDate = BuildDate;
        return properties;
    }


    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (!disposing)
        {
            return;
        }

        logger.LogInformation("Closing server...");
        DomainManager?.Dispose();
        base.Dispose(disposing);
        _disposed = true;
    }
}