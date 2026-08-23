using Hoeyer.OpcUa.Client.Abstractions.Browsing;
using Hoeyer.OpcUa.IntegrationTest.Browsing;
using Hoeyer.OpcUa.IntegrationTest.Fixtures;
using Playground.Modelling.Models;

namespace Playground.Application.EndToEndTest.Browsing;

[Category("Browser tests")]
[InheritsTests]
[ClassDataSource<IntegrationTestFixture<IEntityBrowser<AllPropertyTypesEntity>>>(Shared = SharedType.PerTestSession)]
public sealed class AllPropertyTypesEntityBrowseTest(
    IntegrationTestFixture<IEntityBrowser<AllPropertyTypesEntity>> context)
    : EntityBrowserTest<AllPropertyTypesEntity>(context.TestedService);