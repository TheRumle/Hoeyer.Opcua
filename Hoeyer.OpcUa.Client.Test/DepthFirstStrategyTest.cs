using Hoeyer.OpcUa.Client.Application.Browsing;
using Hoeyer.OpcUa.Test.Adapter.Client.Api;
using Hoeyer.OpcUa.Test.Simulation;
using JetBrains.Annotations;

namespace OpcUa.Client.TestFramework;

[InheritsTests]
[TestSubject(typeof(DepthFirstStrategy))]
[ClassDataSource<AdaptedSimulationFixture>(Shared = SharedType.PerTestSession)]
public sealed class DepthFirstStrategyTest(ISimulationTestSession fixture)
    : NodeTreeTraverserTest(fixture, nameof(DepthFirstStrategy), fixture.GetService<BreadthFirstStrategy>);