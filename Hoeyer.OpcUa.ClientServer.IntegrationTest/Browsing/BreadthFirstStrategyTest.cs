using Hoeyer.OpcUa.Client.Abstractions.Browsing;
using Hoeyer.OpcUa.Client.Abstractions.Connection;
using Hoeyer.OpcUa.Client.Application.Browsing;
using Hoeyer.OpcUa.Core.Abstractions;
using Hoeyer.OpcUa.Fixtures.Server.ServerFixture;

namespace Hoeyer.OpcUa.IntegrationTest.Browsing;

[InheritsTests]
[ClassDataSource<RuntimeSelectedServer>(Shared = SharedType.PerTestSession)]
public sealed class BreadthFirstStrategyTest(RuntimeSelectedServer fixture)
    : NodeTreeTraverserTest
{
    protected override INodeTreeTraverser TestedService => fixture.GetService<BreadthFirstStrategy>();

    protected override Task<IEntitySession> GetSession() =>
        fixture.GetService<IEntitySessionFactory>().GetSessionForAsync<BreadthFirstStrategyTest>();

    protected override IEnumerable<IBrowseNameCollection> GetBrowseNameCollection() =>
        fixture.GetService<IEnumerable<IBrowseNameCollection>>();
}