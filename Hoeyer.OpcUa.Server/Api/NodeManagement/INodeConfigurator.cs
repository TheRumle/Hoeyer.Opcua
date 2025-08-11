using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Api.NodeManagement;

public interface INodeConfigurator
{
    public void Configure(IManagedEntityNode managed, ISystemContext context);
}

public interface INodeConfigurator<T> : INodeConfigurator;