using Hoeyer.OpcUa.Core.Entity;
using Hoeyer.OpcUa.Server.Entity.Handle;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Test.Fixtures.Application;

public sealed record PropertyReaderFixture(PropertyState PropertyState, string EntityName)
{
    public IEntityNodeHandle PropertyHandle => new PropertyHandle(PropertyState);
    /// <inheritdoc />
    public override string ToString()
    {
        return $"{EntityName}, {PropertyState.BrowseName.Name} ({PropertyState.TypeDefinitionId})";
    }
}

public sealed record EntityReaderFixture(IEntityNode Node, string EntityName, ISet<PropertyState> Properties)
{

    public IEntityNodeHandle EntityHandle => new EntityHandle(Node.Entity);
    /// <inheritdoc />
    public override string ToString()
    {
        return $"{EntityName}";
    }
}