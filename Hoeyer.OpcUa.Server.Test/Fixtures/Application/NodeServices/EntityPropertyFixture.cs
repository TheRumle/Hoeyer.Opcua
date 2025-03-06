using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Test.Fixtures.Application.NodeServices;

public sealed record EntityPropertyFixture(PropertyState PropertyState, string EntityName)
{
    /// <inheritdoc />
    public override string ToString()
    {
        return $"{EntityName}.{PropertyState.BrowseName}";
    }
}