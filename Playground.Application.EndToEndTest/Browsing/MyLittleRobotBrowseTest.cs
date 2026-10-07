using Hoeyer.OpcUa.Client.Abstractions.Browsing;
using Hoeyer.OpcUa.Fixtures.Server.ServerFixture;
using Hoeyer.OpcUa.IntegrationTest.Browsing;
using Playground.Modelling.Models;

namespace Playground.Application.EndToEndTest.Browsing;

[Category("Browser tests")]
[InheritsTests]
[ClassDataSource<RuntimeSelectedServer>(Shared = SharedType.PerTestSession)]
public sealed class MyLittleRobotBrowseTest(RuntimeSelectedServer context)
    : EntityBrowserTest<MyLittleRobot>(context.GetService<IEntityBrowser<MyLittleRobot>>());