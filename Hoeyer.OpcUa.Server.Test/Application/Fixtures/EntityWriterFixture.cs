using Hoeyer.OpcUa.Core.Entity.Node;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Test.Application.Fixtures;

public sealed record EntityWriterFixture(IEntityNode Node, ISet<PropertyState> Properties)
{
}