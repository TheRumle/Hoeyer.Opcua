using Hoeyer.OpcUa.Client.Application.Browsing;
using Hoeyer.OpcUa.IntegrationTest.Fixtures;

namespace Hoeyer.OpcUa.IntegrationTest.Browsing;

[InheritsTests]
[ClassDataSource<IsolatedServerFixture<DepthFirstStrategy>>(Shared = SharedType.PerTestSession)]
public sealed class DepthFirstStrategyTest(IsolatedServerFixture<DepthFirstStrategy> fixture)
    : NodeTreeTraverserTest<DepthFirstStrategy>(fixture);