using Hoeyer.OpcUa.Client.Abstractions.Browsing;
using Hoeyer.OpcUa.IntegrationTest.Browsing;
using Hoeyer.OpcUa.IntegrationTest.Fixtures;
using Playground.Modelling.Models;

namespace Playground.Application.EndToEndTest.Browsing;

[Category("Readonly")]
[Category("Browser tests")]
[InheritsTests]
[ClassDataSource<IntegrationTestFixture<IEntityBrowser<Gantry>>>(Shared = SharedType.PerTestSession)]
public sealed class GantryBrowseTest(IntegrationTestFixture<IEntityBrowser<Gantry>> context)
    : EntityBrowserTest<Gantry>(context.TestedService);