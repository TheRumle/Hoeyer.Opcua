using Hoeyer.OpcUa.Core.Services.OpcUaServices;
using Hoeyer.OpcUa.TestEntities.Methods;
using Hoeyer.OpcUa.TestEntities.Methods.Generated;

namespace Hoeyer.OpcUa.Core.Test.Services.OpcUaServices;

public sealed class OpcUaAgentTypesTest
{
    [Test]
    public async Task Contains_GantryMethodCaller()
    {
        var gantryMethodsIsRegisteredAsServiceType =
            OpcUaAgentTypes.ServiceTypes.Any(e => e.GetInterfaces().Contains(typeof(IGantryMethods)));
        var gantryMethodsIsRegisteredAsBehaviour =
            OpcUaAgentTypes.AgentBehaviours.Any(e => e.service == typeof(IGantryMethods));

        using (Assert.Multiple())
        {
            await Assert.That(gantryMethodsIsRegisteredAsServiceType).IsTrue().Because(
                $" the generated implementation is annotated with {nameof(OpcUaAgentServiceAttribute)}");
            await Assert.That(gantryMethodsIsRegisteredAsBehaviour).IsTrue();
            await Assert.That(OpcUaAgentTypes.TypesFromReferencingAssemblies)
                .Contains(typeof(GantryMethodsRemoteCaller))
                .Because(" the type should be found when scanning the assemblies");
        }
    }
}