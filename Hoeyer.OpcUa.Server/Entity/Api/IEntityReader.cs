using System.Collections.Generic;
using Hoeyer.OpcUa.Server.Application.RequestResponse;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Entity.Api;

public interface IEntityReader
{
    public IEnumerable<EntityValueReadResponse> ReadProperties(IEnumerable<ReadValueId> valuesToRead);
    
}