namespace Hoeyer.OpcUa.Server.Api.NodeManagement;

public interface IPreinitializedNodeConfigurator
{
    public void Configure(IManagedEntityNode node);
}

public interface IPreinitializedNodeConfigurator<T> : IPreinitializedNodeConfigurator;