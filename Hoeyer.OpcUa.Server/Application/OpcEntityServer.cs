using System;
using System.Collections.Generic;
using System.Linq;
using Hoeyer.OpcUa.Entity;
using Hoeyer.OpcUa.Server.Application.NodeManagement;
using Hoeyer.OpcUa.Server.Application.NodeManagement.Entity;
using Hoeyer.OpcUa.Server.ServiceConfiguration;
using Opc.Ua;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Application;

public sealed class OpcEntityServer : StandardServer
{
    private readonly IEnumerable<IEntityNodeCreator> _nodeCreators;
    private readonly EntityServerConfiguration _applicationProductDetails;
    
    public IServerInternal Server => ServerInternal;
    public readonly IEnumerable<Uri> EndPoints;
    private readonly EntityNodeManagerFactory _managerFactory;
    public EntityMasterNodeManager EntityManager { get; private set; }
    
    public OpcEntityServer(EntityServerConfiguration details, IEnumerable<IEntityNodeCreator> nodeCreators, EntityNodeManagerFactory factory)
    {
        _nodeCreators = nodeCreators;
        _applicationProductDetails = details;
        _managerFactory = factory;
        EndPoints = [..details.Endpoints.Select(e=>new Uri(e))];
    }

    protected override MasterNodeManager CreateMasterNodeManager(IServerInternal server, ApplicationConfiguration configuration)
    {
        var additionalManagers = _nodeCreators
            .Select(nodeCreator => _managerFactory.Create(server, configuration, nodeCreator))
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
        var a = base.ActivateSession(requestHeader, clientSignature, clientSoftwareCertificates, localeIds, userIdentityToken, userTokenSignature, out serverNonce, out results, out diagnosticInfos);
        return a;
    }

    /// <inheritdoc />
    protected override ServerProperties LoadServerProperties()
    {
        ServerProperties properties = base.LoadServerProperties();
        properties.BuildDate = DateTime.UtcNow;
        properties.ProductName = _applicationProductDetails.ServerName;
        properties.ProductUri = this._applicationProductDetails.ApplicationNamespace.ToString();
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
        Console.WriteLine($"browsing {string.Join(",",nodesToBrowse.Select(e=>e.NodeId.ToString()))}");
        var a = base.Browse(requestHeader, view, requestedMaxReferencesPerNode, nodesToBrowse, out results, out diagnosticInfos);
        return a;
    }

}