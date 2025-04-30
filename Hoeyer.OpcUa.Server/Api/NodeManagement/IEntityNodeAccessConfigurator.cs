using Hoeyer.OpcUa.Core.Entity.Node;

namespace Hoeyer.OpcUa.Server.Api.NodeManagement;

public interface IEntityNodeAccessConfigurator
{
    public void ConfigureAccess(IEntityNode node);

}