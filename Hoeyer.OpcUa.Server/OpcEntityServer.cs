using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hoeyer.Common.Extensions.LoggingExtensions;
using Hoeyer.OpcUa.Core.Configuration;
using Hoeyer.OpcUa.Core.Extensions.Logging;
using Hoeyer.OpcUa.Core.Services.OpcUaServices;
using Hoeyer.OpcUa.Server.Api.NodeManagement;
using Hoeyer.OpcUa.Server.Application;
using Hoeyer.OpcUa.Server.Extensions;
using Microsoft.Extensions.Logging;
using Opc.Ua;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server;

internal sealed class OpcEntityServer(
    IOpcUaEntityServerInfo applicationProductDetails,
    IEnumerable<IEntityNodeManagerFactory> entityManagerFactories,
    ILogger<OpcEntityServer> logger)
    : StandardServer
{
    private static readonly DateTime buildDate = DateTime.UtcNow;
    public readonly IEnumerable<Uri> EndPoints = [..applicationProductDetails.Endpoints];
    public readonly IOpcUaEntityServerInfo ServerInfo = applicationProductDetails;

    private bool _disposed;

    public int a;
    public DomainMasterNodeManager DomainManager { get; } = null!;

    /// <inheritdoc />
    protected override void OnServerStarted(IServerInternal server)
    {
        base.OnServerStarted(server);
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
            List<AggregateException> exceptions =
                managerCreationTasks.Select(e => e.Exception).Where(e => e != null).ToList();

            if (exceptions.Any()) throw new AggregateException(exceptions);

            return new DomainMasterNodeManager(server, configuration,
                managerCreationTasks.Select(e => e.Result).ToArray());
        })!;
    }


    /// <inheritdoc />
    public override ResponseHeader ActivateSession(RequestHeader requestHeader, SignatureData clientSignature,
        SignedSoftwareCertificateCollection clientSoftwareCertificates, StringCollection localeIds,
        ExtensionObject userIdentityToken, SignatureData userTokenSignature, out byte[] serverNonce,
        out StatusCodeCollection results, out DiagnosticInfoCollection diagnosticInfos)
    {
        try
        {
            using (logger.BeginScope("Activating session for {@SessionDetails}", new
                   {
                       ClientSignature = clientSignature.Signature,
                       UserTokenSignature = userTokenSignature.Signature,
                       UserIdentityToken = userIdentityToken.Body,
                       RequestHeader = requestHeader.ToLoggingObject(),
                   }))
            {
                var header = base.ActivateSession(requestHeader, clientSignature, clientSoftwareCertificates, localeIds,
                    userIdentityToken, userTokenSignature, out serverNonce, out results, out diagnosticInfos);
                return LogResponseHeader(header, diagnosticInfos)!;
            }
        }
        catch (Exception e)
        {
            logger.LogCritical(e, "An exception occurred when trying to activate session with RequestHeader {@Header}",
                requestHeader.ToLoggingObject());
            diagnosticInfos = new DiagnosticInfoCollection();
            results = new StatusCodeCollection();
            serverNonce = null!;
            return new ResponseHeader
            {
                Timestamp = DateTime.UtcNow,
                ServiceResult = StatusCodes.BadConnectionRejected
            };
        }
    }

    /// <inheritdoc />
    public override ResponseHeader CloseSession(RequestHeader requestHeader, bool deleteSubscriptions)
    {
        try
        {
            using (logger.BeginScope("Closing session with handle {@Handle}", requestHeader.ToLoggingObject()))
            {
                var header = base.CloseSession(requestHeader, deleteSubscriptions);
                return LogResponseHeader(header)!;
            }
        }
        catch (Exception e)
        {
            logger.LogCritical(e, "An exception occurred when trying to close session with RequestHeader {@Header}",
                requestHeader.ToLoggingObject());
            return new ResponseHeader
            {
                Timestamp = DateTime.UtcNow,
                ServiceResult = StatusCodes.BadInternalError
            };
        }
    }

    /// <inheritdoc />
    protected override void StartApplication(ApplicationConfiguration configuration)
    {
        try
        {
            logger.LogInformation("Starting application with configuration {@Configuration}",
                configuration.ToLoggingObject());
            base.StartApplication(configuration);
        }
        catch (Exception e)
        {
            logger.LogCritical(e, " A critical error occured when trying to start the OpcEntityServer.");
            throw new OpcUaEntityServiceConfigurationException([e]);
        }
    }

    /// <inheritdoc />
    protected override ServerProperties LoadServerProperties()
    {
        var properties = base.LoadServerProperties();
        properties.ProductName = ServerInfo.ApplicationName;
        properties.ProductUri = ServerInfo.ApplicationNamespace.ToString();
        properties.SoftwareVersion = "0.0.01";
        properties.BuildDate = buildDate;
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

        DomainManager.Dispose();
        base.Dispose(disposing);
        _disposed = true;
    }

    private ResponseHeader? LogResponseHeader(ResponseHeader? responseHeader,
        DiagnosticInfoCollection? diagnosticInfos = null)
    {
        if (diagnosticInfos is { Count: > 0 })
        {
            logger.LogError("Diagnostics: {@Diagnostics}", diagnosticInfos.Select(e => new
            {
                e.AdditionalInfo,
                e.InnerStatusCode,
                Message = e.ToString()
            }).ToArray());
        }

        if (responseHeader == null)
        {
            logger.LogError("Response header is null!");
        }
        else if (StatusCode.IsBad(responseHeader.ServiceResult))
        {
            logger.LogError("Header status code is bad: {Code}\n" +
                            "{@Header}", StatusCodes.GetBrowseName(responseHeader.ServiceResult.Code),
                responseHeader.ToLoggingObject());
        }

        return responseHeader;
    }
}