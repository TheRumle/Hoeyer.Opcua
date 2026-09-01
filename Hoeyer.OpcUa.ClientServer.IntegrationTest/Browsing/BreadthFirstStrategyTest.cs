using Hoeyer.OpcUa.Client.Application.Browsing;
using Hoeyer.OpcUa.IntegrationTest.Fixtures;

namespace Hoeyer.OpcUa.IntegrationTest.Browsing;

[InheritsTests]
[ClassDataSource<IntegrationTestFixture<BreadthFirstStrategy>>(Shared = SharedType.PerClass)]
public sealed class BreadthFirstStrategyTest(IntegrationTestFixture<BreadthFirstStrategy> fixture)
    : NodeTreeTraverserTest<BreadthFirstStrategy>(fixture);