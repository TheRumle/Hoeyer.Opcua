using Hoeyer.OpcUa.Client.Abstractions.Browsing;
using Hoeyer.OpcUa.IntegrationTest.AbstractTests;
using Hoeyer.OpcUa.IntegrationTest.Fixtures;
using Playground.Modelling.Models;

namespace Playground.Application.EndToEndTest.Browsing;

[Category("Browser tests")]
[InheritsTests]
[ClassDataSource<IntegrationTestFixture<IEntityBrowser<MyLittleRobot>>>(Shared = SharedType.PerTestSession)]
public sealed class MyLittleRobotBrowseTest(IntegrationTestFixture<IEntityBrowser<MyLittleRobot>> context)
    : EntityBrowserTest<MyLittleRobot>(context);