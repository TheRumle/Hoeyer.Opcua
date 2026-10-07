using Hoeyer.OpcUa.Client.Abstractions.Browsing;
using Hoeyer.OpcUa.Fixtures.Server.ServerFixture;
using Hoeyer.OpcUa.IntegrationTest.Browsing;
using Playground.Modelling.Models;

namespace Playground.Application.EndToEndTest.Browsing;

[Category("Browser tests")]
[InheritsTests]
[ClassDataSource<RuntimeSelectedServer>(Shared = SharedType.PerTestSession)]
public sealed class AllPropertyTypesEntityBrowseTest(
    RuntimeSelectedServer context)
    : EntityBrowserTest<AllPropertyTypesEntity>(context.GetService<IEntityBrowser<AllPropertyTypesEntity>>());