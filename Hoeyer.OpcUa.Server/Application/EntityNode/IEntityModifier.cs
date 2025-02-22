using System.Collections.Generic;
using FluentResults;
using Opc.Ua;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Application.EntityNode;

public interface IEntityModifier
{
    public IEnumerable<Result<ServiceResult>> Write(ISystemContext systemContext, IEnumerable<WriteValue> nodesToWrite);
}