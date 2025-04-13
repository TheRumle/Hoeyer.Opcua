using Hoeyer.OpcUa.Core.Application.Observation;
using Hoeyer.OpcUa.Core.Entity.Node;

namespace Hoeyer.OpcUa.Server.Entity.Application;

public class EntitySubscriber() : IStateChangeSubscriber<IEntityNode>
{
    /// <inheritdoc />
    public void OnStateChange(IEntityNode stateChange)
    {
        throw new System.NotImplementedException();
    }
}