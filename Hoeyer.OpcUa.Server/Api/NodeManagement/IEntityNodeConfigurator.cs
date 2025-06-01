using Hoeyer.OpcUa.Core.Api;

namespace Hoeyer.OpcUa.Server.Api.NodeManagement;

public interface IEntityNodeConfigurator
{
    public void Configure(IEntityNode node);
}