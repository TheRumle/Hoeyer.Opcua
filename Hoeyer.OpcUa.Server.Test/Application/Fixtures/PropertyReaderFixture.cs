using Hoeyer.OpcUa.Core.Entity.Node;
using Hoeyer.OpcUa.Server.Application.Handle;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Test.Application.Fixtures;

public sealed record PropertyReaderFixture(PropertyState PropertyState, string EntityName)
{
    public IEntityNodeHandle PropertyHandle => new PropertyHandle(PropertyState);

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{EntityName}, {PropertyState.BrowseName.Name} ({PropertyState.TypeDefinitionId})";
    }
}