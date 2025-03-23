using System.Threading.Tasks;
using Hoeyer.OpcUa.Core.Entity.Node;

namespace Hoeyer.OpcUa.Core.Entity;

public interface IEntityInitializer
{
    public string EntityName { get; }
    public Task<IEntityNode> CreateNode(ushort namespaceIndex);
}

public sealed class EntityInitializer<T>(IEntityLoader<T> value, IEntityTranslator<T> translator, IEntityNodeStructureFactory<T> structureFactory) : IEntityInitializer
{
    public string EntityName { get; } = typeof(T).Name;
    public async Task<IEntityNode> CreateNode(ushort namespaceIndex)
    {
        T entity = await value.LoadCurrentState();
        var nodeRepresentation = structureFactory.Create(namespaceIndex);
        translator.AssignToNode(entity, nodeRepresentation);
        return nodeRepresentation;
    }
}