using Hoeyer.OpcUa.Test;
using Hoeyer.OpcUa.Test.Client;

namespace Playground.Application.EndToEndTest.OpcUaTestFramework;

[InheritsTests]
[ClassDataSource<ClientTestFixture>(Shared = SharedType.Keyed, Key = FixtureKeys.ReadOnlyFixture)]
public sealed class BreadthFirstSearchStrategyTests(ClientTestFixture fixture) : BreadthFirstStrategyTest(fixture);