using Hoeyer.OpcUa.Server.Api.NodeManagement;

namespace Hoeyer.OpcUa.Server.Application;

/// <summary>
/// A node manager that is not initialised until the OpcUa Entity server has been started, as marked by the <see cref="ServerStartedHealthCheck"/> being completed.
/// </summary>
public interface IMaybeInitializedEntityManager
{
    public string EntityName { get; }
    bool HasValue { get; }
    IEntityNodeManager? Manager { get; }
}

public interface IMaybeInitializedEntityManager<T> : IMaybeInitializedEntityManager;