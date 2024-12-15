﻿using Hoeyer.Machines.OpcUa.Infrastructure.Configuration.Entities.Builder;
using Opc.Ua;

namespace Hoeyer.Machines.OpcUa.Infrastructure.Configuration.Entities.Property;

public static class NodeIdConfigurationToNodeIdConverter
{
    public static NodeId Create(NodeIdConfiguration nodeId)
    {
        return new NodeId(nodeId.IdString);
    }
    
    public static NodeId Create(NodeIdConfiguration nodeId, RootIdentity identity)
    {
        return new NodeId($"ns={identity.NameSpaceIndex};{nodeId.IdString}");
    }
}