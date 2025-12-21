using Hoeyer.OpcUa.Client.Application.Browsing;
using Hoeyer.OpcUa.EndToEndTest.Fixtures.Simulation;
using JetBrains.Annotations;

namespace Hoeyer.OpcUa.EndToEndTest.ClientTests;

[InheritsTests]
[TestSubject(typeof(DepthFirstStrategy))]
[ClassDataSource<SimulationFixture>(Key = FixtureKeys.ReadOnlyFixture, Shared = SharedType.Keyed)]
public sealed class DepthFirstStrategyTest(SimulationFixture fixture)
    : NodeTreeTraverserTest(fixture, nameof(DepthFirstStrategy), fixture.GetService<BreadthFirstStrategy>);