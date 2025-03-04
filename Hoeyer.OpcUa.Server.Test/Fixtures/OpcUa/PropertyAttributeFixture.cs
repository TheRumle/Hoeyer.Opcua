using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Test.Fixtures.OpcUa;

public sealed record PropertyAttributeFixture(uint AttributeId, PropertyState PropertyState, string EntityName)
{
    /// <inheritdoc />
    public override string ToString()
    {
        return $"{EntityName}.{PropertyState.BrowseName}: {Attributes.GetBrowseName(AttributeId)}";
    }
}