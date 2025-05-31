using Hoeyer.OpcUa.Core;
using Hoeyer.OpcUa.Server.Api.NodeManagement;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Server.Application;

/// <summary>
/// A node manager that is not initialised until the OpcUa Entity server has been started, as marked by the <see cref="Hoeyer.OpcUa.Server.Api.EntityServerStartedMarker"/> being completed.
/// </summary>
public interface IMaybeInitializedEntityManager
{
    public string EntityName { get; }
    bool HasValue { get; }
    IEntityNodeManager? Manager { get; }
}

[OpcUaEntityService(typeof(MaybeInitializedEntityManager<>), ServiceLifetime.Singleton)]
public sealed class MaybeInitializedEntityManager<T> : IMaybeInitializedEntityManager
{
    /// <inheritdoc />
    public string EntityName { get; } = typeof(T).Name;

    public bool HasValue => Manager != null;
    public IEntityNodeManager? Manager { get; internal set; }
}