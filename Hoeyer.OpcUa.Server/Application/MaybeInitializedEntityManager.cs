using Hoeyer.OpcUa.Core;
using Hoeyer.OpcUa.Server.Api.NodeManagement;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Server.Application;

[OpcUaEntityService(typeof(MaybeInitializedEntityManager<>), ServiceLifetime.Singleton)]
public sealed class MaybeInitializedEntityManager<T> : IMaybeInitializedEntityManager
{
    /// <inheritdoc />
    public string EntityName { get; } = typeof(T).Name;

    public bool HasValue => Manager != null;
    public IEntityNodeManager? Manager { get; internal set; }
}