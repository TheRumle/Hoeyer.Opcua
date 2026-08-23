using Hoeyer.OpcUa.Client.Application.Browsing;
using Hoeyer.OpcUa.IntegrationTest.Browsing;
using Hoeyer.OpcUa.IntegrationTest.Fixtures;

namespace Playground.Application.EndToEndTest.Browsing;

[InheritsTests]
[ClassDataSource<IntegrationTestFixture<BreadthFirstStrategy>>(Shared = SharedType.PerClass)]
public sealed class BreadthFirstStrategyTest(IntegrationTestFixture<BreadthFirstStrategy> fixture)
    : NodeTreeTraverserTest<BreadthFirstStrategy>(fixture);