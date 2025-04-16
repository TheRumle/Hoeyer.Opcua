using System.Collections.Generic;
using Hoeyer.OpcUa.Server.Api.RequestResponse;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Api;

public interface IEntityReader
{
    public IEnumerable<EntityValueReadResponse> ReadAttributes(IEnumerable<ReadValueId> valuesToRead);
}