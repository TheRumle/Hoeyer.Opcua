using Hoeyer.OpcUa.Core.Entity;
using Hoeyer.OpcUa.Core.Entity.Node;
using Hoeyer.OpcUa.Server.Entity.Handle;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Test.Application.Fixtures;

public sealed record EntityReaderFixture(IEntityNode Node, ISet<PropertyState> Properties)
{

    public IEntityNodeHandle EntityHandle => new EntityHandle(Node.Entity);
    /// <inheritdoc />
    public override string ToString()
    {
        return $"{Node.Entity.BrowseName.Name}";
    }
}