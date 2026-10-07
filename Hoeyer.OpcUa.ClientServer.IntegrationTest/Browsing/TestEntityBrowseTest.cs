using Hoeyer.OpcUa.Client.Abstractions.Browsing;
using Hoeyer.OpcUa.Fixtures.Server.ServerFixture;
using Hoeyer.OpcUa.IntegrationTest.Fixtures.TestEntities;

namespace Hoeyer.OpcUa.IntegrationTest.Browsing;

[Category("Browser tests")]
[InheritsTests]
[ClassDataSource<RuntimeSelectedServer>(Shared = SharedType.PerTestSession)]
public sealed class TestEntityBrowseTest(
    RuntimeSelectedServer context)
    : EntityBrowserTest<DifferentFieldsEntity>(context.GetService<IEntityBrowser<DifferentFieldsEntity>>());