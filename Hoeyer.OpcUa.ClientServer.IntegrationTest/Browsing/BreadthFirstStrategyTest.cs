using Hoeyer.OpcUa.Client.Application.Browsing;
using Hoeyer.OpcUa.IntegrationTest.Fixtures;

namespace Hoeyer.OpcUa.IntegrationTest.Browsing;

[InheritsTests]
[ClassDataSource<IsolatedServerFixture<BreadthFirstStrategy>>(Shared = SharedType.PerTestSession)]
public sealed class BreadthFirstStrategyTest(IsolatedServerFixture<BreadthFirstStrategy> fixture)
    : NodeTreeTraverserTest<BreadthFirstStrategy>(fixture);