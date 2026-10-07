using Hoeyer.OpcUa.Client.Abstractions.Browsing;
using Hoeyer.OpcUa.IntegrationTest.Fixtures;
using Hoeyer.OpcUa.IntegrationTest.Fixtures.TestEntities;

namespace Hoeyer.OpcUa.IntegrationTest.Browsing;

[Category("Browser tests")]
[InheritsTests]
[ClassDataSource<IsolatedServerFixture<IEntityBrowser<TestEntity>>>(Shared = SharedType.PerTestSession)]
public sealed class TestEntityBrowseTest(
    IsolatedServerFixture<IEntityBrowser<TestEntity>> context)
    : EntityBrowserTest<TestEntity>(context.TestedService);