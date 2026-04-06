using System;
using Hoeyer.OpcUa.Server.Api.NodeManagement;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Application;

internal sealed class AlarmSetupConfigurator<T> : INodeConfigurator<T>
{
    public void Configure(IManagedEntityNode managed, ISystemContext context)
    {
        managed.ChangeState(entityNode =>
        {
            var index = entityNode.BaseObject.NodeId.NamespaceIndex;
            foreach (var (property, alarmNode) in entityNode.AlarmsByProperty)
            {
                var nodeId = new NodeId(
                    $"{entityNode.BaseObject.BrowseName.Name}/Alarms/{property.BrowseName.Name}",
                    index
                );

                alarmNode.Create(context, nodeId, alarmNode.BrowseName, alarmNode.DisplayName, true);
                alarmNode.InputNode.Value = property.NodeId;

                alarmNode.SetEnableState(context, true);

                alarmNode.Initialize(
                    context,
                    entityNode.BaseObject,
                    EventSeverity.Min,
                    "Initialized");

                property.OnWriteValue +=
                    (
                        ISystemContext systemContext,
                        NodeState node,
                        NumericRange range,
                        QualifiedName encoding,
                        ref object value,
                        ref StatusCode code,
                        ref DateTime timestamp
                    ) =>
                    {
                        if (value is double val)
                        {
                            EvaluateAlarm(val, alarmNode);
                        }

                        return ServiceResult.Good;
                    };
            }
        });
    }

    private void EvaluateAlarm(double val, LimitAlarmState alarmNode)
    {
        throw new NotImplementedException();
    }
}