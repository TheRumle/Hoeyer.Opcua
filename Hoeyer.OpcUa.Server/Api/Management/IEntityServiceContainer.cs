using System.Threading.Tasks;
using Hoeyer.Common.Messaging;
using Hoeyer.OpcUa.Core.Entity.Node;

namespace Hoeyer.OpcUa.Server.Api.Management;

public interface IEntityServiceContainer
{
    IMessagePublisher<IEntityNode> EntityChangedPublisher { get; }
    public string EntityName { get; }
    public Task<IEntityNode> CreateNode(ushort namespaceIndex);
}