using Hoeyer.OpcUa.Server.Api.NodeManagement;
using Microsoft.Extensions.Logging;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Application;

internal sealed class AlarmLoggingConfigurator<T>(ILogger<AlarmLoggingConfigurator<T>> logger) : INodeConfigurator<T>
{
    public void Configure(IManagedEntityNode managed, ISystemContext context)
    {
        managed.ChangeState(entityNode =>
        {
            foreach (var alarmNode in entityNode.AlarmsByProperty.Values)
            {
                alarmNode.OnStateChanged += (context, instance, state) =>
                {
                    logger.LogDebug("Alarm state changed for alarm '{Alarm}'", alarmNode.BrowseName.Name);
                };

                alarmNode.OnReportEvent += (ctx, state, filter) =>
                {
                    logger.LogDebug("ReportEvent called for alarm '{Alarm}'", alarmNode.BrowseName.Name);
                };
            }
        });
    }
}