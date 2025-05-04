using Hoeyer.OpcUa.Core.Api;

namespace Hoeyer.OpcUa.Server.Api.NodeManagement;

public interface IEntityNodeAccessConfigurator
{
    public void ConfigureAccess(IEntityNode node);

}