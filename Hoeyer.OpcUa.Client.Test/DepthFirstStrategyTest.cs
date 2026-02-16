using Hoeyer.OpcUa.Client.Application.Browsing;
using Hoeyer.OpcUa.Test.Api;
using JetBrains.Annotations;

namespace Hoeyer.OpcUa.Test.Client;

[InheritsTests]
[TestSubject(typeof(DepthFirstStrategy))]
[ClassDataSource<ClientTestFixture>(Shared = SharedType.PerTestSession)]
public abstract class DepthFirstStrategyTest(ISimulationTestSession fixture)
    : NodeTreeTraverserTest(fixture, nameof(DepthFirstStrategy), fixture.GetService<BreadthFirstStrategy>);