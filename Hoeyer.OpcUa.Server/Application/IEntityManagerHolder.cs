using Hoeyer.OpcUa.Server.Abstractions.NodeManagement;

namespace Hoeyer.OpcUa.Server.Application;

public interface IEntityManagerHolder
{
    public string EntityName { get; }
    bool HasValue => Manager != null;
    public IEntityNodeManager? Manager { get; }
}

public interface IEntityManagerHolder<T> : IEntityManagerHolder;