namespace Hoeyer.OpcUa.Server.Api.NodeManagement;

public interface IPreinitializedNodeConfigurator
{
    public void Configure(IManagedEntityNode managed);
}

public interface IPreinitializedNodeConfigurator<T> : IPreinitializedNodeConfigurator;