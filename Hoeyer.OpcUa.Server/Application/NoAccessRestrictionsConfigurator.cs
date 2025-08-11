using Hoeyer.OpcUa.Server.Api.NodeManagement;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Application;

internal sealed class NoAccessRestrictionsConfigurator : IAgentAccessConfigurator
{
    public void Configure(IManagedAgent managed, ISystemContext context)
    {
        managed.ChangeState(Agent =>
        {
            foreach (var managedEntityPropertyState in Agent.PropertyStates)
            {
                managedEntityPropertyState.UserAccessLevel = AccessLevels.CurrentRead | AccessLevels.CurrentWrite;
                managedEntityPropertyState.AccessLevel = AccessLevels.CurrentRead | AccessLevels.CurrentWrite;
                managedEntityPropertyState.MinimumSamplingInterval = 500;
            }
        });
    }
}