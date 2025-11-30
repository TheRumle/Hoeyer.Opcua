using System.Collections.Generic;
using System.Linq;
using Hoeyer.OpcUa.Server.Services.Configuration;
using Opc.Ua;
using Opc.Ua.Server;

namespace Hoeyer.OpcUa.Server.Application;

internal sealed class PingManager(IServerInternal server, IOpcUaTargetServerSetup info)
    : CustomNodeManager(server, info.ApplicationNamespace + "/ping")
{
    public const string PING_BROWSENAME = "ServerHealth-ping";
    public string NameSpace => NamespaceUris.First();


    public override void CreateAddressSpace(IDictionary<NodeId, IList<IReference>> externalReferences)
    {
        var healthVar = new BaseDataVariableState(null)
        {
            NodeId = new NodeId(PING_BROWSENAME, NamespaceIndex),
            BrowseName = new QualifiedName(PING_BROWSENAME, NamespaceIndex),
            DisplayName = new LocalizedText(PING_BROWSENAME),
            DataType = DataTypeIds.String,
            ValueRank = ValueRanks.Scalar,
            AccessLevel = AccessLevels.CurrentRead,
            UserAccessLevel = AccessLevels.CurrentRead,
            Value = "OK"
        };

        var wantedPlacement = ObjectIds.RootFolder;
        AddPredefinedNode(SystemContext, healthVar);
        if (!externalReferences.TryGetValue(wantedPlacement, out var references))
        {
            references ??= new List<IReference>();
            externalReferences[wantedPlacement] = references;
        }

        references.Add(new NodeStateReference(ReferenceTypeIds.Organizes, false, healthVar.NodeId));
    }
}