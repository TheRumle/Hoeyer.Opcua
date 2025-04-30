using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Server.Api.NodeManagement;
using Opc.Ua;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Application;

internal sealed class DomainMasterNodeManagerFactory(IEnumerable<IEntityNodeManagerFactory> entityManagerFactories) : IDomainMasterManagerFactory
{
    public DomainMasterNodeManager ConstructMasterManager(IServerInternal server,
        ApplicationConfiguration applicationConfiguration)
    {
        var managerCreationTasks = entityManagerFactories
            .Select(async factory => await factory.CreateEntityManager(server))
            .ToArray();
        
        Task.WhenAll(managerCreationTasks);
        return new DomainMasterNodeManager(server, applicationConfiguration, managerCreationTasks.Select(e=>e.Result).ToArray());
    }
}