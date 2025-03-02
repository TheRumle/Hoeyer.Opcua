using System.Collections.Generic;
using Hoeyer.OpcUa.Core.Entity;
using Opc.Ua;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Extensions;

public static class EntityNodeExtensions
{
    public static NodeMetadata ConstructMetadata(this IEntityNode entityNode, ISystemContext serverContext,
        byte accessLevel = AccessLevels.HistoryReadOrWrite | AccessLevels.CurrentReadOrWrite)
    {
        var entity = entityNode.Entity;
        var values = entity.ReadAttributes(
            serverContext,
            Attributes.WriteMask,
            Attributes.UserWriteMask,
            Attributes.DataType,
            Attributes.ValueRank,
            Attributes.ArrayDimensions,
            Attributes.AccessLevel,
            Attributes.UserAccessLevel,
            Attributes.EventNotifier,
            Attributes.Executable,
            Attributes.UserExecutable);


        // construct the metadata object.
        var metadata = new NodeMetadata(entity, entity.NodeId)
        {
            NodeClass = entity.NodeClass,
            BrowseName = entity.BrowseName,
            DisplayName = entity.DisplayName,
            TypeDefinition = entity.TypeDefinitionId,
            ModellingRule = entity.ModellingRuleId,
            DataType = DataTypes.ObjectNode,
            ArrayDimensions = (IList<uint>)values[4],
            AccessLevel = accessLevel
        };

        if (values[0] != null && values[1] != null)
            metadata.WriteMask = (AttributeWriteMask)((uint)values[0] & (uint)values[1]);
        if (values[3] != null) metadata.ValueRank = (int)values[3];
        if (values[7] != null) metadata.EventNotifier = (byte)values[7];
        if (values[8] != null && values[9] != null) metadata.Executable = (bool)values[8] && (bool)values[9];

        return metadata;
    }
}