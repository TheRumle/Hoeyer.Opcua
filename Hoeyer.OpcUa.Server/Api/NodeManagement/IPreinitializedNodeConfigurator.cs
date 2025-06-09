using Hoeyer.OpcUa.Core.Api;

namespace Hoeyer.OpcUa.Server.Api.NodeManagement;

public interface IPreinitializedNodeConfigurator
{
    public void Configure(IEntityNode node);
}

public interface IPreinitializedNodeConfigurator<T> : IPreinitializedNodeConfigurator;