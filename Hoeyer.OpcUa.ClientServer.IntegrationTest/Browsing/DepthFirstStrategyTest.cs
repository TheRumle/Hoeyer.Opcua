using Hoeyer.OpcUa.Client.Application.Browsing;
using Hoeyer.OpcUa.IntegrationTest.Fixtures;

namespace Hoeyer.OpcUa.IntegrationTest.Browsing;

[InheritsTests]
[ClassDataSource<IntegrationTestFixture<DepthFirstStrategy>>(Shared = SharedType.PerTestSession)]
public sealed class DepthFirstStrategyTest(IntegrationTestFixture<DepthFirstStrategy> fixture)
    : NodeTreeTraverserTest<DepthFirstStrategy>(fixture);