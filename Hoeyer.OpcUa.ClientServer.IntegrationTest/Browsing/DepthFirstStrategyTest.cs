using Hoeyer.OpcUa.Client.Abstractions.Browsing;
using Hoeyer.OpcUa.Client.Abstractions.Connection;
using Hoeyer.OpcUa.Client.Application.Browsing;
using Hoeyer.OpcUa.Core.Abstractions;
using Hoeyer.OpcUa.Fixtures.Server.ServerFixture;

namespace Hoeyer.OpcUa.IntegrationTest.Browsing;

[InheritsTests]
[ClassDataSource<RuntimeSelectedServer>(Shared = SharedType.PerTestSession)]
public sealed class DepthFirstStrategyTest(RuntimeSelectedServer fixture)
    : NodeTreeTraverserTest
{
    protected override INodeTreeTraverser TestedService => fixture.GetService<DepthFirstStrategy>();

    protected override Task<IEntitySession> GetSession() =>
        fixture.GetService<IEntitySessionFactory>().GetSessionForAsync<DepthFirstStrategyTest>();

    protected override IEnumerable<IBrowseNameCollection> GetBrowseNameCollection() =>
        fixture.GetService<IEnumerable<IBrowseNameCollection>>();
}