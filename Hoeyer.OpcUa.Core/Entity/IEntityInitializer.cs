using System.Threading.Tasks;
using Hoeyer.Common.Messaging;
using Hoeyer.OpcUa.Core.Entity.Node;

namespace Hoeyer.OpcUa.Core.Entity;

public interface IEntityInitializer
{
    public string EntityName { get; }
    public Task<(IEntityNode node, IMessagePublisher<IEntityNode> nodeChangedPublisher)> CreateNode(ushort namespaceIndex);
}