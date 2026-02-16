using Hoeyer.OpcUa.Client.Api.Browsing;
using Hoeyer.OpcUa.Test;
using Hoeyer.OpcUa.Test.Api;
using Hoeyer.OpcUa.Test.Client;
using Hoeyer.OpcUa.Test.Simulation;

namespace Playground.Application.EndToEndTest.OpcUaTestFramework;

[InheritsTests]
[ClassDataSource<AdaptedSimulationServiceContext<IEntityBrowser>>(Key = FixtureKeys.ReadOnlyFixture,
    Shared = SharedType.Keyed)]
public sealed class BrowserTest(List<ISpecifiedTestSession<IEntityBrowser>> sessions) : EntityBrowserTest(sessions);