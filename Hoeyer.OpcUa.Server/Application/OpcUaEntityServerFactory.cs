using System.Collections.Generic;
using System.Linq;
using Hoeyer.OpcUa.Configuration;
using Hoeyer.OpcUa.Entity;
using Hoeyer.OpcUa.Server.Application.EntityNode;
using Hoeyer.OpcUa.Server.Application.EntityNode.Operations;
using Hoeyer.OpcUa.Server.ServiceConfiguration;
using Microsoft.Extensions.Logging;
using Opc.Ua;
using Opc.Ua.Configuration;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Application;



public sealed class OpcUaEntityServerFactory(
    OpcUaEntityServerSetup serverSetup,
    IEnumerable<IEntityNodeCreator> entityObjectCreators,
    ILoggerFactory loggerFactory)
{
    public IStartableEntityServer CreateServer()
    {
        var configuration = ServerApplicationConfigurationFactory.CreateServerConfiguration(serverSetup);

        var application = new ApplicationInstance
        {
            ApplicationConfiguration = configuration,
            ApplicationName = serverSetup.ApplicationName,
            ApplicationType = ApplicationType.Server
        };

        application.LoadApplicationConfiguration(false);
        
        var masterfactory = new DomainMasterNodeManagerFactory(new EntityNodeManagerFactory(loggerFactory), entityObjectCreators, serverSetup);
        var server = new OpcEntityServer(serverSetup, masterfactory, loggerFactory.CreateLogger<OpcEntityServer>());
        return new StartableEntityServer(application, server);
    }
}

internal sealed class DomainMasterNodeManagerFactory(EntityNodeManagerFactory entityManagerFactory,
    IEnumerable<IEntityNodeCreator> creators,
    IOpcUaEntityServerInfo info
    ) : IDomainMasterManagerFactory
{
    /// <inheritdoc />
    public DomainMasterNodeManager ConstructMasterManager(IServerInternal server, ApplicationConfiguration applicationConfiguration)
    {
        var additionalManagers = creators.Select(e => entityManagerFactory.Create(server, info.Host, e)).ToArray();
        return new DomainMasterNodeManager(server, applicationConfiguration, additionalManagers);
    }
    
    private static FolderState CreateRootFolder(string folderName, ushort namespaceIndex)
    {
        // Create a root folder for this entity.
        var rootFolder = new FolderState(null)
        {
            SymbolicName = folderName,
            BrowseName = new QualifiedName(folderName),
            DisplayName = folderName,
            NodeId = new NodeId(folderName, namespaceIndex),
            TypeDefinitionId = ObjectTypeIds.FolderType
        };

        return rootFolder;
    }
}