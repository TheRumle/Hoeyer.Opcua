using Hoeyer.OpcUa.Server.Api.NodeManagement;

namespace Hoeyer.OpcUa.Server.Application;

public sealed class MaybeInitializedEntityManager<T> : IMaybeInitializedEntityManager<T>
{
    /// <inheritdoc />
    public string EntityName { get; } = typeof(T).Name;

    public bool HasValue => Manager != null;
    public IEntityNodeManager? Manager { get; internal set; }
}