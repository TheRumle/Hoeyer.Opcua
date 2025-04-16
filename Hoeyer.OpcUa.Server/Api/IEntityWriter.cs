using System.Collections.Generic;
using Hoeyer.OpcUa.Server.Api.RequestResponse;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Api;

public interface IEntityWriter
{
    public IEnumerable<EntityWriteResponse> Write(IEnumerable<WriteValue> nodesToWrite);
}