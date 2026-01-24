using Hoeyer.OpcUa.Client.Application.Browsing;
using Hoeyer.OpcUa.Test.Adapter.Client.Api;
using Hoeyer.OpcUa.Test.Simulation;
using JetBrains.Annotations;

namespace OpcUa.Client.TestFramework;

[InheritsTests]
[TestSubject(typeof(BreadthFirstStrategy))]
[ClassDataSource<AdaptedSimulationFixture>(Shared = SharedType.PerTestSession)]
public sealed class BreadthFirstStrategyTest(ISimulationTestSession fixture)
    : NodeTreeTraverserTest(fixture, nameof(BreadthFirstStrategy), fixture.GetService<BreadthFirstStrategy>);