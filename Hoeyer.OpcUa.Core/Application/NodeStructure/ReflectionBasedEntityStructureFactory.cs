using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Core.Api;
using Hoeyer.OpcUa.Core.Api.NodeStructure;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Application.NodeStructure;

public sealed class ReflectionBasedEntityStructureFactory<T>(
    IEntityTypeModel<T> entityModel)
    : IEntityNodeStructureFactory<T>
{
    private readonly Type _type = typeof(T);
    private IEntityNode? _node;

    public IEntityNode Create(ushort applicationNamespaceIndex)
    {
        if (_node != null)
        {
            return _node;
        }

        var type = typeof(T);
        var browseName = entityModel.EntityName;

        BaseObjectState entity = new BaseObjectState(null)
        {
            BrowseName = new QualifiedName(browseName, applicationNamespaceIndex),
            NodeId = new NodeId(browseName, applicationNamespaceIndex),
            DisplayName = browseName
        };
        entity.AccessRestrictions = AccessRestrictionType.None;

        var properties = type
            .GetProperties()
            .Select(e => new OpcPropertyTypeInfo(entityModel.PropertyNames[e.Name], e, entity))
            .ToList();

        List<OpcMethodTypeInfo> nodeMethods = CreateMethods(entity).ToList();
        Exception[] errors = VerifyNoDuplicateMethodNames(nodeMethods)
            .Union(VerifyProperties(properties))
            .ToArray();

        if (errors.Length != 0) throw new AggregateException(errors);
        AssignReferences(properties, entity);
        AssignMethods(nodeMethods, entity);

        var alarms = CreateAlarms(entity).ToList();
        AssignAlarms(alarms, entity);

        _node = new EntityNode(entity,
            new HashSet<PropertyState>(properties.Select(e => e.OpcProperty)),
            new HashSet<MethodState>(nodeMethods.Select(e => e.Method)));
        return _node;
    }

    private void AssignAlarms(List<OffNormalAlarmState> alarms, BaseObjectState entity)
    {
        throw new NotImplementedException();
    }

    private IEnumerable<OffNormalAlarmState> CreateAlarms(BaseObjectState parent)
    {
        return entityModel
            .PropertyAlarms
            .Values
            .SelectMany(attribute => attribute)
            .Select(e => e.BrowseName).Select(alarm => new OffNormalAlarmState(parent)
            {
                NodeId = new NodeId($"{parent.NodeId}.{alarm}.Alarm", parent.NodeId.NamespaceIndex),
                BrowseName = new QualifiedName(alarm, parent.BrowseName.NamespaceIndex),
                DisplayName = alarm
            });
    }

    private IEnumerable<Exception> VerifyProperties(IList<OpcPropertyTypeInfo> properties)
    {
        return properties
            .Where(e => e.TypeId is null)
            .Select(e =>
                new InvalidEntityConfigurationException(_type.FullName!,
                    $"The property {e.PropertyInfo.Name} is of type {e.PropertyInfo.PropertyType.FullName} and could not be translated to NodeId representing the type."));
    }

    private IEnumerable<Exception> VerifyNoDuplicateMethodNames(IList<OpcMethodTypeInfo> methods)
    {
        IEnumerable<string> methodNames = methods.Select(e => e.Method.BrowseName.Name);
        List<IGrouping<string, string>> duplicateNames = methodNames.GroupBy(x => x).Where(g => g.Count() > 1).ToList();
        return duplicateNames
            .Select(name => new InvalidEntityConfigurationException(
                _type.FullName!,
                $"{_type.FullName} has multiple definitions of the following method: {name}"));
    }

    private static void AssignMethods(IEnumerable<IOpcTypeInfo> methods, BaseObjectState entity)
    {
        foreach (var pr in methods.Select(type => type.InstanceState))
        {
            entity.AddReference(ReferenceTypeIds.HasComponent, false, pr.NodeId);
            entity.AddChild(pr);
        }
    }

    private static void AssignReferences(IEnumerable<IOpcTypeInfo> values, BaseObjectState entity)
    {
        foreach (var pr in values)
        {
            var referenceTypeid = pr switch
            {
                OpcMethodTypeInfo => ReferenceTypeIds.HasComponent,
                OpcPropertyTypeInfo => ReferenceTypeIds.HasProperty,
                var _ => throw new ArgumentOutOfRangeException(pr.GetType().Name + " is not a handled case")
            };

            entity.AddChild(pr.InstanceState);
            entity.AddReference(referenceTypeid, false, pr.InstanceState.NodeId);
        }
    }

    private IEnumerable<OpcMethodTypeInfo> CreateMethods(BaseObjectState entity)
    {
        return entityModel.Methods
            .Select(method =>
            {
                var browseName = entityModel.MethodNames[method.Name];
                return new OpcMethodTypeInfo(
                    methodName: browseName,
                    parent: entity,
                    returnType: method.ReturnType == typeof(Task) && !method.ReturnType.IsGenericType
                        ? null
                        : method.ReturnType,
                    arguments: method.GetParameters()
                );
            });
    }

    private OffNormalAlarmState AddBoolAlarm(
        SystemContext context,
        BaseObjectState entity,
        string alarmName,
        bool initialValue)
    {
        var stateVariable = new BaseDataVariableState<bool>(entity)
        {
            NodeId = new NodeId($"{entity.NodeId}.{alarmName}.State", entity.NodeId.NamespaceIndex),
            BrowseName = new QualifiedName($"{alarmName}State", entity.BrowseName.NamespaceIndex),
            DisplayName = alarmName + " State",
            DataType = DataTypeIds.Boolean,
            ValueRank = ValueRanks.Scalar,
            Value = initialValue,
            StatusCode = StatusCodes.Good,
            Timestamp = DateTime.UtcNow
        };

        entity.AddChild(stateVariable);
        var alarm = new OffNormalAlarmState(entity)
        {
            NodeId = new NodeId($"{entity.NodeId}.{alarmName}.Alarm", entity.NodeId.NamespaceIndex),
            BrowseName = new QualifiedName(alarmName, entity.BrowseName.NamespaceIndex),
            DisplayName = alarmName
        };

        alarm.Create(context, null, alarm.BrowseName, alarm.BrowseName.Name, true);

        alarm.EventId.Value = Guid.NewGuid().ToByteArray();
        entity.AddChild(alarm);
        return alarm;
    }
}