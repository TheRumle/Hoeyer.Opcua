using Hoeyer.OpcUa.Core.Entity.Node;
using Hoeyer.OpcUa.Server.Application.Handle;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Test.Application.Fixtures;

public sealed record EntityFixture(IEntityNode Node)
{
    public ISet<PropertyState> Properties => Node.PropertyStates.Values.ToHashSet();
    public IEntityNodeHandle EntityHandle => new EntityHandle(Node.BaseObject);

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{Node.BaseObject.BrowseName.Name}";
    }
}