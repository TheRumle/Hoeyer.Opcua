using Hoeyer.OpcUa.Client.Application.Browsing;
using Hoeyer.OpcUa.EndToEndTest.Fixtures.Simulation;
using JetBrains.Annotations;

namespace Hoeyer.OpcUa.EndToEndTest.ClientTests;

[InheritsTests]
[TestSubject(typeof(BreadthFirstStrategy))]
[ClassDataSource<SimulationFixture>(Key = FixtureKeys.ReadOnlyFixture, Shared = SharedType.Keyed)]
public sealed class BreadthFirstStrategyTest(SimulationFixture fixture)
    : NodeTreeTraverserTest(fixture, nameof(BreadthFirstStrategy), fixture.GetService<BreadthFirstStrategy>);