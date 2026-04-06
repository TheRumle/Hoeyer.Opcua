using Hoeyer.OpcUa.Client.Api.Browsing;
using Hoeyer.OpcUa.Client.Api.Writing;
using Hoeyer.OpcUa.Core;
using Hoeyer.OpcUa.Core.Api.NodeStructure;
using Hoeyer.OpcUa.Core.Configuration.Modelling;
using Hoeyer.OpcUa.Core.Configuration.ServerTarget;
using Hoeyer.OpcUa.Test.ServiceInjection;
using Hoeyer.OpcUa.Test.Simulation;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.TestFramework.Test;

public sealed class GenericEntityServiceDescriptionMatcherTest
{
    private static readonly SimulationServicesTestCollection TestTestCollection =
        new(
            new ServiceCollection(),
            new ClientServicesAdapterArgs
            {
                Protocol = WebProtocol.OpcTcp,
                OpcUaServerId = "id",
                OpcUaServerName = "Name",
                HostName = "localhost",
                Port = 0
            }, [typeof(MyClass)], []);

    public static List<Type> GenericServicesToFind() =>
    [
        typeof(IEntityWriter<>),
        typeof(IEntityBrowser<>),
        typeof(IEntityNodeAlarmAssigner<>),
        typeof(IEntityTypeModel<>)
    ];

    [Test]
    [MethodDataSource(nameof(GenericServicesToFind))]
    [DisplayName("Can find open generic type $t")]
    public async Task WhenTryingToFindService_CanFindService(Type t)
    {
        var matchers = ConstructMatcherFor(t).GetMatchingDescriptors().ToList();
        await Assert.That(matchers.Count).IsEqualTo(1);
    }

    private static GenericEntityServiceDescriptionMatcher ConstructMatcherFor(Type type) => new(
        type,
        TestTestCollection.Collection,
        TestTestCollection.ServiceProvider.GetRequiredService<EntityTypesCollection>()
    );

    [OpcUaEntity]
    public class MyClass
    {
        public int MyInt { get; set; }
    }
}