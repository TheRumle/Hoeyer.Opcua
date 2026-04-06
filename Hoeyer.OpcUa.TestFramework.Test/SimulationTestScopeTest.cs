using Hoeyer.OpcUa.Test.Api;
using Hoeyer.OpcUa.Test.Simulation;
using Hoeyer.OpcUa.Test.Simulation.Scope;
using TUnit.Core.Enums;

namespace Hoeyer.OpcUa.TestFramework.Test;

public class SimulationTestScopeTest
{
    private static readonly TestAdapter Adapter = new();

    private static readonly TestBuilderContext TestBuildContext = new()
    {
        TestMetadata = new MethodMetadata
        {
            Type = typeof(SimulationTestScopeTest),
            Class = null!,
            GenericTypeCount = 0,
            Name = "MYName",
            Parameters = [],
            ReturnTypeInfo = new ConcreteType(typeof(int)),
            TypeInfo = new ConcreteType(typeof(int)),
            ReturnType = typeof(int)
        }
    };

    private static readonly DataGeneratorMetadata Metadata = new()
    {
        ClassInstanceArguments = [],
        MembersToGenerate = [],
        TestBuilderContext = new TestBuilderContextAccessor(TestBuildContext),
        TestInformation = null,
        TestSessionId = Guid.NewGuid().ToString(),
        Type = DataGeneratorType.TestParameters,
        InstanceFactory = null,
        TestClassInstance = null
    };


    [Test]
    [DisplayName(
        "When calling create, does not return null")]
    public async Task CallingSetupTwice_ReturnsNotNull()
    {
        var testScope = SimulationTestScope.PerTestSession(Adapter);
        var first = testScope.Create(Metadata, setup => setup);
        await Assert.That(first).IsNotNull();
    }

    [Test]
    [DisplayName(
        "When calling create twice with readonly scope, the exact same instance of simulation setup is returned")]
    public async Task CallingSetupTwice_GivesSameInstance()
    {
        var testScope = SimulationTestScope.PerTestSession(Adapter);
        var first = testScope.Create(Metadata, setup => setup);
        var second = testScope.Create(Metadata, setup => setup);
        await Assert.That(first).IsSameReferenceAs(second);
    }

    public static IEnumerable<Func<Func<SimulationSetup, ISimulationSession>>> SimulationSessions()
    {
        yield return () => setup => new SimulationTestSession(setup);
        yield return () => setup => new SimulationServiceContext<int>(setup);
    }

    [Test]
    [MethodDataSource(nameof(SimulationSessions))]
    [DisplayName("When calling create twice and initializing twice, the simualtion setup is only initialized one")]
    public async Task CallingSetupTwice_OnlyOneInstantiation(
        Func<SimulationSetup, ISimulationSession> sessionKindFactory)
    {
        var testScope = SimulationTestScope.PerTestSession(Adapter);
        var setup1 = testScope.Create(Metadata, sessionKindFactory);
        var setup2 = testScope.Create(Metadata, sessionKindFactory);

        await setup1.InitializeAsync();
        await setup2.InitializeAsync();

        await Assert.That(setup1.SimulationSetup.SimulationTarget).IsAssignableFrom<TestSimulationTargetConfig>();
        await Assert.That(setup2.SimulationSetup.SimulationTarget).IsAssignableTo<TestSimulationTargetConfig>();

        var firstServer = (TestSimulationTargetConfig)setup2.SimulationSetup.SimulationTarget;
        var secondServer = (TestSimulationTargetConfig)setup2.SimulationSetup.SimulationTarget;
        await Assert.That(firstServer.NumberInits).IsEqualTo(1);
        await Assert.That(secondServer.NumberInits).IsEqualTo(1);
    }
}