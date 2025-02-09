using System;
using System.Collections.Generic;
using System.Linq;
using Hoeyer.OpcUa.Server.Application.NodeManagement.Entity;
using Hoeyer.OpcUa.Server.ServiceConfiguration;
using Opc.Ua;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Application;

public sealed class OpcEntityServer : StandardServer
{
    private readonly EntityMasterNodeManagerFactory _managerFactory;
    private readonly EntityServerConfiguration applicationProductDetails;
    
    public IServerInternal Server => ServerInternal;
    public IEnumerable<Uri> EndPoints => CurrentInstance.EndpointAddresses;
    public EntityMasterNodeManager EntityManager { get; private set; }
    
    public OpcEntityServer(EntityMasterNodeManagerFactory managerFactory, EntityServerConfiguration details)
    {
        _managerFactory = managerFactory;
        applicationProductDetails = details;
    }

    protected override MasterNodeManager CreateMasterNodeManager(IServerInternal server,
        ApplicationConfiguration configuration)
    {
        EntityManager =  _managerFactory.Create(server, configuration, applicationProductDetails);
        
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
        properties.ProductName = applicationProductDetails.ServerName;
        properties.ProductUri = this.applicationProductDetails.Urn.ToString();
        properties.SoftwareVersion = "1.0";
        return properties;
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