using Hoeyer.OpcUa.Server.Api.NodeManagement;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Application;

internal sealed class NoAccessRestrictionsConfigurator : IEntityNodeAccessConfigurator
{
    public void Configure(IManagedEntityNode managed, ISystemContext context)
    {
        managed.ChangeState(entityNode =>
        {
            foreach (var managedEntityPropertyState in entityNode.PropertyStates)
            {
                managedEntityPropertyState.UserAccessLevel = AccessLevels.CurrentRead | AccessLevels.CurrentWrite;
                managedEntityPropertyState.AccessLevel = AccessLevels.CurrentRead | AccessLevels.CurrentWrite;
                managedEntityPropertyState.MinimumSamplingInterval = 500;
            }
        });
    }
}