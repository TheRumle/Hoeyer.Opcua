using Hoeyer.OpcUa.Core.Entity.Node;

namespace Hoeyer.OpcUa.Server.Entity.Handle;

public sealed class StartupCreator<T>(T initial, IEntityNodeFactory<T> translator) : IEntityNodeCreator
{
    /// <inheritdoc />
    public string EntityName { get; } = typeof(T).Name;

    /// <inheritdoc />
    public IEntityNode CreateEntityOpcUaNode(ushort assignedNamespace)
    {
        return translator.Create(initial, assignedNamespace);
    }
}