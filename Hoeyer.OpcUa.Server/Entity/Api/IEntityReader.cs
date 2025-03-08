using System.Collections.Generic;
using Hoeyer.OpcUa.Server.Entity.Api.RequestResponse;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Entity.Api;

public interface IEntityReader
{
    public IEnumerable<EntityValueReadResponse> ReadAttributes(IEnumerable<ReadValueId> valuesToRead);
}