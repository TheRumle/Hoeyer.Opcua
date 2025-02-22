using System;
using System.Collections.Generic;
using System.Linq;
using Hoeyer.OpcUa.Configuration;
using Hoeyer.OpcUa.Entity;
using Hoeyer.OpcUa.Server.Application.EntityNode;
using Hoeyer.OpcUa.Server.Application.EntityNode.Operations;
using Microsoft.Extensions.Logging;
using Opc.Ua;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Application;

public sealed class OpcEntityServer(OpcUaEntityServerConfiguration applicationProductDetails,
    IEnumerable<IEntityNodeCreator> nodeCreators,
    EntityNodeManagerFactory managerFactory,
    ILogger<OpcEntityServer> logger) 
    : StandardServer //ServerBase? instead? 
{
    public IServerInternal Server => ServerInternal;
    public readonly IEnumerable<Uri> EndPoints = [..applicationProductDetails.Endpoints.Select(e=>new Uri(e))];
    public EntityMasterNodeManager EntityManager { get; private set; } = null!;

    protected override MasterNodeManager CreateMasterNodeManager(IServerInternal server, ApplicationConfiguration configuration)
    {
        logger.LogInformation("Creating entity managers");
        var additionalManagers = nodeCreators
            .Select(nodeCreator => managerFactory.Create(server, applicationProductDetails, nodeCreator))
            .ToArray();
        
        EntityManager = new EntityMasterNodeManager(server, configuration, additionalManagers);
        return EntityManager;
    }
    

    /// <inheritdoc />
    public override ResponseHeader ActivateSession(RequestHeader requestHeader, SignatureData clientSignature,
        SignedSoftwareCertificateCollection clientSoftwareCertificates, StringCollection localeIds,
        ExtensionObject userIdentityToken, SignatureData userTokenSignature, out byte[] serverNonce,
        out StatusCodeCollection results, out DiagnosticInfoCollection diagnosticInfos)
    {
        try
        {
            using var scope = logger.BeginScope("ActivateSession for {@Session}", requestHeader);
            var a = base.ActivateSession(requestHeader, clientSignature, clientSoftwareCertificates, localeIds, userIdentityToken, userTokenSignature, out serverNonce, out results, out diagnosticInfos);
            return a;
        }
        catch (Exception e)
        {
            logger.LogCritical(e,
                "An exception occured when trying to activate session for {@SessionInfo}.",
                new
                {
                    Header = requestHeader
                });

            diagnosticInfos = new DiagnosticInfoCollection();
            results = new StatusCodeCollection();
            serverNonce = null!;
            return new ResponseHeader()
            {
                Timestamp = DateTime.UtcNow,
                ServiceResult = StatusCodes.BadNotConnected,
            };
        }
        
 
    }

    /// <inheritdoc />
    protected override ServerProperties LoadServerProperties()
    {
        ServerProperties properties = base.LoadServerProperties();
        properties.BuildDate = DateTime.UtcNow;
        properties.ProductName = applicationProductDetails.ServerName;
        properties.ProductUri = applicationProductDetails.ApplicationNamespace.ToString();
        properties.SoftwareVersion = "1.0";
        return properties;
    }

    /// <inheritdoc />
    protected override void OnNodeManagerStarted(IServerInternal server)
    {
        base.OnNodeManagerStarted(server);
    }

    /// <inheritdoc />
    public override ResponseHeader Browse(RequestHeader requestHeader, ViewDescription view, uint requestedMaxReferencesPerNode,
        BrowseDescriptionCollection nodesToBrowse, out BrowseResultCollection results,
        out DiagnosticInfoCollection diagnosticInfos)
    {
        var a = base.Browse(requestHeader, view, requestedMaxReferencesPerNode, nodesToBrowse, out results, out diagnosticInfos);
        return a;
    }

    private bool _disposed;

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (_disposed) return;
        if (!disposing) return;
        
        EntityManager.Dispose();
        base.Dispose(disposing);
        _disposed = true;
    }
}