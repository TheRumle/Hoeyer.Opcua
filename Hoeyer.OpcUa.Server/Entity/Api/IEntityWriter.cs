using System.Collections.Generic;
using Hoeyer.OpcUa.Server.Application.RequestResponse;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Entity.Api;

public interface IEntityWriter
{
    public IEnumerable<EntityWriteResponse> Write(IEnumerable<WriteValue> nodesToWrite, ISystemContext context);
}