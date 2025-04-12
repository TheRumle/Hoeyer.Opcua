﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Hoeyer.OpcUa.Core.Configuration;
using Hoeyer.OpcUa.Server.Entity.Management;
using Hoeyer.OpcUa.Server.Extensions;
using Microsoft.Extensions.Logging;
using Opc.Ua;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Core;

public sealed class OpcEntityServer(
    EntityServerStartedMarker marker,
    IOpcUaEntityServerInfo applicationProductDetails,
    IDomainMasterManagerFactory managerFactory,
    ILogger<OpcEntityServer> logger)
    : StandardServer //ServerBase? instead? 
{
    public readonly IEnumerable<Uri> EndPoints = [..applicationProductDetails.Endpoints];
    public readonly IOpcUaEntityServerInfo ServerInfo = applicationProductDetails;

    private bool _disposed;
    public DomainMasterNodeManager DomainManager { get; } = null!;
    
    /// <inheritdoc />
    protected override void OnServerStarted(IServerInternal server)
    {
        base.OnServerStarted(server);
        marker.MarkCompleted();
    }


    protected override MasterNodeManager CreateMasterNodeManager(IServerInternal server,
        ApplicationConfiguration configuration)
    {
        logger.BeginScope("Creating master manager");
        return managerFactory.ConstructMasterManager(server, configuration);
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
                var header =  base.ActivateSession(requestHeader, clientSignature, clientSoftwareCertificates, localeIds,
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
        logger.LogInformation("Starting application at {@ApplicationUri} with configuration {@Configuration}", configuration.ApplicationUri,   JsonSerializer.Serialize(new
        {
           ProductUri = configuration.ProductUri,
           Properties = configuration.Properties,
           Extensions = configuration.ExtensionObjects,
           Other = new
           {
               DomainNames = configuration.GetServerDomainNames()
           }
        }, new JsonSerializerOptions { WriteIndented = true,  }));
        base.StartApplication(configuration);
    }

    /// <inheritdoc />
    protected override ServerProperties LoadServerProperties()
    {
        logger.BeginScope("Loading server properties...");
        var properties = base.LoadServerProperties();
        properties.BuildDate = DateTime.UtcNow;
        properties.ProductName = ServerInfo.ApplicationName;
        properties.ProductUri = ServerInfo.ApplicationNamespace.ToString();
        properties.SoftwareVersion = "1.0";
        return properties;
    }


    /// <inheritdoc />
    public override ResponseHeader Browse(RequestHeader requestHeader, ViewDescription view,
        uint requestedMaxReferencesPerNode,
        BrowseDescriptionCollection nodesToBrowse, out BrowseResultCollection results,
        out DiagnosticInfoCollection diagnosticInfos)
    {
        using var scope = logger.BeginScope("Browse: {@Header}", requestHeader.ToLoggingObject());
        var header = base.Browse(requestHeader, view, requestedMaxReferencesPerNode, nodesToBrowse, out results,
            out diagnosticInfos);
        return LogResponseHeader(header, diagnosticInfos)!;
    }

    /// <inheritdoc />
    public override ResponseHeader Read(RequestHeader requestHeader, double maxAge, TimestampsToReturn timestampsToReturn,
        ReadValueIdCollection nodesToRead, out DataValueCollection results, out DiagnosticInfoCollection diagnosticInfos)
    {
        using var scope = logger.BeginScope("Read: {@Header}", requestHeader.ToLoggingObject());
        var responseHeader = base.Read(requestHeader, maxAge, timestampsToReturn, nodesToRead, out results, out diagnosticInfos);
        return LogResponseHeader(responseHeader, diagnosticInfos)!;
    }

    /// <inheritdoc />
    public override ResponseHeader Call(RequestHeader requestHeader, CallMethodRequestCollection methodsToCall,
        out CallMethodResultCollection results, out DiagnosticInfoCollection diagnosticInfos)
    {
        using var scope = logger.BeginScope("Call: {@Header}", requestHeader.ToLoggingObject());
        var responseHeader = base.Call(requestHeader, methodsToCall, out results, out diagnosticInfos);
        return LogResponseHeader(responseHeader, diagnosticInfos)!;
    }

    /// <inheritdoc />
    public override ResponseHeader Write(RequestHeader requestHeader, WriteValueCollection nodesToWrite, out StatusCodeCollection results,
        out DiagnosticInfoCollection diagnosticInfos)
    {
        using var scope = logger.BeginScope("Write: {@Header}", requestHeader.ToLoggingObject());
        var responseHeader = base.Write(requestHeader, nodesToWrite, out results, out diagnosticInfos);
        return LogResponseHeader(responseHeader, diagnosticInfos)!;
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
    
    private ResponseHeader? LogResponseHeader(ResponseHeader? responseHeader, DiagnosticInfoCollection? diagnosticInfos = null)
    {
        if (diagnosticInfos is { Count: > 0 })
        {
            logger.LogError("Diagnostics: {Diagnostics}", diagnosticInfos.Select(e=>e));
        }
        
        if (responseHeader == null)
        {
            logger.LogError("Response header is null!");
        }
        else if (StatusCode.IsBad(responseHeader.ServiceResult))
        {
            logger.LogError("Header status code is bad: {Code}\n" +
                            "@{Header}", StatusCodes.GetBrowseName(responseHeader.ServiceResult.Code), responseHeader.ToLoggingObject());
        }
        return responseHeader;
    }

}