using Hoeyer.OpcUa.Core.Entity.Node;

namespace Hoeyer.OpcUa.Server.Entity.Observability;

public interface IEntityChangedMessenger
{
    void Publish(IEntityNode node);
}