using Hoeyer.OpcUa.Server.Api.NodeManagement;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Application;

internal sealed class NoAccessRestrictionsConfigurator : IEntityNodeAccessConfigurator
{
    public void Configure(IManagedEntityNode node)
    {
        foreach (var managedEntityPropertyState in node.PropertyStates)
        {
            managedEntityPropertyState.UserAccessLevel = AccessLevels.CurrentRead | AccessLevels.CurrentWrite;
            managedEntityPropertyState.AccessLevel = AccessLevels.CurrentRead | AccessLevels.CurrentWrite;
            managedEntityPropertyState.MinimumSamplingInterval = 500;
        }
    }
}