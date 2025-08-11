using Hoeyer.OpcUa.Server.Api.NodeManagement;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Application;

internal sealed class NoAccessRestrictionsConfigurator : IAgentAccessConfigurator
{
    public void Configure(IManagedAgent managed, ISystemContext context)
    {
        managed.ChangeState(Agent =>
        {
            foreach (var managedAgentPropertyState in Agent.PropertyStates)
            {
                managedAgentPropertyState.UserAccessLevel = AccessLevels.CurrentRead | AccessLevels.CurrentWrite;
                managedAgentPropertyState.AccessLevel = AccessLevels.CurrentRead | AccessLevels.CurrentWrite;
                managedAgentPropertyState.MinimumSamplingInterval = 500;
            }
        });
    }
}