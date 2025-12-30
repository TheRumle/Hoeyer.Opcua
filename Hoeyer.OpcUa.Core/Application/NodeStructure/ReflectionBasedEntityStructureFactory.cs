using System.Collections.Generic;
using System.Linq;
using Hoeyer.OpcUa.Core.Api;
using Hoeyer.OpcUa.Core.Api.NodeStructure;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Application.NodeStructure;

public sealed class ReflectionBasedEntityStructureFactory<T>(
    IBrowseNameCollection<T> entityModel,
    IEntityNodePropertyAssigner<T> entityNodePropertyAssigner,
    IEntityNodeMethodAssigner<T> entityNodeMethodAssigner,
    IEntityNodeAlarmAssigner<T> entityNodeAlarmAssigner)
    : IEntityNodeStructureFactory<T>
{
    private IEntityNode? _node;

    public IEntityNode Create(ushort applicationNamespaceIndex)
    {
        if (_node != null)
        {
            return _node;
        }

        var browseName = entityModel.EntityName;
        var entity = new BaseObjectState(null)
        {
            BrowseName = new QualifiedName(browseName, applicationNamespaceIndex),
            NodeId = new NodeId(browseName, applicationNamespaceIndex),
            DisplayName = browseName
        };
        entity.AccessRestrictions = AccessRestrictionType.None;
        var properties = entityNodePropertyAssigner.AssignProperties(entity).ToList();
        var methods = entityNodeMethodAssigner.AssignMethods(entity);
        var alarms = entityNodeAlarmAssigner.AssignAlarms(properties);


        _node = new EntityNode(
            entity,
            new HashSet<PropertyState>(properties),
            new HashSet<MethodState>(methods),
            alarms);
        return _node;
    }
}