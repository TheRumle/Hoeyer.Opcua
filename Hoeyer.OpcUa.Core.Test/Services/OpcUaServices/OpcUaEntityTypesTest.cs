using Hoeyer.OpcUa.Core.Services.OpcUaServices;
using Hoeyer.OpcUa.TestEntities.Methods;
using Hoeyer.OpcUa.TestEntities.Methods.Generated;

namespace Hoeyer.OpcUa.Core.Test.Services.OpcUaServices;

public sealed class OpcUaEntityTypesTest
{
    [Test]
    public async Task Contains_GantryMethodCaller()
    {
        var gantryMethodsIsRegisteredAsServiceType =
            OpcUaEntityTypes.ServiceTypes.Any(e => e.GetInterfaces().Contains(typeof(IGantryMethods)));
        var gantryMethodsIsRegisteredAsBehaviour =
            OpcUaEntityTypes.EntityBehaviours.Any(e => e.service == typeof(IGantryMethods));

        using (Assert.Multiple())
        {
            await Assert.That(gantryMethodsIsRegisteredAsServiceType).IsTrue().Because(
                $" the generated implementation is annotated with {nameof(OpcUaEntityServiceAttribute)}");
            await Assert.That(gantryMethodsIsRegisteredAsBehaviour).IsTrue();
            await Assert.That(OpcUaEntityTypes.TypesFromReferencingAssemblies)
                .Contains(typeof(GantryMethodsRemoteCaller))
                .Because(" the type should be found when scanning the assemblies");
        }
    }
}