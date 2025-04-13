using System.Threading.Tasks;
using Hoeyer.OpcUa.Core.Entity.Node;

namespace Hoeyer.OpcUa.Core.Entity;

public interface IEntityInitializer
{
    public string EntityName { get; }
    public Task<IEntityNode> CreateNode(ushort namespaceIndex);
}