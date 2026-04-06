using Hoeyer.OpcUa.Client.Api.Browsing;
using Hoeyer.OpcUa.Client.Test;
using Hoeyer.OpcUa.Test.Api;
using Hoeyer.OpcUa.Test.Api.Attributes;

namespace Playground.Application.EndToEndTest.Core;

[InheritsTests]
[ReadonlySimulationFixture<IEntityBrowser>]
[NotInParallel(nameof(EntityBrowserTest))]
public sealed class BrowserTest(SimulationServiceContext<IEntityBrowser> sessions) : EntityBrowserTest(sessions);