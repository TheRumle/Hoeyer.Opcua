using System.Collections.Generic;
using System.Linq;
using FluentResults;
using Hoeyer.OpcUa.Entity;
using Hoeyer.OpcUa.Server.Extensions;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Application.EntityNode.Operations;

internal class EntityReader(IEntityNode entityNode) : IEntityReader
{
    public IEnumerable<Result<ReadResponse>> Read(IEnumerable<ReadValueId> readables)
    {
        return readables.Select(Read);
    }

    /// <inheritdoc />
    public Result<ReadResponse> Read(ReadValueId toRead)
    {
        if (toRead.Processed) return Result.Fail($"Cannot read node with NodeId {toRead.NodeId} as it is already marked as processed!");
        if (entityNode.PropertyStates.TryGetValue(toRead.NodeId, out var readNodeFunc))
            return Result.Ok(new ReadResponse(readNodeFunc.ToDataValue(), toRead));
        if (entityNode.Entity.NodeId.Equals(toRead.NodeId))
            return Result.Ok(new ReadResponse(entityNode.Entity.ToDataValue(), toRead));
            
        return Result.Fail($"Cannot read node with NodeId {toRead.NodeId} as it is not related to the entity!");
    }
}