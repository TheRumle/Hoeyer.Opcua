using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Api.NodeManagement;

public interface INodeConfigurator
{
    public void Configure(IManagedAgent managed, ISystemContext context);
}

public interface INodeConfigurator<T> : INodeConfigurator;