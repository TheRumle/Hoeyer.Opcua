using Hoeyer.OpcUa.Client.Application.Browsing;
using Hoeyer.OpcUa.IntegrationTest.Browsing;
using Hoeyer.OpcUa.IntegrationTest.Fixtures;

namespace Playground.Application.EndToEndTest.Browsing;

[InheritsTests]
[ClassDataSource<IntegrationTestFixture<DepthFirstStrategy>>(Shared = SharedType.PerClass)]
public sealed class DepthFirstStrategyTest(IntegrationTestFixture<DepthFirstStrategy> fixture)
    : NodeTreeTraverserTest<DepthFirstStrategy>(fixture);