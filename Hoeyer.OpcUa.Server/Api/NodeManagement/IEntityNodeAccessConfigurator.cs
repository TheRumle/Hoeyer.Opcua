using Hoeyer.OpcUa.Core.Api;

namespace Hoeyer.OpcUa.Server.Api.NodeManagement;

public interface IEntityNodeAccessConfigurator : IEntityNodeConfigurator;

public interface IEntityNodeConfigurator
{
    public void Configure(IEntityNode node);
}